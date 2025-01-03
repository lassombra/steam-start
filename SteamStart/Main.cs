using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DV;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using HarmonyLib;
using UnityModManagerNet;

namespace SteamStart;

public static class Main
{
	public static UnityModManager.ModEntry.ModLogger? Logger { get; private set; }
	private static List<GeneralLicenseType_v2> previousLIcenses = new List<GeneralLicenseType_v2>();
	private static String[] itemPrefabs = new String[] { "lighter", "shovel", "EOTLantern" };

	// Unity Mod Manage Wiki: https://wiki.nexusmods.com/index.php/Category:Unity_Mod_Manager
	static bool Load(UnityModManager.ModEntry modEntry)
	{
		modEntry.OnToggle = OnToggle;
		return true;
	}
	private static void PatchStartupLicensesAndItems()
	{
		previousLIcenses = LicenseManager.TutorialGeneralLicenses;
		LicenseManager.TutorialGeneralLicenses.RemoveAll(license => license.v1 == GeneralLicenseType.DE2);
		LicenseManager.TutorialGeneralLicenses.Add(GeneralLicenseType.S060.ToV2());
		GeneralLicenseType.DE2.ToV2().price = 20000f;
	}
	private static void UnpatchStartupLicensesAndItems()
	{
		var licenseManagerType = typeof(LicenseManager);
		var licenseGeneralLicensesField = licenseManagerType.GetField("TutorialGeneralLicenses");
		licenseGeneralLicensesField.SetValue(null, previousLIcenses);
	}
	static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
	{
		if (value)
		{
			Logger = modEntry.Logger;
			PatchStartupLicensesAndItems();
			var harmony = new Harmony(modEntry.Info.Id);
			harmony.PatchAll(Assembly.GetExecutingAssembly());
			}
		else
		{
			UnpatchStartupLicensesAndItems();
		}
		return true;
	}
}
