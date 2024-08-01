<template>
	<!-- Begin form field -->
	<div class="field w-full mt-6">
		<div class="p-float-label w-full">
			<DatePicker :id="id" v-bind="componentAttributes" v-model="v$.modelValue.$model" :class="componentClasses">
				<template v-for="(_, slot) of $slots" v-slot:[slot]="scope">
					<slot :name="slot" v-bind="scope" />
				</template>
			</DatePicker>
			<label :for="id" :class="{ 'p-error': v$.modelValue.$invalid && showError }">{{ props.label }}{{ required ?
				'*'
				: '' }}</label>
		</div>
		<small :id="id + '-help'" v-if="helpText">{{ helpText }}<br></small>
		<span v-if="v$.modelValue.$invalid && showTextErrors">
			<span :id="id + '-error'" v-for="(error, index) of v$.modelValue.$errors" :key="index">
				<small class="p-error">{{ error.$message }}</small>
			</span>
		</span>
	</div>
	<!-- End form Field -->
</template>

<script setup lang="ts">
import { useVuelidate } from "@vuelidate/core";
import { uniqueId } from 'lodash-es';
import { computed, reactive, useAttrs, watch } from "vue";
import { getRules, type DatePickerProps } from "../FormInputTypes";
import DatePicker from "primevue/datepicker";

const props = defineProps<DatePickerProps>();

const modelValue = defineModel<Date | null | string>();
const attrs = useAttrs();
const id = attrs["id"] as string ?? uniqueId("input");

const rules: any = getRules(props, true, props.label);

const v$ = useVuelidate({ modelValue: rules }, reactive({ modelValue }));

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
