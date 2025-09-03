<template>
    <div class="keybind-input">
        <div class="keybind-display">
            <button
                class="keybind-button"
                :class="{ 'keybind-button--recording': isRecording }"
                @click="startRecording"
                @keydown="handleKeyDown"
                @blur="cancelRecording"
                tabindex="0"
            >
                <span v-if="isRecording">{{ displayValue }}</span>
                <span v-else>
                    <kbd class="shortcut-key"> {{ displayValue }}</kbd></span
                >
            </button>
            <div v-if="isRecording" class="keybind-help">
                Press any key combination, then press Enter to confirm or Escape
                to cancel
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, computed, onUnmounted } from "vue";
import { AppService } from "@/lib/appService";

interface Props {
    modelValue: {
        modifiers: string;
        key: string;
    };
}

interface Emits {
    (e: "update:modelValue", value: { modifiers: string; key: string }): void;
}

const props = defineProps<Props>();
const emit = defineEmits<Emits>();

const isRecording = ref(false);
const pendingModifiers = ref<string[]>([]);
const pendingKey = ref<string>("");

const displayValue = computed(() => {
    if (isRecording.value) {
        if (pendingKey.value) {
            const modifierText =
                pendingModifiers.value.length > 0
                    ? pendingModifiers.value.join(" + ") + " + "
                    : "";
            return modifierText + pendingKey.value;
        } else if (pendingModifiers.value.length > 0) {
            // Show modifiers only if no key is pressed yet
            return pendingModifiers.value.join(" + ") + " + ...";
        }
        return "Press any key...";
    }

    const modifiers = props.modelValue.modifiers
        .split(",")
        .filter((m) => m.trim())
        .join(" + ");

    return modifiers
        ? `${modifiers} + ${props.modelValue.key}`
        : props.modelValue.key;
});

const startRecording = async () => {
    if (isRecording.value) return;

    isRecording.value = true;
    pendingModifiers.value = [];
    pendingKey.value = "";

    // Pause the global hotkey while recording
    try {
        await AppService.pauseHotkey();
    } catch (error) {
        console.error("Failed to pause hotkey:", error);
    }
};

const cancelRecording = async () => {
    if (!isRecording.value) return;

    isRecording.value = false;
    pendingModifiers.value = [];
    pendingKey.value = "";

    // Resume the global hotkey
    try {
        await AppService.resumeHotkey();
    } catch (error) {
        console.error("Failed to resume hotkey:", error);
    }
};

const handleKeyDown = async (event: KeyboardEvent) => {
    if (!isRecording.value) return;

    event.preventDefault();
    event.stopPropagation();

    // Handle special keys for confirmation/cancellation
    if (event.key === "Escape") {
        await cancelRecording();
        return;
    }

    if (event.key === "Enter") {
        await confirmRecording();
        return;
    }

    // Collect modifiers
    const modifiers: string[] = [];
    if (event.ctrlKey) modifiers.push("Control");
    if (event.altKey) modifiers.push("Alt");
    if (event.shiftKey) modifiers.push("Shift");
    if (event.metaKey) modifiers.push("Win");

    // Check if this is just a modifier key press (ignore standalone modifier keys)
    const isModifierKey = ["Control", "Alt", "Shift", "Meta", "OS"].includes(
        event.key
    );

    if (isModifierKey) {
        // Only update modifiers, don't set a key
        pendingModifiers.value = modifiers;
        pendingKey.value = ""; // Clear any previous key
        return;
    }

    // Get the actual key (excluding modifiers)
    let key = event.key;

    // Normalize key names
    switch (event.code) {
        case "Space":
            key = "Space";
            break;
        case "Escape":
            key = "Escape";
            break;
        case "Enter":
            key = "Enter";
            break;
        case "Tab":
            key = "Tab";
            break;
        case "Delete":
            key = "Delete";
            break;
        default:
            // For letter keys, use the code to get the actual letter
            if (event.code.startsWith("Key")) {
                key = event.code.slice(3); // Remove "Key" prefix
            }
            // For digit keys
            else if (event.code.startsWith("Digit")) {
                key = event.code.slice(5); // Remove "Digit" prefix
            }
            // For function keys
            else if (event.code.startsWith("F") && /^F\d+$/.test(event.code)) {
                key = event.code;
            }
            break;
    }

    pendingModifiers.value = modifiers;
    pendingKey.value = key;
};

const confirmRecording = async () => {
    if (!isRecording.value || !pendingKey.value) return;

    // Don't allow just modifier keys
    if (
        !pendingKey.value ||
        ["Control", "Alt", "Shift", "Meta"].includes(pendingKey.value)
    ) {
        await cancelRecording();
        return;
    }

    const newValue = {
        modifiers: pendingModifiers.value.join(","),
        key: pendingKey.value,
    };

    emit("update:modelValue", newValue);

    await cancelRecording();
};

// Cleanup on unmount
onUnmounted(async () => {
    if (isRecording.value) {
        await cancelRecording();
    }
});
</script>

<style scoped lang="scss">
@import "@/styles/_fonts.scss";
@import "@/styles/variables";

.keybind-input {
    display: flex;
    flex-direction: column;
    gap: 8px;
}

.keybind-display {
    display: flex;
    flex-direction: column;
    gap: 4px;
}

// DUPLICATE
.shortcut-key {
    background: $color-bg-primary;
    border: 1px solid $color-border-primary;
    border-radius: $radius-secondary;
    padding: 0.25rem 0.5rem;
    font-size: 0.75rem;
    font-family: monospace;
    color: $color-text-secondary;
}

.keybind-button {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    min-width: 120px;
    height: 32px;
    padding: 0 12px;
    background: var(--color-background-secondary);
    border: 1px solid var(--color-border-secondary);
    border-radius: 6px;
    color: var(--color-text-primary);
    font-family: "Consolas", monospace;
    font-size: 13px;
    font-weight: 500;
    cursor: pointer;
    transition: all 0.2s ease;

    &:hover {
        background: var(--color-background-tertiary);
        border-color: var(--color-border-tertiary);
    }

    &:focus {
        outline: 2px solid var(--color-accent-primary);
        outline-offset: 2px;
    }

    &--recording {
        background: var(--color-error);
        border-color: var(--color-error);
        color: white;
        animation: pulse 1.5s ease-in-out infinite;

        &:hover {
            background: var(--color-error);
            border-color: var(--color-error);
        }
    }
}

.keybind-help {
    font-size: 12px;
    color: var(--color-error);
    font-weight: 500;
    line-height: 1.4;
}

@keyframes pulse {
    0%,
    100% {
        opacity: 1;
    }
    50% {
        opacity: 0.7;
    }
}
</style>
