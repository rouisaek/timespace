<script setup lang="ts">
import TimespaceLogoIcon from '@/features/shared/logos/TimespaceLogoIcon.vue';
import InputText from 'primevue/inputtext';
import IconField from 'primevue/iconfield';
import InputIcon from 'primevue/inputicon';
import UserAccountMenu from './UserAccountMenu.vue';
import { RouterLink } from 'vue-router';

</script>

<template>
	<header
		class="flex flex-row place-items-center justify-between p-2 lg:p-4 gap-2 lg:gap-8 border-b border-surface-300 dark:border-surface-700 dark:bg-surface-900">
		<RouterLink :to="{
			name: 'dashboard'
		}">
			<TimespaceLogoIcon class="min-w-10 h-10" />
		</RouterLink>

		<IconField class="w-full flex items-center">
			<InputIcon>
				<iconify-icon icon="heroicons:magnifying-glass" height="none" class="h-5 w-5 pb-1" />
			</InputIcon>
			<InputText :placeholder="$t('dashboardHeader.search')" class="w-full" size="large" />
		</IconField>

		<UserAccountMenu />
	</header>
</template>

<style scoped></style>
