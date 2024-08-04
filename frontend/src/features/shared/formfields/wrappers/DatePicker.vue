<template>
	<!-- Begin form field -->
	<div class="field w-full mt-6">
		<FloatLabel>
			<DatePicker :id="id" v-bind="componentAttributes" v-model="v$.modelValue.$model" :class="componentClasses">
				<template v-for="(_, slot) of $slots" v-slot:[slot]="scope">
					<slot :name="slot" v-bind="scope" />
				</template>
			</DatePicker>
			<label :for="id" :class="{ 'dark:text-red-300 text-red-700': v$.modelValue.$invalid && showError }">{{
				props.label }}{{
					required ?
						'*'
						: '' }}</label>
		</FloatLabel>
		<small :id="id + '-help'" v-if="helpText">{{ helpText }}<br></small>
		<span v-if="v$.modelValue.$invalid && showTextErrors">
			<span :id="id + '-error'" v-for="(error, index) of v$.modelValue.$errors" :key="index">
				<small class="dark:text-red-300 text-red-700">{{ error.$message }}</small>
			</span>
		</span>
	</div>
	<!-- End form Field -->
</template>

<script setup lang="ts">
import { useVuelidate } from "@vuelidate/core";
import { uniqueId } from 'lodash-es';
import { computed, reactive, ref, useAttrs, watch } from "vue";
import { getRules, type DatePickerProps } from "../FormInputTypes";
import DatePicker from "primevue/datepicker";
import FloatLabel from "primevue/floatlabel";

const props = defineProps<DatePickerProps>();

const modelValue = defineModel<Temporal.PlainDate | null>();
const internalValue = ref<Date | null>(new Date(modelValue.value?.toString()!) ?? null);
const attrs = useAttrs();
const id = attrs["id"] as string ?? uniqueId("input");

const rules: any = getRules(props, true, props.label);

const v$ = useVuelidate({ modelValue: rules }, reactive({ modelValue: internalValue }));

// watchEffect(() => {
// 	internalValue.value = modelValue.value?.toDate() ?? null;
// 	modelValue.value = dayjs(v$.value.modelValue.$model);
// });

const recursing = ref(false);

watch(
	() => internalValue.value,
	(newVal) => {
		console.log('internalValue changed', newVal);
		console.log('internalvalue as plainDate', newVal?.toTemporalInstant()
			.toZonedDateTimeISO(Temporal.Now.timeZoneId())
			.toPlainDate());

		if (newVal === null) {
			modelValue.value = null;
			return;
		}

		if (recursing.value) {
			recursing.value = false;
			return;
		}

		modelValue.value = newVal
			.toTemporalInstant()
			.toZonedDateTimeISO(Temporal.Now.timeZoneId())
			.toPlainDate();
	},
	{ deep: true }
);

watch(
	() => modelValue.value,
	(newVal) => {
		if (newVal === null) {
			internalValue.value = null;
			return;
		}

		recursing.value = true;
		internalValue.value = new Date(newVal?.toString()!) ?? null;
	},
	{ deep: true }
);

const componentClasses = reactive<any>({
	"border-red-700 dark:border-red-300": computed(() => v$.value.modelValue.$invalid && props.showError),
	"w-full": true,
});

const componentAttributes = reactive<any>({ ...props });

watch(
	() => attrs,
	(newAttrs) => {
		Object.assign(componentAttributes, { ...componentAttributes, ...newAttrs });
	},
	{ deep: true, immediate: true }
);

v$.value.$touch();
</script>
