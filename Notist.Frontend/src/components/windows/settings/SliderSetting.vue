<template>
    <BaseSetting
        :setting-name="settingName"
        :requires-restart="requiresRestart"
    >
        <template #control>
            <div class="setting-control">
                <SliderRoot
                    :model-value="[modelValue]"
                    :min="min"
                    :max="max"
                    :step="step"
                    @update:model-value="handleSliderChange"
                    class="setting-slider-root"
                >
                    <SliderTrack class="setting-slider-track">
                        <SliderRange class="setting-slider-range" />
                    </SliderTrack>
                    <SliderThumb class="setting-slider-thumb" />
                </SliderRoot>
                <span class="setting-value">
                    {{ formatValue(modelValue) }}
                </span>
            </div>
        </template>
    </BaseSetting>
</template>

<script setup lang="ts">
import { SliderRange, SliderRoot, SliderThumb, SliderTrack } from "radix-vue";
import BaseSetting from "./BaseSetting.vue";

interface Props {
    settingName: string;
    requiresRestart?: boolean;
    modelValue: number;
    min?: number;
    max?: number;
    step?: number;
    formatValue?: (value: number) => string;
}

withDefaults(defineProps<Props>(), {
    requiresRestart: false,
    min: 0,
    max: 100,
    step: 1,
    formatValue: (value: number) => value.toString(),
});

const emit = defineEmits<{
    "update:modelValue": [value: number];
}>();

function handleSliderChange(value: number[] | undefined) {
    if (value && value[0] !== undefined) {
        emit("update:modelValue", value[0]);
    }
}
</script>

<style lang="scss" scoped>
@import "./styles/settings";
@import "./styles/variables";
@import "@/styles/variables";

// Radix Slider Styles - Using :deep() for third-party component styling
:deep(.setting-slider-root) {
    position: relative;
    display: flex;
    align-items: center;
    flex: 1;
    height: 20px;
    cursor: pointer;
    min-width: 120px;
    touch-action: none;
    user-select: none;
}

:deep(.setting-slider-track) {
    background: $color-border-primary;
    position: relative;
    flex: 1;
    height: 6px;
    border-radius: 3px;
    transition: $transition-rapid;
}

:deep(.setting-slider-range) {
    position: absolute;
    background: $color-accent-primary;
    height: 100%;
    border-radius: 3px;
    transition: $transition-rapid;
}

:deep(.setting-slider-thumb) {
    display: block;
    width: 18px;
    height: 18px;
    background: $slider-thumb-background;
    border: 2px solid $color-accent-primary;
    border-radius: 50%;
    box-shadow: $shadow-light;
    cursor: pointer;
    transition: $transition-rapid;

    &:hover {
        border-color: $color-accent-primary-dark;
        transform: scale(1.1);
        box-shadow: $shadow-medium;
    }

    &:focus {
        outline: none;
        box-shadow: $shadow-focus;
        transform: scale(1.1);
    }

    &[data-state="dragging"] {
        transform: scale(1.15);
        box-shadow: $shadow-medium;
    }
}
</style>
