import { defineStore } from 'pinia'
import type { ToastMessageOptions } from 'primevue/toast'
import { ref } from 'vue'

// wrap this as composable, so you don't clutter the rendering component
export const useToastStore = defineStore('toast', () => {
	const toasts = ref<ToastMessageOptions[]>([])

	const add = (toast: ToastMessageOptions) => {
		toasts.value.push(toast)
	}

	return { toasts, add }
})
