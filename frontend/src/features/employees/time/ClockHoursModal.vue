<script setup lang="ts">
import { computed, reactive, ref, watchEffect, inject, type Ref } from 'vue'
import * as Form from '@/features/_shared/formfields'
import useVuelidate from '@vuelidate/core'
import Button from 'primevue/button'
import { useToastStore } from '@/infrastructure/stores/toastStore'
import { useI18n } from 'vue-i18n'
import Message from 'primevue/message'
import ScaleInTransition from '@/features/_shared/components/transitions/ScaleInTransition.vue'
import type { DynamicDialogInstance } from 'primevue/dynamicdialogoptions'
import { apiClient } from '@/infrastructure/api'
import { msToTime } from '@/features/_shared/timeDisplayHelpers'
import { useQueryClient } from '@tanstack/vue-query'

const { t } = useI18n()
const toast = useToastStore()
const dialogRef = inject<Ref<DynamicDialogInstance> | null>('dialogRef', null)
const queryClient = useQueryClient()

const state = reactive<{
	date: Temporal.PlainDate
	startTime: Temporal.PlainTime | null
	endTime: Temporal.PlainTime | null
	breakMs: number
}>({
	date: Temporal.Now.plainDateISO(),
	startTime: null,
	endTime: null,
	breakMs: 0
})

const submitted = ref(false)
const loading = ref(false)
const v$ = useVuelidate()

const startDt = computed(() => {
	if (state.startTime === null) {
		return null
	}

	return state.startTime.toPlainDateTime(state.date)
})

const endDt = computed(() => {
	if (state.endTime === null || state.startTime === null) {
		return null
	}

	if (Temporal.PlainTime.compare(state.startTime, state.endTime) === 1) {
		return state.endTime.toPlainDateTime(state.date.add(Temporal.Duration.from({ days: 1 })))
	}

	return state.endTime.toPlainDateTime(state.date)
})

const workedTime = computed(() => {
	if (startDt.value === null || endDt.value === null) {
		return '00:00'
	}

	const diffMs = endDt.value.since(startDt.value).total({ unit: 'milliseconds' }) ?? 0
	return msToTime(diffMs - state.breakMs)
})

const breakTime = computed(() => msToTime(state.breakMs))

const increaseBreak = (ms: number) => {
	if (startDt.value === null || endDt.value === null) {
		return
	}

	const diffMs = endDt.value.since(startDt.value).total({ unit: 'milliseconds' }) ?? 0

	if (state.breakMs + ms > diffMs) {
		console.log('break exceeds worked time')
		toast.add({
			severity: 'error',
			summary: t('error'),
			detail: t('clockHoursModal.breakTimeExceedsWorkedTime'),
			life: 3000
		})
		return
	}

	state.breakMs += ms
}

const decreaseBreak = (ms: number) => {
	if (state.breakMs - ms < 0) {
		toast.add({
			severity: 'error',
			summary: t('error'),
			detail: t('clockHoursModal.breakTimeNegative'),
			life: 3000
		})
		return
	}

	state.breakMs -= ms
}

const submit = () => {
	submitted.value = true

	if (v$.value.$invalid || startDt.value === null || endDt.value === null) {
		return
	}

	loading.value = true
	apiClient
		.post('/timesheet', {
			shiftStart: startDt.value.toZonedDateTime(Temporal.Now.timeZoneId()).toInstant().toString(),
			shiftEnd: endDt.value.toZonedDateTime(Temporal.Now.timeZoneId()).toInstant().toString(),
			breakTime: Temporal.Duration.from({ milliseconds: state.breakMs }).toString(),
			timeZoneId: Temporal.Now.timeZoneId()
		})
		.then(async () => {
			toast.add({
				severity: 'success',
				summary: t('success'),
				detail: t('clockHoursModal.saved'),
				life: 3000
			})
			await queryClient.invalidateQueries({ queryKey: ['employee-timesheet'] })
			loading.value = false
			dialogRef?.value.close()
		})
		.catch(() => {
			toast.add({
				severity: 'error',
				summary: t('error'),
				detail: t('clockHoursModal.errorSaving'),
				life: 3000
			})
			loading.value = false
		})
}

watchEffect(() => {
	if (state.startTime === null || state.endTime === null) {
		state.breakMs = 0
	}
})

let isAfterMidnight = computed(() => {
	return Temporal.Now.zonedDateTimeISO().hour > 0 && Temporal.Now.zonedDateTimeISO().hour < 6
})

const startedBeforeMidnight = () => {
	warningDismissed.value = true
	state.date = state.date.subtract(Temporal.Duration.from({ days: 1 }))
}

const warningDismissed = ref(false)
</script>

<template>
	<div class="flex flex-col gap-4">
		<ScaleInTransition appear>
			<Message severity="warn" v-if="isAfterMidnight && !warningDismissed" class="mt-2">
				<div class="flex flex-col gap-2 w-full">
					<div class="flex flex-row items-center gap-6">
						<iconify-icon icon="heroicons:exclamation-triangle" height="none" class="w-10 h-10" />
						<div class="flex flex-col">
							<h1 class="text-xl font-semibold">{{ t('watchOut') }}</h1>
							<h2 class="">{{ t('clockHoursModal.afterMidnightWarning') }}</h2>
						</div>
					</div>
					<Button
						severity="warn"
						:label="t('clockHoursModal.startedBeforeMidnight')"
						class="w-full"
						@click="startedBeforeMidnight"
					/>
				</div>
			</Message>
		</ScaleInTransition>
		<div>
			<Form.DatePicker
				v-model="state.date"
				:label="t('clockHoursModal.dateLabel')"
				showButtonBar
				selectOtherMonths
				:showError="submitted"
				:showTextErrors="submitted"
			/>
			<div class="flex flex-row gap-3">
				<Form.Time
					v-model="state.startTime"
					:label="t('clockHoursModal.startTimeLabel')"
					required
					:showError="submitted"
					:showTextErrors="submitted"
					@input="state.breakMs = 0"
					autofocus
				/>
				<Form.Time
					v-model="state.endTime"
					:label="t('clockHoursModal.endTimeLabel')"
					required
					:showError="submitted"
					:showTextErrors="submitted"
					@input="state.breakMs = 0"
				/>
			</div>
			<div class="flex flex-col gap-2">
				<Form.Text disabled v-model="breakTime" :label="t('clockHoursModal.breakTimeLabel')" />
				<ScaleInTransition>
					<div class="flex flex-row gap-2" v-if="state.startTime && state.endTime">
						<Button severity="secondary" @click="increaseBreak(60000)" class="w-full">+1</Button>
						<Button severity="secondary" @click="increaseBreak(900000)" class="w-full">+15</Button>
						<Button severity="secondary" @click="increaseBreak(1800000)" class="w-full">+30</Button>
						<Button severity="secondary" @click="increaseBreak(3600000)" class="w-full">+60</Button>
					</div>
				</ScaleInTransition>
				<ScaleInTransition>
					<div class="flex flex-row gap-2" v-if="state.breakMs > 0">
						<TransitionGroup name="scale-in">
							<Button
								severity="secondary"
								@click="decreaseBreak(60000)"
								v-if="state.breakMs >= 60000"
								class="w-full"
								>-1</Button
							>
							<Button
								severity="secondary"
								@click="decreaseBreak(900000)"
								v-if="state.breakMs >= 900000"
								class="w-full"
								>-15</Button
							>
							<Button
								severity="secondary"
								@click="decreaseBreak(1800000)"
								v-if="state.breakMs >= 1800000"
								class="w-full"
								>-30</Button
							>
							<Button
								severity="secondary"
								@click="decreaseBreak(3600000)"
								v-if="state.breakMs >= 3600000"
								class="w-full"
								>-60</Button
							>
						</TransitionGroup>
					</div>
				</ScaleInTransition>
				<div class="flex justify-center p-5 border-indigo-700 border-2">
					<span class="text-tprimary font-mono text-xl">{{ workedTime }}</span>
				</div>
				<Button @click="submit" label="Opslaan" class="w-full" :loading="loading" />
			</div>
		</div>
	</div>
</template>

<style scoped></style>
