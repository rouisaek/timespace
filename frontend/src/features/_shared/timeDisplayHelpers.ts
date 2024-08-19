export const msToTime = (ms: number) => {
	const diffFullHours = Math.floor(ms / 1000 / 60 / 60)
	const diffFullMinutes = Math.floor((ms - diffFullHours * 1000 * 60 * 60) / 1000 / 60)

	return `${diffFullHours.toString().padStart(2, '0')}:${diffFullMinutes.toString().padStart(2, '0')}`
}
