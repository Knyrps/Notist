<template>
    <div
        class="markdown-editor"
        :class="{
            'markdown-editor--side-by-side': props.layout === 'sideBySide',
        }"
    >
        <!-- Toggle buttons (only for tabs layout) -->
        <div v-if="props.layout === 'tabs'" class="mode-toggle">
            <button
                :class="{ active: isEditMode }"
                @click="setMode('edit')"
                class="mode-button"
            >
                Edit
            </button>
            <button
                :class="{ active: !isEditMode }"
                @click="setMode('preview')"
                class="mode-button"
            >
                Preview
            </button>
        </div>

        <!-- Editor container -->
        <div
            class="editor-container"
            :class="{
                'editor-container--side-by-side': props.layout === 'sideBySide',
            }"
        >
            <!-- Tabs layout -->
            <template v-if="props.layout === 'tabs'">
                <!-- Edit mode -->
                <div v-if="isEditMode" class="input-section">
                    <textarea
                        v-model="markdownContent"
                        @input="handleInput"
                        @keydown="markdown.handleKeyboardEvent"
                        placeholder="Type your markdown here..."
                        class="markdown-input"
                        ref="textareaRef"
                    ></textarea>
                </div>

                <!-- Preview mode -->
                <div v-else class="output-section">
                    <div class="markdown-output" v-html="markdown.html"></div>
                </div>
            </template>

            <!-- Side by side layout -->
            <template v-else>
                <!-- Edit side -->
                <div class="input-section input-section--side">
                    <h3 class="section-title">Edit</h3>
                    <textarea
                        v-model="markdownContent"
                        @input="handleInput"
                        @keydown="markdown.handleKeyboardEvent"
                        placeholder="Type your markdown here..."
                        class="markdown-input markdown-input--side"
                        ref="textareaRef"
                    ></textarea>
                </div>

                <!-- Preview side -->
                <div class="output-section output-section--side">
                    <h3 class="section-title">Preview</h3>
                    <div
                        class="markdown-output markdown-output--side"
                        v-html="markdown.html"
                    ></div>
                </div>
            </template>
        </div>

        <!-- Keyboard shortcuts (only show in edit mode for tabs, always show for side-by-side) -->
        <!-- <div v-if="props.layout === 'sideBySide' || (props.layout === 'tabs' && isEditMode)" class="keyboard-shortcuts">
            <h4>Keyboard Shortcuts:</h4>
            <ul>
                <li><kbd>Ctrl+B</kbd> - Bold text</li>
                <li><kbd>Ctrl+I</kbd> - Italic text</li>
                <li><kbd>Ctrl+K</kbd> - Insert link</li>
                <li><kbd>Tab</kbd> - Add indentation</li>
                <li v-if="props.layout === 'tabs'"><kbd>Ctrl+P</kbd> - Toggle preview</li>
            </ul>
        </div>
        -->
    </div>
</template>

<script setup lang="ts">
import { ref, watch, nextTick, computed } from "vue";
import { useMarkdown } from "../../../lib/markdown/markdownService";

// Props
const props = withDefaults(
    defineProps<{
        layout?: "tabs" | "sideBySide";
        initialContent?: string;
        modelValue?: string;
    }>(),
    {
        layout: "tabs",
        initialContent:
            "# Hello World\n\nThis is a **markdown** editor with *keyboard shortcuts*!",
        modelValue: "",
    }
);

// Emits
const emit = defineEmits<{
    "update:modelValue": [value: string];
}>();

// Mode toggle state (only used for tabs layout)
const isEditMode = ref(true);
const textareaRef = ref<HTMLTextAreaElement>();

// Use modelValue if provided, otherwise use initialContent
const initialText = computed(() => props.modelValue || props.initialContent);

// Initialize the markdown composable
const markdown = useMarkdown({
    initialContent: initialText.value,
    customKeyHandler: (event: KeyboardEvent, content: string) => {
        // Handle toggle preview shortcut
        if (event.ctrlKey && event.key === "p") {
            event.preventDefault();
            toggleMode();
            return;
        }

        // Custom keyboard handling logic
        console.log("Custom key handler:", event.key, content.length);
    },
});

// Local reactive content for the textarea
const markdownContent = ref(markdown.raw);

// Mode toggle functions
const setMode = (mode: "edit" | "preview") => {
    isEditMode.value = mode === "edit";

    // Focus textarea when switching to edit mode
    if (isEditMode.value) {
        nextTick(() => {
            textareaRef.value?.focus();
        });
    }
};

const toggleMode = () => {
    setMode(isEditMode.value ? "preview" : "edit");
};

// Handle input changes
const handleInput = (event: Event) => {
    const target = event.target as HTMLTextAreaElement;
    markdown.updateContent(target.value);
    markdownContent.value = target.value;

    // Emit the value for v-model support
    emit("update:modelValue", target.value);
};

// Watch for external changes to the markdown content
watch(
    () => markdown.raw,
    (newValue) => {
        markdownContent.value = newValue;
        emit("update:modelValue", newValue);
    }
);

// Watch for modelValue changes from parent
watch(
    () => props.modelValue,
    (newValue) => {
        if (newValue !== markdownContent.value) {
            markdownContent.value = newValue;
            markdown.updateContent(newValue);
        }
    }
);
</script>

<style lang="scss" scoped>
@import "@/styles/_variables";

.markdown-editor {
    background: $color-bg-overlay;
    border-radius: $radius-primary;
    box-shadow: $shadow-medium;
    backdrop-filter: $blur-light;
    overflow: hidden;
}

.mode-toggle {
    display: flex;
    gap: 0;
    border-bottom: 1px solid $color-border-primary;
    background: $color-bg-secondary;
}

.mode-button {
    flex: 1;
    padding: 0.75rem 1.5rem;
    border: none;
    background: transparent;
    color: $color-text-secondary;
    cursor: pointer;
    transition: all $transition-fast;
    font-size: $font-size-xs;
    font-weight: 500;
    position: relative;

    &:hover {
        background: $color-bg-hover;
        color: $color-text-primary;
    }

    &.active {
        background: $color-bg-overlay;
        color: $color-accent-primary;

        &::after {
            content: "";
            position: absolute;
            bottom: 0;
            left: 0;
            right: 0;
            height: 2px;
            background: $color-accent-primary;
        }
    }

    &:focus-visible {
        outline: 2px solid $color-accent-primary;
        outline-offset: -2px;
    }
}

.editor-container {
    min-height: 400px;
}

.input-section,
.output-section {
    padding: 1.5rem;
    background: $color-bg-overlay;
    height: 400px;
    overflow-y: auto;
    scrollbar-gutter: auto;
    box-sizing: border-box;

    &::-webkit-scrollbar {
        width: 8px;
    }

    &::-webkit-scrollbar-track {
        background: $color-bg-secondary;
        border-radius: $radius-secondary;
    }

    &::-webkit-scrollbar-thumb {
        background: $color-border-primary;
        border-radius: $radius-secondary;
        transition: all $transition-fast;

        &:hover {
            background: $color-border-hover;
        }

        &:active {
            background: $color-accent-primary;
        }
    }

    scrollbar-width: thin;
    scrollbar-color: $color-border-primary $color-bg-secondary;
}

.markdown-input {
    width: 100%;
    height: 100%;
    border: none;
    background: $color-bg-input;
    color: $color-text-primary;
    font-family: "Courier New", monospace;
    font-size: $font-size-base;
    resize: none;
    line-height: 1.6;
    outline: none;
    transition: all $transition-medium;
    box-sizing: border-box;

    &:focus {
        // border-color: $color-border-focus;
        // box-shadow: $shadow-focus;
        // background: $color-bg-active;
    }

    &::placeholder {
        color: $color-text-disabled;
    }
}

.markdown-output {
    height: 100%;
    overflow-y: auto;
    background: $color-bg-input;
    line-height: 1.6;
    color: $color-text-primary;

    // Modern scrollbar styling
    &::-webkit-scrollbar {
        width: 8px;
    }

    &::-webkit-scrollbar-track {
        background: rgba(255, 255, 255, 0.1);
        border-radius: $radius-secondary;
    }

    &::-webkit-scrollbar-thumb {
        background: $color-border-primary;
        border-radius: $radius-secondary;
        transition: all $transition-fast;

        &:hover {
            background: $color-border-hover;
        }

        &:active {
            background: $color-accent-primary;
        }
    }

    // Firefox scrollbar styling
    scrollbar-width: thin;
    scrollbar-color: $color-border-primary rgba(255, 255, 255, 0.1);

    // Style the rendered markdown content
    :deep(h1),
    :deep(h2),
    :deep(h3),
    :deep(h4),
    :deep(h5),
    :deep(h6) {
        margin-top: 1.5em;
        margin-bottom: 0.5em;
        color: $color-text-primary;
        font-weight: 600;
    }

    :deep(h1) {
        border-bottom: 2px solid $color-border-primary;
        padding-bottom: 0.3em;
        font-size: $font-size-3xl;
    }

    :deep(h2) {
        border-bottom: 1px solid $color-border-primary;
        padding-bottom: 0.2em;
        font-size: $font-size-2xl;
    }

    :deep(h3) {
        font-size: $font-size-xl;
    }
    :deep(h4) {
        font-size: $font-size-lg;
    }
    :deep(h5) {
        font-size: $font-size-md;
    }
    :deep(h6) {
        font-size: $font-size-base;
    }

    :deep(p) {
        margin-bottom: 1em;
        line-height: 1.7;
    }

    :deep(code) {
        background: $color-accent-light;
        border: 1px solid $color-accent-medium;
        padding: 0.125rem 0.25rem;
        border-radius: $radius-secondary;
        font-family: "JetBrains Mono", "Courier New", monospace;
        font-size: $font-size-code-inline;
        color: $color-accent-primary-dark;
    }

    :deep(pre) {
        background: $color-bg-secondary;
        border: 1px solid $color-border-primary;
        border-radius: $radius-secondary;
        padding: 1rem;
        overflow-x: auto;
        margin: 1em 0;

        code {
            background: transparent;
            border: none;
            padding: 0;
            color: $color-text-primary;
        }
    }

    :deep(blockquote) {
        border-left: 4px solid $color-accent-primary;
        margin: 1em 0;
        padding-left: 1rem;
        color: $color-text-secondary;
        background: $color-accent-light;
        border-radius: 0 $radius-secondary $radius-secondary 0;
        padding: 0.75rem 1rem;
    }

    :deep(ul),
    :deep(ol) {
        margin: 1em 0;
        padding-left: 1.5rem;
    }

    :deep(li) {
        margin-bottom: 0.25em;
    }

    :deep(a) {
        color: $color-accent-primary;
        text-decoration: none;
        border-bottom: 1px solid transparent;
        transition: all $transition-fast;

        &:hover {
            border-bottom-color: $color-accent-primary;
        }
    }

    :deep(table) {
        width: 100%;
        border-collapse: collapse;
        margin: 1em 0;

        th,
        td {
            border: 1px solid $color-border-primary;
            padding: 0.5rem 0.75rem;
            text-align: left;
        }

        th {
            background: $color-bg-secondary;
            font-weight: 600;
        }
    }

    :deep(hr) {
        border: none;
        border-top: 2px solid $color-border-primary;
        margin: 2em 0;
    }
}

.keyboard-shortcuts {
    background: $color-bg-secondary;
    padding: 1rem 1.5rem;
    border-top: 1px solid $color-border-primary;

    h4 {
        margin: 0 0 0.75rem 0;
        color: $color-text-primary;
        font-size: $font-size-base;
        font-weight: 600;
    }

    ul {
        margin: 0;
        padding: 0;
        list-style: none;
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
        gap: 0.5rem;
    }

    li {
        font-size: $font-size-sm-alt;
        color: $color-text-secondary;
    }

    kbd {
        background: $color-bg-overlay;
        border: 1px solid $color-border-primary;
        border-radius: $radius-secondary;
        padding: 0.125rem 0.375rem;
        font-size: $font-size-sm;
        font-family: "JetBrains Mono", monospace;
        color: $color-text-primary;
        box-shadow: $shadow-light;
        margin-right: 0.5rem;
    }
}

@media (max-width: 768px) {
    .markdown-editor {
        margin: 0;
        border-radius: 0;
        border-left: none;
        border-right: none;
    }

    .mode-button {
        padding: 0.625rem 1rem;
        font-size: $font-size-sm-alt;
    }

    .input-section,
    .output-section {
        padding: 1rem;
    }

    .markdown-input {
        height: 300px;
        padding: 0.75rem;
    }

    .markdown-output {
        min-height: 300px;
        padding: 0.75rem;
    }

    .keyboard-shortcuts {
        padding: 0.75rem 1rem;

        ul {
            grid-template-columns: 1fr;
        }
    }
}

// Side by side layout styles
.markdown-editor--side-by-side {
    .mode-toggle {
        display: none; // Hide toggle for side-by-side
    }
}

.editor-container--side-by-side {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 1rem;
    min-height: 500px;
}

.input-section--side,
.output-section--side {
    display: flex;
    flex-direction: column;
    height: 500px;
    padding: 1rem;

    .section-title {
        margin: 0 0 0.75rem 0;
        font-size: $font-size-base;
        font-weight: 600;
        color: $color-text-primary;
        padding-bottom: 0.5rem;
        border-bottom: 1px solid $color-border-primary;
    }
}

.markdown-input--side {
    flex: 1;
    height: auto;
}

.markdown-output--side {
    flex: 1;
    height: auto;
}
</style>
