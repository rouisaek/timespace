import { apiClient } from '@/infrastructure/api'
import { useQuery } from '@tanstack/vue-query'

export interface ApprovableTimesheetEntry {
	id: number
	shiftStart: Temporal.ZonedDateTime
	shiftEnd: Temporal.ZonedDateTime
	breakTime: Temporal.Duration
	userName: string
	createdAt: Temporal.ZonedDateTime
	updatedAt: Temporal.ZonedDateTime
}

function mapToTimesheetEntry(data: any): ApprovableTimesheetEntry {
	return {
		id: data.id,
		shiftStart: Temporal.Instant.from(data.shiftStart).toZonedDateTimeISO(
			Temporal.TimeZone.from(data.timeZoneId)
		),
		shiftEnd: Temporal.Instant.from(data.shiftEnd).toZonedDateTimeISO(
			Temporal.TimeZone.from(data.timeZoneId)
		),
		breakTime: Temporal.Duration.from(data.breakTime),
		userName: data.userName,
		createdAt: Temporal.Instant.from(data.createdAt).toZonedDateTimeISO(
			Temporal.TimeZone.from(data.timeZoneId)
		),
		updatedAt: Temporal.Instant.from(data.updatedAt).toZonedDateTimeISO(
			Temporal.TimeZone.from(data.timeZoneId)
		)
	}
}

function managerApprovableEntriesFetcher(): Promise<ApprovableTimesheetEntry[]> {
	return apiClient
		.get<ApprovableTimesheetEntry[]>('/timesheet/approvable-entries')
		.then((response) => response.data.map(mapToTimesheetEntry))
		.catch(() => [])
}

export function useGetApprovableTimesheetEntriesQuery() {
	return useQuery({
		queryKey: ['approvable-entries'],
		queryFn: () => managerApprovableEntriesFetcher(),
		staleTime: 1000 * 60 * 5,
		retry: true
	})
}
