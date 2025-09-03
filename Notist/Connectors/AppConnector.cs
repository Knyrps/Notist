using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Windows;

namespace Notist.Connectors
{
    [ComVisible(true)]
    public class AppConnector
    {
        #region Public Methods

        /// <summary>
        /// Displays a message box to the user
        /// </summary>
        /// <param name="msg">The message to display</param>
        /// <returns>ConnectorInvocationResult indicating success or failure</returns>
        public ConnectorInvocationResult ShowMessage(string msg)
        {
            try
            {
                MessageBox.Show(msg, "Message from Frontend");
                return new ConnectorInvocationResult { Success = true };
            }
            catch (Exception ex)
            {
                return new ConnectorInvocationResult { Success = false, Error = ex.Message };
            }
        }

        /// <summary>
        /// Toggles the overlay window visibility
        /// </summary>
        /// <returns>ConnectorInvocationResult indicating success or failure</returns>
        public ConnectorInvocationResult ToggleOverlay()
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (Application.Current.MainWindow is OverlayWindow ow)
                        {
                            if (ow.IsVisible)
                            {
                                ow.Hide();
                            }
                            else
                            {
                                ow.Show();
                            }
                        }
                    }
                );
                return new ConnectorInvocationResult { Success = true };
            }
            catch (Exception ex)
            {
                return new ConnectorInvocationResult { Success = false, Error = ex.Message };
            }
        }

		public ConnectorInvocationResult GetVersion()
		{
            try
            {
                return new ConnectorInvocationResult
                {
                    Success = true,
                    Data = "0.0.0"
                };
            }
            catch (Exception ex)
            {
                return new ConnectorInvocationResult
                {
                    Success = false,
                    Error = ex.Message
                };
            }
		}

        /// <summary>
        /// Quits the application gracefully
        /// </summary>
        /// <returns>ConnectorInvocationResult indicating success or failure</returns>
        public ConnectorInvocationResult QuitApplication()
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (Application.Current is App app)
                    {
                        app.OnExit(null, new RoutedEventArgs());
                    }
                    else
                    {
                        Application.Current.Shutdown();
                    }
                });
                return new ConnectorInvocationResult { Success = true };
            }
            catch (Exception ex)
            {
                return new ConnectorInvocationResult { Success = false, Error = ex.Message };
            }
        }

        /// <summary>
        /// Updates the overlay window transparency for preview purposes
        /// </summary>
        /// <param name="transparencyJson">JSON-serialized transparency value (0.1 to 1.0)</param>
        /// <returns>ConnectorInvocationResult indicating success or failure</returns>
        public ConnectorInvocationResult UpdateTransparency(string transparencyJson)
        {
            try
			{
				var transparency = JsonConvert.DeserializeObject<double>(transparencyJson);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (Application.Current.MainWindow is OverlayWindow ow)
                    {
                        ow.UpdateTransparency(transparency);
                    }
                });

                return new ConnectorInvocationResult { Success = true };
            }
            catch (Exception ex)
            {
                return new ConnectorInvocationResult { Success = false, Error = ex.Message };
            }
        }

        /// <summary>
        /// Pauses the global hotkey registration
        /// </summary>
        /// <returns>ConnectorInvocationResult indicating success or failure</returns>
        public ConnectorInvocationResult PauseHotkey()
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (Application.Current is App app)
                    {
                        app.UnregisterHotkey();
                    }
                });
                return new ConnectorInvocationResult { Success = true };
            }
            catch (Exception ex)
            {
                return new ConnectorInvocationResult { Success = false, Error = ex.Message };
            }
        }

        /// <summary>
        /// Resumes the global hotkey registration
        /// </summary>
        /// <returns>ConnectorInvocationResult indicating success or failure</returns>
        public ConnectorInvocationResult ResumeHotkey()
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (Application.Current is App app)
                    {
                        app.RegisterHotkey();
                    }
                });
                return new ConnectorInvocationResult { Success = true };
            }
            catch (Exception ex)
            {
                return new ConnectorInvocationResult { Success = false, Error = ex.Message };
            }
        }

        #endregion
    }
}
