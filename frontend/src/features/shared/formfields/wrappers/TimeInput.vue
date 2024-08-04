<script setup lang="ts">
import useVuelidate from '@vuelidate/core';
import { useAttrs, reactive, computed, ref } from 'vue';
import { getRules, type BaseInputProps } from '../FormInputTypes';
import InputText from 'primevue/inputtext';
import FloatLabel from 'primevue/floatlabel';
import dayjs, { Dayjs } from 'dayjs';
import { uniqueId } from 'lodash-es';
import i18n from '@/infrastructure/i18n/i18n';
import { createI18nMessage, helpers } from '@vuelidate/validators';

const props = defineProps<BaseInputProps>();

const modelValue = defineModel<Dayjs | null>();
const internalValue = ref<string | null>(modelValue.value?.format('HH:mm') ?? null);
const attrs = useAttrs();
const id = attrs["id"] as string ?? uniqueId("input");

const withI18nMessage = createI18nMessage({ t: i18n.global.t.bind(i18n) })
const timeRule = (value: string) => {
	if (!value) return true;
	const timeRegex = /^([01]?[0-9]|2[0-3]):[0-5][0-9]$/;

	return withI18nMessage(
		helpers.withParams({ label: props.label }, (value: unknown) => timeRegex.test(value as string)),
		{ withArguments: true }
	)
};

const rules: any = getRules(props, false, props.label);

rules.time = timeRule;

const v$ = useVuelidate({ modelValue: rules }, reactive({ modelValue: internalValue }));

// eslint-disable-next-line vue/no-setup-props-destructure

const componentClasses = reactive<any>({
	"border-red-700 dark:border-red-300": computed(() => v$.value.modelValue.$invalid && props.showError),
	"w-full": true,
});

function onlyAllowed(str: string) {
	return /^[0-9:]*$/.test(str);
}

const inputChange = () => {
	if (internalValue.value === '' || internalValue.value === null) {
		modelValue.value = null;
		return;
	}

	let hours: any = 0;
	let minutes: any = 0;

	// Check if input contains illegal characters
	if (onlyAllowed(internalValue.value)) {
		if (!internalValue.value.includes('-')) {
			// Check if input has hours + minutes
			if (internalValue.value.includes(':')) {
				hours = internalValue.value.split(':')[0];
				minutes = internalValue.value.split(':')[1];

				let hoursNumber = Number.parseInt(hours);
				if (isNaN(hoursNumber))
					hours = '0';

				if (hours > 23)
					hours = 23;

				let minutesNumber = Number.parseInt(minutes);
				if (isNaN(minutesNumber))
					minutes = '0';

				if (minutes > 59)
					minutes = 59;

				// hours = roundHours(minutes, hours, userInfo.value?.roundHoursTo);
				// minutes = roundMinutes(minutes, userInfo.value?.roundHoursTo);

			} else {
				if (internalValue.value.length === 1 || internalValue.value.length === 2) {
					hours = internalValue.value;
					let hoursNumber = Number.parseInt(hours);
					if (isNaN(hoursNumber)) {
						hours = '0';
					}

					if (hours > 24)
						hours = 24;

					minutes = 0;
				} else if (internalValue.value.length === 3 || internalValue.value.length === 4) {
					hours = internalValue.value.substring(0, 2);
					minutes = internalValue.value.substring(2, 4);

					let hoursNumber = Number.parseInt(hours);
					if (isNaN(hoursNumber)) {
						hours = '0';
					}

					if (hours > 24)
						hours = 24;

					let minutesNumber = Number.parseInt(minutes);
					if (isNaN(minutesNumber)) {
						minutes = '0';
					}

					if (minutes > 59)
						minutes = 59;

					// hours = roundHours(minutes, hours, userInfo.value?.roundHoursTo);
					// minutes = roundMinutes(minutes, userInfo.value?.roundHoursTo);

				} else {
					hours = '0';
					minutes = '0';
				}
			}
		}
	} else {
		internalValue.value = null;
		modelValue.value = null;
		return;
	}

	modelValue.value = dayjs().hour(hours).minute(minutes).second(0).millisecond(0);
	internalValue.value = dayjs().hour(hours).minute(minutes).second(0).millisecond(0).format('HH:mm');
}

v$.value.$touch();
</script>

<template>
	<!-- Begin form field -->
	<div class="field w-full mt-6">
		<FloatLabel>
			<InputText :id="id" v-model="v$.modelValue.$model" :class="componentClasses"
				:invalid="v$.modelValue.$invalid && props.showError" @change="inputChange()"
				@focus="(!props.readonly ? ($event.target as HTMLInputElement).select() : '')">
				<template v-for="(_, slot) of $slots" v-slot:[slot]="scope">
					<slot :name="slot" v-bind="scope" />
				</template>
			</InputText>
			<label :for="id" :class="{ 'dark:text-red-300 text-red-700': v$.modelValue.$invalid && showError }">{{
				props.label }}{{
					required ?
						'*'
						: '' }}</label>
		</FloatLabel>
		<small :id="id + '-help'" v-if="helpText" class="text-tsecondary">{{ helpText }}<br></small>
		<span v-if="v$.modelValue.$invalid && showTextErrors">
			<span :id="id + '-error'" v-for="(error, index) of v$.modelValue.$errors" :key="index">
				<small class="dark:text-red-300 text-red-700">{{ error.$message }}</small>
			</span>
		</span>
	</div>
	<!-- End form Field -->
</template>

<style scoped></style>
