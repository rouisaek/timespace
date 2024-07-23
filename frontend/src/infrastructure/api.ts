import router from '@/router'
import axios from 'axios'

const apiClient = axios.create({
	baseURL: import.meta.env.VITE_API_BASE_URL,
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
			if (window.location.pathname.includes('accounts/sign-up')) return Promise.reject(error)
			if (window.location.pathname.includes('accounts/login')) return Promise.reject(error)
			if (window.location.pathname.includes('accounts/email-confirmation'))
				return Promise.reject(error)
			router.push({ name: 'login' })
		}

		return Promise.reject(error)
	}
)

export { apiClient }
