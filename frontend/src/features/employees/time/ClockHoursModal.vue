<script setup lang="ts">
import { computed, reactive, ref, watchEffect } from 'vue';
import * as Form from '@/features/shared/formfields/formInputs';
import dayjs from 'dayjs';
import useVuelidate from '@vuelidate/core';
import Button from 'primevue/button';
import { toastStore } from '@/infrastructure/stores/toastStore';
import { useI18n } from 'vue-i18n';
import Message from 'primevue/message';
import ScaleInTransition from '@/features/shared/transitions/ScaleInTransition.vue';

const { t } = useI18n();

const state = reactive<
	{
		date: Temporal.PlainDate;
		startTime: dayjs.Dayjs | null;
		endTime: dayjs.Dayjs | null;
		breakMs: number;
	}
>({
	date: Temporal.Now.plainDateISO(),
	startTime: null,
	endTime: null,
	breakMs: 0,
})

const submitted = ref(false);
const v$ = useVuelidate();

const startDt = computed(() => {
	if (state.startTime === null || state.date === null) {
		return null;
	}

	return state.startTime!.set('date', state.date.day).set('month', state.date.month).set('year', state.date.year);
})

const endDt = computed(() => {
	if (state.endTime === null || state.date === null) {
		return null;
	}

	let provisional = state.endTime!.set('date', state.date.day).set('month', state.date.month).set('year', state.date.year);

	if (provisional.isBefore(startDt.value)) {
		console.log('endDt is before startDt');
		provisional = provisional.add(1, 'day');
	}

	return provisional;
})

const msToTime = (ms: number) => {
	const diffFullHours = Math.floor(ms / 1000 / 60 / 60);
	const diffFullMinutes = Math.floor((ms - diffFullHours * 1000 * 60 * 60) / 1000 / 60);

	return `${diffFullHours.toString().padStart(2, '0')}:${diffFullMinutes.toString().padStart(2, '0')}`;
}

const workedTime = computed(() => {
	if (startDt.value === null || endDt.value === null) {
		return '00:00';
	}

	const diffMs = endDt.value.diff(startDt.value);
	return msToTime(diffMs - state.breakMs);
})

const breakTime = computed(() => msToTime(state.breakMs));

const increaseBreak = (ms: number) => {
	if (startDt.value === null || endDt.value === null) {
		return;
	}

	const diffMs = endDt.value.diff(startDt.value);

	if (state.breakMs + ms > diffMs) {
		toastStore.toasts.push({ severity: 'error', summary: t('error'), detail: t('clockHoursModal.breakTimeExceedsWorkedTime') });
		return;
	}

	state.breakMs += ms;
}

const decreaseBreak = (ms: number) => {
	if (state.breakMs - ms < 0) {
		toastStore.toasts.push({ severity: 'error', summary: t('error'), detail: t('clockHoursModal.breakTimeNegative') });
		return;
	}

	state.breakMs -= ms;
}

const submit = () => {
	submitted.value = true;

	if (v$.value.$invalid) {
		return;
	}
}

watchEffect(() => {
	if (state.startTime === null || state.endTime === null) {
		state.breakMs = 0;
	}
})

let isAfterMidnight = computed(() => {
	return true;
})

const startedBeforeMidnight = () => {
	warningDismissed.value = true;
	state.date = state.date.subtract(Temporal.Duration.from({ days: 1 }));
}

const warningDismissed = ref(false);
</script>

<template>
	<div class="flex flex-col gap-4">
		<pre>{{ state }}</pre>
		<pre>{{ state.date.toString() }}</pre>
		<ScaleInTransition appear>
			<Message severity="warn" v-if="isAfterMidnight && !warningDismissed" class="mt-2">
				<div class="flex flex-col gap-2 w-full">
					<div class="flex flex-row items-center gap-6">
						<iconify-icon icon="heroicons:exclamation-triangle" height="none" class="w-10 h-10" />
						<div class="flex flex-col">
							<h1 class="text-xl font-semibold ">{{ t('watchOut') }}</h1>
							<h2 class="">{{ t('clockHoursModal.afterMidnightWarning') }}</h2>
						</div>
					</div>
					<Button severity="warn" :label="t('clockHoursModal.startedBeforeMidnight')" class="w-full"
						@click="startedBeforeMidnight" />
				</div>
			</Message>
		</ScaleInTransition>
		<div>
			<Form.DatePicker v-model="state.date" :label="t('clockHoursModal.dateLabel')" showButtonBar
				selectOtherMonths :showError="submitted" :showTextErrors="submitted" />
			<div class="flex flex-row gap-3">
				<Form.Time v-model="state.startTime" :label="t('clockHoursModal.startTimeLabel')" required
					:showError="submitted" :showTextErrors="submitted" @input="state.breakMs = 0" autofocus />
				<Form.Time v-model="state.endTime" :label="t('clockHoursModal.endTimeLabel')" required
					:showError="submitted" :showTextErrors="submitted" @input="state.breakMs = 0" />
			</div>
			<div class="flex flex-col gap-2">
				<Form.Text disabled v-model="breakTime" :label="t('clockHoursModal.breakTimeLabel')" />
				<ScaleInTransition>
					<div class="flex flex-row gap-2" v-if="startDt && endDt">
						<Button severity="secondary" @click="increaseBreak(60000)" class="w-full">+1</Button>
						<Button severity="secondary" @click="increaseBreak(900000)" class="w-full">+15</Button>
						<Button severity="secondary" @click="increaseBreak(1800000)" class="w-full">+30</Button>
						<Button severity="secondary" @click="increaseBreak(3600000)" class="w-full">+60</Button>
					</div>
				</ScaleInTransition>
				<ScaleInTransition>
					<div class="flex flex-row gap-2" v-if="state.breakMs > 0">
						<TransitionGroup name="scale-in">
							<Button severity="secondary" @click="decreaseBreak(60000)" v-if="state.breakMs >= 60000"
								class="w-full">-1</Button>
							<Button severity="secondary" @click="decreaseBreak(900000)" v-if="state.breakMs >= 900000"
								class="w-full">-15</Button>
							<Button severity="secondary" @click="decreaseBreak(1800000)" v-if="state.breakMs >= 1800000"
								class="w-full">-30</Button>
							<Button severity="secondary" @click="decreaseBreak(3600000)" v-if="state.breakMs >= 3600000"
								class="w-full">-60</Button>
						</TransitionGroup>
					</div>
				</ScaleInTransition>
				<div class="flex justify-center p-5 border-indigo-700 border-2">
					<span class="text-tprimary font-mono text-xl">{{ workedTime }}</span>
				</div>
				<Button @click="submit" label="Opslaan" class="w-full" />
			</div>
		</div>
	</div>
</template>

<style scoped></style>
