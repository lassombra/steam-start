using DV;
using DV.Items;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DV.Items.StartingItems;

namespace SteamStart
{
	[HarmonyPatch(typeof(StartingItems))]
	internal class ItemsPatch
	{
		private static string[] excludes = new string[] { "HandheldGameConsole", "Boombox", "RemoteController", "Flashlight" };

		[HarmonyPatch(nameof(StartingItems.GetStartingItems))]
		[HarmonyPrefix]
		public static bool PatchGetStartingItems(StartingItems __instance, ref List<StartingItem> __result)
		{
			if (__instance.startingItemsType == GameParams.StartingItemsType.Basic)
			{
				__result = ExpandedStartingItems;
				return false;
			}
			return true;
		}

		private static List<StartingItem>? startingItems = null;
		private static List<StartingItem> ExpandedStartingItems
		{ get {
				if (startingItems == null)
				{
					Main.Logger?.Log("Loading starting items");
					startingItems = Globals.G.Items.GetStartingItemsAsset(GameParams.StartingItemsType.Expanded).GetStartingItems().ToList();
					Main.Logger?.Log("Starting Items Loaded: " + startingItems.Count);
					startingItems = (from i in startingItems
									where !excludes.Contains(i.ItemPrefabName)
									select i).ToList();
					foreach (var item in startingItems)
					{
						Main.Logger?.Log("Item in starting items: " + item.ItemPrefabName);
					}
				}
				return startingItems.ToList();
			}
		}
	}
}
