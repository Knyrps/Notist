import { ref } from "vue";
import { marked } from "marked";

export type MarkdownData = {
    content: string;
};

type UseMarkdownReturn = {
    raw: string;
    html: string;
    updateContent: (content: string) => void;
    handleKeyboardEvent: (event: KeyboardEvent) => void;
};

type UseMarkdownParams = {
    initialContent?: string;
    customKeyHandler?: (event: KeyboardEvent, currentContent: string) => void;
};

export function useMarkdown(params: UseMarkdownParams = {}): UseMarkdownReturn {
    const { initialContent = "", customKeyHandler } = params;

    // Reactive raw content
    const rawContent = ref<string>(initialContent);

    // Reactive HTML content
    const htmlContent = ref<string>("");

    // Watch for changes in raw content and update HTML
    const updateHtml = async () => {
        try {
            const result = await marked(rawContent.value);
            htmlContent.value =
                typeof result === "string" ? result : await result;
        } catch (error) {
            console.error("Markdown parsing error:", error);
            htmlContent.value = rawContent.value; // Fallback to raw content
        }
    };

    // Initial HTML generation
    updateHtml();

    // Update content method
    const updateContent = (content: string) => {
        rawContent.value = content;
        // Don't await here to keep the function synchronous, but handle the async update
        updateHtml().catch(console.error);
    };

    // Keyboard event handler
    const handleKeyboardEvent = (event: KeyboardEvent) => {
        // Call custom handler if provided
        if (customKeyHandler) {
            customKeyHandler(event, rawContent.value);
        }

        // Default keyboard shortcuts
        switch (true) {
            case event.ctrlKey && event.key === "b":
                event.preventDefault();
                insertMarkdown("**", "**");
                break;
            case event.ctrlKey && event.key === "i":
                event.preventDefault();
                insertMarkdown("*", "*");
                break;
            case event.ctrlKey && event.key === "k":
                event.preventDefault();
                insertMarkdown("[", "](url)");
                break;
            case event.key === "Tab":
                event.preventDefault();
                insertMarkdown("  "); // Two spaces for indentation
                break;
        }
    };

    // Helper function to insert markdown at cursor position
    const insertMarkdown = (before: string, after: string = "") => {
        const target = event?.target as HTMLTextAreaElement;
        if (!target) return;

        const start = target.selectionStart;
        const end = target.selectionEnd;
        const selectedText = rawContent.value.substring(start, end);

        const newContent =
            rawContent.value.substring(0, start) +
            before +
            selectedText +
            after +
            rawContent.value.substring(end);

        updateContent(newContent);

        // Restore cursor position
        setTimeout(() => {
            const newCursorPos = start + before.length + selectedText.length;
            target.setSelectionRange(newCursorPos, newCursorPos);
            target.focus();
        }, 0);
    };

    return {
        raw: rawContent.value,
        html: htmlContent.value,
        updateContent,
        handleKeyboardEvent,
    };
}
