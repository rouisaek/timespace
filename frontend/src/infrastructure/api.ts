import router from '@/router'
import axios from 'axios'

const apiClient = axios.create({
	baseURL: '/api',
	withCredentials: true,
	headers: {
		Accept: 'application/json',
		'Content-Type': 'application/json'
	}
})

apiClient.defaults.headers.common['Accept'] = 'application/json'

apiClient.interceptors.response.use(
	(response) => {
		return response
	},
	(error) => {
		if (error.response.status === 401) {
			if (window.location.pathname.includes('accounts/')) return Promise.reject(error)
			router.push({ name: 'login' })
		}

		return Promise.reject(error)
	}
)

export { apiClient }
