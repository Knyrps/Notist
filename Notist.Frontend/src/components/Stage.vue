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
            v-for="note in notes"
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
import type { Position, Note, DragData } from "@/types/note";

// Reactive data
const stageRef = ref<HTMLElement>();
const notes = ref<Note[]>([
    {
        id: "note-1",
        title: "First Note",
        content: "This is the content of the first note",
        position: { x: 50, y: 50 },
        isDragging: false,
        isEditing: false,
        zIndex: 1,
    },
    {
        id: "note-2",
        title: "Second Note",
        content: "This is the content of the second note",
        position: { x: 150, y: 100 },
        isDragging: false,
        isEditing: false,
        zIndex: 2,
    },
    {
        id: "note-3",
        title: "Third Note",
        content: "This is the content of the third note",
        position: { x: 250, y: 150 },
        isDragging: false,
        isEditing: false,
        zIndex: 3,
    },
]);

// Drag state
const dragData = ref<DragData | null>(null);
const isHoveringHeader = ref(false);

// Note stacking system
function bringNoteToFront(noteId: string) {
    const note = notes.value.find((n) => n.id === noteId);
    if (!note) return;

    // Get the current highest z-index
    const maxZIndex = Math.max(...notes.value.map((n) => n.zIndex));

    // Only update if this note isn't already on top
    if (note.zIndex < maxZIndex) {
        note.zIndex = maxZIndex + 1;
    }
}

// Drag handlers
function handleDragStart(note: Note, event: DragEvent) {
    if (!event.dataTransfer) return;

    // Bring the dragged note to front
    bringNoteToFront(note.id);

    note.isDragging = true;

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
    note.isDragging = false;
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
        const note = notes.value.find((n) => n.id === data.noteId);

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

        note.position = newPosition;
        note.isDragging = false;

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
        const note = notes.value.find((n) => n.id === dragData.value?.noteId);
        if (note) {
            note.isDragging = false;
        }
    }
}

// Note editing handlers
function handleNoteEdit(note: Note) {
    // Bring the edited note to front
    bringNoteToFront(note.id);

    // Close any other editing notes
    notes.value.forEach((n) => {
        if (n.id !== note.id) {
            n.isEditing = false;
        }
    });

    // Open editing mode for this note
    note.isEditing = true;
}

function handleNoteFocus(note: Note) {
    // Bring the focused note to front
    bringNoteToFront(note.id);
}
function handleNoteSave(noteId: string, title: string, content: string) {
    const note = notes.value.find((n) => n.id === noteId);
    if (note) {
        note.title = title;
        note.content = content;
        note.isEditing = false;
    }
}

function handleNoteCancel(noteId: string) {
    const note = notes.value.find((n) => n.id === noteId);
    if (note) {
        note.isEditing = false;
    }
}

function handleStageClick(event: MouseEvent) {
    // Close all editing modes when clicking on the stage
    const target = event.target as HTMLElement;
    if (target === stageRef.value) {
        notes.value.forEach((note) => {
            note.isEditing = false;
        });
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
