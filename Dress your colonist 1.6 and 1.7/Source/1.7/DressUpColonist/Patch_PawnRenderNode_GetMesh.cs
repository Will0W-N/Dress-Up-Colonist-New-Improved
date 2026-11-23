using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace DressUpColonist;

[HarmonyPatch(typeof(PawnRenderNode))]
[HarmonyPatch("GetMesh")]
internal class Patch_PawnRenderNode_GetMesh
{
	public static void Postfix(PawnRenderNode __instance, ref Mesh __result, PawnDrawParms parms)
	{
		if (parms.facing != Rot4.West || __instance is PawnRenderNode_Spastic || !__instance.HasGraphic(parms.pawn) || __instance.PrimaryGraphic.WestFlipped || ((object)__instance).GetType().Namespace.Equals("Varse"))
		{
			return;
		}
		DuC_Comp_DressUpCore duC_Comp_DressUpCore = ThingCompUtility.TryGetComp<DuC_Comp_DressUpCore>((Thing)(object)parms.pawn);
		if (duC_Comp_DressUpCore != null && duC_Comp_DressUpCore.animalEnable && duC_Comp_DressUpCore.reverseWest)
		{
			bool flag = false;
			MethodInfo methodInfo = AccessTools.PropertyGetter(typeof(PawnRenderNode), "FlipGraphic");
			if ((bool)methodInfo.Invoke(__instance, null))
			{
				flag = !flag;
			}
			Hediff hediff = __instance.hediff;
			if (((hediff == null) ? ((bool?)null) : hediff.Part?.flipGraphic) == true)
			{
				flag = !flag;
			}
			if (__instance.Props.drawData != null && __instance.Props.drawData.FlipForRot(parms.facing))
			{
				flag = !flag;
			}
			if (flag)
			{
				__result = MeshPool.GridPlaneFlip(__result);
			}
			else
			{
				__result = MeshPool.GridPlane(MeshPool.GetMetaData(__result).size);
			}
		}
	}
}
