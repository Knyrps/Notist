<template>
    <span class="note-link" @click="handleClick">
        <slot>Note {{ id }}</slot>
    </span>
</template>

<script setup lang="ts">
const props = defineProps<{ id: string }>();

function handleClick() {
    // Vue-controlled behavior here
    console.log("Clicked note:", props.id);
    // e.g. emit event, navigate, open modal, etc.
}
</script>

<style lang="sass" scoped></style>
