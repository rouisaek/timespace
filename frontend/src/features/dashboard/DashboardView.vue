<script setup lang="ts">
import { useDialog } from 'primevue/usedialog'
import ClockHoursModal from '../employees/time/ClockHoursModal.vue'
import { useI18n } from 'vue-i18n'
import Button from 'primevue/button'
import PeriodSelector from '../_shared/components/PeriodSelector.vue'
import { ref } from 'vue'
import { type PeriodSelection } from '../_shared/components/PeriodSelector'
import { useEmployeeTimesheetQuery } from '../employees/time/queries/useEmployeeTimesheetQuery'
import TimesheetTimeStatistics from '../employees/time/TimesheetTimeStatistics.vue'
import TimesheetEntryDetailCard from '../employees/time/TimesheetEntryDetailCard.vue'
import ContainerCard from '../_shared/components/ContainerCard.vue'

const dialogService = useDialog()
const { t } = useI18n()

const timePeriod = ref<PeriodSelection>({
	start: null,
	end: null
})

const { data: timesheetEntries } = useEmployeeTimesheetQuery(timePeriod)

const openClockHoursModal = () => {
	dialogService.open(ClockHoursModal, {
		props: {
			header: t('clockHoursModal.title'),
			modal: true
		}
	})
}
</script>

<template>
	<div class="flex flex-col gap-4">
		<ContainerCard class="h-full">
			<PeriodSelector v-model="timePeriod" />
			<TimesheetTimeStatistics :timesheetEntries="timesheetEntries" />
			<Button @click="openClockHoursModal" class="w-full">
				<template #default>
					<span>{{ $t('dashboard.clockHoursButton') }}</span>
					<iconify-icon
						icon="heroicons:plus"
						height="none"
						class="w-6 h-6 ml-2 text-primary-contrast"
					/>
				</template>
			</Button>
		</ContainerCard>
		<div class="flex flex-col gap-4">
			<TimesheetEntryDetailCard v-for="shift in timesheetEntries" :key="shift.id" :entry="shift" />
		</div>
	</div>
</template>

<style scoped></style>
