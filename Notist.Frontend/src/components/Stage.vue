<template>
    <div
        class="stage droppable"
        :class="{ 'stage--header-snap': isHoveringHeader }"
        @drop="handleDrop"
        @dragover="handleDragOver"
        @click="handleStageClick"
        ref="stageRef"
    >
        <NoteComponent
            v-for="note in allNotes"
            :key="note.id"
            :note="note"
            @dragstart="handleDragStart"
            @dragend="handleDragEnd"
            @edit="handleNoteEdit"
            @focus="handleNoteFocus"
            @save="handleNoteSave"
            @cancel="handleNoteCancel"
        />
    </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from "vue";
import NoteComponent from "@/components/notes/Note.vue";
import { useNotes } from "@/lib/useNotes";
import type { Position, Note, DragData } from "@/types/note";

// Use the notes composable
const {
    allNotes,
    updateNote,
    setNotePosition,
    setNoteDragging,
    setNoteEditing,
    bringNoteToFront,
    closeAllEditing,
    createNote,
} = useNotes();

// Reactive data
const stageRef = ref<HTMLElement>();

// Drag state
const dragData = ref<DragData | null>(null);
const isHoveringHeader = ref(false);

// Drag handlers
function handleDragStart(note: Note, event: DragEvent) {
    if (!event.dataTransfer) return;

    // Bring the dragged note to front
    bringNoteToFront(note.id);

    setNoteDragging(note.id, true);

    const rect = (event.target as HTMLElement).getBoundingClientRect();
    const offset: Position = {
        x: event.clientX - rect.left,
        y: event.clientY - rect.top,
    };

    stageRef.value?.classList.add("stage--isdragging");

    dragData.value = {
        noteId: note.id,
        offset,
    };

    event.dataTransfer.effectAllowed = "move";
    event.dataTransfer.setData(
        "application/json",
        JSON.stringify(dragData.value)
    );
}

function handleDragEnd(note: Note) {
    setNoteDragging(note.id, false);
    dragData.value = null;
    isHoveringHeader.value = false;
    stageRef.value?.classList.remove("stage--isdragging");
}

function handleDragOver(event: DragEvent) {
    event.preventDefault();
    if (event.dataTransfer) {
        event.dataTransfer.dropEffect = "move";
    }

    // Dynamically calculate header height and snap threshold
    const headerElement = document.getElementById("header");
    const headerHeight = headerElement ? headerElement.offsetHeight : 64; // fallback to 4rem
    const snapThreshold = 20; // reasonable default

    // Check if cursor is near the header area for snapping
    const cursorY = event.clientY;
    isHoveringHeader.value = cursorY <= headerHeight + snapThreshold;
}

function handleDrop(event: DragEvent) {
    event.preventDefault();

    if (!event.dataTransfer || !stageRef.value) return;

    try {
        const data: DragData = JSON.parse(
            event.dataTransfer.getData("application/json")
        );
        const note = allNotes.value.find((n) => n.id === data.noteId);

        if (!note) return;

        const stageRect = stageRef.value.getBoundingClientRect();
        let newPosition: Position = {
            x: event.clientX - stageRect.left - data.offset.x,
            y: event.clientY - stageRect.top - data.offset.y,
        };

        // Header snap logic - if hovering near header, snap to top of stage
        if (isHoveringHeader.value) {
            newPosition.y = 0;
        }

        // Boundary constraints
        const noteElement = document.getElementById(note.id);
        if (noteElement) {
            const noteRect = noteElement.getBoundingClientRect();
            const maxX = stageRef.value.offsetWidth - noteRect.width;
            const maxY = stageRef.value.offsetHeight - noteRect.height;

            newPosition.x = Math.max(0, Math.min(maxX, newPosition.x));
            if (!isHoveringHeader.value) {
                // Only apply Y boundary if not snapping to header
                newPosition.y = Math.max(0, Math.min(maxY, newPosition.y));
            }
        }

        setNotePosition(note.id, newPosition);
        setNoteDragging(note.id, false);

        // Reset header hover state
        isHoveringHeader.value = false;
    } catch (error) {
        console.error("Error handling drop:", error);
    }
}

// Global drag handlers for preventing drops outside stage
function handleGlobalDragOver(event: DragEvent) {
    const target = event.target as HTMLElement;
    const isDroppable = target?.closest(".droppable") !== null;

    if (event.dataTransfer) {
        event.dataTransfer.dropEffect = isDroppable ? "move" : "none";
    }

    if (!isDroppable) {
        event.preventDefault();
    }
}

function handleGlobalDrop(event: DragEvent) {
    const target = event.target as HTMLElement;
    const isDroppable = target?.closest(".droppable") !== null;

    if (!isDroppable && dragData.value) {
        event.preventDefault();
        // Reset note position if dropped outside stage
        const note = allNotes.value.find(
            (n) => n.id === dragData.value?.noteId
        );
        if (note) {
            setNoteDragging(note.id, false);
        }
    }
}

// Note editing handlers
function handleNoteEdit(note: Note) {
    // Bring the edited note to front
    bringNoteToFront(note.id);

    // Set this note to editing mode (composable will close others)
    setNoteEditing(note.id, true);
}

function handleNoteFocus(note: Note) {
    // Bring the focused note to front
    bringNoteToFront(note.id);
}

async function handleNoteSave(noteId: string, title: string, content: string) {
    await updateNote(noteId, title, content);
}

function handleNoteCancel(noteId: string) {
    setNoteEditing(noteId, false);
}

function handleStageClick(event: MouseEvent) {
    // Close all editing modes when clicking on the stage
    const target = event.target as HTMLElement;
    if (target === stageRef.value) {
        closeAllEditing();
    }
}

// Lifecycle
onMounted(() => {
    document.addEventListener("dragover", handleGlobalDragOver);
    document.addEventListener("drop", handleGlobalDrop);
});

onUnmounted(() => {
    document.removeEventListener("dragover", handleGlobalDragOver);
    document.removeEventListener("drop", handleGlobalDrop);
});
</script>

<style scoped lang="scss">
@import "@/styles/_variables.scss";

.stage {
    box-sizing: border-box;
    height: 100%;
    width: 100%;
    position: relative;

    overflow: visible;
    transition: all 0.3s ease;

    &::after {
        opacity: 0;
        content: "";
        position: absolute;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        height: 2px;
        border-top: 2px dashed $color-border-primary;
        transition: opacity 0.3s ease;
    }
}

.stage--isdragging::after {
    opacity: 1;
    animation: shimmer 1.5s ease-in-out infinite;
}

.stage--header-snap::before {
    content: "";
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    height: 3px;
    background: linear-gradient(
        90deg,
        $color-accent-primary,
        $color-accent-primary-dark,
        $color-accent-primary
    );
    border-radius: 2px 2px 0 0;
    animation: shimmer 1.5s ease-in-out infinite;
}

@keyframes shimmer {
    0%,
    100% {
        opacity: 0.5;
    }
    50% {
        opacity: 1;
    }
}
</style>
