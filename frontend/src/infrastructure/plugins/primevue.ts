import type { App } from 'vue'
import PrimeVue from 'primevue/config'
import ToastService from 'primevue/toastservice'
import DialogService from 'primevue/dialogservice'
import Tooltip from 'primevue/tooltip'
import ConfirmationService from 'primevue/confirmationservice'
import { definePreset } from '@primevue/themes'
import Aura from '@primevue/themes/aura'

const CustomAuraPreset = definePreset(Aura, {
	semantic: {
		primary: {
			50: '{indigo.50}',
			100: '{indigo.100}',
			200: '{indigo.200}',
			300: '{indigo.300}',
			400: '{indigo.400}',
			500: '{indigo.500}',
			600: '{indigo.600}',
			700: '{indigo.700}',
			800: '{indigo.800}',
			900: '{indigo.900}',
			950: '{indigo.950}'
		},
		colorScheme: {
			light: {
				formField: {
					background: '{white}'
				},
				dialog: {
					background: '{white}'
				}
			},
			dark: {
				formField: {
					background: '{neutral.900}'
				},
				dialog: {
					background: '{neutral.800}'
				}
			}
		}
	},
	options: {
		cssLayer: {
			name: 'primevue',
			order: 'tailwind-base, primevue, tailwind-utilities'
		},
		options: {
			darkModeSelector: '.app-dark'
		}
	}
})

/**
 * Initialize PrimeVue component
 * @param app vue instance
 */
export const plugin = {
	install(Vue: App) {
		Vue.use(PrimeVue, {
			ripple: true,
			zIndex: {
				menu: 1000,
				modal: 1000,
				overlay: 1000,
				tooltip: 1100
			},
			theme: {
				preset: CustomAuraPreset
			}
		})

		Vue.use(ConfirmationService)
		Vue.use(ToastService)
		Vue.use(DialogService)

		Vue.directive('tooltip', Tooltip)
	}
}
