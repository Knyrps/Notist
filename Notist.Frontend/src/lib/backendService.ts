import type {
    ConnectorInvocationResult,
    AppConnector,
    NotistConnector,
    SettingsConnector,
} from "@/types/backendConnectors";

export class BackendService {
    private static isWebView2Available(): boolean {
        return (
            typeof window !== "undefined" &&
            !!window.chrome &&
            !!window.chrome.webview &&
            !!window.chrome.webview.hostObjects
        );
    }

    /**
     * Helper method to handle connector results consistently
     */
    private static async handleConnectorResult(
        result: ConnectorInvocationResult,
        allowNullData: boolean = false
    ): Promise<string | null> {
        // Await proxy properties to get actual values
        const success = await result.Success;
        const error = await result.Error;
        const data = await result.Data;

        if (!success) {
            throw new Error(error || "Unknown error occurred");
        }

        if (!data && !allowNullData) {
            throw new Error("No data returned from connector");
        }

        return data ?? null;
    }

    /**
     * Gets the app connector if available
     */
    static getAppConnector(): AppConnector | null {
        if (
            !this.isWebView2Available() ||
            !window.chrome.webview.hostObjects.app_connector
        ) {
            return null;
        }
        return window.chrome.webview.hostObjects.app_connector;
    }

    /**
     * Gets the Notist connector if available
     */
    static getNotistConnector(): NotistConnector | null {
        if (
            !this.isWebView2Available() ||
            !window.chrome.webview.hostObjects.notist_connector
        ) {
            return null;
        }
        return window.chrome.webview.hostObjects.notist_connector;
    }

    /**
     * Gets the settings connector if available
     */
    static getSettingsConnector(): SettingsConnector | null {
        if (
            !this.isWebView2Available() ||
            !window.chrome.webview.hostObjects.settings_connector
        ) {
            return null;
        }
        return window.chrome.webview.hostObjects.settings_connector;
    }

    /**
     * Shows a message using the app connector
     */
    static async showMessage(message: string): Promise<void> {
        const connector = this.getAppConnector();
        if (!connector) {
            console.log("Show message (dev mode):", message);
            return;
        }

        try {
            const result = await connector.ShowMessage(message);
            await this.handleConnectorResult(result, true);
        } catch (error) {
            console.error("Failed to show message:", error);
            throw error;
        }
    }

    /**
     * Toggles the overlay using the app connector
     */
    static async toggleOverlay(): Promise<void> {
        const connector = this.getAppConnector();
        if (!connector) {
            console.log("Toggle overlay (dev mode)");
            return;
        }

        try {
            const result = await connector.ToggleOverlay();
            await this.handleConnectorResult(result, true);
        } catch (error) {
            console.error("Failed to toggle overlay:", error);
            throw error;
        }
    }

    /**
     * Gets the application version
     */
    static async getVersion(): Promise<string> {
        const connector = this.getAppConnector();
        if (!connector) {
            return "0.0.0-dev";
        }

        try {
            const result = await connector.GetVersion();
            const version = await this.handleConnectorResult(result);
            return version || "0.0.0-unknown";
        } catch (error) {
            console.error("Failed to get version:", error);
            return "0.0.0-error";
        }
    }

    /**
     * Quits the application using the app connector
     */
    static async quitApplication(): Promise<void> {
        const connector = this.getAppConnector();
        if (!connector) {
            console.log("Quit application (dev mode)");
            return;
        }

        try {
            const result = await connector.QuitApplication();
            await this.handleConnectorResult(result, true);
        } catch (error) {
            console.error("Failed to quit application:", error);
            throw error;
        }
    }
}
