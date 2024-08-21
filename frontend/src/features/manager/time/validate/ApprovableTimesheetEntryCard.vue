<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import type { ApprovableTimesheetEntry } from './queries/useGetApprovableTimesheetEntriesQuery'
import { computed, ref } from 'vue'
import { msToTime } from '@/features/_shared/timeDisplayHelpers'
import Button from 'primevue/button'
import ContainerCard from '@/features/_shared/components/ContainerCard.vue'
import { apiClient } from '@/infrastructure/api'
import { useToastStore } from '@/infrastructure/stores/toastStore'
import { useQueryClient } from '@tanstack/vue-query'
const queryClient = useQueryClient()
import { useDialog } from 'primevue/usedialog'
import DenyTimesheetEntryModal from './DenyTimesheetEntryModal.vue'

const props = defineProps<{
	entry: ApprovableTimesheetEntry
}>()

const { t, locale, getDateTimeFormat } = useI18n()
const toast = useToastStore()
const dialog = useDialog()
const loading = ref(false)

const totalWorkedMs = computed(() => {
	return props.entry.shiftEnd
		.since(props.entry.shiftStart)
		.subtract(props.entry.breakTime)
		.total({ unit: 'milliseconds' })
})

const approve = (id: number) => {
	loading.value = true
	apiClient
		.post('timesheet/approve', {
			timesheetEntryId: id
		})
		.then(async () => {
			await queryClient.invalidateQueries({ queryKey: ['approvable-entries'] })
			toast.add({
				severity: 'success',
				summary: t('success'),
				detail: t('approvableTimesheetEntryCard.approveSuccess'),
				life: 2000
			})
			loading.value = false
		})
}

const deny = (id: number) => {
	dialog.open(DenyTimesheetEntryModal, {
		props: {
			header: t('approvableTimesheetEntryCard.denyModalTitle'),
			modal: true,
			dismissableMask: true
		},
		data: {
			timesheetEntryId: id
		}
	})
}
</script>

<template>
	<ContainerCard class="space-y-3 p-4">
		<div class="flex flex-row justify-between items-center">
			<div class="flex flex-col">
				<span class="text-tprimary font-semibold">{{ entry.userName }}</span>
				<span class="text-ttertiary">{{
					entry.shiftStart.toPlainDate().toLocaleString(locale, {
						weekday: 'long',
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
					>{{ t('approvableTimesheetEntryCard.created') }}:
					{{ entry.createdAt.toLocaleString(locale, getDateTimeFormat(locale)['long']) }}</span
				>
				<span
					class="text-tsecondary"
					v-if="entry.updatedAt.epochMilliseconds !== entry.createdAt.epochMilliseconds"
					>{{ t('approvableTimesheetEntryCard.updated') }}:
					{{ entry.updatedAt.toLocaleString(locale, getDateTimeFormat(locale)['long']) }}</span
				>
			</div>
			<div>
				<span class="font-mono text-xl text-tprimary"> {{ msToTime(totalWorkedMs) }}</span>
			</div>
		</div>
		<div class="flex-grow"></div>
		<div class="flex flex-row space-x-2">
			<Button
				severity="danger"
				class="min-w-32 w-1/3"
				outlined
				@click="deny(entry.id)"
				:disabled="loading"
			>
				<iconify-icon icon="heroicons:x-mark" height="none" class="w-4 h-4 mt-0.5 mr-1" />
				<span>{{ t('approvableTimesheetEntryCard.deny') }}</span>
			</Button>
			<Button severity="success" class="w-full" @click="approve(entry.id)" :loading="loading">
				<iconify-icon icon="heroicons:check" height="none" class="w-4 h-4 mt-0.5 mr-1" />
				<span>{{ t('approvableTimesheetEntryCard.approve') }}</span>
			</Button>
		</div>
	</ContainerCard>
</template>

<style scoped></style>
