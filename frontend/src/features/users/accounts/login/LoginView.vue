<script setup lang="ts">
import * as Form from "@/features/shared/formfields/formInputs";
import { reactive, ref } from "vue";
import Button from "primevue/button";
import TimespaceLogoWithWordmark from "@/features/shared/logos/TimespaceLogoWithWordmark.vue";
import useVuelidate from "@vuelidate/core";
import { RouterLink } from "vue-router";
import { useDialog } from "primevue/usedialog";
import ForgotPasswordModal from "./forgot-password/ForgotPasswordModal.vue";
import { useI18n } from "vue-i18n";
import { apiClient } from "@/infrastructure/api";
import Message from "primevue/message";
import request from "axios";
import { useToast } from "primevue/usetoast";

const dialog = useDialog();
const toast = useToast();
const { t } = useI18n();

const state = reactive({
	email: null,
	password: null,
});

const v$ = useVuelidate();
const submitted = ref(false);
const loading = ref(false);
const showLoginFailedError = ref(false);
const showEmailConfirmationError = ref(false);

function submit() {
	submitted.value = true;
	if (v$.value.$invalid) return;
	loading.value = true;

	apiClient.post("/accounts/login", state).then(() => {
		loading.value = false;
	}).catch((err) => {
		if (request.isAxiosError(err)) {
			switch (err.response?.data.type) {
				case 'email-not-confirmed':
					showEmailConfirmationError.value = true;
					break;
				case 'login-failed':
					showLoginFailedError.value = true;
					break;
				default:
					break;
			}
		}
		loading.value = false;
	});

	console.log("submitting", state);
}

function openResetPasswordModal() {
	dialog.open(ForgotPasswordModal, {
		props: {
			header: t("loginPage.forgotPasswordModalHeader"),
			style: {
				width: '50vw',
			},
			breakpoints: {
				'960px': '75vw',
				'640px': '90vw'
			},
			modal: true
		},
		data: {
			email: state.email
		}
	});
}

function resendConfirmationEmail() {
	apiClient.post("/accounts/email-confirmation/resend", { email: state.email }).then(() => {
		toast.add({ severity: "success", summary: t('success'), detail: t('loginPage.emailConfirmationSent'), life: 5000 });
		showEmailConfirmationError.value = false;
	}).catch(() => {
		toast.add({ severity: "error", summary: t('error'), detail: t('loginPage.emailConfirmationNotSent'), life: 5000 });
		showEmailConfirmationError.value = false;
	});
}
</script>

<template>
	<div class="flex w-full h-full place-items-center justify-center gradient-bg">
		<div
			class="p-12 shadow-2xl border-gray-200 dark:border-gray-900 border rounded bg-white dark:bg-neutral-800 min-w-[50%] md:min-w-[30%]">
			<div class="flex justify-center mb-6">
				<TimespaceLogoWithWordmark />
			</div>
			<Message v-if="showLoginFailedError" severity="error" class="mb-4">{{ t('errors.login-failed') }}</Message>
			<Message v-if="showEmailConfirmationError" severity="error" class="mb-4" :pt="{
				text: {
					class: 'w-full'
				}
			}">
				<div class="flex flex-col w-full gap-3">
					<span>
						{{ $t('errors.email-not-confirmed') }}
					</span>
					<Button class="w-full" @click="resendConfirmationEmail">{{
						$t('errors.email-not-confirmed.resend-email') }}
					</Button>

				</div>
			</Message>
			<h1 class="font-bold text-3xl mb-6">{{ $t('loginPage.title') }}</h1>
			<Form.Text id="email" :label="$t('commonFieldLabels.email')" v-model="state.email" email size="large"
				:show-text-errors="submitted" :show-error="submitted" required />
			<Form.Password id="password" :label="$t('loginPage.passwordFieldLabel')" v-model="state.password"
				:toggleMask="true" size="large" :show-text-errors="submitted" :show-error="submitted" required />
			<span role="button" @click="openResetPasswordModal"
				class="text-neutral-700 dark:text-neutral-300 cursor-pointer">{{
					$t('loginPage.forgotPasswordText')
				}}</span>
			<div class="flex flex-col gap-4 items-center">
				<Button :label="$t('loginPage.loginButtonText')" class="mt-6 w-full" size="large" @click="submit"
					:loading="loading" />
				<div class="flex flex-row gap-1">
					<span class="text-neutral-700 dark:text-neutral-300">{{ $t('loginPage.signUpText1') }}</span>
					<RouterLink class="text-indigo-700 dark:text-indigo-300 font-semibold" :to="{ name: 'sign-up' }">{{
						$t('loginPage.signUpText2') }}</RouterLink>
				</div>
			</div>
		</div>
	</div>
</template>

<style scoped></style>
