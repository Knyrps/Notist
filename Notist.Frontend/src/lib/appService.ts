import { isWebView2Available } from "./utils";

export class AppService {
    /**
     * Updates the UI transparency immediately for preview purposes
     */
    static async updateTransparencyPreview(
        transparency: number
    ): Promise<void> {
        if (!isWebView2Available()) {
            console.log(
                "Update transparency preview (dev mode):",
                transparency
            );
            return;
        }

        try {
            const transparencyJson = JSON.stringify(transparency);
            const result =
                await window.chrome.webview.hostObjects.app_connector.UpdateTransparency(
                    transparencyJson
                );

            if (result) {
                const success = await result.Success;
                if (!success) {
                    const error = await result.Error;
                    console.error("Failed to update UI transparency:", error);
                }
            }
        } catch (error) {
            console.error("Error updating UI transparency:", error);
        }
    }

    /**
     * Toggles the overlay window visibility
     */
    static async toggleOverlay(): Promise<void> {
        if (!isWebView2Available()) {
            console.log("Toggle overlay (dev mode)");
            return;
        }

        try {
            const result =
                await window.chrome.webview.hostObjects.app_connector.ToggleOverlay();

            if (result) {
                const success = await result.Success;
                if (!success) {
                    const error = await result.Error;
                    console.error("Failed to toggle overlay:", error);
                }
            }
        } catch (error) {
            console.error("Error toggling overlay:", error);
        }
    }

    /**
     * Quits the application
     */
    static async quitApplication(): Promise<void> {
        if (!isWebView2Available()) {
            console.log("Quit application (dev mode)");
            return;
        }

        try {
            const result =
                await window.chrome.webview.hostObjects.app_connector.QuitApplication();

            if (result) {
                const success = await result.Success;
                if (!success) {
                    const error = await result.Error;
                    console.error("Failed to quit application:", error);
                }
            }
        } catch (error) {
            console.error("Error quitting application:", error);
        }
    }

    /**
     * Pauses the global hotkey registration
     */
    static async pauseHotkey(): Promise<void> {
        if (!isWebView2Available()) {
            console.log("Pause hotkey (dev mode)");
            return;
        }

        try {
            const result =
                await window.chrome.webview.hostObjects.app_connector.PauseHotkey();

            if (result) {
                const success = await result.Success;
                if (!success) {
                    const error = await result.Error;
                    console.error("Failed to pause hotkey:", error);
                }
            }
        } catch (error) {
            console.error("Error pausing hotkey:", error);
        }
    }

    /**
     * Resumes the global hotkey registration
     */
    static async resumeHotkey(): Promise<void> {
        if (!isWebView2Available()) {
            console.log("Resume hotkey (dev mode)");
            return;
        }

        try {
            const result =
                await window.chrome.webview.hostObjects.app_connector.ResumeHotkey();

            if (result) {
                const success = await result.Success;
                if (!success) {
                    const error = await result.Error;
                    console.error("Failed to resume hotkey:", error);
                }
            }
        } catch (error) {
            console.error("Error resuming hotkey:", error);
        }
    }
}
