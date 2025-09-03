<template>
    <BaseSetting
        :setting-name="settingName"
        :requires-restart="requiresRestart"
    >
        <template #control>
            <div class="setting-control setting-control--vertical">
                <KeybindInput
                    :model-value="modelValue"
                    @update:model-value="$emit('update:modelValue', $event)"
                />
            </div>
        </template>
    </BaseSetting>
</template>

<script setup lang="ts">
import KeybindInput from "@/components/common/KeybindInput.vue";
import BaseSetting from "./BaseSetting.vue";

interface KeybindValue {
    modifiers: string;
    key: string;
}

interface Props {
    settingName: string;
    requiresRestart?: boolean;
    modelValue: KeybindValue;
}

withDefaults(defineProps<Props>(), {
    requiresRestart: false,
});

defineEmits<{
    "update:modelValue": [value: KeybindValue];
}>();
</script>

<style lang="scss" scoped>
@import "./styles/settings";
@import "@/styles/variables";
</style>
