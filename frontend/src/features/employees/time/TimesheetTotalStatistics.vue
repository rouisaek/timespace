<script setup lang="ts">
import { computed } from 'vue'
import { TimesheetEntryStatus, type TimesheetEntry } from './queries/useEmployeeTimesheetQuery'
import { msToTime } from '@/features/_shared/timeDisplayHelpers'

const props = defineProps<{
	timesheetEntries?: TimesheetEntry[]
}>()

const workedHourEntries = computed(() => {
	if (!props.timesheetEntries) return []

	return props.timesheetEntries
		.filter((x) => x.status === TimesheetEntryStatus.Approved)
		.map((entry) => {
			return {
				...entry,
				workedMs: entry.shiftEnd
					.since(entry.shiftStart)
					.subtract(entry.breakTime)
					.total({ unit: 'milliseconds' })
			}
		})
})

const totalApprovedMs = computed(() => {
	return workedHourEntries.value.reduce((acc, entry) => {
		return acc + entry.workedMs
	}, 0)
})
</script>

<template>
	<div class="flex flex-row space-x-2">
		<div>
			<span>{{ $t('timesheetTotalStatistics.totalHours') }}</span>
			<span>{{ msToTime }}</span>
		</div>
	</div>
</template>

<style scoped></style>
