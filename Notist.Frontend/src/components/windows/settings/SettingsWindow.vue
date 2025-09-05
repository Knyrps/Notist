<template>
    <Window
        title="Settings"
        width="slim"
        :before-close="beforeClose"
        @close="onClose"
    >
        <div v-if="loading" class="loading-state">
            <p>Loading settings...</p>
        </div>
        <div v-else-if="loadError" class="error-state">
            <p>Failed to load settings. Using default values.</p>
            <button
                class="button button--secondary"
                @click="loadSettingsWithErrorHandling"
            >
                Retry Loading
            </button>
        </div>
        <div v-else>
            <SettingsSection name="Appearance">
                <template #children>
                    <SelectSetting
                        setting-name="Theme"
                        v-model="settings.Theme"
                        :options="THEME_OPTIONS"
                        placeholder="Select theme..."
                        @update:model-value="onThemeChange"
                    />
                    <SliderSetting
                        setting-name="Transparency"
                        v-model="settings.Transparency"
                        :min="0.4"
                        :max="1"
                        :step="0.05"
                        :format-value="(value: number) => Math.round(value * 100) + '%'"
                        @update:model-value="onTransparencyChange"
                    />
                </template>
            </SettingsSection>

            <SettingsSection name="Behavior">
                <template #children>
                    <SwitchSetting
                        setting-name="Show on startup"
                        v-model="settings.OpenOnLaunch"
                        @update:model-value="onOpenOnLaunchChange"
                    />
                    <SwitchSetting
                        setting-name="Enable notifications"
                        v-model="settings.EnableNotifications"
                        @update:model-value="onNotificationsChange"
                    />
                </template>
            </SettingsSection>

            <SettingsSection name="Shortcuts">
                <template #children>
                    <KeybindSetting
                        setting-name="Global Hotkey"
                        :model-value="{
                            modifiers: settings.HotkeyModifiers,
                            key: settings.HotkeyKey,
                        }"
                        :requires-restart="true"
                        @update:model-value="onKeybindChange"
                    />
                </template>
            </SettingsSection>
        </div>

        <template #footer>
            <button class="button button--secondary" @click="cancelChanges">
                Cancel
            </button>
            <button
                class="button button--primary"
                @click="saveChanges"
                :disabled="saving || !hasChanges()"
            >
                {{
                    saving
                        ? "Saving..."
                        : hasChanges()
                        ? "Save Changes"
                        : "No Changes"
                }}
            </button>
        </template>
    </Window>
</template>

<script setup lang="ts">
import { onMounted } from "vue";
import Window from "@/components/windows/Window.vue";
import SelectSetting from "@/components/windows/settings/SelectSetting.vue";
import SliderSetting from "@/components/windows/settings/SliderSetting.vue";
import SwitchSetting from "@/components/windows/settings/SwitchSetting.vue";
import KeybindSetting from "@/components/windows/settings/KeybindSetting.vue";
import SettingsSection from "@/components/windows/settings/SettingsSection.vue";
import { useSettings } from "@/lib/useSettings";
import { useAlertManager } from "@/lib/alertManager";
import { THEME_OPTIONS } from "@/lib/themeService";

// shared settings styles
import "@/components/windows/settings/styles/settings.scss";
import { faTrashCan } from "@fortawesome/free-solid-svg-icons";

const emit = defineEmits<{
    close: [];
}>();

const { showWarningDialog, showConfirmDialog } = useAlertManager();
const {
    settings,
    loading,
    loadError,
    saving,
    lastSaveError,
    loadSettings,
    hasChanges,
    saveToFile,
    cancelChanges: cancelSettingsChanges,
    setEnableNotifications,
    setOpenOnLaunch,
    setTheme,
    setTransparency,
    setHotkeyModifiers,
    setHotkeyKey,
} = useSettings();

onMounted(async () => {
    await loadSettingsWithErrorHandling();
});

const loadSettingsWithErrorHandling = async () => {
    try {
        await loadSettings();
    } catch (error) {
        console.error("Failed to load settings:", error);

        // Show error to user with retry option
        showWarningDialog(
            "Settings Load Error",
            "Failed to load current settings. Default values will be used. You can try refreshing the settings or contact support if the problem persists.",
            () => loadSettingsWithErrorHandling(), // retry loading
            () => {} // continue with defaults
        );
    }
};

/**
 * BeforeClose callback that validates if the window can be closed
 * Returns false to prevent closing
 */
const beforeClose = (): boolean => {
    // Prevent closing if there's a recent save error
    if (lastSaveError) {
        showWarningDialog(
            "Save Error",
            "There was an error saving your last change. Please resolve the error before closing or your changes may be lost."
        );
        return false;
    }

    // If there are unsaved changes, confirm before closing
    if (hasChanges()) {
        showConfirmDialog(
            "Unsaved Changes",
            "You have unsaved changes. Are you sure you want to close without saving?",
            async () => {
                await cancelSettingsChanges();
                emit("close");
            }, // onConfirm - close anyway
            () => {}, // onCancel - stay open
            faTrashCan
        );
        return false;
    }

    // No issues, allow closing
    return true;
};

/**
 * Handles the close event from the Window component
 */
const onClose = () => {
    emit("close");
};

// Handle individual setting changes for immediate feedback
const onNotificationsChange = async () => {
    try {
        await setEnableNotifications(settings.EnableNotifications);
    } catch (error) {
        console.error("Failed to update notifications setting:", error);

        showWarningDialog(
            "Settings Error",
            "Failed to update notification settings. Please try again."
        );
    }
};

const onOpenOnLaunchChange = async () => {
    try {
        await setOpenOnLaunch(settings.OpenOnLaunch);
    } catch (error) {
        console.error("Failed to update open on launch setting:", error);

        showWarningDialog(
            "Settings Error",
            "Failed to update startup settings. Please try again."
        );
    }
};

const onThemeChange = async () => {
    try {
        await setTheme(settings.Theme);
    } catch (error) {
        console.error("Failed to update theme setting:", error);

        showWarningDialog(
            "Settings Error",
            "Failed to update theme setting. Please try again."
        );
    }
};

const onTransparencyChange = async (value?: number) => {
    try {
        const transparencyValue = value ?? settings.Transparency;
        // Update the settings object when using the component
        if (value !== undefined) {
            settings.Transparency = value;
        }
        await setTransparency(transparencyValue);
    } catch (error) {
        console.error("Failed to update transparency setting:", error);

        showWarningDialog(
            "Settings Error",
            "Failed to update transparency setting. Please try again."
        );
    }
};

const onKeybindChange = (value: { modifiers: string; key: string }) => {
    try {
        setHotkeyModifiers(value.modifiers);
        setHotkeyKey(value.key);
    } catch (error) {
        console.error("Failed to update keybind setting:", error);
        showWarningDialog(
            "Settings Error",
            "Failed to update keybind setting. Please try again."
        );
    }
};

const saveChanges = async () => {
    try {
        await saveToFile();
        // Close the settings window after successful save
        emit("close");
    } catch (error) {
        console.error("Error saving settings:", error);

        showWarningDialog(
            "Save Error",
            "Failed to save settings to file. Please try again."
        );
    }
};

const cancelChanges = async () => {
    // Check if there are unsaved changes
    if (hasChanges()) {
        showConfirmDialog(
            "Discard Changes",
            "Are you sure you want to discard your changes?",
            async () => {
                await cancelSettingsChanges();
                emit("close");
            },
            () => {},
            faTrashCan
        );
    } else {
        // No changes, close directly
        emit("close");
    }
};
</script>

<style lang="scss">
@import "@/styles/variables";
@import "./styles/settings";

.loading-state,
.error-state {
    padding: 2rem;
    text-align: center;

    p {
        margin: 0 0 1rem 0;
        color: $color-text-primary;
        font-size: 0.875rem;
    }
}

.error-state {
    .button {
        margin-top: 1rem;
    }
}
</style>
