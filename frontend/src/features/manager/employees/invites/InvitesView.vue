<script setup lang="ts">
import ContainerCard from '@/features/_shared/components/ContainerCard.vue'
import Button from 'primevue/button'
import Column from 'primevue/column'
import DataTable from 'primevue/datatable'
import ProgressSpinner from 'primevue/progressspinner'
import { useTenantInvitesQuery } from './useTenantInvitesQuery'
import { useDialog } from 'primevue/usedialog'
import AddInviteModal from './AddInviteModal.vue'
import { useI18n } from 'vue-i18n'
import { useRouter } from 'vue-router'
import { useToastStore } from '@/infrastructure/stores/toastStore'
import { apiClient } from '@/infrastructure/api'
import { useConfirm } from 'primevue/useconfirm'
import { useQueryClient } from '@tanstack/vue-query'

const { t, locale, getDateTimeFormat } = useI18n()
const { data, isLoading } = useTenantInvitesQuery()
const dialog = useDialog()
const router = useRouter()
const toast = useToastStore()
const confirm = useConfirm()
const queryClient = useQueryClient()

const openAddInviteModal = () => {
	dialog.open(AddInviteModal, {
		props: {
			header: t('invitesView.addModalTitle'),
			modal: true
		}
	})
}

const copyLink = (inviteToken: string) => {
	const inviteUrl = router.resolve({
		name: 'accounts-invite',
		params: { inviteToken }
	}).href

	const a = document.createElement('a')
	a.href = inviteUrl

	if (navigator.share && navigator.canShare({ text: inviteToken })) {
		navigator.share({ url: a.href })
	} else {
		navigator.clipboard.writeText(a.href)
		toast.add({
			severity: 'success',
			summary: t('success'),
			detail: t('invitesView.linkCopied'),
			life: 5000
		})
	}
}

const deleteInvite = (inviteId: string) => {
	confirm.require({
		message: t('deleteConfirmationMessage'),
		header: t('confirmation'),
		rejectProps: {
			label: t('cancel'),
			severity: 'secondary',
			outlined: true
		},
		acceptProps: {
			label: t('delete'),
			severity: 'danger'
		},
		accept: () => {
			apiClient
				.delete(`/tenant/members/invites/${inviteId}`)
				.then(async () => {
					await queryClient.invalidateQueries({ queryKey: ['tenant/members/invites'] })
					toast.add({
						severity: 'success',
						summary: t('success'),
						detail: t('invitesView.inviteDeleted'),
						life: 5000
					})
				})
				.catch(() => {
					toast.add({
						severity: 'error',
						summary: t('error'),
						detail: t('invitesView.inviteNotDeletedError'),
						life: 5000
					})
				})
		}
	})
}
</script>

<template>
	<div class="flex flex-col gap-4">
		<ContainerCard class="flex justify-between items-center px-4">
			<h1 class="text-xl text-tprimary whitespace-nowrap">{{ $t('invitesView.title') }}</h1>
			<Button @click="openAddInviteModal" text>
				<div class="flex items-center gap-2">
					<span>{{ $t('invitesView.add') }}</span>
					<iconify-icon icon="heroicons:plus" height="none" class="w-[1.125rem]" />
				</div>
			</Button>
		</ContainerCard>
		<ContainerCard v-if="isLoading" class="flex place-items-center justify-center py-10">
			<ProgressSpinner />
		</ContainerCard>
		<ContainerCard
			v-else-if="data?.length === 0"
			class="flex place-items-center justify-center py-10"
		>
			<span class="text-tsecondary">{{ $t('invitesView.noInvites') }}</span>
		</ContainerCard>
		<ContainerCard v-else class="max-w-full">
			<DataTable :value="data" scrollable scroll-height="flex">
				<Column field="email" :header="$t('invitesView.table.emailHeader')"></Column>
				<Column field="expiresAt" :header="$t('invitesView.table.expiresAtHeader')">
					<template #body="slotProps">
						<span class="whitespace-nowrap">{{
							slotProps.data.expiresAt.toLocaleString(locale, getDateTimeFormat(locale)['long'])
						}}</span>
					</template>
				</Column>
				<Column :header="$t('invitesView.table.actionsHeader')">
					<template #body="slotProps">
						<div class="flex gap-2">
							<Button @click="copyLink(slotProps.data.token)">
								<iconify-icon icon="heroicons:link" height="none" class="w-[1.125rem] mr-2" />
								<span class="whitespace-nowrap">{{ $t('invitesView.table.copyLinkButton') }}</span>
							</Button>
							<Button @click="deleteInvite(slotProps.data.inviteId)" severity="danger" outlined>
								<iconify-icon icon="heroicons:trash" height="none" class="w-[1.125rem]" />
							</Button>
						</div>
					</template>
				</Column>
			</DataTable>
		</ContainerCard>
	</div>
</template>

<style scoped></style>
