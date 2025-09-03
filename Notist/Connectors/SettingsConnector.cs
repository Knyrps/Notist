using System.Runtime.InteropServices;
using Notist.Core.Settings;
using Newtonsoft.Json;

namespace Notist.Connectors
{
    [ComVisible(true)]
    public class SettingsConnector
    {
        #region Public Methods

        /// <summary>
        /// Gets the current settings as a JSON string
        /// </summary>
        /// <returns>ConnectorInvocationResult with JSON representation of current settings</returns>
        public ConnectorInvocationResult GetSettings()
        {
            try
            {
                var currentSettings = AppSettings.Current;
                if (currentSettings == null)
                {
                    return new ConnectorInvocationResult
                    {
                        Success = false,
                        Error = "AppSettings.Current is null"
                    };
                }

                var settings = new
                {
                    EnableNotifications = currentSettings.EnableNotifications,
                    OpenOnLaunch = currentSettings.OpenOnLaunch,
                    Theme = currentSettings.Theme,
                    Transparency = currentSettings.Transparency,
                    HotkeyModifiers = currentSettings.HotkeyModifiers,
                    HotkeyKey = currentSettings.HotkeyKey
                };

                var json = JsonConvert.SerializeObject(settings);
                return new ConnectorInvocationResult
                {
                    Success = true,
                    Data = json
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
        /// Updates the settings from a JSON string and saves to file
        /// </summary>
        /// <param name="settingsJson">JSON string containing the settings to update</param>
        /// <returns>ConnectorInvocationResult indicating success or failure</returns>
        public ConnectorInvocationResult UpdateSettings(string settingsJson)
        {
            try
            {
                dynamic? settings = JsonConvert.DeserializeObject(settingsJson);

                if (settings != null)
                {
                    if (settings.EnableNotifications != null)
                        AppSettings.Current.EnableNotifications = (bool)settings.EnableNotifications;

                    if (settings.OpenOnLaunch != null)
                        AppSettings.Current.OpenOnLaunch = (bool)settings.OpenOnLaunch;

                    if (settings.Theme != null)
                        AppSettings.Current.Theme = (string)settings.Theme;

                    if (settings.Transparency != null)
                        AppSettings.Current.Transparency = (double)settings.Transparency;

                    if (settings.HotkeyModifiers != null)
                        AppSettings.Current.HotkeyModifiers = (string)settings.HotkeyModifiers;

                    if (settings.HotkeyKey != null)
                        AppSettings.Current.HotkeyKey = (string)settings.HotkeyKey;

                    // Always save to file when updating settings
                    AppSettings.Current.Save();
                    return new ConnectorInvocationResult { Success = true };
                }

                return new ConnectorInvocationResult
                {
                    Success = false,
                    Error = "Invalid settings data provided"
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

        #endregion
    }
}