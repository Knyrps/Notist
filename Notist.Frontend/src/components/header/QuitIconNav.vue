<template>
    <div class="dropdown-nav" ref="dropdown">
        <!-- Main button -->
        <button
            class="nav__button nav__button--icon"
            @click="$emit('click')"
            :title="title"
        >
            <FontAwesomeIcon :icon="icon" />
        </button>

        <!-- Dropdown trigger button -->
        <button
            class="nav__button nav__button--dropdown"
            @click="toggleDropdown"
            :title="'Options'"
        >
            <FontAwesomeIcon :icon="faChevronDown" />
        </button>

        <!-- Dropdown menu -->
        <div v-if="isOpen" class="dropdown-menu" @click.stop>
            <button class="dropdown-item" @click="handleCloseOverlay">
                <FontAwesomeIcon :icon="faXmark" />
                <span>Close Overlay</span>
            </button>
            <button
                class="dropdown-item dropdown-item--danger"
                @click="handleQuitClick"
            >
                <FontAwesomeIcon :icon="faPowerOff" />
                <span>Quit</span>
            </button>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from "vue";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";
import {
    faChevronDown,
    faXmark,
    faPowerOff,
} from "@fortawesome/free-solid-svg-icons";
import { useAlertManager } from "@/lib/alertManager";

defineProps<{
    icon: any;
    title: string;
}>();

const emit = defineEmits<{
    click: [];
    closeOverlay: [];
    quit: [];
}>();

const { showQuitDialog } = useAlertManager();

const isOpen = ref(false);
const dropdown = ref<HTMLElement>();

const toggleDropdown = () => {
    isOpen.value = !isOpen.value;
};

const closeDropdown = () => {
    isOpen.value = false;
};

const handleCloseOverlay = () => {
    emit("closeOverlay");
    closeDropdown();
};

const handleQuitClick = () => {
    closeDropdown();
    showQuitDialog(
        () => emit("quit"), // onConfirm
        () => {} // onCancel (do nothing)
    );
};

// Close dropdown when clicking outside
const handleClickOutside = (event: MouseEvent) => {
    if (dropdown.value && !dropdown.value.contains(event.target as Node)) {
        closeDropdown();
    }
};

onMounted(() => {
    document.addEventListener("click", handleClickOutside);
});

onUnmounted(() => {
    document.removeEventListener("click", handleClickOutside);
});
</script>

<style lang="scss">
@import "@/styles/variables";

.dropdown-nav {
    position: relative;
    display: flex;
    align-items: center;

    .nav__button--icon {
        display: inline-flex;
        align-items: center;
        justify-content: center;
        padding: 0.5rem;
        width: 2.5rem;
        height: 2.5rem;
        background: $color-bg-primary;
        border: 1px solid $color-border-primary;
        border-radius: $radius-primary 0 0 $radius-primary;
        color: $color-text-secondary;
        cursor: pointer;
        transition: all $transition-medium;
        backdrop-filter: $blur-light;
        box-shadow: $shadow-light;

        &:hover {
            background: $color-bg-hover;
            border-color: $color-border-hover;
            color: $color-text-primary;
        }

        &:active {
            background: $color-bg-active;
            transform: translateY(1px);
        }

        svg {
            width: 1rem;
            height: 1rem;
        }
    }

    .nav__button--dropdown {
        display: inline-flex;
        align-items: center;
        justify-content: center;
        padding: 0.5rem 0.25rem;
        height: 2.5rem;
        background: $color-bg-primary;
        border: 1px solid $color-border-primary;
        border-left: none;
        border-radius: 0 $radius-primary $radius-primary 0;
        color: $color-text-secondary;
        cursor: pointer;
        transition: all $transition-medium;
        backdrop-filter: $blur-light;
        box-shadow: $shadow-light;

        &:hover {
            background: $color-bg-hover;
            border-color: $color-border-hover;
            color: $color-text-primary;
        }

        &:active {
            background: $color-bg-active;
            transform: translateY(1px);
        }

        svg {
            width: 0.625rem;
            height: 0.625rem;
        }
    }

    .dropdown-menu {
        position: absolute;
        top: 100%;
        right: 0;
        margin-top: 0.25rem;
        min-width: 12rem;
        background: $color-bg-primary;
        border: 1px solid $color-border-primary;
        border-radius: $radius-secondary;
        box-shadow: $shadow-strong;
        backdrop-filter: $blur-strong;
        z-index: 1000;
        overflow: hidden;

        .dropdown-item {
            display: flex;
            align-items: center;
            gap: 0.75rem;
            width: 100%;
            padding: 0.75rem 1rem;
            background: transparent;
            border: none;
            color: $color-text-primary;
            cursor: pointer;
            transition: all $transition-fast;
            font-size: 0.875rem;

            &:hover {
                background: $color-bg-hover;
            }

            &:active {
                background: $color-bg-active;
            }

            &--danger {
                color: $color-error;

                &:hover {
                    background: $color-error-light;
                    color: $color-error;
                }
            }

            svg {
                width: 1rem;
                height: 1rem;
                flex-shrink: 0;
            }

            span {
                flex: 1;
                text-align: left;
            }
        }
    }
}

// Ensure hover effects work together for the combined button
.dropdown-nav:hover {
    .nav__button--icon,
    .nav__button--dropdown {
        background: $color-bg-hover;
        border-color: $color-border-hover;
        box-shadow: $shadow-medium;
        color: $color-text-primary;
    }
}
</style>
