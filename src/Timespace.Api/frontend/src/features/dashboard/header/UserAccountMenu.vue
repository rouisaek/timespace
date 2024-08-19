<script setup lang="ts">
import { useUserinfoQuery } from '@/features/users/accounts/queries/useUserInfoQuery'
import { join } from 'lodash-es'
import { ref } from 'vue'
import Menu from 'primevue/menu'
import { useI18n } from 'vue-i18n'
import type { MenuItem } from 'primevue/menuitem'
import { apiClient } from '@/infrastructure/api'
import { useToastStore } from '@/infrastructure/stores/toastStore'
import { useRouter } from 'vue-router'
import { useQueryClient } from '@tanstack/vue-query'

const { data: userInfo } = useUserinfoQuery()
const { t } = useI18n()
const router = useRouter()
const menu = ref()
const toast = useToastStore()
const queryClient = useQueryClient()

const toggle = (event: any) => {
	menu.value.toggle(event)
}

const items = ref<MenuItem[]>([
	{
		label: t('userAccountMenu.profileTitle'),
		items: [
			{
				label: t('userAccountMenu.settingsTitle'),
				icon: 'heroicons:cog-6-tooth'
			},
			{
				label: t('userAccountMenu.logoutTitle'),
				icon: 'heroicons:arrow-right-end-on-rectangle',
				command: () => {
					apiClient
						.post('/accounts/logout')
						.then(async () => {
							queryClient.invalidateQueries({ queryKey: ['/accounts/me'] })
							await router.push({ name: 'login' })
							toast.add({
								severity: 'success',
								summary: t('success'),
								detail: t('userAccountMenu.logoutSuccessMessage')
							})
						})
						.catch(() => {
							toast.add({
								severity: 'error',
								summary: t('error'),
								detail: t('userAccountMenu.logoutErrorMessage')
							})
						})
				}
			}
		]
	}
])
</script>

<template>
	<div
		class="flex flex-row place-items-center justify-between gap-3 cursor-pointer"
		@click="toggle"
	>
		<div
			class="h-11 w-11 bg-indigo-100 dark:bg-indigo-900 border border-indigo-600 dark:border-indigo-400 rounded flex place-items-center justify-center"
		>
			<span class="font-bold text-indigo-900 dark:text-indigo-100 text-xl">{{
				userInfo?.firstName.charAt(0)
			}}</span>
		</div>
		<div class="flex-col items-start hidden lg:flex">
			<span class="font-bold text-tprimary">{{
				join([userInfo?.firstName, userInfo?.middleName, userInfo?.lastName], ' ')
			}}</span>
			<span class="text-tsecondary">{{ userInfo?.email }}</span>
		</div>
		<iconify-icon icon="heroicons:chevron-down" height="none" class="h-6 w-6 text-tprimary" />
	</div>
	<Menu :model="items" class="w-full md:w-60" ref="menu" popup>
		<template #submenulabel="{ item }">
			<span class="font-bold">{{ item.label }}</span>
		</template>
		<template #item="{ item, props }">
			<a class="flex items-center gap-1" v-bind="props.action">
				<iconify-icon :icon="item.icon" height="none" class="h-6 w-6" />
				<span>{{ item.label }}</span>
			</a>
		</template>
	</Menu>
</template>

<style scoped></style>
