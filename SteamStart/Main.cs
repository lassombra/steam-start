using System;
using System.Collections.Generic;
using System.Reflection;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using HarmonyLib;
using UnityModManagerNet;

namespace SteamStart;

public static class Main
{
	// Unity Mod Manage Wiki: https://wiki.nexusmods.com/index.php/Category:Unity_Mod_Manager
	private static bool Load(UnityModManager.ModEntry modEntry)
	{
		Harmony? harmony = null;

		try
		{
			harmony = new Harmony(modEntry.Info.Id);
			harmony.PatchAll(Assembly.GetExecutingAssembly());

			PatchLicenseList();
		}
		catch (Exception ex)
		{
			modEntry.Logger.LogException($"Failed to load {modEntry.Info.DisplayName}:", ex);
			harmony?.UnpatchAll(modEntry.Info.Id);
			return false;
		}

		return true;
	}
	private static void PatchLicenseList()
	{
		var licenseManagerType = typeof(LicenseManager);
		var licenseGeneralLicensesField = licenseManagerType.GetField("TutorialGeneralLicenses");
		licenseGeneralLicensesField.SetValue(null, new List<GeneralLicenseType_v2>
			{
				GeneralLicenseType.TrainDriver.ToV2(),
				GeneralLicenseType.S060.ToV2()
			});
		GeneralLicenseType.DE2.ToV2().price = 20000f;
	}
}
