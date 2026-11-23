using HarmonyLib;
using UnityEngine;
using Verse;

namespace DressUpColonist;

[HarmonyPatch(typeof(PawnRenderer))]
[HarmonyPatch("BaseHeadOffsetAt")]
internal class Patch_PawnRenderer_BaseHeadOffsetAt
{
	public static void Postfix(ref Vector3 __result, Pawn ___pawn)
	{
		DuC_Comp_DressUpCore duC_Comp_DressUpCore = ThingCompUtility.TryGetComp<DuC_Comp_DressUpCore>((Thing)(object)___pawn);
		if (duC_Comp_DressUpCore != null && duC_Comp_DressUpCore.bodyChangeEnable)
		{
			if (((Thing)___pawn).Rotation == Rot4.South)
			{
				__result.x += duC_Comp_DressUpCore.headOffset.x;
				__result.z += duC_Comp_DressUpCore.headOffset.z;
			}
			else if (((Thing)___pawn).Rotation == Rot4.North)
			{
				__result.x -= duC_Comp_DressUpCore.headOffset.x;
				__result.z += duC_Comp_DressUpCore.headOffset.z;
			}
			else if (((Thing)___pawn).Rotation == Rot4.East)
			{
				__result.z += duC_Comp_DressUpCore.headOffset.x + duC_Comp_DressUpCore.headOffset.z;
			}
			else
			{
				__result.z -= duC_Comp_DressUpCore.headOffset.x + duC_Comp_DressUpCore.headOffset.z;
			}
		}
	}
}
