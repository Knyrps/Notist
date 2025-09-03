import { settingsStore, GlobalSettingsStore } from "./settingsStore";

/**
 * Composable for accessing global settings
 * Use this in any component that needs to read or react to settings changes
 */
export function useSettings() {
    return {
        // Reactive settings state
        settings: settingsStore.current,

        // Original settings (for comparison)
        originalSettings: settingsStore.original,

        // State flags
        loading: settingsStore.loading,
        loadError: settingsStore.loadError,
        saving: settingsStore.saving,
        lastSaveError: settingsStore.lastSaveError,

        // Methods
        loadSettings: GlobalSettingsStore.loadSettings,
        hasChanges: GlobalSettingsStore.hasChanges,
        saveToFile: GlobalSettingsStore.saveSettingsToFile,
        cancelChanges: GlobalSettingsStore.cancelChanges,

        // Individual setting updaters (in-memory)
        setEnableNotifications: GlobalSettingsStore.setEnableNotifications,
        setOpenOnLaunch: GlobalSettingsStore.setOpenOnLaunch,
        setTheme: GlobalSettingsStore.setTheme,
        setTransparency: GlobalSettingsStore.setTransparency,
        setHotkeyModifiers: GlobalSettingsStore.setHotkeyModifiers,
        setHotkeyKey: GlobalSettingsStore.setHotkeyKey,
    };
}
