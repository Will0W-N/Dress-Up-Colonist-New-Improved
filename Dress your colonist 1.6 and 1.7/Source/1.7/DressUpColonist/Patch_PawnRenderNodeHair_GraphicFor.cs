using HarmonyLib;
using RimWorld;
using Verse;

namespace DressUpColonist;

[HarmonyPatch(typeof(PawnRenderNode_Hair))]
[HarmonyPatch("GraphicFor")]
internal class Patch_PawnRenderNodeHair_GraphicFor
{
	public static bool Prefix(Pawn pawn, ref Graphic __result, PawnRenderNode_Hair __instance)
	{
		DuC_Comp_DressUpCore duC_Comp_DressUpCore = ThingCompUtility.TryGetComp<DuC_Comp_DressUpCore>((Thing)(object)pawn);
		if (duC_Comp_DressUpCore != null && duC_Comp_DressUpCore.animalEnable)
		{
			return false;
		}
		if (duC_Comp_DressUpCore != null && duC_Comp_DressUpCore.bodyChangeEnable && duC_Comp_DressUpCore.bodyTabEnableList[2] && duC_Comp_DressUpCore.hairName != "")
		{
			HairDef named = DefDatabase<HairDef>.GetNamed(duC_Comp_DressUpCore.hairName, true);
			__result = ((StyleItemDef)named).GraphicFor(pawn, duC_Comp_DressUpCore.hairColor);
			return false;
		}
		return true;
	}
}
