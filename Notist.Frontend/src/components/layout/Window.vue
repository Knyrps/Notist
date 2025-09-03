<template>
    <div class="window">
        <div class="window-header">
            <h2 class="window-title">{{ title }}</h2>
            <button class="close-button" @click="handleClose" title="Close">
                <FontAwesomeIcon :icon="faXmark" />
            </button>
        </div>

        <div class="window-content">
            <slot />
        </div>

        <div v-if="$slots.footer" class="window-footer">
            <slot name="footer" />
        </div>
    </div>
</template>

<script setup lang="ts">
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";
import { faXmark } from "@fortawesome/free-solid-svg-icons";

const props = defineProps<{
    title: string;
    beforeClose?: () => boolean | Promise<boolean>;
}>();

const emit = defineEmits<{
    close: [];
}>();

/**
 * Handles the close button click with optional beforeClose validation
 */
const handleClose = async () => {
    if (props.beforeClose) {
        const canClose = await props.beforeClose();
        if (!canClose) {
            return; // Cancel closing
        }
    }

    emit("close");
};
</script>

<style lang="scss">
@import "@/styles/variables";

.window {
    background: $color-bg-secondary;
    border: 1px solid $color-border-primary;
    border-radius: $radius-primary;
    box-shadow: $shadow-strong;
    backdrop-filter: $blur-strong;
    width: 500px;
    max-width: 100%;
    overflow: visible;
    position: relative;
    z-index: 1;
}

.window-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 0.5rem 1rem;
    border-bottom: 1px solid $color-border-primary;
    background: $color-bg-primary;

    .window-title {
        margin: 0;
        font-size: 1rem;
        font-weight: 600;
        color: $color-text-primary;
    }

    .close-button {
        display: inline-flex;
        align-items: center;
        justify-content: center;
        width: 1.5rem;
        height: 1.5rem;
        padding: 0;
        background: transparent;
        border: 1px solid $color-border-primary;
        border-radius: $radius-secondary;
        color: $color-text-secondary;
        cursor: pointer;
        transition: all $transition-medium;

        &:hover {
            background: $color-bg-hover;
            border-color: $color-border-hover;
            color: $color-text-primary;
        }

        &:active {
            background: $color-bg-active;
        }

        svg {
            width: 0.75rem;
            height: 0.75rem;
        }
    }
}

.window-content {
    padding: 1.5rem;
    max-height: 60vh;
    overflow-y: auto;
    position: relative;
    z-index: 1;
}

.window-footer {
    display: flex;
    justify-content: flex-end;
    gap: 0.75rem;
    padding: 0.5rem 1rem;
    border-top: 1px solid $color-border-primary;
    background: $color-bg-primary;
}
</style>
