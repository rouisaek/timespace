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
	shiftStart: Temporal.Instant
	shiftEnd: Temporal.Instant
	breakTime: Temporal.Duration
	timeZoneId: Temporal.TimeZone | Temporal.TimeZoneProtocol
	status: TimesheetEntryStatus
	denialReason?: any
	createdAt: Temporal.Instant
	updatedAt: Temporal.Instant
}

function mapToTimesheetEntry(data: any): TimesheetEntry {
	return {
		id: data.id,
		shiftStart: Temporal.Instant.from(data.shiftStart),
		shiftEnd: Temporal.Instant.from(data.shiftEnd),
		breakTime: Temporal.Duration.from(data.breakTime),
		timeZoneId: Temporal.TimeZone.from(data.timeZoneId),
		status: data.status,
		denialReason: data.denialReason,
		createdAt: Temporal.Instant.from(data.createdAt),
		updatedAt: Temporal.Instant.from(data.updatedAt)
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
