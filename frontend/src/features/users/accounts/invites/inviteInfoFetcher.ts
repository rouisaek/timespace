import { apiClient } from '@/infrastructure/api'

export interface InviteInfo {
	email: string
	firstName: string
	middleName?: any
	lastName?: any
}

export function inviteInfoFetcher(token: string) {
	return apiClient.get<InviteInfo>(`/tenant/members/invites/${token}`)
}
