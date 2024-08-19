<script setup lang="ts">
import type { PeriodSelection } from './PeriodSelector'
import { onMounted, ref, watchEffect } from 'vue'
import SelectButton from 'primevue/selectbutton'
import DatePicker from '../formfields/wrappers/DatePicker.vue'
import { useI18n } from 'vue-i18n'
import Popover from 'primevue/popover'

const { t, locale } = useI18n()
const popOver = ref()
const periodSelectorButton = ref()
const width = ref(0)

enum PeriodPresetValues {
	THIS_MONTH = 'THIS_MONTH',
	LAST_MONTH = 'LAST_MONTH',
	CUSTOM = 'CUSTOM'
}

const buttonOptions = ref([
	{
		name: t('periodSelector.thisMonth'),
		value: PeriodPresetValues.THIS_MONTH
	},
	{
		name: t('periodSelector.lastMonth'),
		value: PeriodPresetValues.LAST_MONTH
	},
	{
		name: t('periodSelector.custom'),
		value: PeriodPresetValues.CUSTOM
	}
])

const modelValue = defineModel<PeriodSelection>({
	required: true,
	default: {
		start: null,
		end: null
	}
})

const buttonValue = ref<PeriodPresetValues>()

watchEffect(() => {
	switch (buttonValue.value) {
		case PeriodPresetValues.THIS_MONTH:
			modelValue.value = {
				start: Temporal.Now.plainDateISO().with({ day: 1 }),
				end: Temporal.Now.plainDateISO().with({ day: 1 }).add({ months: 1 }).add({ days: -1 })
			}
			break
		case PeriodPresetValues.LAST_MONTH:
			modelValue.value = {
				start: Temporal.Now.plainDateISO().with({ day: 1 }).add({ months: -1 }),
				end: Temporal.Now.plainDateISO().with({ day: 1 }).add({ days: -1 })
			}
			break
		case PeriodPresetValues.CUSTOM:
			modelValue.value = {
				start: Temporal.Now.plainDateISO().with({ day: 1 }),
				end: Temporal.Now.plainDateISO().with({ day: 1 }).add({ months: 1 }).add({ days: -1 })
			}
			break
	}
})

onMounted(() => {
	buttonValue.value = PeriodPresetValues.THIS_MONTH
})

const toggle = (event: Event) => {
	width.value = periodSelectorButton.value.offsetWidth
	popOver.value.toggle(event)
}
</script>

<template>
	<div>
		<div
			@click="toggle($event)"
			class="w-full flex items-center px-4 py-1 rounded bg-surface-100 dark:bg-surface-800 border border-surface-200 dark:border-surface-700 cursor-pointer"
			ref="periodSelectorButton"
		>
			<span class="flex-grow text-center text-tprimary"
				>{{ modelValue.start?.toLocaleString(locale, { month: 'short', day: 'numeric' }) }} -
				{{
					modelValue.end?.toLocaleString(locale, {
						month: modelValue.start?.month === modelValue.end?.month ? undefined : 'short',
						day: 'numeric'
					})
				}}</span
			>
			<iconify-icon
				icon="heroicons:chevron-down"
				height="none"
				class="w-5 h-5 ml-auto text-tprimary"
			/>
		</div>
		<Popover
			ref="popOver"
			class="w-[80vw] md:w-[60vw] lg:w-[40vw] centered-arrow"
			:style="{ width: width + 'px' }"
			append-to="self"
		>
			<div class="flex flex-col w-full">
				<SelectButton
					v-model="buttonValue"
					:options="buttonOptions"
					optionLabel="name"
					optionValue="value"
					:allowEmpty="false"
					class="w-full"
				/>
				<div
					class="flex flex-col md:flex-row gap-2"
					v-show="buttonValue === PeriodPresetValues.CUSTOM"
				>
					<DatePicker
						v-model="modelValue.start"
						selectOtherMonths
						:label="t('periodSelector.from')"
					/>
					<DatePicker v-model="modelValue.end" selectOtherMonths :label="t('periodSelector.to')" />
				</div>
			</div>
		</Popover>
	</div>
</template>

<style scoped>
.centered-arrow > div {
	&::before,
	&::after {
		left: 50%;
	}
}
</style>
