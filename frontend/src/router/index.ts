import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router'

const routes: Array<RouteRecordRaw> = [
	{
		path: '/accounts',
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
			}
		]
	}
]

const router = createRouter({
	history: createWebHistory(import.meta.env.BASE_URL),
	routes: routes
})

export default router
