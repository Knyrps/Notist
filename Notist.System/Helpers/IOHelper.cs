using Newtonsoft.Json;

using Notist.Core.Settings;

namespace Notist.Core.Helpers;

internal static class IOHelper
{
	#region Other Fields

	private static readonly string SettingsFile = Path.Combine(GetAppDataFolder(), "settings.json");

	#endregion

	#region Public Methods

	public static string GetAppDataFolder()
	{
		return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "notist");
	}

	public static bool IsDirectoryWritable(string dirPath, bool throwIfFails = false)
	{
		try
		{
			using (FileStream fs = File.Create(Path.Combine(dirPath,
						   Path.GetRandomFileName()
					   ),
					   1,
					   FileOptions.DeleteOnClose
				   )
				  ) { }
			return true;
		} catch
		{
			if (throwIfFails)
			{
				throw;
			}
			return false;
		}
	}

	public static AppSettings LoadOrCreateSettingsFile()
	{
		try
		{
			if (!File.Exists(SettingsFile))
			{
				AppSettings defaults = new();
				SaveSettingsToFile(defaults);
				return defaults;
			}

			string json = File.ReadAllText(SettingsFile);
			return JsonConvert.DeserializeObject<AppSettings>(json) ?? new AppSettings();
		}
		catch (JsonReaderException ex)
		{
			// Handle corrupted file, creating new one.
			System.Diagnostics.Debug.WriteLine($"Settings file corrupted: {ex.Message}");
			AppSettings defaults = new();
			SaveSettingsToFile(defaults);
			return defaults;
		}
		catch (Exception ex)
		{
			// Handle any other file I/O issues
			System.Diagnostics.Debug.WriteLine($"Error loading settings file: {ex.Message}");
			return new AppSettings(); // Return defaults without trying to save
		}
	}

	public static void SaveSettingsToFile(AppSettings settings)
	{
		try
		{
			Directory.CreateDirectory(GetAppDataFolder());
			string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
			File.WriteAllText(SettingsFile, json);
		}
		catch (Exception ex)
		{
			// Log the error but don't throw, as this could break the application
			System.Diagnostics.Debug.WriteLine($"Error saving settings file: {ex.Message}");
		}
	}

	#endregion
}
