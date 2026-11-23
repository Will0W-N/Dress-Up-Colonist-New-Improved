using HarmonyLib;
using RimWorld;
using Verse;

namespace DressUpColonist;

[HarmonyPatch(typeof(PawnRenderNode_Beard))]
[HarmonyPatch("GraphicFor")]
internal class Patch_PawnRenderNodeBeard_GraphicFor
{
	public static bool Prefix(Pawn pawn, ref Graphic __result, PawnRenderNode_Beard __instance)
	{
		DuC_Comp_DressUpCore duC_Comp_DressUpCore = ThingCompUtility.TryGetComp<DuC_Comp_DressUpCore>((Thing)(object)pawn);
		if (duC_Comp_DressUpCore != null && duC_Comp_DressUpCore.bodyChangeEnable && duC_Comp_DressUpCore.bodyTabEnableList[3] && duC_Comp_DressUpCore.beardName != "")
		{
			BeardDef named = DefDatabase<BeardDef>.GetNamed(duC_Comp_DressUpCore.beardName, true);
			__result = ((StyleItemDef)named).GraphicFor(pawn, duC_Comp_DressUpCore.beardColor);
			return false;
		}
		return true;
	}
}
