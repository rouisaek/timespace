<template>
	<!-- Begin form field -->
	<div class="d-flex flex-column gap-2">
		<div
			:class="{
				'flex flex-row-reverse justify-end align-items-center gap-4': props.rightLabel,
				'flex flex-column gap-2': props.topLabel,
				'flex flex-row align-items-center gap-4': props.leftLabel
			}"
		>
			<div
				:class="{
					'dark:text-red-300 text-red-700': v$.modelValue.$invalid && showError,
					'text-nowrap flex-grow-1 cursor-pointer text-tprimary': true
				}"
				@click="modelValue = !modelValue"
			>
				{{ label }}
			</div>
			<ToggleSwitch
				:id="id"
				v-bind="componentAttributes"
				v-model="v$.modelValue.$model"
				:class="componentClasses"
			>
				<template v-for="(_, slot) of $slots" v-slot:[slot]="scope">
					<slot :name="slot" v-bind="scope" />
				</template>
			</ToggleSwitch>
		</div>
		<small :id="id + '-help'" v-if="helpText">{{ helpText }}<br /></small>
		<span v-if="v$.modelValue.$error && showTextErrors">
			<span :id="id + '-error'" v-for="(error, index) of v$.modelValue.$errors" :key="index">
				<small class="dark:text-red-300 text-red-700">{{ error.$message }}</small>
			</span>
		</span>
	</div>
	<!-- End form Field -->
</template>

<script lang="ts">
export default { inheritAttrs: false }
</script>

<script setup lang="ts">
import { useVuelidate } from '@vuelidate/core'
import { uniqueId } from 'lodash-es'
import { computed, reactive, useAttrs } from 'vue'
import { getRules, type InputSwitchProps } from '../FormInputTypes'
import ToggleSwitch from 'primevue/toggleswitch'

const props = defineProps<InputSwitchProps>()

const modelValue = defineModel()
const attrs = useAttrs()
const id = (attrs['id'] as string) ?? uniqueId('input')

const rules: any = getRules(props, false, props.label)

const v$ = useVuelidate({ modelValue: rules }, reactive({ modelValue }))

// eslint-disable-next-line vue/no-setup-props-destructure

const componentClasses = reactive<any>({
	'border-red-700 dark:border-red-300': computed(
		() => v$.value.modelValue.$invalid && props.showError
	),
	'flex-shrink-1': true
})

const inputProps = { ...props }
if (props.disabled) inputProps.disabled = props.disabled

const componentAttributes = reactive<any>({ ...inputProps })

v$.value.$touch()
</script>
