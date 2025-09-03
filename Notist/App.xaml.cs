using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using H.NotifyIcon;

using Microsoft.Extensions.DependencyInjection;

using Notist.Core.DependencyInjection;
using Notist.Core.Managers;
using Notist.Core.Settings;

namespace Notist
{
    public partial class App : Application
	{
		public static IServiceProvider Services { get; private set; } = null!;

        // keep a strong reference so the tray icon is not GC'd
        private TaskbarIcon? _trayIcon;
        private OverlayWindow? _overlayWindow;
        private HwndSource? _messageWindow;

        // Hotkey constants
        private const int HOTKEY_ID = 0x9000;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        protected void OnStartup(object sender, StartupEventArgs e)
		{
			DI.Init();

            // load taskbar icon
            _trayIcon = (TaskbarIcon)FindResource("TrayIcon");

            // invisible window to get messages
            var parameters = new HwndSourceParameters("HotkeyWindow")
			{
				Width = 0,
				Height = 0,
				ParentWindow = new IntPtr(-3), // HWND_MESSAGE
				WindowStyle = 0
			};

            _messageWindow = new HwndSource(parameters);
            _messageWindow.AddHook(WndProc);

            // register command using settings
			RegisterHotkey();

            // keep the app alive with tray icon; no main window shown at startup
            this._trayIcon.ForceCreate();

            // this also serves as the initializer for AppSettings, performing an initial file read.
			if (AppSettings.Current.OpenOnLaunch)
			{
                this.ToggleOverlay();
			}
        }

        // Called from the TaskbarIcon context menu (wired in App.xaml)
        public void OnShowOverlay(object? sender, RoutedEventArgs e)
        {
            ToggleOverlay();
        }

        public void OnExit(object? sender, RoutedEventArgs e)
        {
            ShutdownApp();
        }

		public void OnOpenSettings(object? sender, RoutedEventArgs e)
		{
			// TODO: Settings
		}

        private void ToggleOverlay()
        {
            if (_overlayWindow == null)
            {
                _overlayWindow = new OverlayWindow();
                _overlayWindow.Closed += (s, ev) => _overlayWindow = null;
            }

            if (_overlayWindow.IsVisible)
            {
                _overlayWindow.Hide();
            }
            else
            {
                _overlayWindow.Show();
                _overlayWindow.Activate();
            }
        }

        private void ShutdownApp()
        {
            try
            {
                if (_messageWindow != null)
                {
                    UnregisterHotKey(_messageWindow.Handle, HOTKEY_ID);
                    _messageWindow.RemoveHook(WndProc);
                    _messageWindow.Dispose();
                    _messageWindow = null;
                }
            }
            catch { /* ignore cleanup errors */ }

            try
            {
                _trayIcon?.Dispose();
                _trayIcon = null;
            }
            catch { /* ignore */ }

            Shutdown();
        }

        /// <summary>
        /// Public method to register hotkey from settings
        /// </summary>
        public void RegisterHotkey()
        {
            if (_messageWindow != null)
            {
                var modifiers = GetModifiersFromSettings();
                var keyCode = GetKeyCodeFromSettings();
                RegisterHotKey(_messageWindow.Handle, HOTKEY_ID, modifiers, keyCode);
            }
        }

        /// <summary>
        /// Public method to unregister hotkey
        /// </summary>
        public void UnregisterHotkey()
        {
            if (_messageWindow != null)
            {
                UnregisterHotKey(_messageWindow.Handle, HOTKEY_ID);
            }
        }

        /// <summary>
        /// Get modifiers from settings
        /// </summary>
        private uint GetModifiersFromSettings()
        {
            var settingsModifiers = Notist.Core.Settings.AppSettings.Current.HotkeyModifiers;
            var modifiers = 0u;
            
            if (settingsModifiers.Contains("Control"))
                modifiers |= Keys.Modifiers.MOD_CONTROL;
            if (settingsModifiers.Contains("Alt"))
                modifiers |= Keys.Modifiers.MOD_ALT;
            if (settingsModifiers.Contains("Shift"))
                modifiers |= Keys.Modifiers.MOD_SHIFT;
            if (settingsModifiers.Contains("Win"))
                modifiers |= Keys.Modifiers.MOD_WIN;
                
            return modifiers;
        }

        /// <summary>
        /// Get key code from settings
        /// </summary>
        private uint GetKeyCodeFromSettings()
        {
            var settingsKey = Notist.Core.Settings.AppSettings.Current.HotkeyKey;
            
            // Map common key names to virtual key codes
            return settingsKey switch
            {
                "Space" => Keys.VirtualKeys.VK_SPACE,
                "A" => Keys.VirtualKeys.VK_A,
                "B" => Keys.VirtualKeys.VK_B,
                "C" => Keys.VirtualKeys.VK_C,
                "D" => Keys.VirtualKeys.VK_D,
                "E" => Keys.VirtualKeys.VK_E,
                "F" => Keys.VirtualKeys.VK_F,
                "G" => Keys.VirtualKeys.VK_G,
                "H" => Keys.VirtualKeys.VK_H,
                "I" => Keys.VirtualKeys.VK_I,
                "J" => Keys.VirtualKeys.VK_J,
                "K" => Keys.VirtualKeys.VK_K,
                "L" => Keys.VirtualKeys.VK_L,
                "M" => Keys.VirtualKeys.VK_M,
                "N" => Keys.VirtualKeys.VK_N,
                "O" => Keys.VirtualKeys.VK_O,
                "P" => Keys.VirtualKeys.VK_P,
                "Q" => Keys.VirtualKeys.VK_Q,
                "R" => Keys.VirtualKeys.VK_R,
                "S" => Keys.VirtualKeys.VK_S,
                "T" => Keys.VirtualKeys.VK_T,
                "U" => Keys.VirtualKeys.VK_U,
                "V" => Keys.VirtualKeys.VK_V,
                "W" => Keys.VirtualKeys.VK_W,
                "X" => Keys.VirtualKeys.VK_X,
                "Y" => Keys.VirtualKeys.VK_Y,
                "Z" => Keys.VirtualKeys.VK_Z,
                "0" => Keys.VirtualKeys.VK_0,
                "1" => Keys.VirtualKeys.VK_1,
                "2" => Keys.VirtualKeys.VK_2,
                "3" => Keys.VirtualKeys.VK_3,
                "4" => Keys.VirtualKeys.VK_4,
                "5" => Keys.VirtualKeys.VK_5,
                "6" => Keys.VirtualKeys.VK_6,
                "7" => Keys.VirtualKeys.VK_7,
                "8" => Keys.VirtualKeys.VK_8,
                "9" => Keys.VirtualKeys.VK_9,
                "F1" => Keys.VirtualKeys.VK_F1,
                "F2" => Keys.VirtualKeys.VK_F2,
                "F3" => Keys.VirtualKeys.VK_F3,
                "F4" => Keys.VirtualKeys.VK_F4,
                "F5" => Keys.VirtualKeys.VK_F5,
                "F6" => Keys.VirtualKeys.VK_F6,
                "F7" => Keys.VirtualKeys.VK_F7,
                "F8" => Keys.VirtualKeys.VK_F8,
                "F9" => Keys.VirtualKeys.VK_F9,
                "F10" => Keys.VirtualKeys.VK_F10,
                "F11" => Keys.VirtualKeys.VK_F11,
                "F12" => Keys.VirtualKeys.VK_F12,
                "Escape" => Keys.VirtualKeys.VK_ESCAPE,
                "Enter" => Keys.VirtualKeys.VK_ENTER,
                "Tab" => Keys.VirtualKeys.VK_TAB,
                "Delete" => Keys.VirtualKeys.VK_DELETE,
                _ => Keys.VirtualKeys.VK_SPACE // Default fallback
            };
        }

        // receive WM_HOTKEY
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            if (msg == WM_HOTKEY && wParam.ToInt32() == HOTKEY_ID)
            {
                // toggle overlay on UI thread
                Dispatcher.Invoke(ToggleOverlay);
                handled = true;
            }
            return IntPtr.Zero;
        }
    }
}
