<script setup lang="ts">
import ContainerCard from '@/features/_shared/components/ContainerCard.vue'
import ApprovableTimesheetEntryCard from './ApprovableTimesheetEntryCard.vue'
import Button from 'primevue/button'
import { useGetApprovableTimesheetEntriesQuery } from './queries/useGetApprovableTimesheetEntriesQuery'
import { apiClient } from '@/infrastructure/api'
import { useQueryClient } from '@tanstack/vue-query'
const queryClient = useQueryClient()
import { useToastStore } from '@/infrastructure/stores/toastStore'
import { useI18n } from 'vue-i18n'
import ProgressSpinner from 'primevue/progressspinner'

const { data, isLoading } = useGetApprovableTimesheetEntriesQuery()
const toast = useToastStore()
const { t } = useI18n()

const approveAll = () => {
	apiClient
		.post('timesheet/approve-all')
		.then(async () => {
			await queryClient.invalidateQueries({ queryKey: ['approvable-entries'] })
			toast.add({
				severity: 'success',
				summary: t('success'),
				detail: t('validateHoursView.approveAllSuccess')
			})
		})
		.catch(() => {
			toast.add({
				severity: 'error',
				summary: t('error'),
				detail: t('validateHoursView.approveAllError')
			})
		})
}
</script>

<template>
	<div class="flex flex-col gap-4">
		<ContainerCard class="flex justify-between items-center px-4">
			<h1 class="text-xl text-tprimary whitespace-nowrap">{{ $t('validateHoursView.title') }}</h1>
			<Button
				:label="$t('validateHoursView.approveAll')"
				text
				@click="approveAll"
				:disabled="data?.length === 0"
			/>
		</ContainerCard>
		<ContainerCard v-if="isLoading" class="flex place-items-center justify-center py-10">
			<ProgressSpinner />
		</ContainerCard>
		<div
			class="grid gap-3 grid-cols-1 md:grid-cols-2 2xl:grid-cols-3"
			v-else-if="data && data.length > 0"
		>
			<ApprovableTimesheetEntryCard v-for="entry in data" :key="entry?.id" :entry="entry" />
		</div>
		<ContainerCard v-else class="flex place-items-center justify-center py-10"
			><span class="text-tsecondary">{{
				$t('validateHoursView.noHoursToBeApproved')
			}}</span></ContainerCard
		>
	</div>
</template>

<style scoped></style>
