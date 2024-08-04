<script setup lang="ts">
</script>

<template>
	<Transition name="scale-in">
		<slot></slot>
	</Transition>
</template>

<style>
.scale-in-enter-active {
	animation: scale-in 0.5s;
}

.scale-in-leave-active {
	animation: scale-in 0.5s reverse;
}

@keyframes scale-in {
	0% {
		transform: scale(0);
	}

	100% {
		transform: scale(1);
	}
}
</style>
