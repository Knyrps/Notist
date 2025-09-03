<script setup lang="ts">
import AppGrid from "./components/common/AppGrid.vue";
import Header from "./components/header/Header.vue";
import WindowContainer from "./components/layout/WindowContainer.vue";
import AlertContainer from "./components/feedback/AlertContainer.vue";
</script>

<template>
    <Header></Header>
    <main>
        <AppGrid></AppGrid>
    </main>
    <WindowContainer />
    <AlertContainer />
</template>

<style scoped></style>
