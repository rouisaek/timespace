<script setup lang="ts">
import * as Form from '@/features/_shared/formfields'
import { onMounted, reactive, ref } from 'vue'
import Button from 'primevue/button'
import TimespaceLogoWithWordmark from '@/features/_shared/components/logos/TimespaceLogoWithWordmark.vue'
import useVuelidate from '@vuelidate/core'
import { inviteInfoFetcher } from './inviteInfoFetcher'
import { useRoute, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import axios, { AxiosError } from 'axios'
import { useToastStore } from '@/infrastructure/stores/toastStore'
import ProgressSpinner from 'primevue/progressspinner'
import { apiClient } from '@/infrastructure/api'
import { useQueryClient } from '@tanstack/vue-query'

const route = useRoute()
const isLoading = ref(false)
const toast = useToastStore()
const router = useRouter()
const { t } = useI18n()
const queryClient = useQueryClient()

const state = reactive<{
	email: string | null
	firstName: string | null
	middleName: string | null
	lastName: string | null
	password: string | null
}>({
	email: null,
	firstName: null,
	middleName: null,
	lastName: null,
	password: null
})

const v$ = useVuelidate()
const submitted = ref(false)
const signupLoading = ref(false)

function submit() {
	submitted.value = true
	if (v$.value.$invalid) return

	signupLoading.value = true

	apiClient
		.post('/accounts/register/invite', {
			inviteCode: route.params.inviteToken as string,
			firstName: state.firstName,
			middleName: state.middleName,
			lastName: state.lastName,
			password: state.password
		})
		.then(async () => {
			await queryClient.invalidateQueries({ queryKey: ['/accounts/me'] })
			signupLoading.value = false
			await router.push({ name: 'dashboard' })
			toast.add({
				severity: 'success',
				summary: t('success'),
				detail: t('acceptInviteView.success'),
				life: 5000
			})
		})
		.catch(async (err: Error | AxiosError) => {
			if (axios.isAxiosError(err)) {
				if (err.status === 404) {
					toast.add({
						severity: 'error',
						summary: t('error'),
						detail: t('acceptInviteView.inviteNotFound'),
						life: 5000
					})
					await router.push({ name: 'login' })
				} else {
					toast.add({
						severity: 'error',
						summary: t('error'),
						detail: t('acceptInviteView.errorCreatingUser'),
						life: 5000
					})
					await router.push({ name: 'login' })
				}
			}
		})
}

onMounted(() => {
	const inviteToken = route.params.inviteToken as string
	isLoading.value = true

	inviteInfoFetcher(inviteToken)
		.then((data) => {
			isLoading.value = false
			state.email = data.data.email
			state.firstName = data.data.firstName
			state.middleName = data.data.middleName
			state.lastName = data.data.lastName
		})
		.catch(async (err: Error | AxiosError) => {
			if (axios.isAxiosError(err)) {
				if (err.status === 404) {
					toast.add({
						severity: 'error',
						summary: t('error'),
						detail: t('acceptInviteView.inviteNotFound'),
						life: 5000
					})
					await router.push({ name: 'login' })
				} else {
					toast.add({
						severity: 'error',
						summary: t('error'),
						detail: t('acceptInviteView.errorFetchingInvite'),
						life: 5000
					})
					await router.push({ name: 'login' })
				}
			}
		})
})
</script>

<template>
	<div class="flex w-full h-full place-items-center justify-center gradient-bg">
		<div
			class="p-8 md:p-12 shadow-2xl border-surface-200 dark:border-surface-900 border rounded bg-white dark:bg-surface-900 w-[95vw] md:w-[60vw] lg:w-[50vw] xl:w-[40vw]"
		>
			<div class="flex items-center w-full" v-if="isLoading">
				<ProgressSpinner />
			</div>
			<div class="flex flex-col w-full" v-else>
				<div class="flex justify-center mb-6">
					<TimespaceLogoWithWordmark />
				</div>
				<h1 class="font-bold text-3xl text-tprimary">{{ $t('acceptInviteView.title') }}</h1>
				<p class="mb-6 text-tsecondary">{{ $t('acceptInviteView.subtitle') }}</p>
				<Form.Text
					id="email"
					:label="t('commonFieldLabels.email')"
					v-model="state.email"
					email
					:show-text-errors="submitted"
					:show-error="submitted"
					required
					disabled
				/>
				<div class="flex flex-col lg:flex-row lg:gap-2">
					<Form.Text
						:label="t('commonFieldLabels.firstName')"
						v-model="state.firstName"
						required
						:show-error="submitted"
						:show-text-errors="submitted"
						:disabled="isLoading"
					/>
					<Form.Text
						:label="t('commonFieldLabels.middleName')"
						v-model="state.middleName"
						:show-error="submitted"
						:show-text-errors="submitted"
						:disabled="isLoading"
					/>
					<Form.Text
						:label="t('commonFieldLabels.lastName')"
						v-model="state.lastName"
						:show-error="submitted"
						:show-text-errors="submitted"
						:disabled="isLoading"
					/>
				</div>
				<Form.Text
					:label="t('commonFieldLabels.password')"
					v-model="state.password"
					:show-error="submitted"
					:show-text-errors="submitted"
					type="password"
					required
					:disabled="isLoading"
				/>
				<div class="flex flex-col gap-4 items-center">
					<Button class="mt-6 w-full" @click="submit">
						<template #default>
							<span>{{ $t('acceptInviteView.continueButtonText') }}</span>
							<iconify-icon
								icon="heroicons:arrow-right"
								height="none"
								class="h-6 w-6 ml-2"
							></iconify-icon>
						</template>
					</Button>
					<div class="flex flex-col lg:flex-row items-center gap-1">
						<span class="text-tsecondary">{{ $t('acceptInviteView.loginText') }}</span>
						<RouterLink
							class="text-indigo-700 dark:text-indigo-300 font-semibold"
							:to="{ name: 'login' }"
							>{{ $t('signUpPage.loginText2') }}</RouterLink
						>
					</div>
				</div>
			</div>
		</div>
	</div>
</template>

<style scoped></style>
