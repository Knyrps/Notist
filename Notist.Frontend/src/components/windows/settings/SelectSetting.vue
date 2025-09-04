<template>
    <BaseSetting
        :setting-name="settingName"
        :requires-restart="requiresRestart"
    >
        <template #control>
            <div>
                <SelectRoot
                    :model-value="modelValue"
                    @update:model-value="$emit('update:modelValue', $event)"
                >
                    <SelectTrigger class="setting-select">
                        <span class="select-value">{{
                            modelValue || placeholder || "Select..."
                        }}</span>
                        <SelectIcon class="select-icon">
                            <FontAwesomeIcon :icon="faChevronDown" />
                        </SelectIcon>
                    </SelectTrigger>
                    <SelectContent class="select-content">
                        <SelectViewport>
                            <SelectItem
                                v-for="option in options"
                                :key="option.value"
                                :value="option.value"
                                class="select-item"
                            >
                                {{ option.label }}
                            </SelectItem>
                        </SelectViewport>
                    </SelectContent>
                </SelectRoot>
            </div>
        </template>
    </BaseSetting>
</template>

<script setup lang="ts">
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";
import { faChevronDown } from "@fortawesome/free-solid-svg-icons";
import {
    SelectContent,
    SelectIcon,
    SelectItem,
    SelectRoot,
    SelectTrigger,
    SelectViewport,
} from "radix-vue";
import BaseSetting, { type BaseSettingProps } from "./BaseSetting.vue";

interface SelectOption {
    value: string;
    label: string;
}

interface SelectSettingProps extends BaseSettingProps {
    modelValue: string;
    options: SelectOption[];
    placeholder?: string;
}

withDefaults(defineProps<SelectSettingProps>(), {
    requiresRestart: false,
});

defineEmits<{
    "update:modelValue": [value: string];
}>();
</script>

<style lang="scss" scoped>
@import "@/components/windows/settings/styles/settings";
@import "@/components/windows/settings/styles/_variables";
@import "@/styles/_variables";

// Radix Select Styles
.setting-select {
    display: inline-flex;
    align-items: center;
    justify-content: space-between;
    padding: 0.5rem 0.75rem;
    border: 1px solid $color-border-primary;
    border-radius: $radius-secondary;
    background: $color-bg-primary;
    color: $color-text-primary;
    font-size: 0.875rem;
    min-width: 140px;
    cursor: pointer;
    transition: $transition-fast;
    position: relative;
    z-index: 1;

    &:hover {
        border-color: $color-border-hover;
        background: $color-bg-hover;
    }

    &:focus {
        outline: none;
        border-color: $color-border-focus;
        box-shadow: $shadow-focus;
    }

    &[data-state="open"] {
        border-color: $color-accent-primary;
        z-index: 2002;
        box-shadow: $shadow-focus;

        .select-icon {
            transform: scaleY(-1);
            color: $color-accent-primary;
        }
    }
}

.select-value {
    flex: 1;
    text-align: left;
    color: $color-text-primary;
    font-size: 0.875rem;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
}

.select-icon {
    margin-left: 0.5rem;
    color: $color-text-primary;
    transition: $transition-fast;
    transform: scaleY(1);
    flex-shrink: 0;
    font-size: 0.75rem;
}

.select-content {
    background: $color-bg-overlay;
    border: 1px solid $color-border-primary;
    border-radius: $radius-secondary;
    box-shadow: $shadow-strong;
    padding: 0.5rem;
    min-width: var(--radix-select-trigger-width);
    max-height: var(--radix-select-content-available-height);
    z-index: 2001;
    overflow: hidden;
    backdrop-filter: blur(12px);
    position: relative;
    transform: translateZ(0);
    animation: slideDownAndFade 0.2s ease-out;
}

@keyframes slideDownAndFade {
    from {
        opacity: 0;
        transform: translateY(-8px);
    }
    to {
        opacity: 1;
        transform: translateY(0);
    }
}

.select-item {
    display: flex;
    align-items: center;
    padding: 0.75rem 1rem;
    border-radius: $radius-primary;
    cursor: pointer;
    color: $color-text-primary;
    font-size: 0.875rem;
    transition: $transition-fast;
    outline: none;
    margin: 0.125rem 0;

    &:hover {
        background: $color-bg-hover;
    }

    &[data-highlighted] {
        background: $color-accent-light;
        color: $color-text-primary;
    }

    &[data-state="checked"] {
        background: $color-accent-primary;
        color: $select-checked-text;
        font-weight: 500;

        &::after {
            content: "✓";
            margin-left: auto;
            font-size: 0.75rem;
        }
    }
}
</style>
