<template>
	<!-- Begin form field -->
	<div class="field w-full mt-6">
		<FloatLabel>
			<Password :id="id" v-bind="componentAttributes" v-model="v$.modelValue.$model">
				<template v-for="(_, slot) of $slots" v-slot:[slot]="scope">
					<slot :name="slot" v-bind="scope" />
				</template>
			</Password>
			<label :for="id" :class="{ 'p-error': v$.modelValue.$invalid && showError }">{{ props.label }}{{ required ?
				'*'
				: '' }}</label>
		</FloatLabel>
		<small :id="id + '-help'" v-if="helpText">{{ helpText }}<br></small>
		<span v-if="v$.modelValue.$invalid && showTextErrors">
			<span :id="id + '-error'" v-for="(error, index) of v$.modelValue.$errors" :key="index">
				<small class="p-error">{{ error.$message }}</small>
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
import { computed, reactive, useAttrs } from "vue";
import { getRules, type PasswordProps } from "../FormInputTypes";
import Password from "primevue/password";
import FloatLabel from "primevue/floatlabel";

const props = defineProps<PasswordProps>();

const modelValue = defineModel();
const attrs = useAttrs();
const id = attrs["id"] as string ?? uniqueId("input");

const rules: any = getRules(props, false, props.label);

const v$ = useVuelidate({ modelValue: rules }, reactive({ modelValue }));

// eslint-disable-next-line vue/no-setup-props-destructure

const componentClasses = reactive<any>({
	"border-red-700 dark:border-red-300": computed(() => v$.value.modelValue.$invalid && props.showError),
	"w-full": true,
	"p-inputtext-lg": props.size === "large",
});

const inputProps = { ...props };
if (props.disabled) inputProps.disabled = props.disabled;
if (props.readonly) inputProps.readonly = props.readonly;
inputProps.feedback = props?.feedback ?? false;
inputProps.toggleMask = props?.toggleMask ?? false;

const componentAttributes = reactive<any>({ ...inputProps });

v$.value.$touch();
</script>
