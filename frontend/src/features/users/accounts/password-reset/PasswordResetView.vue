<script setup lang="ts">
import TimespaceLogoWithWordmark from '@/features/_shared/components/logos/TimespaceLogoWithWordmark.vue'
import { apiClient } from '@/infrastructure/api'
import { reactive, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import * as Form from '@/features/_shared/formfields'
import { useI18n } from 'vue-i18n'
import { useToastStore } from '@/infrastructure/stores/toastStore'
import useVuelidate from '@vuelidate/core'
import Button from 'primevue/button'

const state = reactive({
	password: null,
	confirmPassword: null
})

const loading = ref(false)
const submitted = ref(false)
const route = useRoute()
const router = useRouter()
const toast = useToastStore()
const { t } = useI18n()
const v$ = useVuelidate()

const submit = async () => {
	const token = route.query.token as string
	const email = route.query.email as string

	submitted.value = true
	if (v$.value.$invalid) return

	if (!token || !email) {
		loading.value = false
		toast.add({
			severity: 'error',
			summary: t('error'),
			detail: t('passwordResetView.invalidTokenError')
		})
		await router.push({ name: 'login' })
		return
	}

	if (state.password !== state.confirmPassword) {
		loading.value = false
		toast.add({
			severity: 'error',
			summary: t('error'),
			detail: t('passwordResetView.passwordsDoNotMatchError')
		})
		return
	}

	loading.value = true

	apiClient
		.post('/accounts/password-reset/reset', {
			email,
			token: token,
			password: state.password
		})
		.then(async () => {
			toast.add({
				severity: 'success',
				summary: t('success'),
				detail: t('passwordResetView.passwordResetSuccess')
			})
			loading.value = false
			await router.push({ name: 'login' })
		})
		.catch(async () => {
			toast.add({
				severity: 'error',
				summary: t('error'),
				detail: t('passwordResetView.passwordResetError')
			})
			loading.value = false
			await router.push({ name: 'login' })
		})
}
</script>

<template>
	<div class="flex w-full h-full place-items-center justify-center gradient-bg">
		<div
			class="p-8 md:p-12 shadow-2xl border-surface-200 dark:border-surface-900 border rounded bg-white dark:bg-surface-900 w-[95vw] md:w-[60vw] lg:w-[50vw] xl:w-[40vw]"
		>
			<div class="flex flex-col w-full">
				<div class="flex justify-center mb-6">
					<TimespaceLogoWithWordmark />
				</div>
				<h1 class="font-bold text-3xl text-tprimary">{{ $t('passwordResetView.title') }}</h1>
				<p class="mb-6 text-tsecondary">{{ $t('passwordResetView.subtitle') }}</p>
				<Form.Text
					:label="t('commonFieldLabels.password')"
					v-model="state.password"
					:show-error="submitted"
					:show-text-errors="submitted"
					type="password"
					required
				/>
				<Form.Text
					:label="t('passwordResetView.confirmPasswordLabel')"
					v-model="state.confirmPassword"
					:show-error="submitted"
					:show-text-errors="submitted"
					type="password"
					required
				/>
				<Button class="mt-6 w-full" @click="submit" :loading="loading">
					<template #default>
						<span>{{ $t('passwordResetView.continueButtonText') }}</span>
						<iconify-icon
							icon="heroicons:arrow-right"
							height="none"
							class="h-6 w-6 ml-2"
						></iconify-icon>
					</template>
				</Button>
			</div>
		</div>
	</div>
</template>

<style scoped></style>
