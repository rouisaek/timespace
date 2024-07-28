import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'
import { plugin as primeVue } from '@/infrastructure/plugins/primevue'
import { VueQueryPlugin } from '@tanstack/vue-query'
import 'iconify-icon'

import App from './App.vue'
import router from './router'
import i18n from './infrastructure/i18n/i18n'

const app = createApp(App)

app.use(createPinia())
app.use(router)
app.use(primeVue)
app.use(VueQueryPlugin)
app.use(i18n)

app.mount('#app')
