<template>
    <nav class="nav nav-bubble nav-icon">
        <button
            class="nav__button nav__button--icon"
            @click="$emit('click')"
            :title="title"
        >
            <FontAwesomeIcon :icon="icon" />
        </button>
    </nav>
</template>

<script setup lang="ts">
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";

defineProps<{
    icon: any;
    title: string;
}>();

defineEmits<{
    click: [];
}>();
</script>

<style lang="scss">
@import "@/styles/variables";

.nav.nav-icon {
    .nav__button--icon {
        display: inline-flex;
        align-items: center;
        justify-content: center;
        padding: 0.5rem;
        width: 2.5rem;
        height: 2.5rem;
        background: $color-bg-primary;
        border: 1px solid $color-border-primary;
        border-radius: $radius-primary;
        color: $color-text-secondary;
        cursor: pointer;
        transition: all $transition-medium;
        backdrop-filter: $blur-light;
        box-shadow: $shadow-light;

        &:hover {
            background: $color-bg-hover;
            border-color: $color-border-hover;
            box-shadow: $shadow-medium;
            color: $color-text-primary;
        }

        &:active {
            background: $color-bg-active;
            transform: translateY(1px);
            box-shadow: $shadow-active;
        }

        svg {
            width: 1rem;
            height: 1rem;
        }
    }
}
</style>
