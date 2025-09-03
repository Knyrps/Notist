<template>
    <Window title="Settings" :before-close="beforeClose" @close="onClose">
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
            <div class="settings-section">
                <h3>Appearance</h3>
                <div class="setting-item">
                    <label>Theme:</label>
                    <div>
                        <SelectRoot
                            v-model="settings.Theme"
                            @update:model-value="onThemeChange"
                        >
                            <SelectTrigger class="setting-select">
                                <span class="select-value">{{
                                    settings.Theme || "Select theme..."
                                }}</span>
                                <SelectIcon class="select-icon">
                                    <FontAwesomeIcon :icon="faChevronDown" />
                                </SelectIcon>
                            </SelectTrigger>
                            <SelectContent class="select-content">
                                <SelectViewport>
                                    <SelectItem
                                        value="Light"
                                        class="select-item"
                                    >
                                        Light
                                    </SelectItem>
                                    <SelectItem
                                        value="Dark"
                                        class="select-item"
                                    >
                                        Dark
                                    </SelectItem>
                                    <SelectItem
                                        value="Auto"
                                        class="select-item"
                                    >
                                        Auto
                                    </SelectItem>
                                </SelectViewport>
                            </SelectContent>
                        </SelectRoot>
                    </div>
                </div>
                <div class="setting-item">
                    <label>Transparency:</label>
                    <div class="setting-control">
                        <SliderRoot
                            :model-value="[settings.Transparency]"
                            :min="0.4"
                            :max="1"
                            :step="0.05"
                            @update:model-value="
                                (value) => {
                                    if (value && value[0] !== undefined) {
                                        settings.Transparency = value[0];
                                        onTransparencyChange();
                                    }
                                }
                            "
                            class="setting-slider-root"
                        >
                            <SliderTrack class="setting-slider-track">
                                <SliderRange class="setting-slider-range" />
                            </SliderTrack>
                            <SliderThumb class="setting-slider-thumb" />
                        </SliderRoot>
                        <span class="setting-value">
                            {{ Math.round(settings.Transparency * 100) }}%
                        </span>
                    </div>
                </div>
                <div class="setting-item">
                    <label>Global Hotkey:</label>
                    <div class="setting-control setting-control--vertical">
                        <KeybindInput
                            :model-value="{
                                modifiers: settings.HotkeyModifiers,
                                key: settings.HotkeyKey,
                            }"
                            @update:model-value="onKeybindChange"
                        />
                        <div class="setting-info">
                            Requires restart to take effect
                        </div>
                    </div>
                </div>
            </div>

            <div class="settings-section">
                <h3>Behavior</h3>
                <div class="setting-item">
                    <label class="setting-switch-label">
                        <SwitchRoot
                            v-model:checked="settings.OpenOnLaunch"
                            @update:checked="onOpenOnLaunchChange"
                            class="setting-switch"
                        >
                            <SwitchThumb class="setting-switch-thumb" />
                        </SwitchRoot>
                        Show on startup
                    </label>
                </div>
                <div class="setting-item">
                    <label class="setting-switch-label">
                        <SwitchRoot
                            v-model:checked="settings.EnableNotifications"
                            @update:checked="onNotificationsChange"
                            class="setting-switch"
                        >
                            <SwitchThumb class="setting-switch-thumb" />
                        </SwitchRoot>
                        Enable notifications
                    </label>
                </div>
            </div>

            <div class="settings-section">
                <h3>Shortcuts</h3>
                <div class="setting-item">
                    <label>Toggle overlay:</label>
                    <kbd class="shortcut-key">{{
                        formatKeybind(
                            settings.HotkeyModifiers,
                            settings.HotkeyKey
                        )
                    }}</kbd>
                </div>
                <div class="setting-item">
                    <label>New note:</label>
                    <kbd class="shortcut-key">Ctrl + N</kbd>
                </div>
            </div>
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
import Window from "../layout/Window.vue";
import KeybindInput from "../common/KeybindInput.vue";
import { useSettings } from "@/lib/useSettings";
import { useAlertManager } from "@/lib/alertManager";
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
import { SwitchRoot, SwitchThumb } from "radix-vue";
import { SliderRange, SliderRoot, SliderThumb, SliderTrack } from "radix-vue";

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
            () => loadSettingsWithErrorHandling(), // onConfirm - retry loading
            () => {} // onCancel - continue with defaults
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
            () => {} // onCancel - stay open
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

const formatKeybind = (modifiers: string, key: string): string => {
    const modifierList = modifiers
        .split(",")
        .filter((m) => m.trim())
        .map((m) => {
            switch (m.trim()) {
                case "Control":
                    return "Ctrl";
                case "Alt":
                    return "Alt";
                case "Shift":
                    return "Shift";
                case "Win":
                    return "Win";
                default:
                    return m;
            }
        });

    return modifierList.length > 0
        ? `${modifierList.join(" + ")} + ${key}`
        : key;
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
            () => {} // onCancel - stay open
        );
    } else {
        // No changes, close directly
        emit("close");
    }
};
</script>

<style lang="scss">
@import "@/styles/variables";

.settings-section {
    margin-bottom: 2.5rem;
    padding-bottom: 1.5rem;
    border-bottom: 1px solid $color-border-primary;

    &:last-child {
        margin-bottom: 0;
        padding-bottom: 0;
        border-bottom: none;
    }

    h3 {
        margin: 0 0 1.25rem 0;
        font-size: 1.1rem;
        font-weight: 600;
        color: $color-text-primary;
        letter-spacing: -0.025em;
    }
}

.setting-item {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 1rem;
    gap: 1.5rem;
    min-height: 2.5rem;

    &:last-child {
        margin-bottom: 0;
    }

    > label:first-child {
        font-size: 0.875rem;
        color: $color-text-primary;
        font-weight: 500;
        min-width: 0;
        flex-shrink: 0;
    }

    // Special layout for switch controls
    .setting-switch-label {
        justify-content: flex-start;
        gap: 0;
        width: auto;
        flex: 1;
    }
}

.setting-slider {
    min-width: 120px;
}

.setting-control {
    display: flex;
    align-items: center;
    gap: 0.75rem;
    min-width: 120px;

    &--vertical {
        flex-direction: column;
        align-items: stretch;
        gap: 4px;
    }
}

.setting-info {
    font-size: 0.7rem;
    color: var(--color-text-secondary);
    opacity: 0.7;
    font-weight: 400;
    font-style: italic;
    line-height: 1.3;
}

.setting-value {
    font-size: 0.75rem;
    color: $color-text-secondary;
    min-width: 3rem;
    text-align: right;
    font-weight: 500;
}

.shortcut-key {
    background: $color-bg-primary;
    border: 1px solid $color-border-primary;
    border-radius: $radius-secondary;
    padding: 0.25rem 0.5rem;
    font-size: 0.75rem;
    font-family: monospace;
    color: $color-text-secondary;
}

.button {
    padding: 0.5rem 1rem;
    border-radius: $radius-secondary;
    font-size: 0.875rem;
    font-weight: 500;
    cursor: pointer;
    transition: all $transition-medium;

    &--secondary {
        background: transparent;
        border: 1px solid $color-border-primary;
        color: $color-text-primary;

        &:hover {
            background: $color-bg-hover;
        }
    }

    &--primary {
        background: $color-accent-primary;
        border: 1px solid $color-accent-primary;
        color: white;

        &:hover {
            background: $color-accent-primary-dark;
            border-color: $color-accent-primary-dark;
        }
    }
}

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
        color: white;
        font-weight: 500;

        &::after {
            content: "✓";
            margin-left: auto;
            font-size: 0.75rem;
        }
    }
}

// Radix Slider Styles
.setting-slider-root {
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

.setting-slider-track {
    background: $color-border-primary;
    position: relative;
    flex: 1;
    height: 6px;
    border-radius: 3px;
    transition: $transition-rapid;
}

.setting-slider-range {
    position: absolute;
    background: $color-accent-primary;
    height: 100%;
    border-radius: 3px;
    transition: $transition-rapid;
}

.setting-slider-thumb {
    display: block;
    width: 18px;
    height: 18px;
    background: white;
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

// Radix Switch Styles
.setting-switch {
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

.setting-switch-thumb {
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
    gap: 0;
    transition: $transition-fast;
    width: 100%;
    justify-content: flex-start;

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
