import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'
import { plugin as primeVue } from '@/infrastructure/plugins/primevue/primevue'
import { VueQueryPlugin } from '@tanstack/vue-query'
import { autoAnimatePlugin } from '@formkit/auto-animate/vue'
import 'iconify-icon'
import 'temporal-polyfill/global'
import dayjs from 'dayjs'
import customParseFormat from 'dayjs/plugin/customParseFormat' // ES 2015

dayjs.extend(customParseFormat)

import App from './App.vue'
import router from './router'
import i18n from './infrastructure/i18n/i18n'

const app = createApp(App)

app.use(createPinia())
app.use(router)
app.use(primeVue)
app.use(VueQueryPlugin)
app.use(i18n)
app.use(autoAnimatePlugin)

app.mount('#app')

const IconifyIcon: any = window.customElements.get('iconify-icon')

IconifyIcon?.loadIcons('heroicons')
	.then(() => {
		console.log('Loaded data for', name)
	})
	.catch(console.error)
