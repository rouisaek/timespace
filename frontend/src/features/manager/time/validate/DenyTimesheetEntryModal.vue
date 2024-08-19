<script setup lang="ts">
import { type DynamicDialogInstance } from 'primevue/dynamicdialogoptions'
import { inject, reactive, ref, type Ref } from 'vue'
import * as Form from '@/features/_shared/formfields'
import { apiClient } from '@/infrastructure/api'
import Button from 'primevue/button'
import { useToastStore } from '@/infrastructure/stores/toastStore'
import { useI18n } from 'vue-i18n'
import { useQueryClient } from '@tanstack/vue-query'
const queryClient = useQueryClient()

const dialogRef = inject<Ref<DynamicDialogInstance | null>>('dialogRef')
const loading = ref(false)
const toast = useToastStore()
const { t } = useI18n()

const state = reactive({
	denialReason: null
})

const deny = () => {
	const timesheetEntryId = dialogRef?.value?.data.timesheetEntryId

	loading.value = true
	apiClient
		.post('timesheet/deny', {
			timesheetEntryId,
			denialReason: state.denialReason
		})
		.then(async () => {
			await queryClient.invalidateQueries({ queryKey: ['approvable-entries'] })
			loading.value = false
			toast.add({
				severity: 'success',
				summary: t('success'),
				detail: t('denyTimesheetEntryModal.successDetail'),
				life: 2000
			})
			dialogRef?.value?.close()
		})
		.catch(() => {
			loading.value = false
		})
}
</script>

<template>
	<div>
		<Form.Textarea
			:label="$t('denyTimesheetEntryModal.reasonLabel')"
			v-model="state.denialReason"
			autofocus
		/>
		<Button
			severity="danger"
			:label="$t('denyTimesheetEntryModal.denyButton')"
			@click="deny"
			:loading="loading"
			class="w-full mt-4"
		/>
	</div>
</template>

<style scoped></style>
