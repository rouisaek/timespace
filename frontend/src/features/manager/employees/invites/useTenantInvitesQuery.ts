import { apiClient } from '@/infrastructure/api'
import { useQuery } from '@tanstack/vue-query'

export interface TenantInvite {
	inviteId: number
	email: string
	token: string
	expiresAt: Temporal.ZonedDateTime
	createdAt: Temporal.Instant
	updatedAt: Temporal.Instant
}

function mapTenantInvite(data: any): TenantInvite {
	return {
		inviteId: data.inviteId,
		email: data.email,
		token: data.token,
		expiresAt: Temporal.Instant.from(data.expiresAt).toZonedDateTimeISO(Temporal.Now.timeZoneId()),
		createdAt: Temporal.Instant.from(data.createdAt),
		updatedAt: Temporal.Instant.from(data.updatedAt)
	}
}

function tenantInvitesFetcher(): Promise<TenantInvite[]> {
	return apiClient
		.get<TenantInvite[]>('/tenant/members/invites')
		.then((response) => response.data.map(mapTenantInvite))
		.catch(() => [])
}

export function useTenantInvitesQuery() {
	return useQuery({
		queryKey: ['tenant/members/invites'],
		queryFn: () => tenantInvitesFetcher(),
		staleTime: 1000 * 60 * 5,
		retry: true
	})
}
