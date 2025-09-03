import { reactive } from "vue";
import { SettingsService } from "./settingsService";
import { AppService } from "./appService";
import { ThemeService, type ThemeMode } from "./themeService";
import { debounceAsync, clamp } from "./utils";
import type { AppSettings } from "@/types/backendConnectors";

/**
 * Frontend-only reactive store for settings management
 * All changes are held in memory until explicitly saved or cancelled
 */
interface SettingsStore {
    current: AppSettings;
    original: AppSettings;
    loading: boolean;
    loadError: string | null;
    saving: boolean;
    lastSaveError: string | null;
}

// Default settings
const defaultSettings: AppSettings = {
    EnableNotifications: true,
    OpenOnLaunch: false,
    Theme: "Auto",
    Transparency: 0.95,
    HotkeyModifiers: "Control",
    HotkeyKey: "Space",
};

export const settingsStore = reactive<SettingsStore>({
    current: { ...defaultSettings },
    original: { ...defaultSettings },
    loading: false,
    loadError: null,
    saving: false,
    lastSaveError: null,
});

// Debounced transparency preview function (300ms delay)
const debouncedTransparencyPreview = debounceAsync(
    AppService.updateTransparencyPreview,
    300
);

/**
 * Global settings store with all operations
 * This is the single source of truth for settings state management
 */
export class GlobalSettingsStore {
    /**
     * Loads settings from backend and resets frontend state
     */
    static async loadSettings(): Promise<void> {
        settingsStore.loading = true;
        settingsStore.loadError = null;

        try {
            const settings = await SettingsService.getSettings();

            Object.assign(settingsStore.current, settings);
            Object.assign(settingsStore.original, settings);

            ThemeService.applyTheme(settings.Theme as ThemeMode);

            console.log("Settings loaded:", settings);
        } catch (error) {
            const errorMessage =
                error instanceof Error ? error.message : "Unknown error";
            settingsStore.loadError = errorMessage;
            console.error("Failed to load settings:", error);
        } finally {
            settingsStore.loading = false;
        }
    }

    /**
     * Checks if there are any unsaved changes
     */
    static hasChanges(): boolean {
        return (
            JSON.stringify(settingsStore.current) !==
            JSON.stringify(settingsStore.original)
        );
    }

    /**
     * Saves all current settings to file via backend
     */
    static async saveSettingsToFile(): Promise<boolean> {
        if (!GlobalSettingsStore.hasChanges()) {
            console.log("No changes to save");
            return true;
        }

        settingsStore.saving = true;
        settingsStore.lastSaveError = null;

        try {
            const success = await SettingsService.updateSettings(
                settingsStore.current
            );

            if (success) {
                Object.assign(settingsStore.original, settingsStore.current);
                console.log("Settings saved successfully");
                return true;
            } else {
                settingsStore.lastSaveError = "Failed to save settings";
                return false;
            }
        } catch (error) {
            const errorMessage =
                error instanceof Error ? error.message : "Unknown error";
            settingsStore.lastSaveError = errorMessage;
            console.error("Failed to save settings:", error);
            return false;
        } finally {
            settingsStore.saving = false;
        }
    }

    /**
     * Cancels all changes and resets to original settings
     */
    static async cancelChanges(): Promise<void> {
        Object.assign(settingsStore.current, settingsStore.original);

        ThemeService.applyTheme(settingsStore.original.Theme as ThemeMode);

        try {
            await AppService.updateTransparencyPreview(
                settingsStore.original.Transparency
            );
        } catch (error) {
            console.error("Failed to reset transparency preview:", error);
        }

        console.log("Settings changes cancelled");
    }

    /**
     * Updates EnableNotifications setting in memory only
     */
    static setEnableNotifications(value: boolean): void {
        settingsStore.current.EnableNotifications = value;
    }

    /**
     * Updates OpenOnLaunch setting in memory only
     */
    static setOpenOnLaunch(value: boolean): void {
        settingsStore.current.OpenOnLaunch = value;
    }

    /**
     * Updates Theme setting in memory and applies it immediately
     */
    static setTheme(value: string): void {
        settingsStore.current.Theme = value;

        ThemeService.applyTheme(value as ThemeMode);
    }
    /**
     * Updates Transparency setting in memory and triggers debounced preview
     */
    static setTransparency(value: number): void {
        const clampedValue = clamp(value, 0.1, 1.0);
        settingsStore.current.Transparency = clampedValue;

        debouncedTransparencyPreview(clampedValue).catch((error) => {
            console.error("Failed to preview transparency:", error);
        });
    }

    /**
     * Updates HotkeyModifiers setting in memory only
     */
    static setHotkeyModifiers(value: string): void {
        settingsStore.current.HotkeyModifiers = value;
    }

    /**
     * Updates HotkeyKey setting in memory only
     */
    static setHotkeyKey(value: string): void {
        settingsStore.current.HotkeyKey = value;
    }
}
