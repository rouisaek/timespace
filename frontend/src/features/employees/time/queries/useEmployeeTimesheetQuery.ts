import type { PeriodSelection } from '@/features/_shared/components/PeriodSelector'
import { apiClient } from '@/infrastructure/api'
import { useQuery } from '@tanstack/vue-query'
import { unref, type MaybeRef } from 'vue'

export enum TimesheetEntryStatus {
	AwaitingApproval = 0,
	Denied = 1,
	Approved = 2
}

export interface TimesheetEntry {
	id: number
	shiftStart: Temporal.ZonedDateTime
	shiftEnd: Temporal.ZonedDateTime
	breakTime: Temporal.Duration
	status: TimesheetEntryStatus
	denialReason?: any
	createdAt: Temporal.ZonedDateTime
	updatedAt: Temporal.ZonedDateTime
}

function mapToTimesheetEntry(data: any): TimesheetEntry {
	return {
		id: data.id,
		shiftStart: Temporal.Instant.from(data.shiftStart).toZonedDateTimeISO(
			Temporal.TimeZone.from(data.timeZoneId)
		),
		shiftEnd: Temporal.Instant.from(data.shiftEnd).toZonedDateTimeISO(
			Temporal.TimeZone.from(data.timeZoneId)
		),
		breakTime: Temporal.Duration.from(data.breakTime),
		status: data.status,
		denialReason: data.denialReason,
		createdAt: Temporal.Instant.from(data.createdAt).toZonedDateTimeISO(
			Temporal.TimeZone.from(data.timeZoneId)
		),
		updatedAt: Temporal.Instant.from(data.updatedAt).toZonedDateTimeISO(
			Temporal.TimeZone.from(data.timeZoneId)
		)
	}
}

function employeeTimesheetFetcher(period: PeriodSelection): Promise<TimesheetEntry[]> {
	if (!period.start || !period.end) {
		return Promise.resolve([])
	}

	return apiClient
		.post<TimesheetEntry[]>('/timesheet/query', {
			fromDate: period.start?.toString(),
			toDate: period.end?.toString()
		})
		.then((response) => response.data.map(mapToTimesheetEntry))
		.catch(() => [])
}

export function useEmployeeTimesheetQuery(period: MaybeRef<PeriodSelection>) {
	return useQuery({
		queryKey: ['employee-timesheet', { period }],
		queryFn: () => employeeTimesheetFetcher(unref(period)),
		staleTime: 1000 * 60 * 5,
		retry: true
	})
}
