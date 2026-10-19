using System;
using UnityEngine;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace SetParentKK
{
	[BepInPlugin(GUID, PluginName, Version)]
	[BepInProcess("Koikatu")]
	[BepInProcess("KoikatuVR")]
	[BepInProcess("Koikatsu Party")]
	[BepInProcess("Koikatsu Party VR")]
	public class KK_SetParentVR : BaseUnityPlugin
	{
		public const string GUID = "MK.KK_SetParentVR";
		public const string Version = "2.1.8";
		public const string PluginName = "SetParentVR";
		public const string AssembName = "KK_SetParentVR";
		internal static SetParent setParentObj;
		internal static ManualLogSource Log;

		private const string SectionGeneral = "General";
		private const string SectionKeys = "Keyboard Shortcuts";
		private const string SectionAdvanced = "Advanced Settings";


		public static ConfigEntry<float> Finishcount { get; private set; }

		public static ConfigEntry<ParentMode> SetParentMode { get; private set; }

		public static ConfigEntry<bool> DisableParentInput { get; private set; }

		public static ConfigEntry<float> StretchLimitArms { get; private set; }

		public static ConfigEntry<float> StretchLimitLegs { get; private set; }

		public static ConfigEntry<float> MenuUpProximity { get; private set; }

		public static ConfigEntry<bool> SetControllerCollider { get; private set; }

		public static ConfigEntry<bool> GazeControl { get; private set; }

		public static ConfigEntry<HideHandMode> GropeHandsDisplay { get; private set; }

		public static ConfigEntry<bool> MenuHideDefault { get; private set; }

		public static ConfigEntry<bool> HideParentConAlways { get; private set; }

		public static ConfigEntry<bool> SetMaleFeetCollider { get; private set; }

		public static ConfigEntry<bool> SyncMaleHands { get; private set; }

		public static ConfigEntry<bool> SetParentMale { get; private set; }

		public static ConfigEntry<ControllerAnimMode> CalcController { get; private set; }


		///
		//////////////////// Keyboard Shortcuts ///////////////////////////
		///
		public static ConfigEntry<KeyboardShortcut> LimbReleaseKey { get; private set; }

		public static ConfigEntry<KeyboardShortcut> MaleFeetToggle { get; private set; }

		public static ConfigEntry<KeyboardShortcut> SetParentToggle { get; private set; }


		///
		//////////////////// Advanced Settings ///////////////////////////
		///
		public static ConfigEntry<float> AnimMaxThreshold { get; private set; }

		public static ConfigEntry<float> AnimStartThreshold { get; private set; }

		public static ConfigEntry<int> MoveCoordinatePoolSize { get; private set; }

		public static ConfigEntry<int> MoveDistancePoolSize { get; private set; }

		public static ConfigEntry<bool> MaleYaw { get; private set; }

		public static ConfigEntry<BodyPart> ParentPart { get; private set; }

		public static ConfigEntry<bool> TrackingMode { get; private set; }

		public static ConfigEntry<float> StrongThresholdMultiplier { get; private set; }

		public static ConfigEntry<float> StrongMotionThreshold { get; private set; }

		public static ConfigEntry<float> WeakMotionThreshold { get; private set; }

		public static ConfigEntry<bool> DiagnosticLog { get; private set; }



		private void Awake()
		{
			Log = Logger;
			BindConfig();

			//Patch only in official VR: Koikatsu VR (KoikatuVR_Data) and Koikatsu Party VR (Koikatsu Party VR_Data).
			//VRHScene only exists in their Assembly-CSharp. In the other processes only the settings are available.
			if (Type.GetType("VRHScene, Assembly-CSharp") == null)
			{
				Log.LogInfo($"{PluginName} {Version}: not official VR, settings only");
				return;
			}

			Harmony harmony = new Harmony(GUID);
			harmony.PatchAll(typeof(Hooks));
			Log.LogInfo($"{PluginName} {Version}: VR patches applied");
		}

		private void BindConfig()
		{
			Finishcount = Bind(SectionGeneral, nameof(Finishcount), 0f, "Autofinish Timer",
				"When the girl's excitement gauge is above the cum threshold (70), she will cum after this set number of seconds \nSet this to 0 to disable the feature");
			SetParentMode = Bind(SectionGeneral, nameof(SetParentMode), ParentMode.PositionAndAnimation, "Control Mode",
				"Use the controller to control the girl's position, animation, or both");
			DisableParentInput = Bind(SectionGeneral, nameof(DisableParentInput), true, "Disable Input of Parent Controller",
				"If enabled, input for the parent controller will be turned off to prevent accidental input");
			StretchLimitArms = Bind(SectionGeneral, nameof(StretchLimitArms), 0.5f, "Distance to Detach Female Arms",
				"When stretched above this distance, the arms that are currently attached to objects will detach. This has no effect when the arms are attached manually via the floating menu button or by the controller. \nSet to 0 to disable automatic attachment of hands to objects");
			StretchLimitLegs = Bind(SectionGeneral, nameof(StretchLimitLegs), 0.7f, "Distance to Detach Female Legs",
				"When stretched above this distance, the legs that are currently attached to objects will detach. This has no effect when the legs are attached manually via the floating menu button or by the controller. \nSet to 0 to disable automatic attachment of legs to objects");
			MenuUpProximity = Bind(SectionGeneral, nameof(MenuUpProximity), 0.25f, "Distance to Display Hidden Menu",
				"When the floating menu is hidden, bring the controller close to the headset within this distance to temporarily display the menu. Unit in meters. \nSet to 0 to disable this feature");
			SetControllerCollider = Bind(SectionGeneral, nameof(SetControllerCollider), true, "Enable Holding Girl's Limbs with Controllers",
				"If enabled, touching the girl's hands or feet with a controller will cause it to stick to the controller");
			GazeControl = Bind(SectionGeneral, nameof(GazeControl), false, "Gaze Control",
				"Enables selecting any menu item by looking at it for more than 1 second");
			GropeHandsDisplay = Bind(SectionGeneral, nameof(GropeHandsDisplay), HideHandMode.Auto, "Groping Hands Display Mode",
				"When male hands are synchronized with the controllers, this controls whether additioanl hands will show on female body parts when you touch them. \nSet to auto to automatically hide them based on the proximity of your hands. to the body parts");
			MenuHideDefault = Bind(SectionGeneral, nameof(MenuHideDefault), true, "Hide Floating Menu by Default",
				"Hides floating menu by default. \nBring it up by holding the menu/B button for more than 1 second, or bring the controller close to the headset when SetParent is active");
			HideParentConAlways = Bind(SectionGeneral, nameof(HideParentConAlways), true, "Hide Parent Controller in Animation Only Mode",
				"Hides the parent controller even in animation only mode. \nDisable this to display parent controller in animation only mode while remain hidden in other modes.");
			SetMaleFeetCollider = Bind(SectionGeneral, nameof(SetMaleFeetCollider), true, "Make Male's Feet Stick to Objects",
				"If enabled, male's feet will automatically grab onto objects when in contact. \nThis is useful to prevent male's feet from clipping into the ground");
			SyncMaleHands = Bind(SectionGeneral, nameof(SyncMaleHands), true, "Synchronize Male's Hands with Controllers",
				"If enabled, the male's hands and arms are synchronized to the controllers by default when SetParentVR is enabled");
			SetParentMale = Bind(SectionGeneral, nameof(SetParentMale), true, "Synchronize Male's Head with Headset",
				"If enabled, the male body will rotate to align his head with the headset");
			CalcController = Bind(SectionGeneral, nameof(CalcController), ControllerAnimMode.SetParentController, "Which Controller Controls Animation",
				"Select which controller affects animation speed and switching between weak/strong motion");

			LimbReleaseKey = Bind(SectionKeys, nameof(LimbReleaseKey), new KeyboardShortcut(KeyCode.None), "Limb Release",
				"Press this key to release all limbs from attachment");
			MaleFeetToggle = Bind(SectionKeys, nameof(MaleFeetToggle), new KeyboardShortcut(KeyCode.None), "Male Feet Fix/Release",
				"Press this key to fix or release male's feet in place");
			SetParentToggle = Bind(SectionKeys, nameof(SetParentToggle), new KeyboardShortcut(KeyCode.None), "SetParent Toggle",
				"Press this key to enable/disable SetParentVR plugin using the left controller as parent");

			AnimMaxThreshold = Bind(SectionAdvanced, nameof(AnimMaxThreshold), 0.23f, "Animation Max Threshold",
				"When movement amount of the controller reaches this value, animation speed will be at max. Unit in meters");
			AnimStartThreshold = Bind(SectionAdvanced, nameof(AnimStartThreshold), 0.02f, "Animation Start Threshold",
				"Movement amount of the controller above this threshold will cause piston animation to start. Unit in meters");
			MoveCoordinatePoolSize = Bind(SectionAdvanced, nameof(MoveCoordinatePoolSize), 30, "Controller Average Position Pool Size (frames)",
				"Position of the controller will be calculated using the average position in this number of frames\n This value is used to calculate movement range of the controller");
			MoveDistancePoolSize = Bind(SectionAdvanced, nameof(MoveDistancePoolSize), 55, "Controller Moved Distance Pool Size (frames)",
				"Movement amount of the controller will be calculated using the sum of distance moved in this number of frames");
			MaleYaw = Bind(SectionAdvanced, nameof(MaleYaw), true, "Male Yaw Rotation",
				"Enable/disable male body's yaw (left/right) rotation when male synchronization is enabled");
			ParentPart = Bind(SectionAdvanced, nameof(ParentPart), BodyPart.Ass, "Part of Girl's Body to Parent with The Controller",
				"Use this body part to act as the center/origin that will correspond to the movement and rotation of the controller");
			TrackingMode = Bind(SectionAdvanced, nameof(TrackingMode), true, "Smooth Tracking",
				"Enables smooth following of the girl's body to the controller. \nDisable to use strict and immediate following");
			StrongThresholdMultiplier = Bind(SectionAdvanced, nameof(StrongThresholdMultiplier), 1.1f, "Strong Motion Speed Maximum Multiplier",
				"In strong motion, multiply the animation speed max threshold by this number to avoid reaching the maximum speed too easily due to the wide range of motion");
			StrongMotionThreshold = Bind(SectionAdvanced, nameof(StrongMotionThreshold), 0.04f, "Strong Motion Threshold",
				"If the movement range of the controller exceeds this threshold, animation will switch to strong motion. Unit in meters");
			WeakMotionThreshold = Bind(SectionAdvanced, nameof(WeakMotionThreshold), 0.03f, "Weak Motion Threshold",
				"If the movement range of the controller stays within this threshold for 1.5 seconds, animation will switch to weak motion. Unit in meters");
			DiagnosticLog = Bind(SectionAdvanced, nameof(DiagnosticLog), false, "Diagnostic Log",
				"Log what SetParent does (activation, control state, floating menu buttons, limbs, motion changes) to check its behavior from the log");
		}

		/// <summary>Log a diagnostic message if Diagnostic Log is enabled.</summary>
		internal static void LogDiag(string message)
		{
			if (DiagnosticLog != null && DiagnosticLog.Value)
				Log.LogInfo(message);
		}

		/// <summary>
		/// Bind a setting. The keys are the same as the BepInEx 4 version (config.ini); the display name is shown by ConfigurationManager.
		/// Values that BepInEx 4 limited to 0 or more without a slider are not given an AcceptableValueRange,
		/// because ConfigurationManager would show a slider up to float.MaxValue.
		/// </summary>
		private ConfigEntry<T> Bind<T>(string section, string key, T defaultValue, string displayName, string description)
		{
			return Config.Bind(section, key, defaultValue,
				new ConfigDescription(description, null, new ConfigurationManagerAttributes { DispName = displayName }));
		}


		public enum ParentMode
		{
			PositionOnly,
			PositionAndAnimation,
			AnimationOnly
		}

		public enum ControllerAnimMode
		{
			SetParentController,
			OtherController,
			BothControllers
		}

		public enum BodyPart
		{
			Ass,
			Torso,
			Head
		}

		public enum HideHandMode
		{
			AlwaysHide,
			Auto,
			AlwaysShow
		}
	}
}
