<template>
    <div v-if="isEditing" class="note-editor">
        <div class="note-edit-container">
            <MarkdownEditor
                v-model="editContent"
                layout="tabs"
                :initial-content="combinedContent"
            />
        </div>
        <div class="note-editor-actions">
            <button @click.stop="handleCancel" class="button button--secondary">
                Cancel
            </button>
            <button @click.stop="handleSave" class="button button--primary">
                Save
            </button>
        </div>
    </div>
    <div v-else class="note-display">
        <div class="note-header">
            <div class="note-title">{{ title }}</div>
            <button
                @click.stop="openLargeEditor"
                class="btn btn--edit-large"
                title="Open in large editor"
            >
                <FontAwesomeIcon :icon="faPen" />
            </button>
        </div>
        <div class="md-header-preview-separator"></div>
        <div class="note-content md-out" v-html="props.htmlContent"></div>
    </div>
</template>

<script setup lang="ts">
import { ref, watch, computed } from "vue";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";
import { faPen } from "@fortawesome/free-solid-svg-icons";
import { useWindowManager } from "@/lib/windowManager";
import MarkdownEditor from "@/components/common/markdown/MarkdownEditor.vue";

const props = defineProps<{
    title: string;
    content: string;
    htmlContent: string;
    isEditing?: boolean;
    noteId?: string;
}>();

const emit = defineEmits<{
    save: [title: string, content: string];
    cancel: [];
}>();

const { openWindow } = useWindowManager();

// Function to open the large editor
const openLargeEditor = () => {
    const noteData = {
        id: props.noteId || "unknown",
        title: props.title,
        content: props.content,
    };

    openWindow("editNote", noteData);
};

// Editing state
const editContent = ref("");

// Computed property to combine title and content for initial editor content
const combinedContent = computed(() => {
    const titlePart = props.title || "";
    const contentPart = props.content || "";

    if (!titlePart && !contentPart) {
        return "";
    }

    if (!contentPart) {
        return titlePart + "\n";
    }

    return titlePart + "\n" + contentPart;
});

// Watch for editing mode changes
watch(
    () => props.isEditing,
    (newIsEditing) => {
        if (newIsEditing) {
            // Initialize editContent with combined title and content
            editContent.value = combinedContent.value;
        }
    },
    { immediate: true }
);

function handleSave() {
    // Split content into title (first line) and content (rest)
    const lines = editContent.value.split("\n");
    const title = lines[0] || "";
    const content = lines.slice(1).join("\n").trim();

    emit("save", title, content);
}

function handleCancel() {
    emit("cancel");
}
</script>

<style lang="scss" scoped>
@import "@/styles/variables";

// Note Display Styles
.note-display {
    padding: 0.75rem 1rem;
    width: 100%;
    height: 100%;
    overflow: hidden;
    display: flex;
    flex-direction: column;
    box-sizing: border-box;
}

.note-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: 0.5rem;
    width: 100%;
}

.note-title {
    flex: 1;
    font-weight: 600;
    color: $color-text-primary;
    font-size: $font-size-md;
    line-height: 1.2;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
}

.note-content {
    flex: 1;
    font-size: $font-size-base;
    line-height: 1.4;
    color: $color-text-secondary;
    overflow: hidden;
    display: -webkit-box;
    -webkit-line-clamp: 8;
    line-clamp: 8;
    -webkit-box-orient: vertical;
    text-overflow: ellipsis;
}

// Note Editor Styles
.note-editor {
    width: 100%;
    height: 100%;
    display: flex;
    flex-direction: column;
    gap: 8px;
    box-sizing: border-box;
}

.note-edit-container {
    flex: 1;
    display: flex;
    flex-direction: column;
    overflow: hidden;
}

// Editor Action Buttons
.note-editor-actions {
    display: flex;
    gap: 6px;
    justify-content: flex-end;
    padding: 0.75rem;
    box-sizing: border-box;
}

.btn {
    padding: 4px 12px;
    border: 1px solid;
    border-radius: $radius-secondary;
    font-size: $font-size-sm;
    font-weight: 500;
    cursor: pointer;
    transition: all $transition-medium;
    background: transparent;

    &--edit-large {
        color: $color-text-primary;
        border-color: $color-border-primary;
        padding: 0.3rem 0.5rem;
        font-size: $font-size-sm;
        min-width: auto;
        opacity: 0.7;
        transition: all $transition-fast;

        &:hover {
            background: $color-bg-hover;
            color: $color-text-primary;
            opacity: 1;
        }
    }
}
</style>
