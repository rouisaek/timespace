<script setup lang="ts">
import ContainerCard from '@/features/_shared/components/ContainerCard.vue'
import Column from 'primevue/column'
import DataTable from 'primevue/datatable'
import { useTenantMembersQuery } from './useTenantMembersQuery'
import ProgressSpinner from 'primevue/progressspinner'
import { computed } from 'vue'
import Button from 'primevue/button'
import { useDialog } from 'primevue/usedialog'
import RequirePolicy from '@/infrastructure/authorization/RequirePolicy.vue'
import { policies } from '@/infrastructure/authorization/permissions'
import EditMemberModal from './EditMemberModal.vue'
import { useI18n } from 'vue-i18n'
import { useUserinfoQuery } from '@/features/users/accounts/queries/useUserInfoQuery'

const { data, isLoading } = useTenantMembersQuery()
const { data: userInfo } = useUserinfoQuery()
const dialog = useDialog()
const { t } = useI18n()

const formattedData = computed(() => {
	if (!data.value) {
		return []
	}

	return data.value.map((member) => {
		return {
			userId: member.userId,
			employeeCode: member.employeeCode ?? '-',
			fullName: [member.firstName, member.middleName, member.lastName].filter(Boolean).join(' '),
			email: member.email
		}
	})
})

const editEmployee = (userId: number) => {
	const tenantMember = data.value?.find((member) => member.userId === userId)

	dialog.open(EditMemberModal, {
		props: {
			header: t('employeesListView.editModalTitle'),
			modal: true
		},
		data: {
			member: tenantMember
		}
	})
}
</script>

<template>
	<div class="flex flex-col gap-4">
		<ContainerCard class="flex justify-between items-center px-4">
			<h1 class="text-xl text-tprimary whitespace-nowrap">{{ $t('employeesListView.title') }}</h1>
		</ContainerCard>
		<ContainerCard v-if="isLoading" class="flex place-items-center justify-center py-10">
			<ProgressSpinner />
		</ContainerCard>
		<ContainerCard v-else>
			<DataTable :value="formattedData">
				<Column
					field="employeeCode"
					:header="$t('employeesListView.table.employeeCodeHeader')"
					class="max-w-fit"
				></Column>
				<Column field="fullName" :header="$t('employeesListView.table.fullNameHeader')"></Column>
				<Column field="email" :header="$t('employeesListView.table.emailHeader')"></Column>
				<Column :header="$t('employeesListView.table.actionsHeader')" class="max-w-fit">
					<template #body="slotProps">
						<div class="flex gap-2">
							<RequirePolicy :policy="policies.updateMemberEndpointPolicy">
								<Button
									@click="editEmployee(slotProps.data.userId)"
									:label="$t('employeesListView.table.editButton')"
								/>
							</RequirePolicy>
							<RequirePolicy :policy="policies.disableMemberEndpointPolicy">
								<Button severity="danger" outlined v-if="slotProps.data.email !== userInfo?.email">
									<iconify-icon icon="heroicons:trash" height="none" class="w-[1.125rem]" />
								</Button>
							</RequirePolicy>
						</div>
					</template>
				</Column>
			</DataTable>
		</ContainerCard>
	</div>
</template>

<style scoped></style>
