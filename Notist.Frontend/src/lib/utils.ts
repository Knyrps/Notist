/**
 * Global utility functions for the Notist frontend
 */

/**
 * Creates a debounced version of a function that delays its execution until after
 * a specified delay has passed since its last invocation
 * @param func The function to debounce
 * @param delay The delay in milliseconds
 * @returns The debounced function
 */
export function debounce<T extends (...args: any[]) => any>(
    func: T,
    delay: number
): (...args: Parameters<T>) => void {
    let timeoutId: ReturnType<typeof setTimeout>;

    return (...args: Parameters<T>) => {
        clearTimeout(timeoutId);
        timeoutId = setTimeout(() => func(...args), delay);
    };
}

/**
 * Creates an async debounced version of a function that delays its execution until after
 * a specified delay has passed since its last invocation
 * @param func The async function to debounce
 * @param delay The delay in milliseconds
 * @returns The debounced async function
 */
export function debounceAsync<T extends (...args: any[]) => Promise<any>>(
    func: T,
    delay: number
): (...args: Parameters<T>) => Promise<void> {
    let timeoutId: ReturnType<typeof setTimeout>;

    return (...args: Parameters<T>) => {
        return new Promise<void>((resolve) => {
            clearTimeout(timeoutId);
            timeoutId = setTimeout(async () => {
                await func(...args);
                resolve();
            }, delay);
        });
    };
}

/**
 * Clamps a number between a minimum and maximum value
 * @param value The value to clamp
 * @param min The minimum value
 * @param max The maximum value
 * @returns The clamped value
 */
export function clamp(value: number, min: number, max: number): number {
    return Math.min(Math.max(value, min), max);
}

/**
 * Checks if we're running in a WebView2 environment
 * @returns True if WebView2 host objects are available
 */
export function isWebView2Available(): boolean {
    return !!(
        typeof window !== "undefined" &&
        window.chrome &&
        window.chrome.webview &&
        window.chrome.webview.hostObjects
    );
}
