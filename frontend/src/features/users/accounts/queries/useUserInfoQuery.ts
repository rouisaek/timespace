import { apiClient } from '@/infrastructure/api'
import { usePermissionStore } from '@/infrastructure/authorization/permissionStore'
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
		.then(({ data }) => {
			const permissionStore = usePermissionStore()
			permissionStore.setPermissions(data.permissions)

			return data
		})
		.catch((err) => {
			throw new Error(err.response.data.errorCode)
		})
}

export function useUserinfoQuery() {
	return useQuery({ queryKey: ['/accounts/me'], queryFn: userInfoFetcher, retry: false })
}
