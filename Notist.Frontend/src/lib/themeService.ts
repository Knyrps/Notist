/**
 * Theme management service for Notist
 * Handles theme switching and application to the document
 */

export type ThemeMode = "Light" | "Dark" | "Auto";

export interface ThemeOption {
    value: ThemeMode;
    label: string;
}

export const THEME_OPTIONS: ThemeOption[] = [
    { value: "Light", label: "Light" },
    { value: "Dark", label: "Dark" },
    { value: "Auto", label: "Auto" },
];

export class ThemeService {
    private static currentTheme: ThemeMode = "Auto";

    /**
     * Applies the specified theme to the document
     * @param theme The theme to apply
     */
    static applyTheme(theme: ThemeMode): void {
        ThemeService.currentTheme = theme;

        document.documentElement.removeAttribute("data-theme");

        switch (theme) {
            case "Light":
                document.documentElement.setAttribute("data-theme", "light");
                break;
            case "Dark":
                document.documentElement.setAttribute("data-theme", "dark");
                break;
            case "Auto":
                document.documentElement.setAttribute("data-theme", "auto");
                break;
            default:
                console.warn("Unknown theme:", theme);
                document.documentElement.setAttribute("data-theme", "auto");
        }

        console.log("Theme applied:", theme);
    }

    /**
     * Gets the current theme
     */
    static getCurrentTheme(): ThemeMode {
        return ThemeService.currentTheme;
    }

    /**
     * Gets the effective theme based on system preference (for Auto mode)
     */
    static getEffectiveTheme(): "Light" | "Dark" {
        if (ThemeService.currentTheme === "Auto") {
            return window.matchMedia("(prefers-color-scheme: dark)").matches
                ? "Dark"
                : "Light";
        }
        return ThemeService.currentTheme as "Light" | "Dark";
    }

    /**
     * Initializes theme system and sets up system preference listeners
     */
    static initialize(initialTheme: ThemeMode = "Auto"): void {
        ThemeService.applyTheme(initialTheme);

        const mediaQuery = window.matchMedia("(prefers-color-scheme: dark)");
        mediaQuery.addEventListener("change", () => {
            if (ThemeService.currentTheme === "Auto") {
                document.documentElement.removeAttribute("data-theme");
                setTimeout(() => {
                    document.documentElement.setAttribute("data-theme", "auto");
                }, 10);
            }
        });

        console.log("Theme system initialized with:", initialTheme);
    }
}
