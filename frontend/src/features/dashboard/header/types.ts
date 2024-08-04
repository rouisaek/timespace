export interface MenuItem {
	label: string
	icon?: string
	to?: string
	seperator?: boolean
	items?: MenuItem[]
	visible?: boolean
	class?: string
	permission?: string
}
