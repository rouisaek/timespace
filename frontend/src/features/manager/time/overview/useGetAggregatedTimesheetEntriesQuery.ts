import type { PeriodSelection } from '@/features/_shared/components/PeriodSelector'
import {
	mapToTimesheetEntry,
	type TimesheetEntry
} from '@/features/employees/time/queries/useEmployeeTimesheetQuery'
import { apiClient } from '@/infrastructure/api'
import { useQuery } from '@tanstack/vue-query'
import { unref, type MaybeRef } from 'vue'

export interface AggregatedTimesheetEntryResponse {
	tenantUserId: number
	firstName: string
	middleName?: string
	lastName?: string
	entries: TimesheetEntry[]
}

function mapToAggregatedTimesheetEntry(data: any): AggregatedTimesheetEntryResponse {
	return {
		tenantUserId: data.tenantUserId,
		firstName: data.firstName,
		middleName: data.middleName,
		lastName: data.lastName,
		entries: data.entries.map(mapToTimesheetEntry)
	}
}

function getAggregatedTimesheetEntriesFetcher(
	period: PeriodSelection
): Promise<AggregatedTimesheetEntryResponse[]> {
	if (!period.start || !period.end) {
		return Promise.resolve([])
	}

	return apiClient
		.post<AggregatedTimesheetEntryResponse[]>('/timesheet/aggregated', {
			fromDate: period.start?.toString(),
			toDate: period.end?.toString()
		})
		.then((response) => response.data.map(mapToAggregatedTimesheetEntry))
		.catch(() => [])
}

export function useGetAggregatedTimesheetEntriesQuery(period: MaybeRef<PeriodSelection>) {
	return useQuery({
		queryKey: ['/timesheet/aggregated', { period }],
		queryFn: () => getAggregatedTimesheetEntriesFetcher(unref(period)),
		staleTime: 1000 * 60 * 5,
		retry: true
	})
}
