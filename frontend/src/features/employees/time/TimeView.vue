<script setup lang="ts">
import ClockHoursModal from './ClockHoursModal.vue'
import { useI18n } from 'vue-i18n'
import PeriodSelector from '@/features/_shared/components/PeriodSelector.vue'
import { ref } from 'vue'
import { type PeriodSelection } from '@/features/_shared/components/PeriodSelector'
import { useEmployeeTimesheetQuery } from '@/features/employees/time/queries/useEmployeeTimesheetQuery'
import TimesheetTimeStatistics from '@/features/employees/time/TimesheetTimeStatistics.vue'
import TimesheetEntryDetailCard from '@/features/employees/time/TimesheetEntryDetailCard.vue'
import ContainerCard from '@/features/_shared/components/ContainerCard.vue'

const { t } = useI18n()

const timePeriod = ref<PeriodSelection>({
	start: null,
	end: null
})

const { data: timesheetEntries } = useEmployeeTimesheetQuery(timePeriod)
</script>

<template>
	<div class="flex flex-col gap-4">
		<ContainerCard>
			<ClockHoursModal />
		</ContainerCard>
		<ContainerCard class="h-full">
			<PeriodSelector v-model="timePeriod" />
			<TimesheetTimeStatistics :timesheetEntries="timesheetEntries" />
		</ContainerCard>
		<div class="flex flex-col gap-4">
			<TimesheetEntryDetailCard v-for="shift in timesheetEntries" :key="shift.id" :entry="shift" />
		</div>
	</div>
</template>

<style scoped></style>
