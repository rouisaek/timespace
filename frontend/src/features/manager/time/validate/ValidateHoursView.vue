<script setup lang="ts">
import ApprovableTimesheetEntryCard from './ApprovableTimesheetEntryCard.vue'
import { useGetApprovableTimesheetEntriesQuery } from './queries/useGetApprovableTimesheetEntriesQuery'

const { data, isSuccess } = useGetApprovableTimesheetEntriesQuery()
</script>

<template>
	<div v-if="isSuccess">
		<ApprovableTimesheetEntryCard v-for="entry in data" :key="entry.id" :entry="entry" />
	</div>
</template>

<style scoped></style>
