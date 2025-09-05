export interface Position {
    x: number;
    y: number;
}

export interface Note {
    id: string;
    title: string;
    content: string;
    htmlContent: string;
    position: Position;
    isDragging: boolean;
    isEditing?: boolean;
    zIndex: number;
}

export interface DragData {
    noteId: string;
    offset: Position;
}
