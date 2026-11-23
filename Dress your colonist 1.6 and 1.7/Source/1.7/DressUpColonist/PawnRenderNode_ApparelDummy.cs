using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace DressUpColonist;

public class PawnRenderNode_ApparelDummy : PawnRenderNode_Apparel
{
	public ThingDef apparelDef;

	public Color color;

	public ThingStyleDef style;

	public BodyTypeDef body;

	public PawnRenderNode_ApparelDummy(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree, Apparel apparel, ThingDef apDef, Color color, ThingStyleDef style, BodyTypeDef body)
		: base(pawn, props, tree)
	{
		((PawnRenderNode)this).apparel = apparel;
		apparelDef = apDef;
		this.color = color;
		this.style = style;
		this.body = body;
		if (apparel != null)
		{
			apparel.StyleDef = style;
		}
		apparel?.SetColor(color, reportFailure: false);
	}

	public override IEnumerable<Graphic> GraphicsFor(Pawn pawn)
	{
		if (TryGetGraphicApparel(apparelDef, body ?? pawn.story?.bodyType, out Graphic graphic))
		{
			yield return graphic;
		}
	}

	private bool TryGetGraphicApparel(ThingDef apDef, BodyTypeDef bodyType, out Graphic rec)
	{
		if (bodyType == null)
		{
			bodyType = BodyTypeDefOf.Male;
		}
		string wornGraphicPath = apparel?.WornGraphicPath;
		if (apDef?.apparel == null || GenText.NullOrEmpty(wornGraphicPath))
		{
			rec = null;
			return false;
		}
		string text = ((apDef.apparel.LastLayer != ApparelLayerDefOf.Overhead && apDef.apparel.LastLayer != ApparelLayerDefOf.EyeCover && !apparel.RenderAsPack() && wornGraphicPath != BaseContent.PlaceholderImagePath && wornGraphicPath != BaseContent.PlaceholderGearImagePath) ? (wornGraphicPath + "_" + bodyType.defName) : wornGraphicPath);
		Shader val = ShaderDatabase.Cutout;
		if (style?.graphicData.shaderType != null)
		{
			val = style.graphicData.shaderType.Shader;
		}
		else if ((style == null && apDef.apparel.useWornGraphicMask) || (style != null && style.UseWornGraphicMask))
		{
			val = ShaderDatabase.CutoutComplex;
		}
		rec = GraphicDatabase.Get<Graphic_Multi>(text, val, apDef.graphicData.drawSize, color);
		return true;
	}
}
