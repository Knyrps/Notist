<template>
    <nav class="nav nav-bubble">
        <a v-if="props.link" :href="props.link.href" class="nav__button">{{
            props.text
        }}</a>
        <span v-else class="nav__button nav__button--inactive">{{
            props.text
        }}</span>
    </nav>
</template>
<script setup lang="ts">
const props = defineProps<{ text: string; link?: URL }>();
</script>
<style lang="scss">
@import "@/styles/variables";

.nav.nav-bubble {
    .nav__button {
        display: inline-block;
        padding: 0.5rem 1rem;
        background: $color-bg-primary;
        border: 1px solid $color-border-primary;
        border-radius: $radius-primary;
        color: $color-text-primary;
        text-decoration: none;
        font-size: 0.875rem;
        font-weight: 500;
        cursor: pointer;
        transition: all $transition-medium;
        backdrop-filter: $blur-light;
        box-shadow: $shadow-light;

        &:hover:not(&--inactive) {
            background: $color-bg-hover;
            border-color: $color-border-hover;
            box-shadow: $shadow-medium;
        }

        &:active:not(&--inactive) {
            background: $color-bg-active;
            transform: translateY(1px);
            box-shadow: $shadow-active;
        }

        &--inactive {
            cursor: default;
            opacity: 0.9;
        }
    }
}
</style>
