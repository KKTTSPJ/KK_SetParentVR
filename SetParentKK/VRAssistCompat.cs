using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static SetParentKK.KK_SetParentVR;

namespace SetParentKK
{
	/// <summary>
	/// Workarounds for controller input conflicts with KoikatuVRAssistPlugin.
	/// The original KoikatuVRAssistPluginHooks (removed in 6872cb3 because MayouKurayami/KoikatuVRAssistPlugin 1290f0c checks for other
	/// inputs by itself) patched KoikatuVRAssistPlugin's methods. Other editions do not have that check (e.g. KoikatuVRAssistPlugin改 1.2.0.9,
	/// based on 1.1.0), so the conflicts are handled here again, but without patching KoikatuVRAssistPlugin: detouring its methods crashed
	/// Koikatsu VR inside Mono (Harmony.Patch → MonoMod GetFunctionPointer → mono AV), while Koikatsu Party VR was fine.
	/// KoikatuVRAssistPlugin does all of this from its postfix of VRHScene.Update, so a prefix of VRHScene.Update (BeforeAssist) runs first
	/// in the same frame and only writes its fields:
	/// - GripMoveAssistObj.gripDownTime: the grip long-press timer of the game menu (show / drag)
	/// - GripMoveAssistObj.speedRate (static): how fast touching the stick changes the speed gauge
	/// - TouchEvent.IntervalTime (static): touch events are only checked once it exceeds 0.5 seconds
	/// KoikatuVRAssistPlugin is not referenced: its types are looked up by name at the first H scene (this plugin can load before it).
	/// With an edition that checks for other inputs itself, these have no effect beyond what it already does.
	/// </summary>
	internal static class VRAssistCompat
	{
		private const string AssistAssemblyName = "KoikatuVRAssistPlugin";
		private const string AssistTypeName = "KoikatuVRAssistPlugin.GripMoveAssistObj";
		private const string TouchEventTypeName = "KoikatuVRAssistPlugin.TouchEvent";

		//SteamVR button masks
		private const ulong ButtonMenu = 1ul << 1;
		private const ulong ButtonGrip = 1ul << 2;
		private const ulong ButtonTrigger = 1ul << 33;

		private static bool initialized;
		private static Type assistType;
		private static FieldInfo f_gripDownTime;
		private static FieldInfo f_speedRate;
		private static FieldInfo f_intervalTime;

		private static bool menuBlocked;
		private static bool speedBlocked;
		private static float savedSpeedRate;
		private static string touchBlockReason;

		/// <summary>Look up KoikatuVRAssistPlugin's types and fields once, at the first H scene.</summary>
		internal static void Init()
		{
			if (initialized)
				return;
			initialized = true;

			try
			{
				assistType = FindAssistType(AssistTypeName);
				if (assistType == null)
				{
					LogDiag("KoikatuVRAssistPlugin not found: no compatibility workarounds");
					return;
				}
				f_gripDownTime = assistType.GetField("gripDownTime", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (f_gripDownTime != null && f_gripDownTime.FieldType != typeof(float[]))
					f_gripDownTime = null;
				f_speedRate = assistType.GetField("speedRate", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				if (f_speedRate != null && f_speedRate.FieldType != typeof(float))
					f_speedRate = null;
				Type touchEvent = FindAssistType(TouchEventTypeName);
				f_intervalTime = touchEvent?.GetField("IntervalTime", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				if (f_intervalTime != null && f_intervalTime.FieldType != typeof(float))
					f_intervalTime = null;
			}
			catch (Exception e)
			{
				Log.LogWarning($"KoikatuVRAssistPlugin compatibility: lookup failed: {e}");
			}
			Log.LogInfo($"KoikatuVRAssistPlugin compatibility: grip long press on the game menu {(f_gripDownTime != null ? "found" : "not found")}, " +
				$"speed by stick touch {(f_speedRate != null ? "found" : "not found")}, touch events {(f_intervalTime != null ? "found" : "not found")}");
		}

		private static Type FindAssistType(string typeName)
		{
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (assembly.GetName().Name == AssistAssemblyName)
				{
					Type type = assembly.GetType(typeName, false);
					if (type != null)
						return type;
				}
			}
			return null;
		}

		/// <summary>
		/// Called before VRHScene.Update, so before KoikatuVRAssistPlugin's postfix of it in the same frame.
		/// </summary>
		/// <param name="vrHScene">VRHScene (typed object so that this type still loads in normal mode)</param>
		internal static void BeforeAssist(object vrHScene)
		{
			if (assistType == null)
				return;
			try
			{
				SetParent sp = setParentObj;
				bool active = sp != null && sp.setFlag;
				SuppressGameMenu(sp, active && sp.currentCtrlstate >= SetParent.CtrlState.Following, vrHScene);
				SuppressSpeedByTouch(active && (sp.currentCtrlstate >= SetParent.CtrlState.MaleControl || SetParentMode.Value >= ParentMode.PositionAndAnimation));
				HoldOffTouchEvents(active ? TouchBlockReason(sp) : null);
			}
			catch (Exception e)
			{
				//Do not retry every frame
				assistType = null;
				Log.LogError($"KoikatuVRAssistPlugin compatibility stopped: {e}");
			}
		}

		/// <summary>
		/// While SetParent uses the grip of the non-parent controller (Following, MaleControl, FemaleControl), keep restarting
		/// KoikatuVRAssistPlugin's grip long-press timer of that controller, so that it does not show or drag the game menu.
		/// </summary>
		private static void SuppressGameMenu(SetParent sp, bool block, object vrHScene)
		{
			if (block != menuBlocked)
			{
				menuBlocked = block;
				LogDiag($"KoikatuVRAssistPlugin: game menu by grip long press {(block ? "suppressed" : "allowed again")}");
			}
			if (!block || f_gripDownTime == null)
				return;

			VRHScene scene = vrHScene as VRHScene;
			Component assist = scene != null ? scene.GetComponent(assistType) : null;
			float[] gripDownTime = assist != null ? f_gripDownTime.GetValue(assist) as float[] : null;
			if (gripDownTime == null || scene.managerVR == null || scene.managerVR.scrControllerManager == null)
				return;

			//GripMoveAssistObj indexes gripDownTime by VRViveControllerManager.lstController, so find the non-parent controller there
			GameObject nonParent = sp.ParentSideController(oppositeSide: true);
			List<VRViveController> controllers = scene.managerVR.scrControllerManager.lstController;
			for (int i = 0; i < controllers.Count && i < gripDownTime.Length; i++)
			{
				if (controllers[i] != null && controllers[i].gameObject == nonParent)
					gripDownTime[i] = Time.time;
			}
		}

		/// <summary>
		/// While SetParent uses the stick (MaleControl, FemaleControl) or controls the animation speed (PositionAndAnimation, AnimationOnly),
		/// set KoikatuVRAssistPlugin's speed rate of touching the stick to 0, and put it back afterwards.
		/// </summary>
		private static void SuppressSpeedByTouch(bool block)
		{
			if (f_speedRate == null || block == speedBlocked)
				return;
			speedBlocked = block;
			if (block)
			{
				savedSpeedRate = (float)f_speedRate.GetValue(null);
				f_speedRate.SetValue(null, 0f);
			}
			else
				f_speedRate.SetValue(null, savedSpeedRate);
			LogDiag($"KoikatuVRAssistPlugin: speed by stick touch {(block ? "suppressed" : "allowed again")}");
		}

		/// <summary>
		/// The buttons SetParent uses that KoikatuVRAssistPlugin's touch events also react to: the grip and the trigger of the non-parent
		/// controller (character controls, holding still, limbs), and any button of the parent controller when Disable Input of Parent
		/// Controller is on. The menu button of the non-parent controller is left (SetParent only uses it with the trigger, or held for 1 second).
		/// </summary>
		/// <returns>The reason to hold off the touch events, or null</returns>
		private static string TouchBlockReason(SetParent sp)
		{
			SetParent.Side parent = sp.ParentSideEnum();
			SetParent.Side nonParent = sp.ParentSideEnum(oppositeSide: true);
			if (sp.RawPress(nonParent, ButtonGrip) || sp.RawPress(nonParent, ButtonTrigger))
				return $"{nonParent} controller (non-parent) grip/trigger";
			if (DisableParentInput.Value && (sp.RawPress(parent, ButtonGrip) || sp.RawPress(parent, ButtonTrigger) || sp.RawPress(parent, ButtonMenu)))
				return $"{parent} controller (parent)";
			return null;
		}

		/// <summary>
		/// Hold off KoikatuVRAssistPlugin's touch events (clothes, coordinates, insert, finish, ...) while those buttons are held:
		/// it only checks for touch events once TouchEvent.IntervalTime exceeds 0.5 seconds, so keep it at 0.
		/// The touch events come back 0.5 seconds after the buttons are released.
		/// </summary>
		private static void HoldOffTouchEvents(string reason)
		{
			if (f_intervalTime == null)
				return;
			if (reason != touchBlockReason)
			{
				if (reason != null)
					LogDiag($"KoikatuVRAssistPlugin: touch events held off ({reason})");
				else
					LogDiag("KoikatuVRAssistPlugin: touch events allowed again (after 0.5 s)");
				touchBlockReason = reason;
			}
			if (reason != null)
				f_intervalTime.SetValue(null, 0f);
		}
	}
}
