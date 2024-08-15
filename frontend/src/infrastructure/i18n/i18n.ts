import { createI18n } from 'vue-i18n'
import enLocale from './locales/en.json'
import nlLocale from './locales/nl.json'

const messages = {
	en: enLocale,
	nl: nlLocale
}

const getLocaleFromBrowserLocale = () => {
	const browserLocale = window.navigator.language
	const locale = browserLocale.split('-')[0]
	return locale
}

const i18n = createI18n({
	legacy: false,
	locale: getLocaleFromBrowserLocale(),
	globalInjection: true,
	numberFormats: {
		en: {
			decimal: {
				style: 'decimal',
				minimumFractionDigits: 2,
				maximumFractionDigits: 2
			},
			percent: {
				style: 'percent',
				useGrouping: false
			}
		},
		nl: {
			decimal: {
				style: 'decimal',
				minimumFractionDigits: 2,
				maximumFractionDigits: 2
			},
			percent: {
				style: 'percent',
				useGrouping: false
			}
		}
	},
	datetimeFormats: {
		en: {
			short: {
				year: 'numeric',
				month: 'numeric',
				day: 'numeric'
			},
			long: {
				year: 'numeric',
				month: 'long',
				day: 'numeric',
				weekday: 'long',
				hour: 'numeric',
				minute: 'numeric'
			},
			longNoYear: {
				month: 'long',
				day: 'numeric',
				weekday: 'long',
				hour: 'numeric',
				minute: 'numeric'
			}
		},
		nl: {
			short: {
				year: 'numeric',
				month: 'numeric',
				day: 'numeric'
			},
			long: {
				year: 'numeric',
				month: 'long',
				day: 'numeric',
				weekday: 'long',
				hour: 'numeric',
				minute: 'numeric'
			},
			longNoYear: {
				month: 'long',
				day: 'numeric',
				weekday: 'long',
				hour: 'numeric',
				minute: 'numeric'
			}
		}
	},
	messages
})

export default i18n
