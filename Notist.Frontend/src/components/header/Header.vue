<template>
    <header id="header" class="header drag-snap-down">
        <div class="header__block header__block--left">
            <TitleNav
                text="Notist"
                title="Notist"
                description="Lorem Ipsum Dolor Sit Amet"
                :sections="[
                    // {
                    //     title: 'Controls',
                    //     content:
                    //         'Drag notes around the stage. Hover near the header to snap to top.',
                    // },
                    // {
                    //     title: 'Controls',
                    //     content:
                    //         'Drag notes around the stage. Hover near the header to snap to top.',
                    // },
                ]"
                version="v1.0.0"
                author="developed with ❤️ by <a href='https://github.com/Knyrps/Notist'>@Knyrps</a>"
            />
        </div>
        <div class="header__block header__block--center">
            <CollectionSelector
                :options="options"
                :default="options[0].key"
                label="Collections"
                placeholder="Select a Collection..."
            />
        </div>
        <div class="header__block header__block--right">
            <IconNav :icon="faGear" title="Settings" @click="openSettings" />
            <QuitIconNav
                :icon="faXmark"
                title="Close Overlay"
                @click="closeOverlay"
                @close-overlay="closeOverlay"
                @quit="quitApplication"
            />
        </div>
    </header>
</template>
<script setup lang="ts">
import CollectionSelector from "@/components/header/CollectionSelector.vue";
import TitleNav from "@/components/header/TitleNav.vue";
import IconNav from "@/components/header/IconNav.vue";
import QuitIconNav from "@/components/header/QuitIconNav.vue";
import { faGear, faXmark } from "@fortawesome/free-solid-svg-icons";
import { useWindowManager } from "@/lib/windowManager";
import { AppService } from "@/lib/appService";

const { openWindow } = useWindowManager();

const options = [
    {
        key: "work",
        value: "Work Notes",
    },
    { key: "personal", value: "Personal" },
    { key: "projects", value: "Projects" },
    { key: "ideas", value: "Ideas & Drafts" },
];

const openSettings = () => {
    openWindow("settings");
};

const closeOverlay = async () => {
    console.log("Closing overlay...");
    try {
        await AppService.toggleOverlay();
    } catch (error) {
        console.error("Failed to close overlay:", error);
    }
};

const quitApplication = async () => {
    console.log("Quitting application...");
    try {
        await AppService.quitApplication();
    } catch (error) {
        console.error("Failed to quit application:", error);
        // Show an error message to the user
        alert(
            "Failed to quit the application. Please try again or close manually."
        );
    }
};
</script>
<style lang="scss">
@import "@/styles/variables";

.header {
    height: 4rem;
    width: 100%;
    display: grid;
    grid-template-columns: 1fr auto 1fr;
    align-items: center;
    background: $color-bg-transparent;
    padding: 0 1rem;
    box-sizing: border-box;
    z-index: $z-header;

    &__block {
        display: flex;
        flex-direction: row;

        &--left {
            justify-content: flex-start;
        }

        &--center {
            justify-content: center;
        }

        &--right {
            justify-content: flex-end;
            gap: 0.5rem;
        }
    }
}
</style>
