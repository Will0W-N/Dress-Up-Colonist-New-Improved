using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace DressUpColonist;

[HarmonyPatch(typeof(PawnRenderNode_Body))]
[HarmonyPatch("GraphicFor")]
internal class Patch_PawnRenderNodeBody_GraphicFor
{
	public static bool Prefix(Pawn pawn, ref Graphic __result, PawnRenderNode_Body __instance)
	{
		DuC_Comp_DressUpCore duC_Comp_DressUpCore = ThingCompUtility.TryGetComp<DuC_Comp_DressUpCore>((Thing)(object)pawn);
		if (duC_Comp_DressUpCore != null && duC_Comp_DressUpCore.bodyChangeEnable && duC_Comp_DressUpCore.bodyTabEnableList[0])
		{
			if (duC_Comp_DressUpCore.bodyInvisible)
			{
				return false;
			}
			if (duC_Comp_DressUpCore.bodyTypeName != "")
			{
				Shader val = ((PawnRenderNode)__instance).ShaderFor(pawn);
				if ((UnityEngine.Object)(object)val == (UnityEngine.Object)null)
				{
					return true;
				}
				Graphic val2 = GraphicDatabase.Get<Graphic_Multi>(DefDatabase<BodyTypeDef>.GetNamed(duC_Comp_DressUpCore.bodyTypeName, true).bodyNakedGraphicPath, val, Vector2.one, duC_Comp_DressUpCore.bodyColor);
				if (val2 != null)
				{
					__result = val2;
				}
				return false;
			}
		}
		else if (duC_Comp_DressUpCore != null && duC_Comp_DressUpCore.animalEnable)
		{
			PawnKindDef val3 = PawnKindDef.Named(duC_Comp_DressUpCore.animalKindDefName);
			if (val3 != null)
			{
				Graphic graphic = val3.lifeStages[duC_Comp_DressUpCore.animalStage].bodyGraphicData.Graphic;
				graphic.drawSize = duC_Comp_DressUpCore.animalSize;
				graphic.color = duC_Comp_DressUpCore.animalColor;
				__result = graphic.GetColoredVersion(ShaderDatabase.Cutout, graphic.Color, graphic.ColorTwo);
				return false;
			}
		}
		return true;
	}
}
