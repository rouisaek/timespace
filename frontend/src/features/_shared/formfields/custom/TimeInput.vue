<script setup lang="ts">
import useVuelidate from '@vuelidate/core';
import { useAttrs, reactive, computed, ref, nextTick } from 'vue';
import { getRules, type BaseInputProps } from '../FormInputTypes';
import InputText from 'primevue/inputtext';
import FloatLabel from 'primevue/floatlabel';
import { uniqueId } from 'lodash-es';
import i18n from '@/infrastructure/i18n/i18n';
import { createI18nMessage, helpers } from '@vuelidate/validators';

const props = defineProps<BaseInputProps>();

const modelValue = defineModel<Temporal.PlainTime | null>();
const internalValue = ref<string | null>(modelValue.value?.toString({ smallestUnit: 'minute' }) ?? null);
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

const inputChange = async (event: Event) => {
	const target = event.target as HTMLInputElement;
	console.log('inputChange', target.value);

	if (target.value === '' || target.value === null) {
		modelValue.value = null;
		return;
	}

	if (!onlyAllowed(target.value)) {
		internalValue.value = null;
		modelValue.value = null;
		return;
	}

	let result = parseTimeString(target.value);

	console.log(result)

	if (result === null) {
		internalValue.value = null;
		modelValue.value = null;
		return;
	}

	let { hours, minutes } = result;
	const plainTime = Temporal.PlainTime.from({ hour: hours, minute: minutes });
	modelValue.value = plainTime;
	console.log(plainTime, plainTime.toString({ smallestUnit: 'minute' }));
	internalValue.value = plainTime.toString({ smallestUnit: 'minute' });
}

function parseTimeString(timeString: string): { hours: number; minutes: number } | null {
	// Remove any colon (:) from the string for uniformity
	timeString = timeString.replace(":", "");

	let hours, minutes;

	// Depending on the length of the timeString, parse it accordingly
	switch (timeString.length) {
		case 1:
			hours = parseInt(timeString, 10);
			minutes = 0;
			break;
		case 2:
			hours = parseInt(timeString, 10);
			minutes = 0;
			break;
		case 3:
			hours = parseInt(timeString.slice(0, 1), 10);
			minutes = parseInt(timeString.slice(1), 10);
			break;
		case 4:
			hours = parseInt(timeString.slice(0, 2), 10);
			minutes = parseInt(timeString.slice(2), 10);
			break;
		default:
			return null;
	}

	// Ensure that hours and minutes are valid numbers
	if (isNaN(hours) || isNaN(minutes)) {
		throw new Error("Invalid time components");
	}

	return { hours, minutes };
}

v$.value.$touch();
</script>

<template>
	<!-- Begin form field -->
	<div class="field w-full mt-6">
		<FloatLabel>
			<InputText :id="id" v-model="v$.modelValue.$model" :class="componentClasses"
				:invalid="v$.modelValue.$invalid && props.showError" @change="(inputChange($event))" v-bind="attrs"
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
