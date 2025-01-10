using DV;
using DV.Items;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static DV.Items.StartingItems;

namespace SteamStart
{
	[HarmonyPatch]
	internal class ItemsPatch
	{
		private static readonly string[] includes = new string[] { "Lantern", "EOTLantern", "Oiler", "lighter", "shovel" };
		private static List<StartingItem> items = new List<StartingItem>();

		[HarmonyPatch(typeof(StartingItemsController), nameof(StartingItemsController.AddStartingItems))]
		[HarmonyPrefix]
		public static void PatchGetStartingItems()
		{
			Main.Logger?.Log($"Patch get starting items");
			items.Clear();
			var basic = (from i in DV.Globals.G.Items.startingItems
						 where i.startingItemsType == GameParams.StartingItemsType.Basic
						 select i).First();
			var expanded = (from i in DV.Globals.G.Items.startingItems
							where i.startingItemsType == GameParams.StartingItemsType.Expanded
							select i).First();
			Main.Logger?.Log($"{basic.items.Count} - {expanded.items.Count}");
			foreach (var i in expanded.items) {
				if (includes.Contains(i.ItemPrefabName) && !basic.items.Contains(i))
				{
					items.Add(i);
				}
			};
			basic.items.AddRange(items);
			expanded.items.RemoveAll(i => !items.Contains(i));
		}
	}
}
