<script setup lang="ts">
import { usePermissionStore } from './permissionStore'

const permissionStore = usePermissionStore()

defineProps<{
	policy: string
}>()
</script>

<template>
	<slot v-if="permissionStore.hasPermission(policy)"></slot>
</template>

<style scoped></style>
