<script setup lang="ts">
import { computed } from 'vue'
import { TimesheetEntryStatus, type TimesheetEntry } from './queries/useEmployeeTimesheetQuery'
import { msToTime } from '@/features/_shared/timeDisplayHelpers'

const props = defineProps<{
	timesheetEntries?: TimesheetEntry[]
}>()

const workedHourEntries = computed(() => {
	if (!props.timesheetEntries) return []

	return props.timesheetEntries?.map((entry) => {
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
	return workedHourEntries.value
		?.filter((x) => x.status === TimesheetEntryStatus.Approved)
		.reduce((acc, entry) => {
			return acc + entry.workedMs
		}, 0)
})

const totalPendingMs = computed(() => {
	return workedHourEntries.value
		?.filter((x) => x.status === TimesheetEntryStatus.AwaitingApproval)
		.reduce((acc, entry) => {
			return acc + entry.workedMs
		}, 0)
})

const totalRejectedMs = computed(() => {
	return workedHourEntries.value
		?.filter((x) => x.status === TimesheetEntryStatus.Denied)
		.reduce((acc, entry) => {
			return acc + entry.workedMs
		}, 0)
})
</script>

<template>
	<div class="flex flex-row space-x-2">
		<div
			class="w-full text-center p-2 rounded mb-2 mt-2 flex flex-row items-center space-x-2 justify-center bg-amber-100 dark:bg-amber-800/40"
		>
			<iconify-icon
				icon="heroicons:clock"
				height="none"
				class="w-6 h-6 text-amber-900 dark:text-amber-100"
			/>
			<span class="font-mono text-xl text-center text-amber-900 dark:text-amber-100">{{
				msToTime(totalPendingMs)
			}}</span>
		</div>
		<div
			class="w-full text-center p-2 rounded mb-2 mt-2 flex flex-row items-center space-x-2 justify-center bg-green-100 dark:bg-green-800/40"
		>
			<iconify-icon
				icon="heroicons:check"
				height="none"
				class="w-6 h-6 text-green-900 dark:text-green-100"
			/>
			<span class="font-mono text-xl text-center text-green-900 dark:text-green-100">{{
				msToTime(totalApprovedMs)
			}}</span>
		</div>

		<div
			class="w-full text-center p-2 rounded mb-2 mt-2 flex flex-row items-center space-x-2 justify-center bg-red-100 dark:bg-red-800/40"
		>
			<iconify-icon
				icon="heroicons:x-mark"
				height="none"
				class="w-6 h-6 text-red-900 dark:text-red-100"
			/>
			<span class="font-mono text-xl text-center text-red-900 dark:text-red-100">{{
				msToTime(totalRejectedMs)
			}}</span>
		</div>
	</div>
</template>

<style scoped></style>
