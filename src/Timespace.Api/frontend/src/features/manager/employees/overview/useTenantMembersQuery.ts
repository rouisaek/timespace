import { apiClient } from '@/infrastructure/api'
import { useQuery } from '@tanstack/vue-query'

export interface TenantMember {
	userId: number
	employeeCode: string
	firstName: string
	middleName: string
	lastName: string
	email: string
}

function tenantMemberFetcher(): Promise<TenantMember[]> {
	return apiClient
		.get<TenantMember[]>('/tenant/members')
		.then((response) => response.data)
		.catch(() => [])
}

export function useTenantMembersQuery() {
	return useQuery({
		queryKey: ['tenant/members'],
		queryFn: () => tenantMemberFetcher(),
		staleTime: 1000 * 60 * 5,
		retry: true
	})
}
