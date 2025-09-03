<template>
    <Teleport to="body">
        <div
            v-if="isWindowOpen"
            class="window-overlay"
            @click="handleOverlayClick"
        >
            <div class="window-container" @click.stop>
                <SettingsWindow
                    v-if="isWindowActive('settings')"
                    @close="closeWindow"
                />
            </div>
        </div>
    </Teleport>
</template>

<script setup lang="ts">
import { useWindowManager } from "@/lib/windowManager";
import SettingsWindow from "../windows/SettingsWindow.vue";

const { isWindowOpen, closeWindow, isWindowActive } = useWindowManager();

const handleOverlayClick = () => {
    closeWindow();
};
</script>

<style lang="scss">
@import "@/styles/variables";

.window-overlay {
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background: rgba(0, 0, 0, 0.4);
    backdrop-filter: blur(4px);
    z-index: 2000;
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 2rem;
    box-sizing: border-box;
}

.window-container {
    max-width: 90vw;
    max-height: 90vh;
    overflow: auto;
}
</style>
