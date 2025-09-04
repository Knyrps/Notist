<template>
    <div class="stage droppable" :ondrop="drop" :ondragover="dragOver">
        <div draggable="true" :ondragstart="dragStart" id="note-1" class="note">
            Note 1
        </div>
        <div draggable="true" :ondragstart="dragStart" id="note-2" class="note">
            Note 2
        </div>
        <div draggable="true" :ondragstart="dragStart" id="note-3" class="note">
            Note 3
        </div>
    </div>
</template>

<script setup lang="ts">
type DragEventData = {
    id: string;
    position: {
        x: number;
        y: number;
    };
    originalPosition?: {
        x: number;
        y: number;
    };
    originalOpacity: string;
};

function handleWindowDrop(event: DragEvent) {
    // Only handle if not dropped on a droppable area
    const droppable =
        (event.target as HTMLElement)?.classList?.contains("droppable") ||
        (event.target as HTMLElement)?.closest?.(".droppable");
    if (droppable) {
        return;
    }

    event.preventDefault();
    const data: DragEventData | null = event.dataTransfer
        ? JSON.parse(event.dataTransfer.getData("text/json"))
        : null;
    if (!data) {
        return;
    }

    const draggableElement = document.getElementById(data.id);
    if (!draggableElement) {
        return;
    }

    // Clear the safety timer since we have a proper drop
    if (draggableElement.dataset.safetyTimer) {
        clearTimeout(parseInt(draggableElement.dataset.safetyTimer));
        delete draggableElement.dataset.safetyTimer;
    }

    // Reset element to its original position (or hide, or any fallback logic)
    draggableElement.style.opacity = data.originalOpacity || "1";
    draggableElement.style.left = data.originalPosition?.x + "px";
    draggableElement.style.top = data.originalPosition?.y + "px";

    event.dataTransfer?.clearData();
}

window.addEventListener("drop", handleWindowDrop);
window.addEventListener("dragover", (event) => {
    const droppable =
        (event.target as HTMLElement)?.classList?.contains("droppable") ||
        (event.target as HTMLElement)?.closest?.(".droppable");
    if (droppable) {
        event.dataTransfer!.dropEffect = "move";
        return;
    }
    event.dataTransfer!.dropEffect = "none";
});

function dragStart(event: DragEvent) {
    if (!event.dataTransfer) return;

    const el = event.target as HTMLElement;
    const rect = el.getBoundingClientRect();

    const offsetX = event.clientX - rect.left;
    const offsetY = event.clientY - rect.top;

    const originalX = rect.left;
    const originalY = rect.top;

    const copyOfElement = el.cloneNode(true) as HTMLElement;
    copyOfElement.style.position = "absolute";
    copyOfElement.style.top = "-9999px";
    copyOfElement.style.left = "-9999px";
    document.body.appendChild(copyOfElement);

    // Get computed opacity or default to "1" if not set
    const initialOpacity = el.style.opacity || "1";
    el.style.opacity = "0";

    const dragEventData: DragEventData = {
        id: el.id,
        position: { x: offsetX, y: offsetY },
        originalOpacity: initialOpacity,
        originalPosition: { x: originalX, y: originalY },
    };

    // Set up dragend event listener to restore visibility if drop doesn't occur
    const handleDragEnd = () => {
        if (el.style.opacity === "0") {
            el.style.opacity = initialOpacity;
        }
        // Clean up the event listener
        el.removeEventListener("dragend", handleDragEnd);
    };

    el.addEventListener("dragend", handleDragEnd);

    event.dataTransfer.setDragImage(copyOfElement, offsetX, offsetY);
    event.dataTransfer.effectAllowed = "move";
    event.dataTransfer.setData("text/json", JSON.stringify(dragEventData));

    setTimeout(() => document.body.removeChild(copyOfElement), 0);
}

function drop(event: DragEvent) {
    event.preventDefault();

    const data: DragEventData | null = event.dataTransfer
        ? JSON.parse(event.dataTransfer.getData("text/json"))
        : null;
    if (!data) {
        return;
    }

    const draggableElement = document.getElementById(data.id);
    if (!draggableElement) {
        return;
    }

    let dropzone = event.target as HTMLElement;

    // allow dropping while the cursor is still over dragged element
    if (dropzone == draggableElement) {
        let maxIterations = 10;
        while (!dropzone.classList.contains("droppable") && maxIterations > 0) {
            dropzone = dropzone.parentElement as HTMLElement;
            maxIterations--;
        }
    }

    dropzone.appendChild(draggableElement);
    draggableElement.style.opacity = data.originalOpacity || "1";

    const { x: offsetX, y: offsetY } = data.position;
    const dropzoneRect = dropzone.getBoundingClientRect();

    const position: { x: number; y: number } = {
        x: event.clientX - dropzoneRect.left - offsetX,
        y: event.clientY - dropzoneRect.top - offsetY,
    };

    if (position.x + draggableElement.offsetWidth > dropzone.offsetWidth) {
        position.x = dropzone.offsetWidth - draggableElement.offsetWidth;
    }

    if (position.y + draggableElement.offsetHeight > dropzone.offsetHeight) {
        position.y = dropzone.offsetHeight - draggableElement.offsetHeight;
    }

    if (position.x < 0) position.x = 0;
    if (position.y < 0) position.y = 0;

    draggableElement.style.left = position.x + "px";
    draggableElement.style.top = position.y + "px";

    event.dataTransfer?.clearData();
}

function dragOver(event: DragEvent) {
    event.dataTransfer!.dropEffect = "move";
    event.preventDefault();
}
</script>

<style scoped>
.stage {
    height: 100%;
    width: 100%;
    display: flex;
    position: relative;

    /* debug props */
    /* background: grey; */
}

.note {
    position: absolute;
    border: red solid 1px;
    background: green;
    height: 100px;
    width: 180px;
    display: flex;
    justify-content: center;
    align-items: center;
}
</style>
