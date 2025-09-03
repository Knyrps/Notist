<template>
    <Teleport to="body">
        <div
            v-if="isAlertOpen"
            class="alert-overlay"
            @click="handleOverlayClick"
        >
            <div class="alert-container" @click.stop>
                <Alert
                    v-if="currentAlert"
                    :title="currentAlert.title"
                    :description="currentAlert.description"
                    :confirm-text="currentAlert.confirmText"
                    :cancel-text="currentAlert.cancelText"
                    :show-cancel="currentAlert.showCancel"
                    :icon="currentAlert.icon"
                    :type="currentAlert.type"
                    @confirm="handleConfirm"
                    @cancel="handleCancel"
                />
            </div>
        </div>
    </Teleport>
</template>

<script setup lang="ts">
import { useAlertManager } from "@/lib/alertManager";
import Alert from "./Alert.vue";

const { isAlertOpen, currentAlert, closeAlert } = useAlertManager();

const handleOverlayClick = () => {
    closeAlert();
};

const handleConfirm = () => {
    if (currentAlert.value?.onConfirm) {
        currentAlert.value.onConfirm();
    }
    closeAlert();
};

const handleCancel = () => {
    if (currentAlert.value?.onCancel) {
        currentAlert.value.onCancel();
    }
    closeAlert();
};
</script>

<style lang="scss">
@import "@/styles/_fonts.scss";
@import "@/styles/variables";

.alert-overlay {
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background: rgba(0, 0, 0, 0.5);
    backdrop-filter: blur(4px);
    z-index: 3000;
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 2rem;
    box-sizing: border-box;
}

.alert-container {
    display: flex;
    align-items: center;
    justify-content: center;
}
</style>
