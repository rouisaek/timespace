<script setup lang="ts">
import * as Form from "@/features/shared/formfields/formInputs";
import useVuelidate from "@vuelidate/core";
import Button from "primevue/button";
import type { DynamicDialogInstance } from 'primevue/dynamicdialogoptions'

import { inject, onMounted, reactive, ref, type Ref } from "vue";

const dialogRef = inject<Ref<DynamicDialogInstance>>("dialogRef");

const state = reactive({
	email: null,
});

const submitted = ref(false);
const v$ = useVuelidate();

function submit() {
	submitted.value = true;
	if (v$.value.$invalid) return;
	console.log("submitting", state);
}

onMounted(() => {
	state.email = dialogRef?.value.data.email;
});
</script>

<template>
	<Form.Text id="email" :label="$t('commonFieldLabels.email')" v-model="state.email" email size="large"
		:show-text-errors="submitted" :show-error="submitted" required autofocus />
	<Button :label="$t('forgotPasswordModal.submitButtonText')" icon="pi pi-arrow-right" icon-pos="right"
		class="mt-4 w-full" @click="submit" />
</template>
