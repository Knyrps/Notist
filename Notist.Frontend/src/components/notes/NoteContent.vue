<template>
    <div v-if="isEditing" class="note-editor">
        <div class="note-edit-container">
            <div
                ref="contentEditableRef"
                class="note-content-editable"
                contenteditable="true"
                @keydown="handleContentKeydown"
                @input="handleContentInput"
                @blur="updateContentFromEditable"
                :data-placeholder="editContent ? '' : 'Write your note here...'"
            ></div>
        </div>
        <div class="note-editor-actions">
            <button @click.stop="handleSave" class="btn btn--save">Save</button>
            <button @click.stop="handleCancel" class="btn btn--cancel">
                Cancel
            </button>
        </div>
    </div>
    <div v-else class="note-display">
        <div class="note-title">{{ title }}</div>
        <div class="note-separator"></div>
        <div class="note-content" v-html="props.content"></div>
    </div>
</template>

<script setup lang="ts">
import { ref, nextTick, watch, computed } from "vue";

const props = defineProps<{
    title: string;
    content: string;
    isEditing?: boolean;
}>();

const emit = defineEmits<{
    save: [title: string, content: string];
    cancel: [];
}>();

// Editing state
const editContent = ref("");
const contentEditableRef = ref<HTMLDivElement>();

// Watch for editing mode changes
watch(
    () => props.isEditing,
    (newIsEditing) => {
        if (newIsEditing) {
            // Combine title and content for editing, ensure at least 2 lines
            const titlePart = props.title || "";
            const contentPart = props.content || "";
            let fullContent = titlePart;

            if (contentPart) {
                fullContent += "\n" + contentPart;
            } else {
                // Ensure we always have at least one newline for the second line
                fullContent += "\n";
            }

            editContent.value = fullContent;

            // Focus content after DOM update
            nextTick(() => {
                if (contentEditableRef.value) {
                    contentEditableRef.value.textContent = fullContent;
                    contentEditableRef.value.focus();

                    // Position cursor at the end
                    const range = document.createRange();
                    const selection = window.getSelection();
                    if (contentEditableRef.value.firstChild) {
                        range.selectNodeContents(contentEditableRef.value);
                        range.collapse(false);
                        selection?.removeAllRanges();
                        selection?.addRange(range);
                    }
                }
            });
        }
    }
);

function handleSave() {
    // Update content from contenteditable before saving
    updateContentFromEditable();

    // Split content into title (first line) and content (rest)
    const lines = editContent.value.split("\n");
    const title = (lines[0] || "").trim();
    // Join remaining lines, filtering out completely empty lines at the start
    const contentLines = lines.slice(1);
    const content = contentLines.join("\n").trim();

    emit("save", title, content);
}

function handleCancel() {
    // Reset the editing content
    editContent.value = "";
    if (contentEditableRef.value) {
        contentEditableRef.value.textContent = "";
    }

    emit("cancel");
}

function handleContentKeydown(event: KeyboardEvent) {
    if (event.key === "Escape") {
        handleCancel();
    } else if (event.key === "Enter" && (event.ctrlKey || event.metaKey)) {
        event.preventDefault();
        handleSave();
    } else if (event.key === "Enter") {
        // Check if we're on the first line
        const selection = window.getSelection();
        if (selection && contentEditableRef.value) {
            const range = selection.getRangeAt(0);
            const textBeforeCursor =
                contentEditableRef.value.textContent?.substring(
                    0,
                    range.startOffset
                ) || "";
            const isFirstLine = !textBeforeCursor.includes("\n");

            if (isFirstLine) {
                // Add a visual separator effect by ensuring proper line break
                event.preventDefault();
                const lineBreak = document.createTextNode("\n");
                range.insertNode(lineBreak);
                range.setStartAfter(lineBreak);
                range.setEndAfter(lineBreak);
                selection.removeAllRanges();
                selection.addRange(range);
                updateContentFromEditable();
            }
        }
    } else if (event.key === "Backspace" || event.key === "Delete") {
        // After deletion, check if we still have minimum lines
        setTimeout(() => updateContentFromEditable(), 0);
    }
}

function handleContentInput() {
    updateContentFromEditable();
}

function updateContentFromEditable() {
    if (contentEditableRef.value) {
        let content = contentEditableRef.value.textContent || "";

        // Ensure at least 2 lines by checking actual content
        const lines = content.split("\n");
        if (lines.length < 2) {
            content = content + "\n";
            // Update the contenteditable to reflect the change, but preserve cursor position
            const selection = window.getSelection();
            const currentRange = selection?.rangeCount
                ? selection.getRangeAt(0)
                : null;
            const cursorOffset =
                currentRange?.startOffset || content.length - 1;

            contentEditableRef.value.textContent = content;

            // Restore cursor position
            if (currentRange) {
                const newRange = document.createRange();
                const textNode = contentEditableRef.value.firstChild;
                if (textNode) {
                    newRange.setStart(
                        textNode,
                        Math.min(
                            cursorOffset,
                            textNode.textContent?.length || 0
                        )
                    );
                    newRange.collapse(true);
                    selection?.removeAllRanges();
                    selection?.addRange(newRange);
                }
            }
        }

        editContent.value = content;
    }
}
</script>

<style scoped lang="scss">
@import "@/styles/_variables.scss";

// Display mode styles
.note-display {
    width: 100%;
    height: 100%;
    box-sizing: border-box;
    display: flex;
    flex-direction: column;
    padding: 16px;
}

.note-title {
    font-size: 0.9rem;
    font-weight: 600;
    color: $color-text-primary;
    margin: 0 0 8px 0;
    line-height: 1.3;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
    box-sizing: border-box;
}

.note-separator {
    height: 1px;
    background: linear-gradient(
        90deg,
        rgba($color-border-primary, 0.2) 0%,
        rgba($color-border-primary, 0.5) 50%,
        rgba($color-border-primary, 0.2) 100%
    );
    margin: 0 0 10px 0;
    opacity: 0.8;
}

.note-content {
    flex: 1;
    font-size: 0.8rem;
    line-height: 1.5;
    color: $color-text-secondary;
    overflow: hidden;

    p {
        margin: 0 0 10px 0;

        &:last-child {
            margin-bottom: 0;
        }
    }

    strong {
        font-weight: 600;
        color: $color-text-primary;
    }

    em {
        font-style: italic;
    }
}

// Editor mode styles
.note-editor {
    width: 100%;
    height: 100%;
    display: flex;
    flex-direction: column;
    padding: 16px;
    gap: 12px;
    box-sizing: border-box;
}

.note-edit-container {
    flex: 1;
    position: relative;
    display: flex;
    flex-direction: column;
}

.note-content-editable {
    flex: 1;
    min-height: 120px;
    padding: 12px 12px 12px 20px;
    border: none;
    background: transparent;
    color: $color-text-primary;
    font-family: inherit;
    font-size: 0.75rem;
    line-height: 1.5;
    outline: none;
    overflow-y: auto;
    white-space: pre-wrap;
    word-wrap: break-word;
    position: relative;

    // Style first line as title with CSS
    &::first-line {
        font-size: 0.9rem;
        font-weight: 600;
        color: $color-text-primary;
    }

    &:empty:before {
        content: attr(data-placeholder);
        color: $color-text-secondary;
        opacity: 0.7;
        pointer-events: none;
    }

    // Vertical line indicator for title
    &::before {
        content: "";
        position: absolute;
        left: 8px;
        top: 12px;
        width: 3px;
        height: calc(1.5em * 0.9 / 0.75); // Adjusted height for title line
        background: $color-accent-primary;
        border-radius: 2px;
        opacity: 0.6;
        pointer-events: none;
    }

    // Add visual separator after the first line
    &::after {
        content: "";
        position: absolute;
        left: 20px;
        right: 12px;
        top: calc(1.5em * 0.9 / 0.75 + 18px); // Positioned after title line
        height: 1px;
        background: linear-gradient(
            90deg,
            rgba($color-border-primary, 0.3) 0%,
            rgba($color-border-primary, 0.6) 50%,
            rgba($color-border-primary, 0.3) 100%
        );
        opacity: 0.4;
        pointer-events: none;
        transition: opacity $transition-medium;
    }

    &:focus::after {
        opacity: 0.7;
    }

    &:focus::before {
        opacity: 0.8;
        background: $color-accent-primary;
    }
}

// Editor Action Buttons
.note-editor-actions {
    display: flex;
    gap: 6px;
    justify-content: flex-end;
    margin-top: 4px;
}

.btn {
    padding: 4px 12px;
    border: 1px solid;
    border-radius: $radius-secondary;
    font-size: 0.7rem;
    font-weight: 500;
    cursor: pointer;
    transition: all $transition-medium;
    background: transparent;

    &--save {
        color: $color-accent-primary;
        border-color: $color-accent-primary;

        &:hover {
            background: $color-accent-primary;
            color: white;
        }
    }

    &--cancel {
        color: $color-text-secondary;
        border-color: $color-border-primary;

        &:hover {
            background: $color-bg-hover;
            color: $color-text-primary;
        }
    }
}
</style>
