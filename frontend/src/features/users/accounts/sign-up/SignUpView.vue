<script setup lang="ts">
import * as Form from "@/features/shared/formfields/formInputs";
import { reactive, ref } from "vue";
import Button from "primevue/button";
import TimespaceLogoWithWordmark from "@/features/shared/logos/TimespaceLogoWithWordmark.vue";
import useVuelidate from "@vuelidate/core";

const state = reactive({
	email: null,
	password: null,
});

const v$ = useVuelidate();
const submitted = ref(false);

function submit() {
	submitted.value = true;
	if (v$.value.$invalid) return;
	console.log("submitting", state);
}
</script>

<template>
	<div class="flex w-full h-full place-items-center justify-center gradient-bg">
		<div
			class="p-6 md:p-12 m-6 md:m-12 shadow-2xl border-gray-200 dark:border-gray-900 border rounded bg-white dark:bg-neutral-800 min-w-[50%] lg:min-w-[30%]">
			<div class="flex justify-center mb-6">
				<TimespaceLogoWithWordmark />
			</div>
			<h1 class="font-bold text-3xl">{{ $t('signUpPage.title') }}</h1>
			<p class="mb-6 text-neutral-600 dark:text-neutral-400">{{ $t('signUpPage.stepOneSubtitle') }}</p>
			<Form.Text id="email" :label="$t('commonFieldLabels.email')" v-model="state.email" email size="large"
				:show-text-errors="submitted" :show-error="submitted" required />
			<div class="flex flex-col gap-4 items-center">
				<Button :label="$t('signUpPage.continueButtonText')" icon="pi pi-arrow-right" icon-pos="right"
					class="mt-6 w-full" size="large" @click="submit" />
				<div class="flex flex-row gap-1">
					<span class="text-neutral-700 dark:text-neutral-300">{{ $t('signUpPage.loginText') }}</span>
					<RouterLink class="text-indigo-700 dark:text-indigo-300 font-semibold" :to="{ name: 'login' }">{{
						$t('signUpPage.loginText2') }}</RouterLink>
				</div>
			</div>
		</div>
	</div>
</template>

<style scoped></style>
