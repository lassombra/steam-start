using DV;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SteamStart
{
	[HarmonyPatch(typeof(CarSpawner), "SpawnCarTypesOnTrack")]
	[HarmonyPriority(Priority.LowerThanNormal)]
	internal class SpawnPatch
	{
		private static List<TrainCarLivery> S060liveries = new List<TrainCarLivery>();


		static void Prefix(ref List<TrainCarLivery> trainCarTypes, List<bool> carsOrientationReversed,
			RailTrack railTrack, bool preventAutoCoupleOnLastCars, bool applyHandbrakeOnLastCars, double startSpan = 0.0, bool flipTrainConsist = false, bool playerSpawnedCars = false)
		{
			var hasLicense = LicenseManager.Instance.IsLicensedForCar(TrainCarType.LocoShunter.ToV2());
			if (hasLicense)
			{
				return;
			}
			var hasDE2 = trainCarTypes.Any(livery => livery.v1 == TrainCarType.LocoShunter);
			if (hasDE2)
			{
				Main.Logger?.Log("DE2 spawn was requested - intercepting");
				if (S060liveries.Count == 0)
				{
					LoadLiveries();
				}
				trainCarTypes = (from t in trainCarTypes
								select t.v1 == TrainCarType.LocoShunter ? S060liveries.GetRandomElement() : t).ToList();
			}
		}
		private static void LoadLiveries()
		{
			foreach (var livery in Globals.G.Types.Liveries)
			{
				if (livery.v1 == TrainCarType.LocoS060)
				{
					S060liveries.Add(livery);
				}
			}
			Main.Logger?.Log("Added 060 liveries " + S060liveries.Count);
		}
	}
}
