import { ref, computed } from "vue";

// Window types that can be opened
export type WindowType = "settings" | "editNote" | null;

// Global window state
const currentWindow = ref<WindowType>(null);
const editingNoteData = ref<{
    id: string;
    title: string;
    content: string;
} | null>(null);

// Composable for window management
export function useWindowManager() {
    const isWindowOpen = computed(() => currentWindow.value !== null);
    const activeWindow = computed(() => currentWindow.value);

    const openWindow = (windowType: WindowType, data?: any) => {
        // Store note data if opening edit note window
        if (windowType === "editNote" && data) {
            editingNoteData.value = data;
        }

        // Close any existing window and open the new one
        currentWindow.value = windowType;
    };

    const closeWindow = () => {
        currentWindow.value = null;
        // Clear note data when closing
        if (editingNoteData.value) {
            editingNoteData.value = null;
        }
    };

    const isWindowActive = (windowType: WindowType) => {
        return currentWindow.value === windowType;
    };

    const getEditingNote = () => {
        return editingNoteData.value;
    };

    return {
        isWindowOpen,
        activeWindow,
        openWindow,
        closeWindow,
        isWindowActive,
        getEditingNote,
    };
}
