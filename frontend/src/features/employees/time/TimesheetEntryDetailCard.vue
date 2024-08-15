<script setup lang="ts">
import { computed } from 'vue'
import { TimesheetEntryStatus, type TimesheetEntry } from './queries/useEmployeeTimesheetQuery'
import { msToTime } from '@/features/_shared/timeDisplayHelpers'
import { useI18n } from 'vue-i18n'

const i18n = useI18n()

const props = defineProps<{
	entry: TimesheetEntry
}>()

const totalWorkedMs = computed(() => {
	return props.entry.shiftEnd
		.since(props.entry.shiftStart)
		.subtract(props.entry.breakTime)
		.total({ unit: 'milliseconds' })
})
</script>

<template>
	<div
		class="bg-surface-50 dark:bg-surface-900 shadow-md w-full px-3 py-4 flex flex-row justify-between items-center cursor-pointer border-l-4 rounded"
		:class="{
			'border-green-500': entry.status === TimesheetEntryStatus.Approved,
			'border-amber-500': entry.status === TimesheetEntryStatus.AwaitingApproval,
			'border-red-500': entry.status === TimesheetEntryStatus.Denied
		}"
	>
		<div class="flex flex-col">
			<div class="flex flex-row space-x-2 items-center">
				<span class="text-tprimary font-semibold shrink-0">
					{{
						entry.shiftStart.toPlainDate().toLocaleString(i18n.locale.value, {
							weekday: 'long',
							year: 'numeric',
							month: 'long',
							day: 'numeric'
						})
					}}
				</span>
				<!-- <RequirePermission :permission="permissions.DeleteClockedHoursPermission">
					<TrashIcon
						class="w-7 h-7 text-red-600 bg-red-100 p-1 rounded shrink-0"
						@click.stop="openDeleteDialog(entry)"
					/>
				</RequirePermission> -->
			</div>
			<span class="text-tsecondary">
				{{ entry.shiftStart.toPlainTime().toString({ smallestUnit: 'minutes' }) }}-{{
					entry.shiftEnd.toPlainTime().toString({ smallestUnit: 'minutes' })
				}}</span
			>
		</div>
		<div class="flex items-center space-x-2">
			<div class="flex flex-col items-end">
				<span class="text-tprimary font-mono text-xl"> {{ msToTime(totalWorkedMs) }}</span>
				<div
					class="text-green-700 dark:text-green-300 rounded-full flex flex-row items-center text-sm space-x-1"
					v-if="entry.status === TimesheetEntryStatus.Approved"
				>
					<iconify-icon icon="heroicons:check" height="none" class="w-5 h-5 shrink-0" />
					<span class="shrink-0">{{ $t('timesheetEntryDetailCard.approved') }}</span>
				</div>
				<div
					class="text-amber-700 dark:text-amber-300 rounded-full flex flex-row items-center text-sm space-x-1"
					v-if="entry.status === TimesheetEntryStatus.AwaitingApproval"
				>
					<iconify-icon icon="heroicons:clock" height="none" class="w-5 h-5 shrink-0" />
					<span class="shrink-0">{{ $t('timesheetEntryDetailCard.awaitingApproval') }}</span>
				</div>
				<div
					class="text-red-700 dark:text-red-300 rounded-full flex flex-row items-center text-sm space-x-1"
					v-if="entry.status === TimesheetEntryStatus.Denied"
				>
					<iconify-icon icon="heroicons:x-mark" height="none" class="w-5 h-5 shrink-0" />
					<span class="shrink-0">{{ $t('timesheetEntryDetailCard.denied') }}</span>
				</div>
			</div>
			<iconify-icon icon="heroicons:chevron-right" height="none" class="w-6 h-6 text-gray-500" />
		</div>
	</div>
</template>

<style scoped></style>
