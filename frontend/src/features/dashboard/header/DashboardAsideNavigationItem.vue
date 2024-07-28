<script setup lang="ts">
import { RouterLink } from 'vue-router';

const props = defineProps<{
	title: string;
	icon: string;
	to: string;
}>();
</script>

<template>
	<RouterLink :to="{
		name: props.to
	}">
		<div class="px-2 py-3 hover:bg-indigo-100 dark:hover:bg-slate-800 rounded">
			<div class="flex flex-row items-center space-x-5">
				<iconify-icon :icon="props.icon" height="none" class="h-6 w-6"></iconify-icon>
				<span class="font-lg">{{ props.title }}</span>
			</div>
		</div>
	</RouterLink>
</template>

<style scoped></style>
