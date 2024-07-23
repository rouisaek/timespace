import { userInfoFetcher } from '@/features/users/accounts/queries/meQuery'
import queryClient from '@/infrastructure/query-client'
import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router'

const routes: Array<RouteRecordRaw> = [
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
			}
		]
	},
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
			// const data = await retrievePortalInfoExtended()
			// if (to.meta.requireFeatureFlags) {
			// 	const featureFlags = to.meta.requireFeatureFlags as string[]
			// 	if (!featureFlags.every((flag) => hasFeatureFlag(flag))) {
			// 		Swal.fire({
			// 			title: i18n.global.t('feature_not_available'),
			// 			text: i18n.global.t('feature_not_available_text'),
			// 			icon: 'error',
			// 			confirmButtonText: 'OK'
			// 		})

			// 		next({ name: data?.defaultPageName })
			// 	}
			// }
			next()
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
