using HarmonyLib;
using RimWorld;
using Verse;

namespace DressUpColonist;

[HarmonyPatch(typeof(PawnRenderer))]
[HarmonyPatch("GetBodyPos")]
internal class Patch_PawnRenderer_GetBodyPos
{
	private static void Postfix(ref bool showBody, Pawn ___pawn)
	{
		if (___pawn != null && ___pawn.RaceProps.Humanlike && ((((Thing)___pawn).Faction != null && ((Thing)___pawn).Faction == Faction.OfPlayer) || ___pawn.IsPrisoner))
		{
			DuC_Comp_DressUpCore duC_Comp_DressUpCore = ThingCompUtility.TryGetComp<DuC_Comp_DressUpCore>((Thing)(object)___pawn);
			if (duC_Comp_DressUpCore != null && duC_Comp_DressUpCore.animalEnable && !showBody && RestUtility.CurrentBed(___pawn) != null)
			{
				showBody = true;
			}
		}
	}
}
