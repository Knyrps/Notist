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
