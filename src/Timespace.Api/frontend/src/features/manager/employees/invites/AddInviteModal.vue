<script setup lang="ts">
import * as Form from '@/features/_shared/formfields'
import type { DynamicDialogInstance } from 'primevue/dynamicdialogoptions'
import Button from 'primevue/button'
import { inject, reactive, ref, type Ref } from 'vue'
import { useI18n } from 'vue-i18n'
import useVuelidate from '@vuelidate/core'
import Divider from 'primevue/divider'
import { apiClient } from '@/infrastructure/api'
import { useToastStore } from '@/infrastructure/stores/toastStore'
import { useQueryClient } from '@tanstack/vue-query'

const { t } = useI18n()
const toast = useToastStore()
const dialogRef = inject<Ref<DynamicDialogInstance>>('dialogRef')
const v$ = useVuelidate()
const submitted = ref(false)
const loading = ref(false)
const queryClient = useQueryClient()

const state = reactive<{
	email: string | null
	expiresAt: Temporal.PlainDate
	employeeCode: string | null
	firstName: string | null
	middleName: string | null
	lastName: string | null
}>({
	email: null,
	expiresAt: Temporal.Now.plainDateISO().add({ days: 7 }),
	employeeCode: null,
	firstName: null,
	middleName: null,
	lastName: null
})

const submit = () => {
	submitted.value = true

	if (v$.value.$invalid) return

	loading.value = true
	apiClient
		.post('/tenant/members/invites', {
			email: state.email,
			expiresAt: state.expiresAt.toZonedDateTime(Temporal.Now.timeZoneId()).toInstant(),
			employeeCode: state.employeeCode,
			firstName: state.firstName,
			middleName: state.middleName,
			lastName: state.lastName
		})
		.then(async () => {
			loading.value = false
			await queryClient.invalidateQueries({ queryKey: ['tenant/members/invites'] })
			toast.add({
				severity: 'success',
				summary: t('success'),
				detail: t('addInviteModal.inviteSent'),
				life: 5000
			})
			dialogRef?.value.close()
		})
		.catch(() => {
			toast.add({
				severity: 'error',
				summary: t('error'),
				detail: t('addInviteModal.inviteNotSentError'),
				life: 5000
			})
			loading.value = false
		})
}
</script>

<template>
	<Form.Text
		:label="t('addInviteModal.emailLabel')"
		v-model="state.email"
		required
		email
		:show-error="submitted"
		:show-text-errors="submitted"
		autofocus
	/>
	<Form.DatePicker
		label="Expires At"
		v-model="state.expiresAt"
		required
		:show-error="submitted"
		:show-text-errors="submitted"
	/>
	<Divider>
		<span class="text-tsecondary bg-surface-0 dark:bg-surface-900 px-2">{{
			$t('addInviteModal.optionalProperties')
		}}</span>
	</Divider>
	<div>
		<Form.Text
			:label="t('addInviteModal.employeeCodeLabel')"
			v-model="state.employeeCode"
			:show-error="submitted"
			:show-text-errors="submitted"
		/>
		<div class="flex flex-col lg:flex-row lg:gap-2">
			<Form.Text
				:label="t('addInviteModal.firstNameLabel')"
				v-model="state.firstName"
				:show-error="submitted"
				:show-text-errors="submitted"
			/>
			<Form.Text
				:label="t('addInviteModal.middleNameLabel')"
				v-model="state.middleName"
				:show-error="submitted"
				:show-text-errors="submitted"
			/>
			<Form.Text
				:label="t('addInviteModal.lastNameLabel')"
				v-model="state.lastName"
				:show-error="submitted"
				:show-text-errors="submitted"
			/>
		</div>
	</div>
	<Button
		type="submit"
		:label="$t('addInviteModal.submitLabel')"
		@click="submit"
		:loading="loading"
		class="mt-4 w-full"
	/>
</template>

<style scoped></style>
