using Notist.Connectors;
using Notist.Core.Settings;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace Notist
{

    public partial class OverlayWindow : Window
    {
        #region Other Fields

        private FrontendNegotiator? frontendNegotiator;

        #endregion

        #region Constructors

        public OverlayWindow()
        {
            this.InitializeComponent();
            this.Loaded += this.OverlayWindow_Loaded;
        }

        #endregion

        #region Private Helpers

        private async void OverlayWindow_Loaded(object? sender, RoutedEventArgs e)
        {
            try
            {
                await this.WebView.EnsureCoreWebView2Async();
                
                // First, load the loading screen
                var loadingPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "loading.html");
                if (File.Exists(loadingPath))
                {
                    var loadingUri = new Uri($"file:///{loadingPath.Replace('\\', '/')}");
                    this.WebView.Source = loadingUri;
                }
                else
                {
                    // Fallback loading screen if file doesn't exist
                    this.WebView.NavigateToString(@"
                        <html><head><style>
                        body { margin: 0; display: grid; place-items: center; height: 100vh; background: #ffffff; color: #111111; font-family: 'Segoe UI', sans-serif; }
                        @media (prefers-color-scheme: dark) { body { background: #0b0b0c; color: #f5f5f5; } }
                        .loader { width: 40px; height: 40px; border: 4px solid rgba(100, 150, 255, 0.3); border-top: 4px solid rgba(100, 150, 255, 0.8); border-radius: 50%; animation: spin 1s linear infinite; }
                        @keyframes spin { 0% { transform: rotate(0deg); } 100% { transform: rotate(360deg); } }
                        </style></head><body><div style='text-align: center;'><h2>NOTIST</h2><div class='loader'></div></div></body></html>
                    ");
                }

                // Set transparent background
                this.WebView.DefaultBackgroundColor = Color.FromArgb(0, 0, 0, 0);

                // Wait a minimum time for smooth loading experience, then load the main app
                _ = Task.Run(async () =>
                {
                    // Minimum loading time for smooth UX
                    await Task.Delay(800);
                    
                    // Switch to main application on UI thread
                    this.Dispatcher.Invoke(async () =>
                    {
                        try
                        {
                            await this.LoadMainApplication();
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error loading main application: {ex.Message}");
                            this.WebView.NavigateToString(
                                $"<html><body style=\"display: flex; justify-content: center; align-items: center;\"><pre style=\"background-color: white; color: red;\">Failed to load main app: {ex.Message}</pre></body></html>"
                            );
                        }
                    });
                });

                // Initialize frontend negotiator (will be ready when main app loads)
                this.frontendNegotiator = new FrontendNegotiator(this.WebView.CoreWebView2);

                // Apply initial transparency setting
                this.ApplyTransparencySetting();
            }
            catch (Exception ex)
            {
                this.WebView.NavigateToString(
                    $"<html><body style=\"display: flex; justify-content: center; align-items: center;\"><pre style=\"background-color: white; color: red;\">WebView2 init failed: {ex.Message}</pre></body></html>"
                );
            }
        }

        /// <summary>
        /// Loads the main application (either dev server or production build)
        /// </summary>
        private Task LoadMainApplication()
        {
#if DEBUG
            // Load Vite dev server during development
            string frontend = "http://localhost:5173";
            this.WebView.Source = new Uri(frontend);
#else
            // Set up virtual host mapping for production
            var frontendPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Frontend");
            
            if (Directory.Exists(frontendPath))
            {
                // Map the frontend assets to a virtual host to avoid CORS issues
                this.WebView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                    "notist.local", 
                    frontendPath, 
                    Microsoft.Web.WebView2.Core.CoreWebView2HostResourceAccessKind.Allow
                );
                
                // Navigate to the virtual host
                this.WebView.Source = new Uri("https://notist.local/index.html");
            }
            else
            {
                // helpful fallback while developing
                WebView.NavigateToString("<html><body style=\"display: flex; justify-content: center; align-items: center;\"><h2 style=\"background-color: white; color: black\">Frontend folder not found in Resources</h2></body></html>");
            }
#endif
            return Task.CompletedTask;
        }

        /// <summary>
        /// Applies the current transparency setting to the window
        /// </summary>
        private void ApplyTransparencySetting()
        {
            try
            {
                // Get current transparency setting (0.0 to 1.0)
                double transparency = AppSettings.Current.Transparency;

                // Clamp the value to valid range
                transparency = Math.Max(0.0, Math.Min(1.0, transparency));

                // Apply transparency to the entire window
                this.Opacity = transparency;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error applying transparency setting: {ex.Message}");
                // Fallback to default transparency
                this.Opacity = 0.95;
            }
        }

        /// <summary>
        /// Updates the transparency setting and applies it immediately
        /// </summary>
        public void UpdateTransparency(double transparency)
        {
            try
            {
                // Clamp the value to valid range
                transparency = Math.Max(0.0, Math.Min(1.0, transparency));

                // Apply to window immediately
                this.Opacity = transparency;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating transparency: {ex.Message}");
            }
        }

        #endregion
    }
}
