<script setup lang="ts">
import * as Form from '@/features/_shared/formfields'
import { reactive, ref } from 'vue'
import Button from 'primevue/button'
import TimespaceLogoWithWordmark from '@/features/_shared/components/logos/TimespaceLogoWithWordmark.vue'
import useVuelidate from '@vuelidate/core'
import { useI18n } from 'vue-i18n'
import { apiClient } from '@/infrastructure/api'
import { useToastStore } from '@/infrastructure/stores/toastStore'
import { useRouter } from 'vue-router'

const { t } = useI18n()
const toast = useToastStore()
const router = useRouter()

const state = reactive<{
	email: string | null
	tenantName: string | null
	firstName: string | null
	middleName: string | null
	lastName: string | null
	password: string | null
}>({
	email: null,
	tenantName: null,
	firstName: null,
	middleName: null,
	lastName: null,
	password: null
})

const v$ = useVuelidate()
const submitted = ref(false)

function submit() {
	submitted.value = true
	if (v$.value.$invalid) return

	apiClient
		.post('/accounts/register', {
			email: state.email,
			tenantName: state.tenantName,
			firstName: state.firstName,
			middleName: state.middleName,
			lastName: state.lastName,
			password: state.password
		})
		.then(async () => {
			toast.add({
				severity: 'success',
				summary: t('success'),
				detail: t('signUpPage.successMessage'),
				life: 10000
			})
			await router.push({ name: 'login' })
		})
		.catch(async () => {
			toast.add({
				severity: 'error',
				summary: t('error'),
				detail: t('signUpPage.error'),
				life: 10000
			})
			await router.push({ name: 'login' })
		})
}
</script>

<template>
	<div class="flex w-full h-full place-items-center justify-center gradient-bg">
		<div
			class="p-8 md:p-12 flex flex-col shadow-2xl border-surface-200 dark:border-surface-900 border rounded bg-white dark:bg-surface-900 w-[95vw] md:w-[60vw] lg:w-[50vw] xl:w-[40vw]"
		>
			<div class="flex justify-center mb-6">
				<TimespaceLogoWithWordmark />
			</div>
			<h1 class="font-bold text-3xl text-tprimary">{{ $t('signUpPage.title') }}</h1>
			<p class="mb-6 text-tsecondary">{{ $t('signUpPage.stepOneSubtitle') }}</p>
			<Form.Text
				:label="t('commonFieldLabels.email')"
				v-model="state.email"
				email
				:show-text-errors="submitted"
				:show-error="submitted"
				required
			/>
			<Form.Text
				:label="t('commonFieldLabels.tenantName')"
				v-model="state.tenantName"
				:show-text-errors="submitted"
				:show-error="submitted"
				required
			/>
			<div class="flex flex-col lg:flex-row lg:gap-2">
				<Form.Text
					:label="t('commonFieldLabels.firstName')"
					v-model="state.firstName"
					required
					:show-error="submitted"
					:show-text-errors="submitted"
				/>
				<Form.Text
					:label="t('commonFieldLabels.middleName')"
					v-model="state.middleName"
					:show-error="submitted"
					:show-text-errors="submitted"
				/>
				<Form.Text
					:label="t('commonFieldLabels.lastName')"
					v-model="state.lastName"
					:show-error="submitted"
					:show-text-errors="submitted"
				/>
			</div>
			<Form.Text
				:label="t('commonFieldLabels.password')"
				v-model="state.password"
				:show-error="submitted"
				:show-text-errors="submitted"
				type="password"
				required
			/>
			<div class="flex flex-col gap-4 items-center">
				<Button class="mt-6 w-full" size="large" @click="submit">
					<template #default>
						<span>{{ $t('signUpPage.continueButtonText') }}</span>
						<iconify-icon
							icon="heroicons:arrow-right"
							height="none"
							class="h-6 w-6 ml-2"
						></iconify-icon>
					</template>
				</Button>
				<div class="flex flex-col xl:flex-row items-center gap-1">
					<span class="text-tsecondary">{{ $t('signUpPage.loginText') }}</span>
					<RouterLink
						class="text-indigo-700 dark:text-indigo-300 font-semibold"
						:to="{ name: 'login' }"
						>{{ $t('signUpPage.loginText2') }}</RouterLink
					>
				</div>
			</div>
		</div>
	</div>
</template>

<style scoped></style>
