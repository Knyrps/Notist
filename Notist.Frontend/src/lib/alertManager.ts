import { ref } from "vue";

export interface AlertConfig {
    title: string;
    description: string;
    confirmText?: string;
    cancelText?: string;
    showCancel?: boolean;
    icon?: any;
    onConfirm?: () => void;
    onCancel?: () => void;
}

const isAlertOpen = ref(false);
const currentAlert = ref<AlertConfig | null>(null);

export function useAlertManager() {
    const showAlert = (config: AlertConfig) => {
        currentAlert.value = {
            confirmText: "Confirm",
            cancelText: "Cancel",
            showCancel: true,
            ...config,
        };
        isAlertOpen.value = true;
    };

    const closeAlert = () => {
        isAlertOpen.value = false;
        currentAlert.value = null;
    };

    const showConfirmDialog = (
        title: string,
        description: string,
        onConfirm?: () => void,
        onCancel?: () => void
    ) => {
        showAlert({
            title,
            description,
            confirmText: "Confirm",
            cancelText: "Cancel",
            showCancel: true,
            onConfirm,
            onCancel,
        });
    };

    const showQuitDialog = (onConfirm?: () => void, onCancel?: () => void) => {
        showAlert({
            title: "Quit Application",
            description:
                "Are you sure you want to quit the application? This will close all windows and end the session.",
            confirmText: "Quit",
            cancelText: "Cancel",
            showCancel: true,
            onConfirm,
            onCancel,
        });
    };

    const showWarningDialog = (
        title: string,
        description: string,
        onConfirm?: () => void,
        onCancel?: () => void
    ) => {
        showAlert({
            title,
            description,
            confirmText: "OK",
            cancelText: "Cancel",
            showCancel: true,
            onConfirm,
            onCancel,
        });
    };

    return {
        isAlertOpen,
        currentAlert,
        showAlert,
        closeAlert,
        showConfirmDialog,
        showQuitDialog,
        showWarningDialog,
    };
}
