import { ref, readonly, computed } from "vue";
import { markdownToHtml } from "./markdown/markdownService";
import type { Note, Position } from "@/types/note";

// Global note state
const notes = ref<Note[]>([
    {
        id: "note-1",
        title: "First Note",
        content:
            "This is **bold text** and *italic text* in the first note.\n\n- List item 1\n- List item 2",
        htmlContent: "",
        position: { x: 50, y: 50 },
        isDragging: false,
        isEditing: false,
        zIndex: 1,
    },
    {
        id: "note-2",
        title: "Second Note",
        content:
            "## Heading 2\n\nThis is a `code snippet` in the second note.\n\n> This is a blockquote",
        htmlContent: "",
        position: { x: 150, y: 100 },
        isDragging: false,
        isEditing: false,
        zIndex: 2,
    },
    {
        id: "note-3",
        title: "Third Note",
        content:
            '### Links and formatting\n\nCheck out [this link](https://example.com)!\n\n```csharp\nStringBuilder x = new StringBuilder();\n\nusing (var sr = new StreamReader()) {\n    System.Console.WriteLine("hello")\n}\n```',
        htmlContent: "",
        position: { x: 250, y: 150 },
        isDragging: false,
        isEditing: false,
        zIndex: 3,
    },
]);

const maxZIndex = ref(3);

// Initialize HTML content for existing notes
const initializeNotes = async () => {
    for (const note of notes.value) {
        if (!note.htmlContent) {
            note.htmlContent = await markdownToHtml(note.content);
        }
    }
};

// Initialize on module load
initializeNotes();

export function useNotes() {
    // Computed values
    const allNotes = computed(() => notes.value);
    const editingNote = computed(() =>
        notes.value.find((note) => note.isEditing)
    );

    // Note CRUD operations
    const createNote = async (
        title: string,
        content: string,
        position: Position
    ): Promise<Note> => {
        const htmlContent = await markdownToHtml(content);
        const newNote: Note = {
            id: `note-${Date.now()}`,
            title,
            content,
            htmlContent,
            position,
            isDragging: false,
            isEditing: false,
            zIndex: ++maxZIndex.value,
        };
        notes.value.push(newNote);
        return newNote;
    };

    const updateNote = async (
        noteId: string,
        title: string,
        content: string
    ) => {
        const note = notes.value.find((n) => n.id === noteId);
        if (note) {
            note.title = title;
            note.content = content;
            note.htmlContent = await markdownToHtml(content);
            note.isEditing = false;
        }
    };

    const deleteNote = (noteId: string) => {
        const index = notes.value.findIndex((n) => n.id === noteId);
        if (index > -1) {
            notes.value.splice(index, 1);
        }
    };

    // Note state management
    const setNotePosition = (noteId: string, position: Position) => {
        const note = notes.value.find((n) => n.id === noteId);
        if (note) {
            note.position = position;
        }
    };

    const setNoteDragging = (noteId: string, isDragging: boolean) => {
        const note = notes.value.find((n) => n.id === noteId);
        if (note) {
            note.isDragging = isDragging;
        }
    };

    const setNoteEditing = (noteId: string, isEditing: boolean) => {
        const note = notes.value.find((n) => n.id === noteId);
        if (note) {
            // Close other editing notes if opening this one
            if (isEditing) {
                notes.value.forEach((n) => {
                    if (n.id !== noteId) {
                        n.isEditing = false;
                    }
                });
            }
            note.isEditing = isEditing;
        }
    };

    const bringNoteToFront = (noteId: string) => {
        const note = notes.value.find((n) => n.id === noteId);
        if (note && note.zIndex < maxZIndex.value) {
            note.zIndex = ++maxZIndex.value;
        }
    };

    const closeAllEditing = () => {
        notes.value.forEach((note) => {
            note.isEditing = false;
        });
    };

    // Helper to get a specific note
    const getNoteById = (noteId: string) => {
        return notes.value.find((n) => n.id === noteId);
    };

    return {
        // Read-only access to notes
        notes: readonly(notes),
        allNotes,
        editingNote,

        // Note operations
        createNote,
        updateNote,
        deleteNote,
        getNoteById,

        // State management
        setNotePosition,
        setNoteDragging,
        setNoteEditing,
        bringNoteToFront,
        closeAllEditing,
    };
}
