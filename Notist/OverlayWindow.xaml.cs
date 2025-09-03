using Notist.Connectors;
using Notist.Core.Settings;
using System.Drawing;
using System.IO;
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
                this.WebView.DefaultBackgroundColor = Color.FromArgb(0, 0, 0, 0);
                this.frontendNegotiator = new FrontendNegotiator(this.WebView.CoreWebView2);

                // Apply initial transparency setting
                this.ApplyTransparencySetting();
            }
            catch (Exception ex)
            {
                this.WebView.NavigateToString(
                    $"<html><body style=\"display: flex; justify-content: center; align-items: center;\"><pre style=background-color: white; \"color: red\">WebView2 init failed: {ex.Message}</pre></body></html>"
                );
            }
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
