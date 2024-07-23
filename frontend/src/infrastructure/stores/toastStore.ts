import type { ToastMessageOptions } from 'primevue/toast'
import { useToast } from 'primevue/usetoast'
import { reactive, watch } from 'vue'

export const toastStore = reactive<{
	toasts: ToastMessageOptions[]
}>({
	toasts: []
})

// wrap this as composable, so you don't clutter the rendering component
export const useToastStore = () => {
	const toast = useToast()
	watch(
		() => toastStore.toasts,
		(toasts) => {
			toasts.length && toast.add(toasts[toasts.length - 1])
		}
	)
}
