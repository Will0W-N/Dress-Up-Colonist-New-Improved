using HarmonyLib;
using Verse;

namespace DressUpColonist;

[HarmonyPatch(typeof(PawnRenderNode_Head))]
[HarmonyPatch("GraphicFor")]
internal class Patch_PawnRenderNodeHead_GraphicFor
{
	public static bool Prefix(Pawn pawn, ref Graphic __result, PawnRenderNode_Head __instance)
	{
		DuC_Comp_DressUpCore duC_Comp_DressUpCore = ThingCompUtility.TryGetComp<DuC_Comp_DressUpCore>((Thing)(object)pawn);
		if (duC_Comp_DressUpCore != null && duC_Comp_DressUpCore.animalEnable)
		{
			return false;
		}
		if (duC_Comp_DressUpCore != null && duC_Comp_DressUpCore.bodyChangeEnable && duC_Comp_DressUpCore.bodyTabEnableList[1])
		{
			if (duC_Comp_DressUpCore.headInvisible)
			{
				return false;
			}
			if (duC_Comp_DressUpCore.headTypeName != "")
			{
				if (!pawn.health.hediffSet.HasHead)
				{
					return true;
				}
				if ((int)pawn.Drawer.renderer.CurRotDrawMode == 2)
				{
					return true;
				}
				HeadTypeDef named = DefDatabase<HeadTypeDef>.GetNamed(duC_Comp_DressUpCore.headTypeName, true);
				if (duC_Comp_DressUpCore.bodyTabEnableList[0])
				{
					_ = duC_Comp_DressUpCore.bodyColor;
					if (true)
					{
						__result = (Graphic)(object)named.GetGraphic(pawn, duC_Comp_DressUpCore.bodyColor);
						goto IL_00f8;
					}
				}
				__result = (Graphic)(object)named.GetGraphic(pawn, ((PawnRenderNode)__instance).ColorFor(pawn));
				goto IL_00f8;
			}
		}
		return true;
		IL_00f8:
		return false;
	}
}
