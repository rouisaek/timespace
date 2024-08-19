export default {
	root: {
		class: [
			'block relative',

			// Base Label Appearance
			'[&>*:nth-child(2)]:text-surface-900/70 dark:[&>*:nth-child(2)]:text-white/70',
			'[&>*:nth-child(2)]:absolute',
			'[&>*:nth-child(2)]:top-1/2',
			'[&>*:nth-child(2)]:-translate-y-1/2',
			'[&>*:nth-child(2)]:left-3',
			'[&>*:nth-child(2)]:pointer-events-none',
			'[&>*:nth-child(2)]:transition-all',
			'[&>*:nth-child(2)]:duration-200',
			'[&>*:nth-child(2)]:ease',

			// Focus Label Appearance
			'[&>*:nth-child(2)]:has-[:focus]:-top-3',
			'[&>*:nth-child(2)]:has-[:focus]:text-sm',

			// Filled Input Label Appearance
			'[&>*:nth-child(2)]:has-[.filled]:-top-3',
			'[&>*:nth-child(2)]:has-[.filled]:text-sm'
		]
	}
}
