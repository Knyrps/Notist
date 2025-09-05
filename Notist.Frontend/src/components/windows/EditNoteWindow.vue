<template>
    <Window
        title="Edit Note"
        width="wide"
        height="large"
        :before-close="beforeClose"
        @close="onClose"
    >
        <div class="edit-note-window">
            <MarkdownEditor
                layout="sideBySide"
                v-model="noteContent"
                :initial-content="noteContent"
            />
        </div>

        <template #footer>
            <button class="button button--primary" @click="handleSave">
                Save Note
            </button>
            <button class="button button--secondary" @click="handleCancel">
                Cancel
            </button>
        </template>
    </Window>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import { useWindowManager } from "@/lib/windowManager";
import { useNotes } from "@/lib/useNotes";
import Window from "@/components/windows/Window.vue";
import MarkdownEditor from "@/components/common/markdown/MarkdownEditor.vue";

const emit = defineEmits<{
    close: [];
}>();

const { getEditingNote, closeWindow } = useWindowManager();
const { updateNote } = useNotes();

// Get the note data from the window manager
const editingNote = computed(() => getEditingNote());
const noteContent = ref("");

// Initialize content when component mounts
onMounted(() => {
    const note = editingNote.value;
    if (note) {
        // Combine title and content for editing
        const titlePart = note.title || "";
        const contentPart = note.content || "";
        let fullContent = titlePart;

        if (contentPart) {
            fullContent += "\n\n" + contentPart;
        }

        noteContent.value = fullContent;
    }
});

const beforeClose = async (): Promise<boolean> => {
    // Could add unsaved changes check here
    return true;
};

const onClose = () => {
    emit("close");
};

const handleSave = async () => {
    const note = editingNote.value;
    if (!note) return;

    // Split content into title (first line) and content (rest)
    const lines = noteContent.value.split("\n");
    const title = lines[0] || "";
    const content = lines.slice(1).join("\n").trim();

    // Use the composable directly - no event emission needed!
    await updateNote(note.id, title, content);
    closeWindow();
};

const handleCancel = () => {
    closeWindow();
};
</script>

<style lang="scss" scoped>
@import "@/styles/variables";

.edit-note-window {
    width: 800px;
    max-width: 90vw;
    height: 600px;
    max-height: 80vh;
}

.edit-note-actions {
    display: flex;
    justify-content: flex-end;
    gap: 0.75rem;
    padding: 1rem;
    border-top: 1px solid $color-border-primary;
    background: $color-bg-secondary;
}

.btn {
    padding: 0.5rem 1rem;
    border: 1px solid $color-border-primary;
    border-radius: $radius-secondary;
    font-size: $font-size-base;
    font-weight: 500;
    cursor: pointer;
    transition: all $transition-fast;

    &--primary {
        background: $color-accent-primary;
        color: $color-text-inverse;
        border-color: $color-accent-primary;

        &:hover {
            background: $color-accent-primary-dark;
            border-color: $color-accent-primary-dark;
        }
    }

    &--secondary {
        background: $color-bg-secondary;
        color: $color-text-primary;

        &:hover {
            background: $color-bg-hover;
            border-color: $color-border-hover;
        }
    }

    &:focus-visible {
        outline: 2px solid $color-accent-primary;
        outline-offset: 2px;
    }
}

@media (max-width: 768px) {
    .edit-note-window {
        width: 100%;
        height: 500px;
    }

    .edit-note-actions {
        flex-direction: column;
    }
}
</style>
