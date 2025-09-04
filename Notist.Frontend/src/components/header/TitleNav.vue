<template>
    <PopoverRoot>
        <PopoverTrigger asChild>
            <nav class="nav nav-bubble">
                <span class="nav__button nav__button--inactive"
                    >{{ props.text }}<small>{{ props.version }}</small></span
                >
            </nav>
        </PopoverTrigger>
        <PopoverPortal>
            <PopoverContent
                className="popover-content"
                :sideOffset="8"
                :arrowPadding="8"
                side="bottom"
                align="start"
            >
                <div class="popover-header">
                    <h3 class="popover-title">{{ title || text }}</h3>
                    <PopoverClose className="popover-close" aria-label="Close">
                        <FontAwesomeIcon :icon="faX" />
                    </PopoverClose>
                </div>

                <div class="popover-body">
                    <div
                        v-if="description"
                        class="popover-section popover-section--description"
                    >
                        <p class="popover-text">{{ description }}</p>
                    </div>

                    <template v-if="sections && sections.length > 0">
                        <template
                            v-for="(section, index) in sections"
                            :key="index"
                        >
                            <Separator
                                v-if="index > 0 || description"
                                className="popover-separator"
                            />
                            <div class="popover-section">
                                <h4
                                    v-if="section.title"
                                    class="popover-section-title"
                                >
                                    {{ section.title }}
                                </h4>
                                <p class="popover-text">
                                    {{ section.content }}
                                </p>
                            </div>
                        </template>
                    </template>

                    <template v-if="version || author">
                        <Separator className="popover-separator" />
                        <div class="popover-section popover-section--footer">
                            <span v-if="version" class="popover-version">{{
                                version
                            }}</span>
                            <Separator
                                orientation="vertical"
                                className="popover-separator--vertical"
                                v-if="version && author"
                            ></Separator>
                            <span
                                v-if="author"
                                class="popover-author"
                                v-html="author"
                            ></span>
                        </div>
                    </template>
                </div>
                <PopoverArrow className="popover-arrow" />
            </PopoverContent>
        </PopoverPortal>
    </PopoverRoot>
</template>
<script setup lang="ts">
import { faX } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";
import {
    PopoverRoot,
    PopoverTrigger,
    PopoverPortal,
    PopoverContent,
    PopoverClose,
    PopoverArrow,
    Separator,
} from "radix-vue";

interface PopoverSection {
    title?: string;
    content: string;
}

const props = defineProps<{
    text: string;
    link?: URL;
    title?: string;
    description?: string;
    sections?: PopoverSection[];
    version?: string;
    author?: string;
}>();
</script>
<style scoped lang="scss">
@import "@/styles/variables";

// Navigation Button Styles
.nav.nav-bubble {
    .nav__button {
        user-select: none;
        -webkit-user-select: none;
        -moz-user-select: none;
        -ms-user-select: none;
        display: inline-flex;
        align-items: center;
        padding: 0.5rem 1rem;
        background: $color-bg-primary;
        border: 1px solid $color-border-primary;
        border-radius: $radius-primary;
        color: $color-text-primary;
        text-decoration: none;
        font-size: 0.875rem;
        font-weight: 500;
        cursor: pointer;
        transition: all $transition-medium;
        backdrop-filter: $blur-light;
        box-shadow: $shadow-light;
        white-space: nowrap;
        gap: 0.2rem;

        &:hover:not(&--inactive) {
            background: $color-bg-hover;
            border-color: $color-border-hover;
            box-shadow: $shadow-medium;
            transform: translateY(-1px);
        }

        &:active:not(&--inactive) {
            background: $color-bg-active;
            transform: translateY(0);
            box-shadow: $shadow-active;
        }

        &--inactive {
            cursor: default;
            opacity: 0.9;
        }
    }
}
</style>

<!-- Global Popover Styles -->
<style lang="scss">
@import "@/styles/variables";

// Popover Content - Must be global for Radix Portal
.popover-content {
    background: $color-bg-secondary;
    border: 1px solid $color-border-primary;
    border-radius: $radius-primary;
    box-shadow: $shadow-strong;
    backdrop-filter: $blur-strong;
    min-width: 280px;
    max-width: 360px;
    z-index: $z-dropdown;
    will-change: transform, opacity;

    // Animation States
    &[data-state="open"] {
        animation: popoverSlideIn 0.2s cubic-bezier(0.16, 1, 0.3, 1);
    }

    &[data-state="closed"] {
        animation: popoverSlideOut 0.15s ease-in;
    }
}

// Popover Header
.popover-header {
    border-top-left-radius: $radius-primary;
    border-top-right-radius: $radius-primary;
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 0.5rem 1rem;
    border-bottom: 1px solid $color-border-primary;
    background: $color-bg-primary;

    .popover-title {
        margin: 0;
        font-size: 0.875rem;
        font-weight: 600;
        color: $color-text-primary;
    }

    .popover-close {
        display: inline-flex;
        align-items: center;
        justify-content: center;
        width: 1.5rem;
        height: 1.5rem;
        padding: 0;
        background: transparent;
        border: 1px solid $color-border-primary;
        border-radius: $radius-secondary;
        color: $color-text-secondary;
        cursor: pointer;
        transition: all $transition-medium;

        &:hover {
            background: $color-bg-hover;
            border-color: $color-border-hover;
            color: $color-text-primary;
        }

        &:active {
            background: $color-bg-active;
            transform: scale(0.95);
        }

        svg {
            width: 0.75rem;
            height: 0.75rem;
        }
    }
}

// Popover Body
.popover-body {
    padding: 0;
}

// Popover Sections
.popover-section {
    padding: 1rem;

    &--description {
        font-style: italic;
        color: $color-text-secondary;
        padding: 0.75rem 1rem;
    }

    &:not(:last-child) {
        border-bottom: 1px solid $color-border-primary;
    }

    &--footer {
        padding: 0.75rem 1rem;
        background: $color-bg-primary;
        border-top: 1px solid $color-border-primary;
    }

    .popover-section-title {
        margin: 0 0 0.75rem 0;
        font-size: 0.75rem;
        font-weight: 600;
        color: $color-text-primary;
        text-transform: uppercase;
        letter-spacing: 0.05em;
    }

    .popover-text {
        margin: 0;
        font-size: 0.8125rem;
        line-height: 1.5;
        color: $color-text-secondary;
    }

    .popover-version,
    .popover-author {
        font-size: 0.6875rem;
        color: $color-text-secondary;
        opacity: 0.7;
        font-family: "Courier New", monospace;
        font-weight: 500;

        & > a {
            color: $color-text-secondary;
            text-decoration: underline;

            &:hover {
                color: $color-text-primary;
            }

            &:visited {
                color: $color-text-secondary;
            }
        }
    }

    &--footer {
        display: flex;
        align-items: center;
        border-bottom-left-radius: $radius-primary;
        border-bottom-right-radius: $radius-primary;
    }
}

// Popover Separator
.popover-separator {
    height: 1px;
    background: $color-border-primary;
    margin: 0;
    border: none;

    &--vertical {
        width: 1px;
        height: 0.8125rem; // Match text line height
        background: $color-border-primary;
        margin: 0 0.5rem;
        display: inline-block;
        vertical-align: middle;
        position: relative;
        top: -1px; // Fine-tune alignment with text baseline
    }
}

// Popover Arrow - Enhanced visibility
.popover-arrow {
    fill: $color-bg-secondary !important;
    stroke: $color-border-primary !important;
    stroke-width: 1.5px !important;
    width: 16px !important;
    height: 10px !important;
    z-index: 99999 !important;
    filter: drop-shadow(0 2px 4px rgba(0, 0, 0, 0.15)) !important;
    display: block !important;

    // Ensure the arrow is visible and properly positioned
    &[data-side="top"] {
        margin-bottom: -1px;
    }

    &[data-side="bottom"] {
        margin-top: -1px;
    }

    &[data-side="left"] {
        margin-right: -1px;
    }

    &[data-side="right"] {
        margin-left: -1px;
    }
}

// Alternative arrow styling for better visibility
[data-radix-popper-content-wrapper] .popover-arrow {
    fill: $color-bg-secondary !important;
    stroke: $color-border-primary !important;
}

// Animations
@keyframes popoverSlideIn {
    from {
        opacity: 0;
        transform: translateY(8px) scale(0.96);
    }
    to {
        opacity: 1;
        transform: translateY(0) scale(1);
    }
}

@keyframes popoverSlideOut {
    from {
        opacity: 1;
        transform: translateY(0) scale(1);
    }
    to {
        opacity: 0;
        transform: translateY(4px) scale(0.98);
    }
}
</style>
