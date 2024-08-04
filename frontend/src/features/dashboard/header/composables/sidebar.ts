import { reactive, readonly } from 'vue'

const layoutState = reactive<{
	mobileMenuOpen: boolean
	activeMenuItem: string | null
}>({
	mobileMenuOpen: false,
	activeMenuItem: null
})

export const useSidebar = () => {
	const toggleMobileMenu = () => {
		layoutState.mobileMenuOpen = !layoutState.mobileMenuOpen
	}

	const closeMobileMenu = () => {
		layoutState.mobileMenuOpen = false
	}

	const setActiveMenuItem = (item: string) => (layoutState.activeMenuItem = item)

	return {
		layoutState: readonly(layoutState),
		toggleMobileMenu,
		closeMobileMenu,
		setActiveMenuItem
	}
}
