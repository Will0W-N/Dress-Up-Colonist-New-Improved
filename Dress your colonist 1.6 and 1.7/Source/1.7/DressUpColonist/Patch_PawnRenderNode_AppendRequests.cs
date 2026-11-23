using HarmonyLib;
using Verse;

namespace DressUpColonist;

[HarmonyPatch(typeof(PawnRenderNode), nameof(PawnRenderNode.AppendRequests))]
internal static class Patch_PawnRenderNode_AppendRequests
{
	private static bool Prefix(PawnRenderNode __instance, PawnDrawParms parms)
	{
		if (__instance?.apparel == null || __instance is PawnRenderNode_ApparelDummy || parms.pawn == null)
		{
			return true;
		}

		DuC_Comp_DressUpCore core = ThingCompUtility.TryGetComp<DuC_Comp_DressUpCore>(parms.pawn);
		return core?.animalEnable != true;
	}
}
