<template>
	<!-- Begin form field -->
	<div class="field w-full mt-6">
		<FloatLabel>
			<InputText :id="id" v-bind="componentAttributes" v-model="v$.modelValue.$model" :class="componentClasses"
				:invalid="v$.modelValue.$invalid && props.showError">
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

<script lang="ts">
export default { inheritAttrs: false };
</script>

<script setup lang="ts">
import { useVuelidate } from "@vuelidate/core";
import { uniqueId } from "lodash-es";
import { computed, reactive, useAttrs, watch } from "vue";
import { getRules, type InputTextProps } from "../FormInputTypes";
import InputText from "primevue/inputtext";
import FloatLabel from "primevue/floatlabel";

const props = defineProps<InputTextProps>();

const modelValue = defineModel();
const attrs = useAttrs();
const id = attrs["id"] as string ?? uniqueId("input");

const rules: any = getRules(props, false, props.label);

const v$ = useVuelidate({ modelValue: rules }, reactive({ modelValue }));

// eslint-disable-next-line vue/no-setup-props-destructure

const componentClasses = reactive<any>({
	// "border-red-700 dark:border-red-300": computed(() => v$.value.modelValue.$invalid && props.showError),
	"w-full": true,
});

const inputProps: any = { ...props, ...attrs };
if (props.readonly) inputProps.readonly = props.readonly;
if (attrs.disabled) inputProps.disabled = props.disabled;

const componentAttributes = reactive<any>({ ...inputProps });

watch(
	() => attrs,
	(newAttrs) => {
		Object.assign(componentAttributes, { ...inputProps, ...newAttrs });
	},
	{ deep: true, immediate: true }
);

v$.value.$touch();
</script>
