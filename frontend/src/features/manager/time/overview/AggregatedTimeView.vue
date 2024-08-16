<script setup lang="ts">
import { computed, ref } from 'vue'
import { useGetAggregatedTimesheetEntriesQuery } from './useGetAggregatedTimesheetEntriesQuery'
import type { PeriodSelection } from '@/features/_shared/components/PeriodSelector'
import PeriodSelector from '@/features/_shared/components/PeriodSelector.vue'
import { TimesheetEntryStatus } from '@/features/employees/time/queries/useEmployeeTimesheetQuery'
import ContainerCard from '@/features/_shared/components/ContainerCard.vue'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import ProgressSpinner from 'primevue/progressspinner'
import { useI18n } from 'vue-i18n'
import Message from 'primevue/message'
import Button from 'primevue/button'
import { useRouter } from 'vue-router'
import ScaleInTransition from '@/features/_shared/components/transitions/ScaleInTransition.vue'

const period = ref<PeriodSelection>({
	start: null,
	end: null
})
const { data, isLoading } = useGetAggregatedTimesheetEntriesQuery(period)
const { t } = useI18n()
const router = useRouter()

const formattedData = computed(() => {
	if (!data.value) return []

	return data.value.map((entry) => {
		const uniqueDates = new Set(
			entry.entries
				.filter((x) => x.status === TimesheetEntryStatus.Approved)
				.map((x) => x.shiftStart.toPlainDate())
		)

		const workedMs = entry.entries
			.filter((x) => x.status === TimesheetEntryStatus.Approved)
			.reduce((acc, entry) => {
				const workedMs = entry.shiftEnd
					.since(entry.shiftStart)
					.subtract(entry.breakTime)
					.total({ unit: 'milliseconds' })

				return acc + workedMs
			}, 0)

		return {
			fullName: [entry.firstName, entry.middleName, entry.lastName].filter(Boolean).join(' '),
			salaryDays: uniqueDates.size,
			workedHours: Temporal.Duration.from({ milliseconds: workedMs }).total('hours')
		}
	})
})

const hasEntriesPendingApproval = computed(() => {
	if (!data.value) return false

	return data.value.some((entry) =>
		entry.entries.some((x) => x.status === TimesheetEntryStatus.AwaitingApproval)
	)
})
</script>

<template>
	<div class="flex flex-col gap-4">
		<ContainerCard class="flex justify-between items-center px-4">
			<h1 class="text-xl text-tprimary whitespace-nowrap">{{ $t('aggregatedTimeView.title') }}</h1>
		</ContainerCard>
		<ContainerCard class="h-full">
			<PeriodSelector v-model="period" />
		</ContainerCard>
		<ContainerCard v-if="isLoading" class="flex place-items-center justify-center py-10">
			<ProgressSpinner />
		</ContainerCard>
		<ContainerCard
			v-else-if="data?.length === 0"
			class="flex place-items-center justify-center py-10"
		>
			<span class="text-tsecondary">{{ $t('aggregatedTimeView.noEntries') }}</span>
		</ContainerCard>
		<ContainerCard v-else class="max-w-full">
			<ScaleInTransition appear>
				<Message severity="warn" v-if="hasEntriesPendingApproval" class="my-2" closable>
					<div class="flex flex-col gap-2 w-full">
						<div class="flex flex-row items-center gap-6">
							<iconify-icon icon="heroicons:exclamation-triangle" height="none" class="w-10 h-10" />
							<div class="flex flex-col">
								<h1 class="text-xl font-semibold">{{ t('watchOut') }}</h1>
								<h2 class="">{{ t('aggregatedTimeView.entriesPendingApprovalWarning') }}</h2>
							</div>
						</div>
						<Button
							class="w-full"
							severity="warn"
							:label="t('aggregatedTimeView.entriesPendingApprovalWarningLinkText')"
							@click="router.push({ name: 'manager-validate-hours' })"
						/>
					</div>
				</Message>
			</ScaleInTransition>
			<DataTable :value="formattedData" scrollable scroll-height="flex">
				<Column field="fullName" :header="$t('aggregatedTimeView.table.fullNameHeader')"></Column>
				<Column field="salaryDays" :header="$t('aggregatedTimeView.table.salaryDays')"> </Column>
				<Column field="workedHours" :header="$t('aggregatedTimeView.table.salaryHours')"> </Column>
			</DataTable>
		</ContainerCard>
	</div>
</template>

<style scoped></style>
