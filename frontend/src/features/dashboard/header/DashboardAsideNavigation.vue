<script setup lang="ts">
import { useUserinfoQuery } from '@/features/users/accounts/queries/meQuery';

const { data: userInfo } = useUserinfoQuery();
</script>

<template>
	<aside class="h-full w-fit">
		<div>
			<div class="h-12 w-12 bg-gray-500">
			</div>
			<span>Bedrijfsnaam</span>
		</div>
	</aside>
</template>

<style scoped></style>
