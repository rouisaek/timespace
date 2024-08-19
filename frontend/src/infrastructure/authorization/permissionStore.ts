import { defineStore } from 'pinia'
import { ref } from 'vue'

export const usePermissionStore = defineStore('permissions', () => {
	const permissions = ref<string[]>([])

	const setPermissions = (newPermissions: string[]) => {
		permissions.value = newPermissions
	}

	const hasPermission = (permission?: string) => {
		if (!permission) {
			return true
		}
		return permissions.value.includes(permission) || permissions.value.includes('timespace:admin')
	}

	return {
		permissions,
		setPermissions,
		hasPermission
	}
})
