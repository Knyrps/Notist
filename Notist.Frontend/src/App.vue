<script setup lang="ts">
import Stage from "@/components/Stage.vue";
import Header from "@/components/header/Header.vue";
import WindowContainer from "@/components/windows/WindowContainer.vue";
import AlertContainer from "@/components/feedback/AlertContainer.vue";
</script>

<template>
    <Header></Header>
    <main>
        <Stage></Stage>
    </main>
    <WindowContainer />
    <AlertContainer />
</template>

<style scoped></style>

<!-- Global styles for markdown content -->
<style lang="scss">
@import "@/styles/markdown";

* {
    scrollbar-gutter: auto;

    &::-webkit-scrollbar {
        width: 8px;
    }

    &::-webkit-scrollbar-track {
        background: $color-bg-secondary;
        border-radius: $radius-secondary;
    }

    &::-webkit-scrollbar-thumb {
        background: $color-border-primary;
        border-radius: $radius-secondary;
        transition: all $transition-fast;

        &:hover {
            background: $color-border-hover;
        }

        &:active {
            background: $color-accent-primary;
        }
    }

    scrollbar-width: thin;
    scrollbar-color: $color-border-primary $color-bg-secondary;
}
</style>
