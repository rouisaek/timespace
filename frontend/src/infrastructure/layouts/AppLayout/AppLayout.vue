<script setup lang="ts">
import DashboardAsideNavigation from '@/features/dashboard/header/DashboardAsideNavigation.vue';
import DashboardHeader from '@/features/dashboard/header/DashboardHeader.vue';

</script>

<template>
	<div class="h-dvh w-dvw flex flex-col">
		<DashboardHeader />
		<div class="flex flex-row">
			<DashboardAsideNavigation />
			<RouterView />
		</div>
	</div>
</template>

<style scoped></style>
