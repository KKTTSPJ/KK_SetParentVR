using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;


namespace SetParentKK
{
	public partial class SetParent
	{
		private Dictionary<Side, GameObject> controllers = new Dictionary<Side, GameObject>();

		private FieldInfo f_device;

		//VRViveController and SteamVR_Controller.Device only exist in the VR assembly. They are kept as Component / object so that
		//this type still loads in normal mode (another plugin listing every type would otherwise get a ReflectionTypeLoadException).
		private Dictionary<Side, Component> viveControllers = new Dictionary<Side, Component>();

		private Dictionary<Side, object> steamVRDevices = new Dictionary<Side, object>();


		private bool TrackPadPressing(Side side)
		{
			return ((viveControllers[side] as VRViveController)?.IsState(VRViveController.EViveButtonKind.Touchpad, -1) ?? false) || ((steamVRDevices[side] as SteamVR_Controller.Device)?.GetPress((ulong)1 << 32) ?? false);
		}

		private bool TrackPadUp(Side side)
		{
			return ((viveControllers[side] as VRViveController)?.IsState(VRViveController.EViveButtonKind.Touchpad_Up, -1) ?? false) || (steamVRDevices[side] as SteamVR_Controller.Device)?.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0).y > 0.7f;
		}

		private bool TrackPadDown(Side side)
		{
			return ((viveControllers[side] as VRViveController)?.IsState(VRViveController.EViveButtonKind.Touchpad_Down, -1) ?? false) || (steamVRDevices[side] as SteamVR_Controller.Device)?.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0).y < -0.7f;
		}

		private bool MenuPressing(Side side)
		{
			return ((viveControllers[side] as VRViveController)?.IsState(VRViveController.EViveButtonKind.Menu, -1) ?? false) || ((steamVRDevices[side] as SteamVR_Controller.Device)?.GetPress((ulong)1 << 1) ?? false);
		}

		private bool MenuPressDown(Side side)
		{
			return ((viveControllers[side] as VRViveController)?.IsPressDown(VRViveController.EViveButtonKind.Menu, -1) ?? false) || ((steamVRDevices[side] as SteamVR_Controller.Device)?.GetPressDown((ulong)1 << 1) ?? false);
		}

		private bool TriggerPressDown(Side side)
		{
			return ((viveControllers[side] as VRViveController)?.IsPressDown(VRViveController.EViveButtonKind.Trigger, -1) ?? false) || ((steamVRDevices[side] as SteamVR_Controller.Device)?.GetPressDown((ulong)1 << 33) ?? false);
		}

		private bool TriggerRelease(Side side)
		{
			return ((viveControllers[side] as VRViveController)?.IsPressUp(VRViveController.EViveButtonKind.Trigger, -1) ?? false) || ((steamVRDevices[side] as SteamVR_Controller.Device)?.GetPressUp((ulong)1 << 33) ?? false);
		}

		private bool TriggerPressing(Side side)
		{
			return ((viveControllers[side] as VRViveController)?.IsState(VRViveController.EViveButtonKind.Trigger, -1) ?? false) || ((steamVRDevices[side] as SteamVR_Controller.Device)?.GetPress((ulong)1 << 33) ?? false);
		}

		private bool GripPressing(Side side)
		{
			return ((viveControllers[side] as VRViveController)?.IsState(VRViveController.EViveButtonKind.Grip, -1) ?? false) || ((steamVRDevices[side] as SteamVR_Controller.Device)?.GetPress((ulong)1 << 3) ?? false);
		}

		private bool GripPressDown(Side side)
		{
			return ((viveControllers[side] as VRViveController)?.IsPressDown(VRViveController.EViveButtonKind.Grip, -1) ?? false) || ((steamVRDevices[side] as SteamVR_Controller.Device)?.GetPressDown((ulong)1 << 3) ?? false);
		}

		/// <summary>Raw SteamVR state of a button (bit mask) on a controller, as other plugins read it (not affected by DisableParentInput).</summary>
		internal bool RawPress(Side side, ulong mask)
		{
			return steamVRDevices.TryGetValue(side, out object device) && device is SteamVR_Controller.Device d && d.GetPress(mask);
		}

		private bool IsDoubleClick(TriggerState input, float threshold)
		{
			if (Time.time - lastTriggerRelease[(int)input] > threshold)
			{
				lastTriggerRelease[(int)input] = Time.time;
				return false;
			}
			else
			{
				lastTriggerRelease[(int)input] = Time.time;
				return true;
			}
		}

		private enum TriggerState
		{
			Left,
			Right,
			LeftWhileGripped,
			RightWhileGripped
		}
	}
}
