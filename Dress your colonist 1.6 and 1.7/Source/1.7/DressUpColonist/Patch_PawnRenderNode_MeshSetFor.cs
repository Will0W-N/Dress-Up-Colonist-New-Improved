using HarmonyLib;
using Verse;

namespace DressUpColonist;

[HarmonyPatch(typeof(PawnRenderNode))]
[HarmonyPatch("MeshSetFor")]
internal class Patch_PawnRenderNode_MeshSetFor
{
	public static bool Prefix(Pawn pawn, ref GraphicMeshSet __result)
	{
		DuC_Comp_DressUpCore duC_Comp_DressUpCore = ThingCompUtility.TryGetComp<DuC_Comp_DressUpCore>((Thing)(object)pawn);
		if (duC_Comp_DressUpCore != null && duC_Comp_DressUpCore.animalEnable)
		{
			__result = MeshPool.GetMeshSetForSize(duC_Comp_DressUpCore.animalSize);
			return false;
		}
		return true;
	}
}
