using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace DressUpColonist;

[HarmonyPatch(typeof(PawnRenderTree), "SetupDynamicNodes")]
internal static class Patch_PawnRenderTree_SetupDynamicNodes
{
	private static void Postfix(PawnRenderTree __instance)
	{
		Pawn pawn = __instance.pawn;
		if (pawn == null || !pawn.RaceProps.Humanlike ||
			(pawn.Faction != Faction.OfPlayer && !pawn.IsPrisoner))
		{
			return;
		}

		DuC_Comp_DressUpCore core = ThingCompUtility.TryGetComp<DuC_Comp_DressUpCore>(pawn);
		if (core == null || !core.dressUpEnable || core.dresses.NullOrEmpty())
		{
			return;
		}

		DuCEx_Comp_DressOffset offsetComp = ThingCompUtility.TryGetComp<DuCEx_Comp_DressOffset>(pawn);
		Dictionary<PawnRenderNode, float> layerOffsets = new Dictionary<PawnRenderNode, float>();

		foreach (StructDress dress in core.dresses)
		{
			ThingDef apparelDef = DefDatabase<ThingDef>.GetNamedSilentFail(dress.dressName);
			if (apparelDef?.apparel == null)
			{
				continue;
			}

			Apparel apparel = GetDummyApparel(core, apparelDef);
			if (apparel == null)
			{
				continue;
			}

			PawnRenderNode headNode = null;
			PawnRenderNode bodyNode = null;
			__instance.TryGetNodeByTag(PawnRenderNodeTagDefOf.ApparelHead, out headNode);
			__instance.TryGetNodeByTag(PawnRenderNodeTagDefOf.ApparelBody, out bodyNode);

			PawnRenderNode parent = null;
			if (apparelDef.apparel.parentTagDef != null)
			{
				__instance.TryGetNodeByTag(apparelDef.apparel.parentTagDef, out parent);
			}

			bool useHeadNode = parent != null
				? parent == headNode
				: apparelDef.apparel.LastLayer == ApparelLayerDefOf.Overhead ||
					apparelDef.apparel.LastLayer == ApparelLayerDefOf.EyeCover;
			parent ??= useHeadNode ? headNode : bodyNode;
			if (parent == null)
			{
				continue;
			}

			float layerOffset = layerOffsets.TryGetValue(parent, out float value) ? value : 0f;
			PawnRenderNodeProperties props = new PawnRenderNodeProperties
			{
				debugLabel = apparelDef.defName,
				workerClass = useHeadNode
					? typeof(PawnRenderNodeWorker_Apparel_Head)
					: typeof(PawnRenderNodeWorker_Apparel_Body),
				nodeClass = typeof(PawnRenderNode_ApparelDummy),
				parentTagDef = parent.Props.tagDef,
				baseLayer = parent.Props.baseLayer + layerOffset,
				drawData = apparelDef.apparel.drawData
			};

			ApplyLayerOffsets(props, apparelDef, parent, offsetComp, dress.dressName);
			if (!__instance.ShouldAddNodeToTree(props))
			{
				continue;
			}

			PawnRenderNode_ApparelDummy node = new PawnRenderNode_ApparelDummy(
				pawn,
				props,
				__instance,
				apparel,
				apparelDef,
				dress.color,
				null,
				DefDatabase<BodyTypeDef>.GetNamed(dress.bodyName, true));

			__instance.AddChild(node, parent);
			layerOffsets[parent] = layerOffset + 1f;
		}
	}

	private static Apparel GetDummyApparel(DuC_Comp_DressUpCore core, ThingDef apparelDef)
	{
		if (core.dummy != null)
		{
			return core.dummy;
		}

		Thing thing = ThingMaker.MakeThing(apparelDef);
		core.dummy = thing as Apparel;
		return core.dummy;
	}

	private static void ApplyLayerOffsets(
		PawnRenderNodeProperties props,
		ThingDef apparelDef,
		PawnRenderNode parent,
		DuCEx_Comp_DressOffset offsetComp,
		string dressName)
	{
		if (props.drawData != null || apparelDef.apparel.shellRenderedBehindHead ||
			apparelDef.apparel.LastLayer != ApparelLayerDefOf.Shell)
		{
			return;
		}

		StructOffset offset = offsetComp?.offsets?.Find(item => item.dressName == dressName);
		float northLayerOffset = offset?.northLayerOffset ?? 0f;
		float southLayerOffset = offset?.southLayerOffset ?? 0f;
		float eastLayerOffset = offset?.eastLayerOffset ?? 0f;
		float westLayerOffset = offset?.westLayerOffset ?? 0f;

		props.drawData = DrawData.NewWithData(new DrawData.RotationalData[4]
		{
			new DrawData.RotationalData(Rot4.North, 88f + northLayerOffset),
			new DrawData.RotationalData(Rot4.South, parent.Props.baseLayer + southLayerOffset),
			new DrawData.RotationalData(Rot4.East, parent.Props.baseLayer + eastLayerOffset),
			new DrawData.RotationalData(Rot4.West, parent.Props.baseLayer + westLayerOffset)
		});
		props.oppositeFacingLayerWhenFlipped = true;
	}
}
