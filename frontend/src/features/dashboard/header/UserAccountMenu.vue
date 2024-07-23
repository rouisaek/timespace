<script setup lang="ts">
import { useUserinfoQuery } from '@/features/users/accounts/queries/meQuery';
import { join } from 'lodash-es';

const { data: userInfo } = useUserinfoQuery();
</script>

<template>
	<div>
		<div class="flex flex-row place-items-center justify-between gap-3">
			<div
				class="h-11 w-11 bg-indigo-100 dark:bg-indigo-900 border border-indigo-600 dark:border-indigo-400 rounded flex place-items-center justify-center">
				<span class="font-bold text-indigo-900 dark:text-indigo-100 text-xl">{{ userInfo?.firstName.charAt(0)
					}}</span>
			</div>
			<div class="flex-col items-start hidden lg:flex">
				<span class="font-bold">{{ join([userInfo?.firstName, userInfo?.middleName, userInfo?.lastName], " ")
					}}</span>
				<span class="text-neutral-600 dark:text-neutral-400">{{ userInfo?.email }}</span>
			</div>
			<i class="pi pi-chevron-down hidden lg:block"></i>
		</div>
	</div>
</template>

<style scoped></style>
