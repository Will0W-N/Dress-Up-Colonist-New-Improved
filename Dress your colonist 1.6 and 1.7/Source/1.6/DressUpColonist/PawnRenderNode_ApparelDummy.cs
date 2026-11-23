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
		if (((PawnRenderNode)this).PrimaryGraphic == null)
		{
			if (TryGetGraphicApparel(apparelDef, body, out var rec))
			{
				primaryGraphic = rec;
			}
		}
	}

	private bool TryGetGraphicApparel(ThingDef apDef, BodyTypeDef bodyType, out Graphic rec)
	{
		if (bodyType == null)
		{
			Log.Error("Getting apparel graphic with undefined body type.");
			bodyType = BodyTypeDefOf.Male;
		}
		if (GenText.NullOrEmpty(apDef.apparel.wornGraphicPath))
		{
			rec = null;
			return false;
		}
		string text = ((apDef.apparel.LastLayer != ApparelLayerDefOf.Overhead && apDef.apparel.LastLayer != ApparelLayerDefOf.EyeCover && !RenderAsPack(apDef) && !(apDef.apparel.wornGraphicPath == BaseContent.PlaceholderImagePath) && !(apDef.apparel.wornGraphicPath == BaseContent.PlaceholderGearImagePath)) ? (apDef.apparel.wornGraphicPath + "_" + ((Def)bodyType).defName) : apDef.apparel.wornGraphicPath);
		Shader val = ShaderDatabase.Cutout;
		if (style?.graphicData.shaderType != null)
		{
			val = style.graphicData.shaderType.Shader;
		}
		else if ((style == null && apDef.apparel.useWornGraphicMask) || (style != null && style.UseWornGraphicMask))
		{
			val = ShaderDatabase.CutoutComplex;
		}
		Graphic val2 = GraphicDatabase.Get<Graphic_Multi>(text, val, apDef.graphicData.drawSize, color);
		rec = val2;
		return true;
	}

	private static bool RenderAsPack(ThingDef apparel)
	{
		return false;
	}
}
