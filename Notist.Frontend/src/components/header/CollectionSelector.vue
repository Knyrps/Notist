<template>
    <nav class="nav nav--collection-selector">
        <SelectRoot v-model="selectedKey">
            <SelectTrigger :aria-label="props.label" class="select-trigger">
                <SelectValue
                    :placeholder="props.placeholder"
                    class="select-value"
                >
                    {{ selected?.value }}
                </SelectValue>
                <FontAwesomeIcon :icon="faChevronDown" class="select-icon" />
            </SelectTrigger>

            <SelectPortal>
                <SelectContent
                    class="select-content"
                    position="popper"
                    side="bottom"
                    align="center"
                >
                    <SelectScrollUpButton class="select-scroll-button">
                        <FontAwesomeIcon :icon="faChevronUp" />
                    </SelectScrollUpButton>

                    <SelectViewport class="select-viewport">
                        <SelectGroup>
                            <SelectItem
                                v-for="(option, index) in props.options"
                                :key="index"
                                :value="option.key"
                                :disabled="option.key === selectedKey"
                                class="select-item"
                            >
                                <SelectItemIndicator
                                    class="select-item-indicator"
                                >
                                    <FontAwesomeIcon :icon="faCheck" />
                                </SelectItemIndicator>
                                <SelectItemText class="select-item-text">
                                    {{ option.value }}
                                </SelectItemText>
                            </SelectItem>
                        </SelectGroup>
                    </SelectViewport>

                    <SelectScrollDownButton class="select-scroll-button">
                        <FontAwesomeIcon :icon="faChevronDown" />
                    </SelectScrollDownButton>
                </SelectContent>
            </SelectPortal>
        </SelectRoot>
    </nav>
</template>

<script setup lang="ts">
import {
    SelectContent,
    SelectGroup,
    SelectItem,
    SelectItemIndicator,
    SelectItemText,
    SelectPortal,
    SelectRoot,
    SelectScrollDownButton,
    SelectScrollUpButton,
    SelectTrigger,
    SelectValue,
    SelectViewport,
} from "radix-vue";
import { ref, computed } from "vue";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";
import {
    faChevronDown,
    faChevronUp,
    faCheck,
} from "@fortawesome/free-solid-svg-icons";

const props = defineProps<{
    label: string;
    placeholder: string;
    options: { key: string; value: string }[];
    default: string;
}>();

const selectedKey = ref<string | undefined>(props.default);

const selected = computed(() =>
    props.options.find((opt) => opt.key === selectedKey.value)
);
</script>

<style lang="scss">
@import "@/styles/variables";

.nav.nav--collection-selector {
    // Trigger button styling - bubble style like nav buttons
    .select-trigger {
        display: inline-flex !important;
        align-items: center !important;
        justify-content: space-between !important;
        gap: 0.5rem !important;
        padding: 0.5rem 1rem !important;
        background: $color-bg-primary !important;
        border: 1px solid $color-border-primary !important;
        border-radius: $radius-primary !important;
        color: $color-text-primary !important;
        font-size: 0.875rem !important;
        font-weight: 500 !important;
        cursor: pointer !important;
        min-width: 150px !important;
        transition: all $transition-medium, border-radius 0s !important;
        backdrop-filter: $blur-light !important;
        box-shadow: $shadow-light !important;

        &:hover {
            background: $color-bg-hover !important;
            border-color: $color-border-hover !important;
            box-shadow: $shadow-medium !important;
        }

        &:focus {
            outline: none !important;
            border-color: $color-border-focus !important;
            box-shadow: $shadow-focus !important;
        }

        &[data-state="open"] {
            background: $color-bg-active !important;
            border-color: $color-border-focus !important;
            border-bottom: none !important;
            border-radius: $radius-primary $radius-primary 0 0 !important;
        }
    }

    .select-value {
        flex: 1 !important;
        text-align: left !important;
    }

    .select-icon {
        width: 1rem !important;
        height: 1rem !important;
        color: $color-text-secondary !important;
        transition: transform $transition-medium !important;
    }

    .select-trigger[data-state="open"] .select-icon {
        transform: rotate(180deg) !important;
    }
}
</style>

<style lang="scss">
@import "@/styles/variables";

// Global styles for portalled dropdown content
.select-content {
    background: $color-bg-secondary !important;
    border: 1px solid $color-border-primary !important;
    border-top: 1px solid $color-border-focus !important;
    border-radius: 0 0 $radius-primary $radius-primary !important;
    padding: 0.25rem !important;
    box-shadow: $shadow-strong !important;
    backdrop-filter: $blur-strong !important;
    z-index: $z-dropdown !important;
    min-width: 150px !important;
    max-height: 300px !important;
    overflow: hidden !important;
}

.select-viewport {
    padding: 0.25rem !important;
}

.select-item {
    display: flex !important;
    align-items: center !important;
    gap: 0.5rem !important;
    padding: 0.5rem 0.75rem !important;
    color: $color-text-primary !important;
    font-size: 0.875rem !important;
    cursor: pointer !important;
    border-radius: $radius-secondary !important;
    transition: background-color $transition-fast !important;
    position: relative !important;

    &:hover {
        background: $color-accent-light !important;
        outline: none !important;
    }

    &[data-highlighted] {
        background: $color-accent-medium !important;
        outline: none !important;
    }

    &[data-disabled] {
        opacity: 0.5 !important;
        cursor: not-allowed !important;
    }

    &[data-state="checked"] {
        background: $color-accent-strong !important;
        font-weight: 600 !important;
    }
}

.select-item-indicator {
    width: 1rem !important;
    height: 1rem !important;
    display: inline-flex !important;
    align-items: center !important;
    justify-content: center !important;

    svg {
        width: 0.75rem !important;
        height: 0.75rem !important;
        color: $color-accent-primary !important;
    }
}

.select-item-text {
    flex: 1 !important;
}

.select-scroll-button {
    display: flex !important;
    align-items: center !important;
    justify-content: center !important;
    height: 1.5rem !important;
    cursor: pointer !important;
    color: $color-text-secondary !important;
    border-radius: $radius-secondary !important;
    transition: background-color $transition-fast !important;

    &:hover {
        background: $color-accent-light !important;
    }

    svg {
        width: 0.875rem !important;
        height: 0.875rem !important;
    }
}
</style>
