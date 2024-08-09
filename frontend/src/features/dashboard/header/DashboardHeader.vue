<script setup lang="ts">
import TimespaceLogoIcon from '@/features/_shared/components/logos/TimespaceLogoIcon.vue';
import InputText from 'primevue/inputtext';
import IconField from 'primevue/iconfield';
import InputIcon from 'primevue/inputicon';
import UserAccountMenu from './UserAccountMenu.vue';
import { RouterLink, useRoute } from 'vue-router';
import Drawer from 'primevue/drawer';
import DashboardAsideNavigationItems from './DashboardAsideNavigationItems.vue';
import { ref, watch } from 'vue';

const drawerOpen = ref(false);

const route = useRoute();

watch(
	() => route.fullPath,
	async () => {
		drawerOpen.value = false;
	}
);

</script>

<template>
	<header
		class="flex flex-row place-items-center justify-between p-2 lg:p-4 gap-2 lg:gap-8 border-b border-surface-300 dark:border-surface-700 dark:bg-surface-900">
		<RouterLink :to="{
			name: 'dashboard'
		}" class="hidden lg:block">
			<TimespaceLogoIcon class="min-w-10 h-10" />
		</RouterLink>
		<div class="flex items-center lg:hidden rounded dark:hover:bg-slate-700 hover:bg-slate-100 cursor-pointer">
			<iconify-icon icon="heroicons:bars-3" height="none" class="h-8 w-8 text-tprimary "
				@click="drawerOpen = true" />

		</div>

		<Drawer v-model:visible="drawerOpen" :show-close-icon="false">
			<DashboardAsideNavigationItems />
		</Drawer>

		<IconField class="w-full flex items-center">
			<InputIcon>
				<iconify-icon icon="heroicons:magnifying-glass" height="none" class="h-5 w-5 mb-2" />
			</InputIcon>
			<InputText :placeholder="$t('dashboardHeader.search')" class="w-full" size="large" />
		</IconField>

		<UserAccountMenu />
	</header>
</template>

<style scoped></style>
