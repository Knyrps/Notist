import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";
import path from "node:path";

// https://vite.dev/config/
export default defineConfig({
    plugins: [vue()],
    css: {
        preprocessorOptions: {
            scss: {
                additionalData: `@import "@/styles/_fonts.scss";`,
            },
        },
    },
    resolve: {
        alias: {
            "@": path.resolve(__dirname, "./src"),
        },
    },
    build: {
        // Output to the backend project's Resources/Frontend directory
        outDir: path.resolve(__dirname, "../Notist/Resources/Frontend"),
        emptyOutDir: true,
        // Generate relative paths for assets
        assetsDir: "assets",
        // Ensure compatibility with WebView2
        target: "es2020",
        // Generate source maps for debugging
        sourcemap: process.env.NODE_ENV === "development",
        rollupOptions: {
            output: {
                // Ensure consistent asset naming
                assetFileNames: "assets/[name]-[hash][extname]",
                chunkFileNames: "assets/[name]-[hash].js",
                entryFileNames: "assets/[name]-[hash].js",
            },
        },
    },
});
