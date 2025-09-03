// Backend connector types for WebView2 host objects

export interface ConnectorInvocationResult {
    Success: boolean;
    Error?: string;
    Data?: string; // For JSON payloads
}

export interface AppConnector {
    ShowMessage(msg: string): ConnectorInvocationResult;
    ToggleOverlay(): ConnectorInvocationResult;
    GetVersion(): ConnectorInvocationResult;
    QuitApplication(): ConnectorInvocationResult;
    UpdateTransparency(transparencyJson: string): ConnectorInvocationResult;
    PauseHotkey(): ConnectorInvocationResult;
    ResumeHotkey(): ConnectorInvocationResult;
}

export interface NotistConnector {
    GetContext(): ConnectorInvocationResult;
    CreateEmptyNote(): ConnectorInvocationResult;
}

export interface SettingsConnector {
    GetSettings(): ConnectorInvocationResult;
    UpdateSettings(settingsJson: string): ConnectorInvocationResult;
}

export interface AppSettings {
    EnableNotifications: boolean;
    OpenOnLaunch: boolean;
    Theme: string;
    Transparency: number;
    HotkeyModifiers: string;
    HotkeyKey: string;
}

export const SettingKeys = {
    EnableNotifications: "enablenotifications",
    OpenOnLaunch: "openonlaunch",
    Theme: "theme",
    Transparency: "transparency",
    HotkeyModifiers: "hotkeymodifiers",
    HotkeyKey: "hotkeykey",
} as const;

export type SettingKey = (typeof SettingKeys)[keyof typeof SettingKeys];

export interface ApplicationContext {
    // Add properties as they are implemented in the backend
}

// Extend the window object to include our host objects
declare global {
    interface Window {
        chrome: {
            webview: {
                hostObjects: {
                    app_connector: AppConnector;
                    notist_connector: NotistConnector;
                    settings_connector: SettingsConnector;
                };
            };
        };
    }
}
