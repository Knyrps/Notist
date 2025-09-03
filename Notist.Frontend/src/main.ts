import { createApp, nextTick } from "vue";
import "./style.scss";
import "normalize.css";
import App from "./App.vue";
import { GlobalSettingsStore } from "./lib/settingsStore";
import { ThemeService } from "./lib/themeService";

ThemeService.initialize("Auto");

const app = createApp(App);

GlobalSettingsStore.loadSettings().catch((error) => {
    console.error("Failed to load initial settings:", error);
});

app.mount("#app");

nextTick(() => {
    window.dispatchEvent(new Event("app:ready"));
});
