<script setup lang="ts">
import { ref, onBeforeMount, watch, unref, computed } from 'vue'
import { useSidebar } from './composables/sidebar'
import { useRoute } from 'vue-router'
import type { MenuItem } from './types'
import { usePermissionStore } from '@/infrastructure/authorization/permissionStore'

const route = useRoute()
const { layoutState, setActiveMenuItem } = useSidebar()
const { hasPermission } = usePermissionStore()

const props = withDefaults(
	defineProps<{
		item: MenuItem | null
		index?: number
		root?: boolean
		parentItemKey?: string | null
	}>(),
	{
		item: null,
		index: 0,
		root: true,
		parentItemKey: null
	}
)

const isActiveMenu = ref(false)
const itemKey = ref<string | null>(null)

onBeforeMount(() => {
	itemKey.value = props.parentItemKey
		? props.parentItemKey + '-' + props.index
		: String(props.index)

	const activeItem = layoutState.activeMenuItem

	isActiveMenu.value =
		activeItem === itemKey.value || activeItem ? activeItem.startsWith(itemKey.value + '-') : false
})

watch(
	() => layoutState.activeMenuItem,
	(newVal) => {
		isActiveMenu.value =
			newVal === itemKey.value || (newVal?.startsWith(itemKey.value + '-') ?? false)
	}
)

const itemClick = (event: any, item: MenuItem) => {
	// if (item.disabled) {
	// 	event.preventDefault();
	// 	return;
	// }

	if (!item.to) {
		const foundItemKey = item.items
			? isActiveMenu.value
				? props.parentItemKey
				: itemKey
			: itemKey.value

		setActiveMenuItem(unref(foundItemKey) ?? '')
	}
}

const checkActiveRoute = (item: MenuItem) => {
	return route.name === item.to
}

const menuItemStyle =
	'text-tprimary cursor-pointer hover:bg-slate-100 dark:hover:bg-slate-700 flex flex-row gap-2 items-center p-2 rounded'

const allowedMenuItem = computed(() => {
	const collectPermissions = (item: MenuItem | null) => {
		let permissions: string[] = []

		if (!item) {
			return permissions
		}

		// Add current item's permission
		if (item?.permission) {
			permissions.push(item.permission)
		}

		// Recursively collect permissions from child items
		if (item?.items) {
			item.items.forEach((childItem) => {
				permissions = permissions.concat(collectPermissions(childItem))
			})
		}

		return permissions
	}

	// Get all permissions from the current item and its children
	const currentItemAndChildPermissions = collectPermissions(props.item)

	if (currentItemAndChildPermissions.length === 0) {
		return true
	}

	// Check if any of these permissions are allowed
	return currentItemAndChildPermissions.some((permission) => hasPermission(permission))
})
</script>

<template>
	<li
		:class="{ 'layout-root-menuitem': root, 'active-menuitem': isActiveMenu }"
		v-if="item && allowedMenuItem"
	>
		<div
			v-if="root && item.visible !== false && item.label.length > 0"
			class="text-tsecondary font-bold text-sm mt-4"
		>
			{{ item.label.toUpperCase() }}
		</div>
		<a
			v-if="(!item.to || item.items) && item.visible !== false && !root"
			@click="itemClick($event, item)"
			:class="[item.class, menuItemStyle]"
			tabindex="0"
		>
			<iconify-icon v-if="item.icon" :icon="item.icon" height="none" class="h-6 w-6" />
			<span class="flex-grow">{{ item.label }}</span>
			<iconify-icon icon="heroicons:chevron-right" v-if="item.items && !isActiveMenu" />
			<iconify-icon icon="heroicons:chevron-down" v-if="item.items && isActiveMenu" />
		</a>
		<router-link
			v-if="item.to && !item.items && item.visible !== false"
			@click="itemClick($event, item)"
			:class="[
				item.class,
				{ 'bg-indigo-50 dark:bg-slate-800': checkActiveRoute(item) },
				menuItemStyle
			]"
			tabindex="0"
			:to="{ name: item.to }"
		>
			<iconify-icon v-if="item.icon" :icon="item.icon" height="none" class="h-6 w-6" />
			<span class="flex-grow">{{ item.label }}</span>
			<iconify-icon icon="heroicons:chevron-right" v-if="item.items && !isActiveMenu" />
			<iconify-icon icon="heroicons:chevron-down" v-if="item.items && isActiveMenu" />
		</router-link>
		<Transition v-if="item.items && item.visible !== false" name="layout-submenu">
			<ul v-show="root ? true : isActiveMenu" :class="{ 'ml-2': item.label.length > 0 }">
				<DashboardAsideNavigationItem
					v-for="(child, i) in item.items"
					:key="child.label"
					:index="i"
					:item="child"
					:parentItemKey="itemKey"
					:root="false"
				/>
			</ul>
		</Transition>
	</li>
</template>

<style scoped>
.layout-submenu-enter-from,
.layout-submenu-leave-to {
	max-height: 0;
}

.layout-submenu-enter-to,
.layout-submenu-leave-from {
	max-height: 1000px;
}

.layout-submenu-leave-active {
	overflow: hidden;
	transition: max-height 0.45s cubic-bezier(0, 1, 0, 1);
}

.layout-submenu-enter-active {
	overflow: hidden;
	transition: max-height 1s ease-in-out;
}
</style>
