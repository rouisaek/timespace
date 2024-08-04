<script setup lang="ts">
import { useDialog } from 'primevue/usedialog';
import ClockHoursModal from '../employees/time/ClockHoursModal.vue';
import { useI18n } from 'vue-i18n';
import Button from 'primevue/button';

const dialogService = useDialog();
const { t } = useI18n();

const openClockHoursModal = () => {
	dialogService.open(ClockHoursModal, {
		props: {
			header: t('clockHoursModal.title'),
			style: {
				width: '50vw',
			},
			breakpoints: {
				'960px': '75vw',
				'640px': '90vw'
			},
			modal: true
		}
	});
}
</script>

<template>
	<div>
		<Button label="Uren klokken" @click="openClockHoursModal" />
	</div>
</template>

<style scoped></style>
