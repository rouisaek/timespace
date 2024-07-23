<script setup lang="ts">
import TimespaceLogoIcon from '@/features/shared/logos/TimespaceLogoIcon.vue';
import InputText from 'primevue/inputtext';
import IconField from 'primevue/iconfield';
import InputIcon from 'primevue/inputicon';
import UserAccountMenu from './UserAccountMenu.vue';

</script>

<template>
	<header
		class="flex flex-row place-items-center justify-between p-2 lg:p-4 gap-2 lg:gap-8 border-b border-neutral-300 dark:border-neutral-800">
		<TimespaceLogoIcon class="min-w-10 h-10" />

		<IconField class="w-full">
			<InputIcon class="pi pi-search" />
			<InputText :placeholder="$t('dashboardHeader.search')" class="w-full" />
		</IconField>

		<UserAccountMenu />
	</header>
</template>

<style scoped></style>
