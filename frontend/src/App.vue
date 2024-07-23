<script setup lang="ts">
import { usePrimeVue } from 'primevue/config';
import { watchEffect } from 'vue';
import { useI18n } from 'vue-i18n';
import { RouterView } from 'vue-router'
import * as enLocale from '@/infrastructure/i18n/locales/primevue/en.json';
import * as nlLocale from '@/infrastructure/i18n/locales/primevue/en.json';
import DynamicDialog from 'primevue/dynamicdialog';
import Toast from 'primevue/toast';
import { useToastStore } from './infrastructure/stores/toastStore';

useToastStore();

const primeVue = usePrimeVue();
const i18n = useI18n();

watchEffect(() => {
	switch (i18n.locale.value) {
		case 'en':
			primeVue.config.locale = enLocale;
			break;
		case 'nl':
			primeVue.config.locale = nlLocale;
			break;
		default:
			primeVue.config.locale = enLocale;
			break;
	}
})
</script>

<template>
	<DynamicDialog />
	<Toast />
	<RouterView />
</template>

<style scoped></style>
