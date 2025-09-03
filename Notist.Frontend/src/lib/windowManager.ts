import { ref, computed } from "vue";

// Window types that can be opened
export type WindowType = "settings" | null;

// Global window state
const currentWindow = ref<WindowType>(null);

// Composable for window management
export function useWindowManager() {
    const isWindowOpen = computed(() => currentWindow.value !== null);
    const activeWindow = computed(() => currentWindow.value);

    const openWindow = (windowType: WindowType) => {
        // Close any existing window and open the new one
        currentWindow.value = windowType;
    };

    const closeWindow = () => {
        currentWindow.value = null;
    };

    const isWindowActive = (windowType: WindowType) => {
        return currentWindow.value === windowType;
    };

    return {
        isWindowOpen,
        activeWindow,
        openWindow,
        closeWindow,
        isWindowActive,
    };
}
