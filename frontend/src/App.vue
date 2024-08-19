<script setup lang="ts">
import { usePrimeVue } from 'primevue/config'
import { watchEffect } from 'vue'
import { useI18n } from 'vue-i18n'
import { RouterView, useRoute } from 'vue-router'
import * as enLocale from '@/infrastructure/i18n/locales/primevue/en.json'
import * as nlLocale from '@/infrastructure/i18n/locales/primevue/en.json'
import DynamicDialog from 'primevue/dynamicdialog'
import ConfirmDialog from 'primevue/confirmdialog'
import Toast from 'primevue/toast'
import { useToastStore } from './infrastructure/stores/toastStore'
import { useToast } from 'primevue/usetoast'

const toastStore = useToastStore()
const toast = useToast()
const route = useRoute()

toastStore.$onAction(({ name, store, args }) => {
	if (name === 'add') {
		toast.add(args[0])
	}
})

const primeVue = usePrimeVue()
const i18n = useI18n()

watchEffect(() => {
	switch (i18n.locale.value) {
		case 'en':
			primeVue.config.locale = enLocale
			break
		case 'nl':
			primeVue.config.locale = nlLocale
			break
		default:
			primeVue.config.locale = enLocale
			break
	}
})
</script>

<template>
	<DynamicDialog />
	<ConfirmDialog />
	<Toast />
	<RouterView :key="route.fullPath" />
</template>

<style scoped></style>
