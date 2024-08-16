import { userInfoFetcher } from '@/features/users/accounts/queries/useUserInfoQuery'
import { usePermissionStore } from '@/infrastructure/authorization/permissionStore'
import { policies } from '@/infrastructure/authorization/permissions'
import i18n from '@/infrastructure/i18n/i18n'
import queryClient from '@/infrastructure/query-client'
import { useToastStore } from '@/infrastructure/stores/toastStore'
import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router'

const routes: Array<RouteRecordRaw> = [
	// Accounts
	{
		path: '/accounts',
		name: 'accounts',
		component: () => import('@/infrastructure/layouts/AuthLayout.vue'),
		children: [
			{
				path: 'login',
				name: 'login',
				component: () => import('@/features/users/accounts/login/LoginView.vue')
			},
			{
				path: 'sign-up',
				name: 'sign-up',
				component: () => import('@/features/users/accounts/sign-up/SignUpView.vue')
			},
			{
				path: 'email-confirmation',
				name: 'email-confirmation',
				component: () =>
					import('@/features/users/accounts/email-confirmation/EmailConfirmationView.vue')
			},
			{
				path: 'invite/:inviteToken',
				name: 'accounts-invite',
				component: () => import('@/features/users/accounts/invites/AcceptInviteView.vue')
			},
			{
				path: 'password-reset',
				name: 'password-reset',
				component: () => import('@/features/users/accounts/password-reset/PasswordResetView.vue')
			}
		]
	},
	// App
	{
		path: '/app',
		component: () => import('@/infrastructure/layouts/AppLayout/AppLayout.vue'),
		meta: {
			auth: true
		},
		children: [
			{
				path: 'dashboard',
				name: 'dashboard',
				component: () => import('@/features/dashboard/DashboardView.vue')
			},
			// Employees
			{
				path: 'employees',
				name: 'employees',
				children: [
					{
						path: 'time',
						name: 'employees-time',
						component: () => import('@/features/employees/time/TimeView.vue'),
						meta: {
							permission: policies.getTimesheetEntriesEndpointPolicy
						}
					}
				]
			},
			// Manager
			{
				path: 'manager',
				name: 'manager',
				children: [
					{
						path: 'time',
						children: [
							{
								path: 'validate',
								name: 'manager-validate-hours',
								component: () => import('@/features/manager/time/validate/ValidateHoursView.vue'),
								meta: {
									permission: policies.getApprovableTimesheetEntriesEndpointPolicy
								}
							},
							{
								path: 'overview',
								name: 'manager-aggregated-time',
								component: () => import('@/features/manager/time/overview/AggregatedTimeView.vue'),
								meta: {
									permission: policies.getAggregatedTimesheetEntriesEndpointPolicy
								}
							}
						]
					},
					{
						path: 'employees',
						children: [
							{
								path: '',
								name: 'manager-employees-list',
								component: () =>
									import('@/features/manager/employees/overview/EmployeesListView.vue')
							},
							{
								path: 'invites',
								name: 'manager-employees-invites',
								component: () => import('@/features/manager/employees/invites/InvitesView.vue')
							}
						]
					}
				]
			}
		]
	}
]

const router = createRouter({
	history: createWebHistory(import.meta.env.BASE_URL),
	routes: routes
})

router.beforeEach(async (to, _, next) => {
	try {
		await queryClient.fetchQuery({
			queryKey: ['/accounts/me'],
			queryFn: userInfoFetcher,
			staleTime: 1000 * 10
		})

		// Go to dashboard if user is logged in
		if (to.matched.some((record) => record.name === 'accounts')) {
			next({ name: 'dashboard' })
		} else {
			const { hasPermission } = usePermissionStore()

			if (to.meta.permission) {
				const permissions = to.matched
					.map((record) => record.meta.permission)
					.filter((n) => n !== undefined) as string[]
				if (permissions.every((permission) => hasPermission(permission))) {
					next()
				} else {
					const toast = useToastStore()
					toast.add({
						severity: 'error',
						life: 5000,
						detail: i18n.global.t('errors.unauthorized')
					})
					return false
				}
			} else {
				next()
			}
		}

		// Scroll page to top on every route change
		window.scrollTo({
			top: 0,
			left: 0,
			behavior: 'smooth'
		})
	} catch (error: unknown) {
		if (to.matched.some((record) => record.meta.auth)) {
			next({ name: 'login' })
		} else {
			next()
		}
	}
})

export default router
