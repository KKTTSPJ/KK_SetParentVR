using System;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using static ItemObject;
using static SetParentKK.KK_SetParentVR;

namespace SetParentKK
{
	public static class Hooks
	{
		//This should hook to a method that loads as late as possible in the loading phase
		//Hooking method "MapSameObjectDisable" because: "Something that happens at the end of H scene loading, good enough place to hook" - DeathWeasel1337/Anon11
		//https://github.com/DeathWeasel1337/KK_Plugins/blob/master/KK_EyeShaking/KK.EyeShaking.Hooks.cs#L20
		[HarmonyPostfix]
		[HarmonyPatch(typeof(VRHScene), "MapSameObjectDisable")]
		//__instance is VRHScene. Parameters are typed object so that this type still loads in normal mode, where VRHScene does not exist
		public static void VRHSceneLoadPostfix(object __instance)
		{
			VRHScene vrHScene = (VRHScene)__instance;
			if (setParentObj != null)
				UnityEngine.Object.Destroy(setParentObj);

			setParentObj = vrHScene.gameObject.AddComponent<SetParent>();
			setParentObj.Init(vrHScene.sprites[0], (List<MotionIK>)Traverse.Create(vrHScene).Field("lstMotionIK").GetValue());
			VRAssistCompat.Init();
		}

		/// <summary>
		/// Runs before KoikatuVRAssistPlugin's postfix of VRHScene.Update in the same frame (see VRAssistCompat)
		/// </summary>
		[HarmonyPrefix]
		[HarmonyPatch(typeof(VRHScene), "Update")]
		public static void VRHSceneUpdatePrefix(object __instance)
		{
			VRAssistCompat.BeforeAssist(__instance);
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(VRHScene), "SetLocalPosition")]
		public static void SetLocalPositionPrefix()
		{
			if (setParentObj != null)
			{
				foreach (SetParent.Limb limb in setParentObj.limbs)
				{
					if (limb.AnchorObj)
						setParentObj.FixLimbToggle(limb);
				}
				setParentObj.txtMaleLeftFoot.text = "男の左足固定";
				setParentObj.txtMaleRightFoot.text = "男の右足固定";

				setParentObj.UnsetP();

				setParentObj.male_p_cf_bodybone.transform.localPosition = Vector3.zero;
				setParentObj.male_p_cf_bodybone.transform.localRotation = Quaternion.identity;
				setParentObj.female_p_cf_bodybone.transform.localPosition = Vector3.zero;
				setParentObj.female_p_cf_bodybone.transform.localRotation = Quaternion.identity;
			}			
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(ItemObject), "LoadItem")]
		public static void LoadItemPostfix(ItemObject __instance)
		{
			if (setParentObj != null)
			{
				Dictionary<int, Item> dictItem = (Dictionary<int, Item>)Traverse.Create(__instance).Field("dicItem").GetValue();
				foreach (Item item in dictItem.Values)
				{
					setParentObj.SetObjectColliders(item.objItem.transform);
				}
			}
		}

		/// <summary>
		/// Disable controller input when controller is being used as parent
		/// </summary>
		[HarmonyPrefix]
		[HarmonyPatch(typeof(VRViveController), "IsPressDown")]
		public static bool IsPressDownPre(Component __instance, ref bool __result)
		{
			if (!DisableParentInput.Value)
				return true;
			
			if ((setParentObj?.setFlag ?? false) && setParentObj.parentController == __instance.gameObject)
			{
				__result = false;
				return false;
			}
			else
				return true;
		}

		/// <summary>
		/// Disable controller input when controller is being used as parent
		/// </summary>
		[HarmonyPrefix]
		[HarmonyPatch(typeof(VRViveController), "IsPressUp")]
		public static bool IsPressUpPre(Component __instance, ref bool __result)
		{
			if (!DisableParentInput.Value)
				return true;

			if ((setParentObj?.setFlag ?? false) && setParentObj.parentController == __instance.gameObject)
			{
				__result = false;
				return false;
			}
			else
				return true;
		}
	}
}
