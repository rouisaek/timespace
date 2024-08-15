<script setup lang="ts">
import * as Form from '@/features/_shared/formfields'
import { apiClient } from '@/infrastructure/api'
import useVuelidate from '@vuelidate/core'
import Button from 'primevue/button'
import type { DynamicDialogInstance } from 'primevue/dynamicdialogoptions'
import { useToast } from 'primevue/usetoast'
import { inject, onMounted, reactive, ref, type Ref } from 'vue'
import { useI18n } from 'vue-i18n'

const dialogRef = inject<Ref<DynamicDialogInstance>>('dialogRef')
const toast = useToast()
const { t } = useI18n()

const state = reactive({
	email: null
})

const submitted = ref(false)
const loading = ref(false)
const v$ = useVuelidate()

function submit() {
	submitted.value = true
	if (v$.value.$invalid) return

	loading.value = true

	apiClient
		.post('/accounts/password-reset/request', state)
		.then(() => {
			loading.value = false
			toast.add({
				severity: 'success',
				summary: t('success'),
				detail: t('forgotPasswordModal.emailSent'),
				life: 50000
			})
			dialogRef?.value.close()
		})
		.catch(() => {
			loading.value = false
			toast.add({
				severity: 'error',
				summary: t('error'),
				detail: t('forgotPasswordModal.emailNotSentError'),
				life: 5000
			})
		})
}

onMounted(() => {
	state.email = dialogRef?.value.data.email
})
</script>

<template>
	<Form.Text
		id="email"
		:label="$t('commonFieldLabels.email')"
		v-model="state.email"
		email
		size="large"
		:show-text-errors="submitted"
		:show-error="submitted"
		required
		autofocus
	/>
	<Button
		:label="$t('forgotPasswordModal.submitButtonText')"
		icon="pi pi-arrow-right"
		icon-pos="right"
		class="mt-4 w-full"
		@click="submit"
		:loading="loading"
	/>
</template>
