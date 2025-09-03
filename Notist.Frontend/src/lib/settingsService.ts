import type { AppSettings } from "@/types/backendConnectors";

export class SettingsService {
    private static isWebView2Available(): boolean {
        const hasWindow = typeof window !== "undefined";
        const hasChrome = hasWindow && !!window.chrome;
        const hasWebview = hasChrome && !!window.chrome.webview;
        const hasHostObjects =
            hasWebview && !!window.chrome.webview.hostObjects;
        const hasSettingsConnector =
            hasHostObjects &&
            !!window.chrome.webview.hostObjects.settings_connector;

        return (
            hasWindow &&
            hasChrome &&
            hasWebview &&
            hasHostObjects &&
            hasSettingsConnector
        );
    }

    /**
     * Gets all settings from the backend
     */
    static async getSettings(): Promise<AppSettings> {
        if (!SettingsService.isWebView2Available()) {
            return {
                EnableNotifications: true,
                OpenOnLaunch: false,
                Theme: "Auto",
                Transparency: 0.95,
                HotkeyModifiers: "Control",
                HotkeyKey: "Space",
            };
        }

        try {
            const result =
                await window.chrome.webview.hostObjects.settings_connector.GetSettings();

            // Check if result is null or undefined
            if (!result) {
                throw new Error(
                    "Settings connector returned null result. The backend may not be properly initialized."
                );
            }

            // Await the proxy properties to get actual values
            const success = await result.Success;
            const error = await result.Error;
            const data = await result.Data;

            if (!success) {
                throw new Error(error || "Failed to get settings");
            }

            if (!data) {
                return {
                    EnableNotifications: true,
                    OpenOnLaunch: false,
                    Theme: "Auto",
                    Transparency: 0.95,
                    HotkeyModifiers: "Control",
                    HotkeyKey: "Space",
                };
            }

            const settings = JSON.parse(data);
            return settings as AppSettings;
        } catch (error) {
            console.error("Failed to get settings from backend:", error);
            throw error;
        }
    }

    /**
     * Updates settings in the backend and saves to file
     */
    static async updateSettings(
        settings: Partial<AppSettings>
    ): Promise<boolean> {
        if (!SettingsService.isWebView2Available()) {
            console.log("Settings update (dev mode):", settings);
            return true;
        }

        try {
            const settingsJson = JSON.stringify(settings);
            const result =
                await window.chrome.webview.hostObjects.settings_connector.UpdateSettings(
                    settingsJson
                );

            // Check if result is null or undefined
            if (!result) {
                console.error(
                    "Settings connector returned null result for UpdateSettings. The backend may not be properly initialized."
                );
                return false;
            }

            return await result.Success;
        } catch (error) {
            console.error("Failed to update settings:", error);
            return false;
        }
    }
}
