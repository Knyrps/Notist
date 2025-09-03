using Notist.Core.Helpers;

namespace Notist.Core.Settings;

public sealed class AppSettings
{
	#region Other Fields

	private static readonly Lazy<AppSettings> _current = new(() =>
	{
		try
		{
			var settings = IOHelper.LoadOrCreateSettingsFile();
			if (settings == null)
			{
				return new AppSettings();
			}
			return settings;
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
			return new AppSettings();
		}
	});

	#endregion

	#region Public Properties

	public static AppSettings Current => _current.Value;

	public bool EnableNotifications { get; set; }

	public bool OpenOnLaunch { get; set; }

	public string Theme { get; set; }

	public double Transparency { get; set; }

	public string HotkeyModifiers { get; set; }

	public string HotkeyKey { get; set; }

	#endregion

	#region Constructors

	public AppSettings()
	{
		// Defaults
		this.EnableNotifications = true;
		this.OpenOnLaunch = false;
		this.Theme = "Auto";
		this.Transparency = 0.95;
		this.HotkeyModifiers = "Control";
		this.HotkeyKey = "Space";
	}

	#endregion

	#region Public Methods

	public void Save()
	{
		IOHelper.SaveSettingsToFile(this);
	}

	#endregion
}
