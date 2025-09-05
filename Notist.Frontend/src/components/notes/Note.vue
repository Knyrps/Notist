<template>
    <div
        :id="note.id"
        class="note"
        :class="{
            'note--editing': note.isEditing,
            'note--dragging': note.isDragging,
        }"
        draggable="true"
        :style="{
            left: note.position.x + 'px',
            top: note.position.y + 'px',
            opacity: note.isDragging ? 0.7 : 1,
            zIndex: note.isEditing ? 1000 : note.zIndex,
        }"
        @dragstart="handleDragStart"
        @dragend="handleDragEnd"
        @mousedown="handleMouseDown"
        @click="handleClick"
    >
        <NoteContent
            :title="note.title"
            :content="note.content"
            :htmlContent="note.htmlContent"
            :isEditing="note.isEditing"
            :noteId="note.id"
            @save="handleSave"
            @cancel="handleCancel"
        />
    </div>
</template>

<script setup lang="ts">
import type { Note } from "@/types/note";
import NoteContent from "./NoteContent.vue";

const props = defineProps<{
    note: Note;
}>();

const emit = defineEmits<{
    dragstart: [note: Note, event: DragEvent];
    dragend: [note: Note];
    edit: [note: Note];
    focus: [note: Note];
    save: [noteId: string, title: string, content: string];
    cancel: [noteId: string];
}>();

function handleDragStart(event: DragEvent) {
    // Prevent editing mode from interfering with drag
    if (props.note.isEditing) {
        event.preventDefault();
        return;
    }
    emit("dragstart", props.note, event);
}

function handleDragEnd() {
    emit("dragend", props.note);
}

function handleMouseDown() {
    // Bring note to front on any mouse interaction
    if (!props.note.isDragging) {
        emit("focus", props.note);
    }
}

function handleClick(event: MouseEvent) {
    // Prevent click during drag or if already editing
    if (props.note.isDragging || props.note.isEditing) {
        return;
    }

    event.stopPropagation();
    emit("edit", props.note);
}

function handleSave(title: string, content: string) {
    emit("save", props.note.id, title, content);
}

function handleCancel() {
    emit("cancel", props.note.id);
}
</script>

<style scoped lang="scss">
@import "@/styles/_variables.scss";

.note {
    position: absolute;
    border: 1px solid $color-border-primary;
    background: $color-bg-primary;
    min-height: 150px;
    width: 380px;
    border-radius: $radius-primary;
    cursor: pointer;
    user-select: none;
    transition: all $transition-fast cubic-bezier(0.34, 1.56, 0.64, 1),
        top $transition-rapid cubic-bezier(0.34, 1.56, 0.64, 1),
        left $transition-rapid cubic-bezier(0.34, 1.56, 0.64, 1);
    box-shadow: $shadow-light;
    backdrop-filter: $blur-light;
    font-weight: 500;
    color: $color-text-primary;
    box-sizing: border-box;
    transform-origin: center;
    border: 1px solid $color-border-primary;

    &:hover:not(&--editing):not(&--dragging) {
        transform: translateY(-2px) scale(1.02);
        background: $color-bg-hover;
        border-color: $color-border-hover;
        box-shadow: $shadow-medium;
    }

    &--editing {
        cursor: default;
        z-index: 1000;
        box-shadow: $shadow-strong;
        height: auto;
        min-height: 300px;
        width: 500px;
        max-width: 90vw;
        padding: 0;
        background: $color-bg-secondary;
        border-color: $color-border-hover;
        transform: translateY(-4px);
        transition: all $transition-medium cubic-bezier(0.25, 0.46, 0.45, 0.94);
    }

    &--dragging {
        cursor: grabbing;
        opacity: 0.8;
        box-shadow: $shadow-medium;
    }
}
</style>
