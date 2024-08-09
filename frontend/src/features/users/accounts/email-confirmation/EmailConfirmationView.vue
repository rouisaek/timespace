<script setup lang="ts">
import TimespaceLogoWithWordmark from '@/features/_shared/components/logos/TimespaceLogoWithWordmark.vue'
import { apiClient } from '@/infrastructure/api'
import { onMounted, ref } from 'vue'
import { RouterLink, useRoute } from 'vue-router'
import ProgressSpinner from 'primevue/progressspinner'
import Message from 'primevue/message'

const loading = ref(true)
const success = ref(false)
const error = ref(false)
const route = useRoute()

onMounted(() => {
	const token = route.query.token as string
	const email = route.query.email as string

	if (!token || !email) {
		loading.value = false
		error.value = true
		return
	}

	apiClient
		.post('/accounts/email-confirmation', {
			email,
			code: token
		})
		.then(() => {
			success.value = true
			loading.value = false
		})
		.catch(() => {
			error.value = true
			loading.value = false
		})
})
</script>

<template>
	<div class="flex w-full h-full place-items-center justify-center gradient-bg">
		<div
			class="p-12 shadow-2xl border-gray-200 dark:border-gray-900 border rounded bg-white dark:bg-slate-800 min-w-[50%] md:min-w-[30%]"
		>
			<div class="flex justify-center mb-6">
				<TimespaceLogoWithWordmark />
			</div>
			<h1 class="font-bold text-3xl mb-6">{{ $t('confirmEmailPage.title') }}</h1>
			<ProgressSpinner v-if="loading" class="h-20 w-20" />
			<template v-if="success">
				<Message severity="success" class="mb-4"
					>{{ $t('confirmEmailPage.successMessage') }}
				</Message>
				<RouterLink
					class="text-indigo-700 dark:text-indigo-300 font-semibold"
					:to="{ name: 'login' }"
				>
					{{ $t('confirmEmailPage.loginText') }}
				</RouterLink>
			</template>
			<Message v-if="error" severity="error" class="mb-4">{{
				$t('confirmEmailPage.errorMessage')
			}}</Message>
		</div>
	</div>
</template>

<style scoped></style>
