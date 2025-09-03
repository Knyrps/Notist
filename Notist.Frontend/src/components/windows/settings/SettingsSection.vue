<template>
    <div class="settings-section">
        <h3>{{ name }}</h3>

        <slot name="children"></slot>
    </div>
</template>

<script setup lang="ts">
type Props = {
    name: string;
};

withDefaults(defineProps<Props>(), {});
</script>

<style lang="scss" scoped>
@import "./styles/settings";
@import "@/styles/variables";
</style>
