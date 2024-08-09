<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import type { ApprovableTimesheetEntry } from './queries/useGetApprovableTimesheetEntriesQuery'
import { computed } from 'vue'
import { msToTime } from '@/features/_shared/timeDisplayHelpers'
import Button from 'primevue/button'

const props = defineProps<{
	entry: ApprovableTimesheetEntry
}>()

const { t, locale, getDateTimeFormat } = useI18n()

const totalWorkedMs = computed(() => {
	return props.entry.shiftEnd
		.since(props.entry.shiftStart)
		.subtract(props.entry.breakTime)
		.total({ unit: 'milliseconds' })
})
</script>

<template>
	<div
		class="px-2 py-4 bg-surface-900 border-2 border-surface-800 rounded flex flex-col space-y-3 shadow-sm"
	>
		<div class="flex flex-row justify-between items-center">
			<div class="flex flex-col">
				<span class="text-tprimary font-semibold">{{ entry.userName }}</span>
				<span class="text-ttertiary">{{
					entry.shiftStart.toPlainDate().toLocaleString(locale, {
						weekday: 'long',
						year: 'numeric',
						month: 'long',
						day: 'numeric'
					})
				}}</span>
				<span class="text-tsecondary"
					>{{ entry.shiftStart.toPlainTime().toString({ smallestUnit: 'minutes' }) }}-{{
						entry.shiftEnd.toPlainTime().toString({ smallestUnit: 'minutes' })
					}}
					({{
						entry.breakTime.total({ unit: 'milliseconds' }) === 0
							? t('approvableTimesheetEntryCard.none')
							: entry.breakTime.toLocaleString(locale, { hour: 'numeric', minute: 'numeric' })
					}}
					{{ t('approvableTimesheetEntryCard.break') }})</span
				>
				<span class="text-tsecondary mt-2"
					>Ingevuld:
					{{ entry.createdAt.toLocaleString(locale, getDateTimeFormat(locale)['long']) }}</span
				>
				<span
					class="text-tsecondary"
					v-if="entry.updatedAt.epochMilliseconds !== entry.createdAt.epochMilliseconds"
					>Bewerkt:
					{{ entry.updatedAt.toLocaleString(locale, getDateTimeFormat(locale)['long']) }}</span
				>
			</div>
			<div>
				<span class="font-mono text-xl text-tprimary"> {{ msToTime(totalWorkedMs) }}</span>
			</div>
		</div>
		<div class="flex flex-row space-x-2">
			<Button severity="danger" class="w-1/3" outlined>
				<iconify-icon icon="heroicons:x-mark" height="none" class="w-4 h-4 mt-0.5" />
				<span>Verwijderen</span>
			</Button>
			<Button severity="success" class="w-full">
				<iconify-icon icon="heroicons:check" height="none" class="w-4 h-4" />
				<span>Goedkeuren</span>
			</Button>
		</div>
	</div>
</template>

<style scoped></style>
