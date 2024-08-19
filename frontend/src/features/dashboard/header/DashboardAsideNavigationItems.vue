<script setup lang="ts">
import { ref } from 'vue'
import DashboardAsideNavigationItem from './DashboardAsideNavigationItem.vue'
import type { MenuItem } from './types'
import { useI18n } from 'vue-i18n'
import { policies } from '@/infrastructure/authorization/permissions'

const { t } = useI18n()

const menuItems = ref<MenuItem[]>([
	{
		label: '',
		items: [
			{
				label: t('sidebarNavigation.dashboard'),
				to: 'dashboard',
				icon: 'heroicons:circle-stack'
			}
		]
	},
	{
		label: t('sidebarNavigation.employees'),
		items: [
			{
				label: t('sidebarNavigation.employees.clockHours'),
				to: 'employees-time',
				icon: 'heroicons:clock',
				permission: policies.getTimesheetEntriesEndpointPolicy
			}
		]
	},
	{
		label: t('sidebarNavigation.manager'),
		items: [
			{
				label: t('sidebarNavigation.manager.time'),
				icon: 'heroicons:clock',
				items: [
					{
						label: t('sidebarNavigation.manager.validateHours'),
						to: 'manager-validate-hours',
						icon: 'heroicons:check-circle',
						permission: policies.getApprovableTimesheetEntriesEndpointPolicy
					},
					{
						label: t('sidebarNavigation.manager.aggregatedTime'),
						to: 'manager-aggregated-time',
						icon: 'heroicons:chart-pie',
						permission: policies.getAggregatedTimesheetEntriesEndpointPolicy
					}
				]
			},
			{
				label: t('sidebarNavigation.manager.employees'),
				icon: 'heroicons:user-group',
				items: [
					{
						label: t('sidebarNavigation.manager.employees.list'),
						to: 'manager-employees-list',
						icon: 'heroicons:user-group',
						permission: policies.getMembersEndpointPolicy
					},
					{
						label: t('sidebarNavigation.manager.employees.invites'),
						to: 'manager-employees-invites',
						icon: 'heroicons:paper-airplane',
						permission: policies.getInvitesEndpointPolicy
					}
				]
			}
		]
	}
])
</script>

<template>
	<ul>
		<template v-for="(item, i) in menuItems" :key="item">
			<DashboardAsideNavigationItem v-if="!item.seperator" :item="item" :index="i" />
			<li v-if="item.seperator" class="menu-separator"></li>
		</template>
	</ul>
</template>

<style scoped></style>
