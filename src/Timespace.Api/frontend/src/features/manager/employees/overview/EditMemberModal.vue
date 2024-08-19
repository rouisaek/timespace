<script setup lang="ts">
import * as Form from '@/features/_shared/formfields'
import type { DynamicDialogInstance } from 'primevue/dynamicdialogoptions'
import Button from 'primevue/button'
import { inject, onMounted, reactive, ref, type Ref } from 'vue'
import { useI18n } from 'vue-i18n'
import useVuelidate from '@vuelidate/core'
import { apiClient } from '@/infrastructure/api'
import { useToastStore } from '@/infrastructure/stores/toastStore'
import { useQueryClient } from '@tanstack/vue-query'
import type { EditMemberModalData } from './EditMemberModal'

const { t } = useI18n()
const toast = useToastStore()
const dialogRef = inject<Ref<DynamicDialogInstance>>('dialogRef')
const v$ = useVuelidate()
const submitted = ref(false)
const loading = ref(false)
const queryClient = useQueryClient()

const state = reactive<{
	userId: string | null
	email: string | null
	employeeCode: string | null
	firstName: string | null
	middleName: string | null
	lastName: string | null
}>({
	userId: null,
	email: null,
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
		.put(`/tenant/members/${state.userId}`, {
			employeeCode: state.employeeCode,
			firstName: state.firstName,
			middleName: state.middleName,
			lastName: state.lastName
		})
		.then(async () => {
			loading.value = false
			await queryClient.invalidateQueries({ queryKey: ['tenant/members'] })
			toast.add({
				severity: 'success',
				summary: t('success'),
				detail: t('editMemberModal.memberUpdated'),
				life: 5000
			})
			dialogRef?.value.close()
		})
		.catch(() => {
			toast.add({
				severity: 'error',
				summary: t('error'),
				detail: t('editMemberModal.inviteNotSentError'),
				life: 5000
			})
			loading.value = false
		})
}

onMounted(() => {
	const data = dialogRef?.value.data.member as EditMemberModalData
	state.userId = data.userId
	state.email = data.email
	state.employeeCode = data.employeeCode
	state.firstName = data.firstName
	state.middleName = data.middleName
	state.lastName = data.lastName
})
</script>

<template>
	<Form.Text
		:label="t('commonFieldLabels.employeeEmail')"
		v-model="state.email"
		required
		email
		:show-error="submitted"
		:show-text-errors="submitted"
		disabled
	/>
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
		:label="$t('editMemberModal.submitLabel')"
		@click="submit"
		:loading="loading"
		class="mt-4 w-full"
	/>
</template>

<style scoped></style>
