using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace DressUpColonist;

[HarmonyPatch(typeof(PawnRenderNode))]
[HarmonyPatch("GetTransform")]
public static class DuCEx_Patch_PawnRenderNode_GetTransform
{
	[HarmonyPostfix]
	public static void Postfix(PawnDrawParms parms, ref Vector3 offset, ref Vector3 scale, PawnRenderNode __instance)
	{
		if (__instance == null || !(__instance is PawnRenderNode_ApparelDummy))
		{
			return;
		}
		if (parms.pawn == null)
		{
			return;
		}
		PawnRenderNode_ApparelDummy apparelDef = (PawnRenderNode_ApparelDummy)(object)__instance;
		DuCEx_Comp_DressOffset offsetComp = ThingCompUtility.TryGetComp<DuCEx_Comp_DressOffset>(parms.pawn);
		List<StructOffset> offsets = offsetComp?.offsets;
		if (GenList.NullOrEmpty<StructOffset>((IList<StructOffset>)offsets))
		{
			return;
		}
		StructOffset structOffset = GenCollection.FirstOrDefault<StructOffset>(offsets, (Predicate<StructOffset>)((StructOffset d) => d.dressName == ((Def)apparelDef.apparelDef).defName));
		if (structOffset == null)
		{
			return;
		}
		if (parms.facing.IsHorizontal)
		{
			scale.x *= structOffset.scale.y;
		}
		else
		{
			scale.x *= structOffset.scale.x;
		}
		scale.z *= structOffset.scale.z;
		if (parms.facing.IsHorizontal)
		{
			if (parms.facing == Rot4.East)
			{
				offset.x += structOffset.offset.y;
			}
			else
			{
				offset.x -= structOffset.offset.y;
			}
		}
		else
		{
			offset.x += structOffset.offset.x;
		}
		offset.z += structOffset.offset.z;
	}
}
