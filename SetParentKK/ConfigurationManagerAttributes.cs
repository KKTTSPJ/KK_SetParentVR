namespace SetParentKK
{
	/// <summary>
	/// Display hints for BepInEx.ConfigurationManager (the in-game plugin settings window).
	/// ConfigurationManager looks these fields up by name on any object passed in ConfigDescription.Tags,
	/// so only the fields this plugin uses are declared.
	/// </summary>
	internal sealed class ConfigurationManagerAttributes
	{
		/// <summary>Name shown in the settings window instead of the config key.</summary>
		public string DispName;
	}
}
