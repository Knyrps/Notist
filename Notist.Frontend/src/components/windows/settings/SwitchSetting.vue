<template>
    <BaseSetting
        :setting-name="settingName"
        :requires-restart="requiresRestart"
    >
        <template #control>
            <label class="setting-switch-label">
                <SwitchRoot
                    :checked="modelValue"
                    @update:checked="$emit('update:modelValue', $event)"
                    class="setting-switch"
                >
                    <SwitchThumb class="setting-switch-thumb" />
                </SwitchRoot>
                <!-- Switch text goes here if needed -->
            </label>
        </template>
    </BaseSetting>
</template>

<script setup lang="ts">
import { SwitchRoot, SwitchThumb } from "radix-vue";
import BaseSetting from "./BaseSetting.vue";

interface Props {
    settingName: string;
    requiresRestart?: boolean;
    modelValue: boolean;
}

withDefaults(defineProps<Props>(), {
    requiresRestart: false,
});

defineEmits<{
    "update:modelValue": [value: boolean];
}>();
</script>

<style lang="scss" scoped>
@import "./styles/settings";
@import "@/styles/variables";

// Radix Switch Styles
:deep(.setting-switch) {
    all: unset;
    width: 48px;
    height: 26px;
    background: rgba(0, 0, 0, 0.2);
    border-radius: 13px;
    position: relative;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    transition: all 0.2s ease;
    cursor: pointer;
    margin-right: 0.75rem;
    display: inline-block;

    &:hover {
        background: rgba(0, 0, 0, 0.25);
    }

    &:focus-visible {
        outline: 2px solid $color-accent-primary;
        outline-offset: 2px;
    }

    &[data-state="checked"] {
        background: $color-accent-primary;

        &:hover {
            background: $color-accent-primary-dark;
        }
    }
}

:deep(.setting-switch-thumb) {
    all: unset;
    display: block;
    width: 22px;
    height: 22px;
    background: white;
    border-radius: 50%;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
    transition: transform 0.2s ease;
    transform: translateX(2px);
    position: absolute;
    top: 2px;
    left: 0;

    &[data-state="checked"] {
        transform: translateX(24px);
    }
}

.setting-switch-label {
    display: flex;
    align-items: center;
    cursor: pointer;
    font-size: 0.875rem;
    color: $color-text-primary;
    gap: 0 !important;
    transition: $transition-fast;
    width: 100% !important;
    justify-content: flex-end !important;

    &:hover {
        color: $color-text-primary;
    }

    &:hover .setting-switch {
        background: rgba(0, 0, 0, 0.3);
    }

    &:hover .setting-switch[data-state="checked"] {
        background: $color-accent-primary-dark;
    }
}
</style>
