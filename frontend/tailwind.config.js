// eslint-disable-next-line no-undef
const { addDynamicIconSelectors } = require('@iconify/tailwind')

/** @type {import('tailwindcss').Config} */
export default {
	content: ['./index.html', './src/**/*.{js,ts,vue}'],
	theme: {
		extend: {}
	},
	plugins: [
		addDynamicIconSelectors({
			scale: 0
		})
	],
	darkMode: 'media'
}
