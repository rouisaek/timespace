<script setup lang="ts">
import DashboardAsideNavigationItem from './DashboardAsideNavigationItem.vue';


</script>

<template>
	<div>
		<span class="font-semibold text-sm text-slate-700 dark:text-slate-300">{{
			$t('dashboardAsideNavigation.employeesTitle').toUpperCase() }}</span>
		<DashboardAsideNavigationItem :title="$t('employeesNavigationItem.hours')" icon="heroicons:clock"
			to="dashboard" />
	</div>
</template>

<style scoped></style>
