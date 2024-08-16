import type { InputTextProps as PInputTextProps } from 'primevue/inputtext'
import type { TextareaProps as PTextareaProps } from 'primevue/textarea'
import type { DatePickerProps as PDatePickerProps } from 'primevue/datepicker'
import type { DropdownProps as PDropdownProps } from 'primevue/dropdown'
import type { InputNumberProps as PInputNumberProps } from 'primevue/inputnumber'
import type { MultiSelectProps as PMultiSelectProps } from 'primevue/multiselect'
import type { ToggleSwitchProps as PToggleSwitchProps } from 'primevue/toggleswitch'
import type { PasswordProps as PPasswordProps } from 'primevue/password'
import type { InputMaskProps as PInputMaskProps } from 'primevue/inputmask'
import type { RadioButtonProps as PRadioButtonProps } from 'primevue/radiobutton'
import type { FileUploadProps as PFileUploadProps } from 'primevue/fileupload'
import type { AutoCompleteProps as PAutocompleteProps } from 'primevue/autocomplete'
// import type { Dayjs } from 'dayjs'
import {
	alpha as alphaRule,
	between as betweenRule,
	decimal as decimalRule,
	email as emailRule,
	maxLength as maxLengthRule,
	maxValue as maxValueRule,
	minLength as minLengthRule,
	minValue as minValueRule,
	numeric as numericRule,
	required as requiredRule,
	sameAs as sameAsRule,
	helpers,
	createI18nMessage
} from '@vuelidate/validators'
import { computed } from 'vue'
import i18n from '@/infrastructure/i18n/i18n'

export type UploadedFile = {
	name: string
	hash: string
	id: number
}

export interface InputTextProps
	extends BaseInputProps,
		/* @vue-ignore */ Omit<PInputTextProps, 'disabled' | 'readonly' | 'required'> {}

export interface InputNumberProps
	extends BaseInputProps,
		/* @vue-ignore */ Omit<PInputNumberProps, 'disabled' | 'readonly' | 'required'> {}

export interface InputFileProps
	extends BaseInputProps,
		/* @vue-ignore */ Omit<PFileUploadProps, 'disabled' | 'readonly' | 'required'> {
	uploadUrl: string
}

export interface InputPhoneNumberProps
	extends BaseInputProps,
		/* @vue-ignore */ Omit<PInputTextProps, 'disabled' | 'readonly' | 'required'> {
	numberType?: 'MOBILE' | 'FIXED_LINE'
	countryCode?: string
}

export interface InputMaskProps
	extends BaseInputProps,
		/* @vue-ignore */ Omit<PInputMaskProps, 'disabled' | 'readonly' | 'required'> {}

export interface InputAutoCompleteProps
	extends BaseInputProps,
		/* @vue-ignore */ Omit<PAutocompleteProps, 'disabled' | 'readonly' | 'required'> {}

export interface PasswordProps
	extends BaseInputProps,
		/* @vue-ignore */ Omit<PPasswordProps, 'disabled' | 'readonly' | 'required'> {
	size?: 'small' | 'large'
}

export interface TextareaProps
	extends BaseInputProps,
		/* @vue-ignore */ Omit<PTextareaProps, 'disabled' | 'readonly' | 'required'> {}

export interface DatePickerProps
	extends BaseInputProps,
		/* @vue-ignore */ Omit<PDatePickerProps, 'disabled' | 'readonly' | 'required' | 'modelValue'> {}

export interface DropdownProps
	extends BaseInputProps,
		/* @vue-ignore */ Omit<PDropdownProps, 'disabled' | 'readonly' | 'required'> {}

export interface MultiSelectProps
	extends BaseInputProps,
		/* @vue-ignore */ Omit<PMultiSelectProps, 'disabled' | 'readonly' | 'required'> {}

export type InputSwitchProps = BaseInputProps &
	/* @vue-ignore */ Omit<PToggleSwitchProps, 'disabled' | 'readonly' | 'required'> & {
		topLabel?: boolean
		rightLabel?: boolean
		leftLabel?: boolean
	}

export interface RadioButtonProps
	extends BaseInputProps,
		/* @vue-ignore */ Omit<PRadioButtonProps, 'disabled' | 'readonly' | 'required'> {}

export type RadioButtonGroupProps = Omit<RadioButtonProps, 'value'> & {
	name: string
	options: { label: string; value: string }[]
}

export interface BaseInputProps extends ValidationProps {
	label?: string
	helpText?: string
	disabled?: boolean
	readonly?: boolean
	showError?: boolean
	showTextErrors?: boolean
}

export interface ValidationProps {
	alpha?: boolean
	between?: { min: number; max: number }
	decimal?: boolean
	email?: boolean
	maxLength?: number
	maxValue?: number
	minLength?: number
	minValue?: number
	numeric?: boolean
	required?: boolean
	sameAs?: unknown
}

export function getRules(
	props: ValidationProps,
	isCalendar: boolean = false,
	label: string = 'Value'
): any {
	const rules: any = {}

	const withI18nMessage = createI18nMessage({ t: i18n.global.t.bind(i18n) })

	if (props.alpha)
		rules.alpha = withI18nMessage(helpers.withParams({ label }, alphaRule), {
			withArguments: true
		})

	if (props.between !== undefined)
		rules.between = withI18nMessage(
			helpers.withParams({ label }, betweenRule(props.between.min, props.between.max)),
			{ withArguments: true }
		)

	if (props.decimal)
		rules.decimal = withI18nMessage(helpers.withParams({ label }, decimalRule), {
			withArguments: true
		})

	if (props.email)
		rules.email = withI18nMessage(helpers.withParams({ label }, emailRule), {
			withArguments: true
		})

	// if (props.maxDate !== undefined)
	// 	rules.maxDate = withI18nMessage(
	// 		helpers.withParams({ label }, (value: Dayjs) => value.toDate() <= props.maxDate!.toDate()),
	// 		{ withArguments: true }
	// 	)

	if (props.maxLength !== undefined)
		rules.maxLength = withI18nMessage(
			helpers.withParams({ label }, maxLengthRule(props.maxLength)),
			{ withArguments: true }
		)

	if (props.maxValue !== undefined)
		rules.maxValue = withI18nMessage(helpers.withParams({ label }, maxValueRule(props.maxValue)), {
			withArguments: true
		})

	// if (props.minDate !== undefined)
	// 	rules.maxDate = withI18nMessage(
	// 		helpers.withParams({ label }, (value: Dayjs) => value.toDate() >= props.minDate!.toDate()),
	// 		{ withArguments: true }
	// 	)

	if (props.minLength !== undefined)
		rules.minLength = withI18nMessage(
			helpers.withParams({ label }, minLengthRule(props.minLength)),
			{ withArguments: true }
		)

	if (props.minValue !== undefined)
		rules.minValue = withI18nMessage(helpers.withParams({ label }, minValueRule(props.minValue)), {
			withArguments: true
		})

	if (props.numeric)
		rules.numeric = withI18nMessage(helpers.withParams({ label }, numericRule), {
			withArguments: true
		})

	if (props.required)
		rules.required = withI18nMessage(helpers.withParams({ label }, requiredRule), {
			withArguments: true
		})

	if (props.sameAs !== undefined)
		rules.sameAs = withI18nMessage(
			helpers.withParams({ label }, sameAsRule(computed(() => props.sameAs))),
			{ withArguments: true }
		)

	if (isCalendar && props.required)
		rules.notNull = withI18nMessage(
			helpers.withParams({ label }, (value: unknown) => value !== null),
			{ withArguments: true }
		)

	return rules
}
