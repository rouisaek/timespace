import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'
import { plugin as primeVue } from '@/infrastructure/plugins/primevue/primevue'
import { VueQueryPlugin } from '@tanstack/vue-query'
import { autoAnimatePlugin } from '@formkit/auto-animate/vue'
import 'iconify-icon'
import '@formatjs/intl-durationformat/polyfill'
import 'temporal-polyfill/global'
import dayjs from 'dayjs'
import customParseFormat from 'dayjs/plugin/customParseFormat' // ES 2015

dayjs.extend(customParseFormat)

import App from './App.vue'
import router from './router'
import i18n from './infrastructure/i18n/i18n'
import queryClient from './infrastructure/query-client'

const app = createApp(App)

app.use(createPinia())
app.use(router)
app.use(primeVue)
app.use(VueQueryPlugin, {
	queryClient: queryClient,
	enableDevtoolsV6Plugin: true
})
app.use(i18n)
app.use(autoAnimatePlugin)

app.mount('#app')
