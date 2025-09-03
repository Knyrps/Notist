<template>
    <div :class="`alert alert--${type}`">
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
import {
    faTriangleExclamation,
    type IconDefinition,
} from "@fortawesome/free-solid-svg-icons";
import type { AlertType } from "@/lib/alertManager";

interface AlertProps {
    title: string;
    description: string;
    confirmText?: string;
    cancelText?: string;
    showCancel?: boolean;
    icon?: IconDefinition;
    type?: AlertType;
}

const props = withDefaults(defineProps<AlertProps>(), {
    confirmText: "Confirm",
    cancelText: "Cancel",
    showCancel: true,
    icon: () => faTriangleExclamation,
    type: "warning",
});

defineEmits<{
    confirm: [];
    cancel: [];
}>();
</script>

<style lang="scss" scoped>
@import "@/styles/_fonts.scss";
@import "./styles/variables";

.alert {
    max-width: 26rem;
    width: 90vw;
    padding: 1.5rem;
    background: $color-bg-primary;
    border: 1px solid $color-border-primary;
    border-radius: $radius-primary;
    box-shadow: $shadow-strong;
    backdrop-filter: $blur-strong;

    &--error {
        .alert__icon {
            background: $alert-error-background;
            border-color: $alert-error-border;

            svg {
                color: $alert-error-icon;
            }
        }

        .alert__button--confirm {
            background: $alert-error-confirm-background;
            color: $alert-error-confirm-text;
            border-color: $alert-error-confirm-background;

            &:hover {
                background: $alert-error-confirm-hover;
                border-color: $alert-error-confirm-hover;
            }

            &:active {
                background: $alert-error-confirm-active;
            }
        }
    }

    &--warning {
        .alert__icon {
            background: $alert-warning-background;
            border-color: $alert-warning-border;

            svg {
                color: $alert-warning-icon;
            }
        }

        .alert__button--confirm {
            background: $alert-warning-confirm-background;
            color: $alert-warning-confirm-text;
            border-color: $alert-warning-confirm-background;

            &:hover {
                background: $alert-warning-confirm-hover;
                border-color: $alert-warning-confirm-hover;
            }

            &:active {
                background: $alert-warning-confirm-active;
            }
        }
    }

    &--success {
        .alert__icon {
            background: $alert-success-background;
            border-color: $alert-success-border;

            svg {
                color: $alert-success-icon;
            }
        }

        .alert__button--confirm {
            background: $alert-success-confirm-background;
            color: $alert-success-confirm-text;
            border-color: $alert-success-confirm-background;

            &:hover {
                background: $alert-success-confirm-hover;
                border-color: $alert-success-confirm-hover;
            }

            &:active {
                background: $alert-success-confirm-active;
            }
        }
    }

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
        border: 2px solid;
        border-radius: 50%;
        flex-shrink: 0;

        svg {
            width: 1.5rem;
            height: 1.5rem;
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
            background: $color-accent-primary;
            color: white;
            border-color: $color-accent-primary;

            &:hover {
                background: $color-accent-primary-dark;
                border-color: $color-accent-primary-dark;
            }

            &:active {
                background: $color-accent-primary-dark;
                transform: translateY(1px);
            }
        }
    }
}
</style>
