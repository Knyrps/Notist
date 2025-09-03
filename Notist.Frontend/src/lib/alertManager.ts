import {
    faPowerOff,
    type IconDefinition,
} from "@fortawesome/free-solid-svg-icons";
import { ref } from "vue";

export interface AlertConfig {
    title: string;
    description: string;
    confirmText?: string;
    cancelText?: string;
    showCancel?: boolean;
    icon?: IconDefinition;
    onConfirm?: () => void;
    onCancel?: () => void;
    type?: AlertType;
}

export type AlertType = "success" | "error" | "warning";

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
        onCancel?: () => void,
        icon?: IconDefinition,
        type?: AlertType
    ) => {
        showAlert({
            title,
            description,
            confirmText: "Confirm",
            cancelText: "Cancel",
            showCancel: true,
            type: type || "warning",
            onConfirm,
            onCancel,
            icon,
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
            icon: faPowerOff,
            onConfirm,
            onCancel,
            type: "error",
        });
    };

    const showWarningDialog = (
        title: string,
        description: string,
        onConfirm?: () => void,
        onCancel?: () => void,
        icon?: IconDefinition,
        type?: AlertType
    ) => {
        showAlert({
            title,
            description,
            confirmText: "OK",
            cancelText: "Cancel",
            showCancel: true,
            type: type || "warning",
            onConfirm,
            onCancel,
            icon,
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
