<template>
    <div class="setting-item">
        <label>
            {{ settingName }}
            <div class="setting-info" v-if="requiresRestart">
                Requires restart to take effect
            </div>
        </label>
        <!-- Control slot for the specific setting input -->
        <slot name="control" />
    </div>
</template>

<script setup lang="ts">
// Base props that all setting components should have
interface Props {
    settingName: string;
    requiresRestart?: boolean;
}

withDefaults(defineProps<Props>(), {
    requiresRestart: false,
});

// Export the interface for other components to use
export type BaseSettingProps = Props;
</script>

<style lang="scss" scoped>
@import "./styles/settings";
</style>
