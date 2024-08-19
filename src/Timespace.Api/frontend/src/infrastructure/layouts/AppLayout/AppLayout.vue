<script setup lang="ts">
import DashboardAsideNavigation from '@/features/dashboard/header/DashboardAsideNavigation.vue'
import DashboardHeader from '@/features/dashboard/header/DashboardHeader.vue'
</script>

<template>
	<div class="h-dvh w-dvw flex flex-col">
		<DashboardHeader />
		<div class="flex flex-row h-full">
			<DashboardAsideNavigation />
			<div class="flex-grow bg-surface-50 dark:bg-surface-950 p-4 lg:p-8 max-w-full">
				<RouterView />
			</div>
		</div>
	</div>
</template>

<style scoped></style>
