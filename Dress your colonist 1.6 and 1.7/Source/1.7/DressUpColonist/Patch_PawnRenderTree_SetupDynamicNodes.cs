using System.Collections.Generic;
using System.Linq;
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
		PawnRenderNode headNode = null;
		PawnRenderNode bodyNode = null;
		__instance.TryGetNodeByTag(PawnRenderNodeTagDefOf.ApparelHead, out headNode);
		__instance.TryGetNodeByTag(PawnRenderNodeTagDefOf.ApparelBody, out bodyNode);

		foreach (StructDress dress in core.dresses
			.Where(item => item != null)
			.OrderBy(item => ApparelLayerOrder(DefDatabase<ThingDef>.GetNamedSilentFail(item.dressName))))
		{
			ThingDef apparelDef = DefDatabase<ThingDef>.GetNamedSilentFail(dress.dressName);
			if (apparelDef?.apparel == null)
			{
				continue;
			}

			Apparel apparel = CreateRenderApparel(apparelDef, dress.styleDef);
			if (apparel == null)
			{
				continue;
			}

			PawnRenderNode parent = ResolveParent(__instance, apparelDef, headNode, bodyNode, out bool useHeadNode);
			if (parent == null)
			{
				continue;
			}

			if (!layerOffsets.TryGetValue(parent, out float layerOffset))
			{
				layerOffset = CountWornApparelForParent(pawn, __instance, parent, headNode, bodyNode);
			}

			PawnRenderNodeProperties props = new PawnRenderNodeProperties
			{
				debugLabel = apparelDef.defName,
				workerClass = useHeadNode
					? typeof(PawnRenderNodeWorker_Apparel_Head)
					: typeof(PawnRenderNodeWorker_Apparel_Body),
				nodeClass = typeof(PawnRenderNode_ApparelDummy),
				parentTagDef = apparelDef.apparel.parentTagDef,
				baseLayer = parent.Props.baseLayer + layerOffset,
				drawData = apparelDef.apparel.drawData
			};

			ApplyLayerOffsets(props, apparelDef, parent, offsetComp, dress.dressName, apparel);
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
				dress.styleDef,
				DefDatabase<BodyTypeDef>.GetNamedSilentFail(dress.bodyName) ??
					pawn.story?.bodyType ??
					BodyTypeDefOf.Male);

			__instance.AddChild(node, parent);
			layerOffsets[parent] = layerOffset + 1f;
		}
	}

	private static int ApparelLayerOrder(ThingDef apparelDef)
	{
		return apparelDef?.apparel?.layers.NullOrEmpty() != false
			? 0
			: apparelDef.apparel.layers.Max(layer => layer.drawOrder);
	}

	private static PawnRenderNode ResolveParent(
		PawnRenderTree tree,
		ThingDef apparelDef,
		PawnRenderNode headNode,
		PawnRenderNode bodyNode,
		out bool useHeadNode)
	{
		PawnRenderNode parent = null;
		if (apparelDef.apparel.parentTagDef != null)
		{
			tree.TryGetNodeByTag(apparelDef.apparel.parentTagDef, out parent);
		}

		useHeadNode = apparelDef.apparel.LastLayer == ApparelLayerDefOf.Overhead ||
			apparelDef.apparel.LastLayer == ApparelLayerDefOf.EyeCover;
		if (parent == headNode && headNode != null)
		{
			useHeadNode = true;
		}
		else if (parent == bodyNode && bodyNode != null)
		{
			useHeadNode = false;
		}

		return parent ?? (useHeadNode ? headNode : bodyNode);
	}

	private static int CountWornApparelForParent(
		Pawn pawn,
		PawnRenderTree tree,
		PawnRenderNode targetParent,
		PawnRenderNode headNode,
		PawnRenderNode bodyNode)
	{
		int count = 0;
		if (pawn.apparel?.WornApparel == null)
		{
			return count;
		}

		foreach (Apparel wornApparel in pawn.apparel.WornApparel)
		{
			if (wornApparel?.def?.apparel == null || wornApparel.def.IsWeapon)
			{
				continue;
			}

			PawnRenderNode parent = ResolveParent(tree, wornApparel.def, headNode, bodyNode, out _);
			if (parent == targetParent)
			{
				count++;
			}
		}

		return count;
	}

	private static Apparel CreateRenderApparel(ThingDef apparelDef, ThingStyleDef styleDef)
	{
		if (apparelDef?.apparel == null)
		{
			return null;
		}

		ThingDef stuff = null;
		if (apparelDef.MadeFromStuff)
		{
			List<StuffCategoryDef> categories = apparelDef.stuffCategories;
			StuffCategoryDef metallic = DefDatabase<StuffCategoryDef>.GetNamedSilentFail("Metallic");
			StuffCategoryDef fabric = DefDatabase<StuffCategoryDef>.GetNamedSilentFail("Fabric");
			StuffCategoryDef leathery = DefDatabase<StuffCategoryDef>.GetNamedSilentFail("Leathery");
			if (metallic != null && categories?.Contains(metallic) == true)
			{
				stuff = DefDatabase<ThingDef>.GetNamedSilentFail("Steel");
			}
			else if (fabric != null && categories?.Contains(fabric) == true)
			{
				stuff = DefDatabase<ThingDef>.GetNamedSilentFail("Cloth");
			}
			else if (leathery != null && categories?.Contains(leathery) == true)
			{
				stuff = DefDatabase<ThingDef>.GetNamedSilentFail("Leather_Plain");
			}
			stuff ??= apparelDef.defaultStuff ?? GenStuff.DefaultStuffFor(apparelDef);
			if (stuff == null)
			{
				return null;
			}
		}

		Apparel apparel = ThingMaker.MakeThing(apparelDef, stuff) as Apparel;
		if (apparel != null)
		{
			apparel.StyleDef = styleDef;
		}
		return apparel;
	}

	private static void ApplyLayerOffsets(
		PawnRenderNodeProperties props,
		ThingDef apparelDef,
		PawnRenderNode parent,
		DuCEx_Comp_DressOffset offsetComp,
		string dressName,
		Apparel apparel)
	{
		if (props.drawData != null || apparelDef.apparel.shellRenderedBehindHead)
		{
			return;
		}

		if (apparelDef.apparel.LastLayer == ApparelLayerDefOf.Shell)
		{
			StructOffset offset = offsetComp?.offsets?.Find(item => item.dressName == dressName);
			props.drawData = DrawData.NewWithData(new DrawData.RotationalData[4]
			{
				new DrawData.RotationalData(Rot4.North, 88f + (offset?.northLayerOffset ?? 0f)),
				new DrawData.RotationalData(Rot4.South, props.baseLayer + (offset?.southLayerOffset ?? 0f)),
				new DrawData.RotationalData(Rot4.East, props.baseLayer + (offset?.eastLayerOffset ?? 0f)),
				new DrawData.RotationalData(Rot4.West, props.baseLayer + (offset?.westLayerOffset ?? 0f))
			});
			props.oppositeFacingLayerWhenFlipped = true;
		}
		else if (apparel.RenderAsPack())
		{
			props.drawData = DrawData.NewWithData(
				new DrawData.RotationalData(Rot4.North, 93f),
				new DrawData.RotationalData(Rot4.South, -3f));
		}
	}
}
