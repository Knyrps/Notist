<template>
    <div class="markup"></div>
</template>

<script setup lang="ts">
import { type MarkdownData } from "@/lib/markdown/markdownService";

type Props = {
    initialContent?: string;
};

withDefaults(defineProps<Props>(), {
    initialContent: "",
});
</script>

<style scoped lang="scss">
@import "@/styles/_variables.scss";
</style>
