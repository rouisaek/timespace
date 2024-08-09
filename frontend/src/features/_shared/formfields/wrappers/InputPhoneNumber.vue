<template>
	<!-- Begin form field -->
	<div class="field w-full mt-6">
		<div class="w-full">
			<input type="text" :id="id" v-bind="componentAttributes" @input="($e) => modelValue = iti.getNumber()"
				@countrychange="($e) => modelValue = iti.getNumber()" :class="componentClasses" />
		</div>
		<small :id="id + '-help'" v-if="helpText">{{ helpText }}<br></small>
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
import { computed, onMounted, reactive, ref, useAttrs, watchEffect } from "vue";
import { getRules, type InputPhoneNumberProps } from "../FormInputTypes";
import intlTelInput from 'intl-tel-input';
import { createI18nMessage, helpers } from "@vuelidate/validators";
import i18n from "@/infrastructure/i18n/i18n";

const props = defineProps<InputPhoneNumberProps>();

const modelValue = defineModel();
const attrs = useAttrs();
const id = attrs["id"] as string ?? uniqueId("input");

const rules: any = getRules(props, false, props.label);
const withI18nMessage = createI18nMessage({ t: i18n.global.t.bind(i18n) });

rules.phoneNumber = withI18nMessage(helpers.withParams({ label: props.label, rules }, (value) => {
	if (typeof value !== 'string') return true;
	if (value.length === 0 && !Object.keys(rules).includes("required")) return true;

	if ((window as any).intlTelInputUtils.isValidNumber(value)) {
		return true;
	}
	return false;
}));

const v$ = useVuelidate({ modelValue: rules }, reactive({ modelValue }));

let iti;
let mounted = ref(false);

onMounted(() => {
	const inputEl = document.querySelector(`#${id}`) as HTMLInputElement;
	iti = intlTelInput(inputEl, {
		initialCountry: props.countryCode?.toLowerCase() ?? 'nl',
		autoInsertDialCode: true,
		formatNumberAsYouType: true,
		containerClass: 'w-full',
		placeholderNumberType: props.numberType ?? "MOBILE",
		customPlaceholder: function (selectedCountryPlaceholder, selectedCountryData) {
			return props.label + " (e.g. " + selectedCountryPlaceholder + ")";
		},
		utilsScript: 'https://cdn.jsdelivr.net/npm/intl-tel-input@19.2.16/build/js/utils.js',
	});

	if (modelValue.value)
		iti.setNumber(modelValue.value);

	mounted.value = true;
})

watchEffect(() => {
	if (props.countryCode && mounted.value) {
		iti.setCountry(props.countryCode.toLowerCase());
	}
})

const updateNumber = (number: string | null) => {
	if (mounted && number !== null)
		iti.setNumber(number);

	modelValue.value = iti.getNumber();
}

defineExpose({ updateNumber });

// eslint-disable-next-line vue/no-setup-props-destructure

const componentClasses = reactive<any>({
	"phone-invalid": computed(() => v$.value.modelValue.$invalid && props.showError),
	"w-full p-inputtext padding-left ": true,
});

const componentAttributes = reactive<any>({ ...props });

v$.value.$touch();
</script>

<style scoped>
.phone-invalid {
	border-color: #e24c4c;
}

.padding-left {
	padding-left: 4rem;
}
</style>
