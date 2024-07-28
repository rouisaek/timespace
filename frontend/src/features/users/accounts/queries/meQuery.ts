import { apiClient } from '@/infrastructure/api'
import { useQuery } from '@tanstack/vue-query'

export interface UserInfoResponse {
	firstName: string
	middleName?: any
	lastName?: any
	email: string
	permissions: string[]
}

export function userInfoFetcher(): Promise<UserInfoResponse> {
	return apiClient
		.get<UserInfoResponse>(`/accounts/me`)
		.then(({ data }) => data)
		.catch((err) => {
			throw new Error(err.response.data.errorCode)
		})
}

export function useUserinfoQuery() {
	return useQuery({ queryKey: ['/accounts/me'], queryFn: userInfoFetcher })
}
