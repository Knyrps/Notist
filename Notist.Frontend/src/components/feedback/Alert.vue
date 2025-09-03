<template>
    <div class="alert">
        <div class="alert__header">
            <div class="alert__icon">
                <FontAwesomeIcon :icon="props.icon" />
            </div>
            <h3 class="alert__title">{{ title }}</h3>
        </div>

        <p class="alert__description">{{ description }}</p>

        <div class="alert__actions">
            <button
                v-if="showCancel"
                class="alert__button alert__button--cancel"
                @click="$emit('cancel')"
            >
                {{ cancelText }}
            </button>
            <button
                class="alert__button alert__button--confirm"
                @click="$emit('confirm')"
            >
                {{ confirmText }}
            </button>
        </div>
    </div>
</template>

<script setup lang="ts">
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";
import { faTriangleExclamation } from "@fortawesome/free-solid-svg-icons";

interface AlertProps {
    title: string;
    description: string;
    confirmText?: string;
    cancelText?: string;
    showCancel?: boolean;
    icon?: any;
}

const props = withDefaults(defineProps<AlertProps>(), {
    confirmText: "Confirm",
    cancelText: "Cancel",
    showCancel: true,
    icon: () => faTriangleExclamation,
});

defineEmits<{
    confirm: [];
    cancel: [];
}>();
</script>

<style lang="scss" scoped>
@import "@/styles/_fonts.scss";
@import "@/styles/variables";

.alert {
    max-width: 26rem;
    width: 90vw;
    padding: 1.5rem;
    background: $color-bg-primary;
    border: 1px solid $color-border-primary;
    border-radius: $radius-primary;
    box-shadow: $shadow-strong;
    backdrop-filter: $blur-strong;

    &__header {
        display: flex;
        align-items: center;
        gap: 1rem;
        margin-bottom: 1rem;
    }

    &__icon {
        display: flex;
        align-items: center;
        justify-content: center;
        width: 3rem;
        height: 3rem;
        background: $color-error-light;
        border: 2px solid $color-error;
        border-radius: 50%;
        flex-shrink: 0;

        svg {
            width: 1.5rem;
            height: 1.5rem;
            color: $color-error;
        }
    }

    &__title {
        margin: 0;
        font-size: 1.125rem;
        font-weight: 600;
        color: $color-text-primary;
        line-height: 1.4;
    }

    &__description {
        margin: 0 0 1.5rem 0;
        font-size: 0.9rem;
        color: $color-text-secondary;
        line-height: 1.5;
    }

    &__actions {
        display: flex;
        gap: 0.75rem;
        justify-content: flex-end;
    }

    &__button {
        padding: 0.625rem 1.25rem;
        border-radius: $radius-secondary;
        border: 1px solid $color-border-primary;
        cursor: pointer;
        font-size: 0.875rem;
        font-weight: 500;
        transition: all $transition-medium;
        min-width: 5rem;

        &--cancel {
            background: $color-bg-primary;
            color: $color-text-primary;

            &:hover {
                background: $color-bg-hover;
                border-color: $color-border-hover;
            }

            &:active {
                background: $color-bg-active;
                transform: translateY(1px);
            }
        }

        &--confirm {
            background: $color-error;
            color: white;
            border-color: $color-error;

            &:hover {
                background: #c82333;
                border-color: #c82333;
            }

            &:active {
                background: #bd2130;
                transform: translateY(1px);
            }
        }
    }
}
</style>
