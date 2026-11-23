using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace DressUpColonist;

public class DuC_ITab_Pawn_DressUpColonist : ITab
{
	private enum DuCTab : byte
	{
		Apparel,
		Body,
		Animal
	}

	public enum BodyChangeElement
	{
		Body,
		Head,
		Hair,
		Beard,
		BodyTatoo,
		HeadTatoo,
		Fur,
		Total
	}

	private List<TabRecord> DuCTabs = new List<TabRecord>();

	private DuCTab curTab;

	public List<ThingDef> apparels;

	private List<PawnKindDef> animalDefs = new List<PawnKindDef>();

	private Vector2 scrollPosition_ApparelAvailable;

	private Vector2 scrollPosition_DressAvailable;

	private Vector2 scrollPosition_BodyAvailable;

	private Vector2 scrollPosition_ElementAreaAvailable;

	private Vector2 scrollPosition_PropertiesAvailable;

	private Vector2 scrollPosition_AnimalAvailable;

	private ThingDef selectedApparel;

	private ThingDef selectedDress;

	private BodyTypeDef selectedBody;

	private int selectedElement;

	public PawnKindDef selectedAnimal;

	private Vector2 curSize = new Vector2(1f, 1f);

	private int curStage = 0;

	private Color curColor = Color.white;

	public List<ThingDef> dressUpList;

	public List<BodyTypeDef> bodyList;

	public List<HeadTypeDef> headList;

	public List<HairDef> hairList;

	public List<BeardDef> beardList;

	public List<TattooDef> bodyTattoList;

	public List<TattooDef> headTattoList;

	public List<FurDef> furList;

	private string searchApparelText;

	private string searchAnimalText;

	private BodyTypeDef selectedBodyType;

	public HeadTypeDef selectedHeadType;

	public HairDef selectedHair;

	public BeardDef selectedBeard;

	public TattooDef selectedBodyTatto;

	public TattooDef selectedHeadTatto;

	public FurDef selectedFur;

	public int preLifeStage = -1;

	private static float guiWidth = 300f;

	private static Vector2 originSize = new Vector2(650f, 585f);

	private Pawn preSelPawn = null;

	private static StructOffset clipboard = new StructOffset("copy", Vector3.zero, Vector3.one, 0f, 0f, 0f, 0f);

	private DuC_Comp_DressUpCore core => ThingCompUtility.TryGetComp<DuC_Comp_DressUpCore>((Thing)(object)((ITab)this).SelPawn);

	public DuC_Comp_DressUpCore compCore
	{
		get
		{
			if (((ThingWithComps)((ITab)this).SelPawn).GetComp<DuC_Comp_DressUpCore>() != null)
			{
				return ((ThingWithComps)((ITab)this).SelPawn).GetComp<DuC_Comp_DressUpCore>();
			}
			return null;
		}
	}

	public override bool IsVisible
	{
		get
		{
			if ((((ITab)this).SelPawn.RaceProps.Humanlike && ((Thing)((ITab)this).SelPawn).Faction != null && ((Thing)((ITab)this).SelPawn).Faction == Faction.OfPlayer) || ((ITab)this).SelPawn.IsPrisoner)
			{
				if (compCore != null)
				{
					return true;
				}
				return false;
			}
			return false;
		}
	}

	public DuC_ITab_Pawn_DressUpColonist()
	{
		((InspectTabBase)this).labelKey = "DuC_TabDressUp";
		((InspectTabBase)this).size = originSize;
	}

	public override void OnOpen()
	{
		((InspectTabBase)this).OnOpen();
		UpdateApparelList(ref apparels, "");
		UpdateAnimalList(ref animalDefs, "");
		bodyList = new List<BodyTypeDef>();
		foreach (BodyTypeDef item in DefDatabase<BodyTypeDef>.AllDefsListForReading)
		{
			bodyList.Add(item);
		}
		headList = new List<HeadTypeDef>();
		foreach (HeadTypeDef item2 in DefDatabase<HeadTypeDef>.AllDefsListForReading)
		{
			headList.Add(item2);
		}
		hairList = new List<HairDef>();
		foreach (HairDef item3 in DefDatabase<HairDef>.AllDefsListForReading)
		{
			hairList.Add(item3);
		}
		beardList = new List<BeardDef>();
		foreach (BeardDef item4 in DefDatabase<BeardDef>.AllDefsListForReading)
		{
			beardList.Add(item4);
		}
		selectedBodyType = DefDatabase<BodyTypeDef>.GetNamed(core.bodyTypeName, true);
		selectedHeadType = DefDatabase<HeadTypeDef>.GetNamed(core.headTypeName, true);
		selectedHair = DefDatabase<HairDef>.GetNamed(core.hairName, true);
		selectedBeard = DefDatabase<BeardDef>.GetNamed(core.beardName, true);
		DuCTabs.Clear();
		DuCTabs.Add(new TabRecord("", (Action)(() =>
		{
			curTab = DuCTab.Apparel;
		}), (Func<bool>)(() => curTab == DuCTab.Apparel)));
		DuCTabs.Add(new TabRecord("", (Action)(() =>
		{
			curTab = DuCTab.Body;
		}), (Func<bool>)(() => curTab == DuCTab.Body)));
		DuCTabs.Add(new TabRecord("", (Action)(() =>
		{
			selectedAnimal = DefDatabase<PawnKindDef>.GetNamed(core.animalKindDefName, true);
			curColor = core.animalColor;
			curSize = core.animalSize;
			curStage = core.animalStage;
			curTab = DuCTab.Animal;
		}), (Func<bool>)(() => curTab == DuCTab.Animal)));
	}

	private void UpdateApparelList(ref List<ThingDef> apparels, string searchText)
	{
		apparels = new List<ThingDef>();
		foreach (ThingDef item in DefDatabase<ThingDef>.AllDefsListForReading.Where((ThingDef d) => d.IsApparel && d.apparel != null && d.apparel.wornGraphicPath != null && d.apparel.wornGraphicPath != "" && d.apparel.layers.IndexOf(ApparelLayerDefOf.Belt) < 0))
		{
			if (searchText != "" && ((Def)item).label.IndexOf(searchText) >= 0)
			{
				apparels.Add(item);
			}
			else if (searchText == "")
			{
				apparels.Add(item);
			}
		}
	}

	private void UpdateAnimalList(ref List<PawnKindDef> animals, string search)
	{
		animals = new List<PawnKindDef>();
		List<PawnKindDef> list = new List<PawnKindDef>();
		list.AddRange(DefDatabase<PawnKindDef>.AllDefsListForReading);
		foreach (PawnKindDef item in list)
		{
			if ((item.RaceProps.Animal || item.RaceProps.IsMechanoid || (int)item.RaceProps.intelligence == 1) && ThingDef.Named(((object)item.race).ToString()) != null)
			{
				if (search != "" && ((Def)item).label.IndexOf(search) >= 0)
				{
					animals.Add(item);
				}
				else if (search == "")
				{
					animals.Add(item);
				}
			}
		}
	}

	public override void FillTab()
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 40f;
		Rect val = new Rect(0f, 0f, ((InspectTabBase)this).size.x, ((InspectTabBase)this).size.y);
		num2 += num3;
		Rect val2 = new Rect(4f, 4f, val.width - 8f, val.height - 8f);
		Widgets.DrawMenuSection(val2);
		TabRecord val3 = TabDrawer.DrawTabsOverflow<TabRecord>(val2, DuCTabs, 90f, 90f);
		Rect val4 = new Rect(4f, 6f, 90f, 29f);
		Widgets.ButtonImageFitted(val4, ContentFinder<Texture2D>.Get("Icon/Tab_Apparel", true));
		Rect val5 = new Rect(84f, 6f, 90f, 29f);
		Widgets.ButtonImageFitted(val5, ContentFinder<Texture2D>.Get("Icon/Tab_Body", true));
		Rect val6 = new Rect(164f, 6f, 90f, 29f);
		Widgets.ButtonImageFitted(val6, ContentFinder<Texture2D>.Get("Icon/Tab_Animal", true));
		TooltipHandler.TipRegion(val6, "coming soon");
		if (((ITab)this).SelPawn != preSelPawn && curTab != DuCTab.Apparel && curTab != DuCTab.Body && curTab == DuCTab.Animal)
		{
			selectedAnimal = DefDatabase<PawnKindDef>.GetNamed(core.animalKindDefName, true);
			curColor = core.animalColor;
			curSize = core.animalSize;
			curStage = core.animalStage;
		}
		preSelPawn = ((ITab)this).SelPawn;
		Text.Font = (GameFont)1;
		Rect rectDispMain = new Rect(10f, num2 + 10f, val.width - 20f, val.height - val.y - 20f);
		if (curTab == DuCTab.Apparel)
		{
			DrawApparelTab(rectDispMain, num2);
		}
		else if (curTab == DuCTab.Body)
		{
			((InspectTabBase)this).size = originSize;
			DrawBodyTab(rectDispMain, num2);
		}
		else if (curTab == DuCTab.Animal)
		{
			((InspectTabBase)this).size = originSize;
			DoAnimalTab(rectDispMain, num2);
		}
	}

	private void DoAnimalTab(Rect rectDispMain, float baseHeight)
	{
		float num = 0f;
		Rect val = new Rect(rectDispMain.x, rectDispMain.y, 250f, rectDispMain.height - 35f - baseHeight);
		Widgets.DrawBoxSolid(val, new Color(0.024f, 0.026f, 0.028f));
		Rect val2 = new Rect(0f, 0f, val.width - 16f, (float)Enumerable.Count(animalDefs) * 32f);
		Vector2 val3 = default;
		Widgets.BeginScrollView(val, ref scrollPosition_AnimalAvailable, val2, true);
		for (int i = 0; i < animalDefs.Count; i++)
		{
			PawnKindDef animal = animalDefs[i];
			float num2 = val3.y - 32f;
			float num3 = val3.y + val2.height;
			if (num > num2 && num < num3)
			{
				DoAnimalRow(new Rect(0f, num, val2.width - 4f, 32f), animal);
			}
			num += 32f;
		}
		Widgets.EndScrollView();
		Rect val4 = new Rect(val.x, val.y + val.height + 5f, val.width - 30f, 30f);
		searchAnimalText = Widgets.TextField(val4, searchAnimalText);
		Rect val5 = new Rect(val4.x + val4.width + 5f, val4.y, 30f, 30f);
		if (Widgets.ButtonImageWithBG(val5, ContentFinder<Texture2D>.Get("Icon/Btn_Search", true), null))
		{
			UpdateAnimalList(ref animalDefs, searchAnimalText);
		}
		if (selectedAnimal != null)
		{
			Text.Font = (GameFont)2;
			Rect val6 = new Rect(val.xMax + 10f, rectDispMain.y + 5f, 400f, 35f);
			Widgets.Label(val6, GenText.Truncate(((Def)selectedAnimal).label, val6.width, (Dictionary<string, string>)null));
			Rect val7 = new Rect(val.xMax + 70f, rectDispMain.y + 40f, 250f, 250f);
			Rect val8 = new Rect(val.xMax + 10f, rectDispMain.y + 40f, 370f, 250f);
			Widgets.DrawBoxSolid(val8, Color.gray);
			Color color = curColor;
			bool flag = false;
			GUI.color = color;
			Widgets.DrawTextureFitted(val7, (Texture)(object)ContentFinder<Texture2D>.Get(selectedAnimal.lifeStages[curStage].bodyGraphicData.texPath + "_south", true), 1f);
			GUI.color = Color.white;
			baseHeight = val7.y + 250f + 15f;
		}
		if (selectedAnimal != null)
		{
			Text.Font = (GameFont)1;
			Rect val9 = new Rect(val.xMax + 10f, baseHeight, 370f, 30f);
			int num4 = selectedAnimal.lifeStages.Count - 1;
			if (num4 > 0)
			{
				curStage = (int)Widgets.HorizontalSlider(val9, (float)curStage, 0f, (float)num4, false, "LifeStage " + curStage, "0", num4.ToString(), 1f);
			}
			if (preLifeStage != curStage)
			{
				curSize.x = Math.Min(selectedAnimal.lifeStages[curStage].bodyGraphicData.drawSize.x, 2f);
				curSize.y = Math.Min(selectedAnimal.lifeStages[curStage].bodyGraphicData.drawSize.y, 2f);
				curColor = selectedAnimal.lifeStages[curStage].bodyGraphicData.color;
			}
			preLifeStage = curStage;
			baseHeight += 40f;
			Rect val10 = new Rect(val.xMax + 10f, baseHeight, 160f, 30f);
			curSize.x = Widgets.HorizontalSlider(val10, curSize.x, 0.1f, 10f, false, "width : " + curSize.x, "0.1", "10.0", 0.1f);
			Rect val11 = val10;
			val11.y += 40f;
			curSize.y = Widgets.HorizontalSlider(val11, curSize.y, 0.1f, 10f, false, "height : " + curSize.y, "0.1", "10.0", 0.1f);
			Rect val12 = val10;
			val12.x += 180f;
			GUI.color = Color.red;
			curColor.r = Widgets.HorizontalSlider(val12, curColor.r, 0f, 1f, false, "Red : " + curColor.r, "0.0", "1.0", 0.01f);
			Rect val13 = val12;
			val13.y += 40f;
			GUI.color = Color.green;
			curColor.g = Widgets.HorizontalSlider(val13, curColor.g, 0f, 1f, false, "Green : " + curColor.g, "0.0", "1.0", 0.01f);
			Rect val14 = val13;
			val14.y += 40f;
			GUI.color = Color.blue;
			curColor.b = Widgets.HorizontalSlider(val14, curColor.b, 0f, 1f, false, "Brue : " + curColor.b, "0.0", "1.0", 0.01f);
			GUI.color = Color.white;
			Rect val15 = new Rect(val.xMax + 10f, val14.yMax + 5f, 370f, 30f);
			Widgets.CheckboxLabeled(val15, "Reverse west graphic", ref compCore.reverseWest, false, (Texture2D)null, (Texture2D)null, true, false);
		}
		if (selectedAnimal != null)
		{
			Rect val16 = new Rect(val.xMax + 120f, rectDispMain.height - 20f, 80f, 30f);
			if (Widgets.ButtonImageWithBG(val16, ContentFinder<Texture2D>.Get("Icon/Btn_ShapeShift", true), null))
			{
				compCore.animalKindDefName = ((Def)selectedAnimal).defName;
				compCore.animalSize = curSize;
				compCore.animalColor = curColor;
				compCore.animalStage = curStage;
				compCore.bodyChangeEnable = false;
				compCore.animalEnable = true;
				((ITab)this).SelPawn.Drawer.renderer.SetAllGraphicsDirty();
			}
			Rect val17 = new Rect(val.xMax + 220f, rectDispMain.height - 20f, 80f, 30f);
			if (Widgets.ButtonImageWithBG(val17, ContentFinder<Texture2D>.Get("Icon/Btn_Release", true), null))
			{
				compCore.animalKindDefName = "";
				compCore.animalSize = new Vector2(1f, 1f);
				compCore.animalColor = Color.white;
				compCore.animalStage = 0;
				compCore.animalEnable = false;
				((ITab)this).SelPawn.Drawer.renderer.SetAllGraphicsDirty();
			}
		}
	}

	private void DrawApparelTab(Rect rectDispMain, float baseHeight)
	{
		float num = 0f;
		Rect val = new Rect(rectDispMain.x, rectDispMain.y, 250f, rectDispMain.height - 35f - baseHeight);
		Widgets.DrawBoxSolid(val, new Color(0.024f, 0.026f, 0.028f));
		Rect val2 = new Rect(0f, 0f, val.width - 16f, (float)Enumerable.Count(apparels) * 32f);
		Vector2 val3 = default;
		Widgets.BeginScrollView(val, ref scrollPosition_ApparelAvailable, val2, true);
		num = 0f;
		for (int i = 0; i < apparels.Count; i++)
		{
			ThingDef apparel = apparels[i];
			float num2 = val3.y - 32f;
			float num3 = val3.y + val2.height;
			if (num > num2 && num < num3)
			{
				DoApparelRow(new Rect(0f, num, val2.width - 4f, 32f), apparel);
			}
			num += 32f;
		}
		Widgets.EndScrollView();
		Rect val4 = new Rect(val.x, val.y + val.height + 5f, val.width - 50f, 30f);
		searchApparelText = Widgets.TextField(val4, searchApparelText);
		Rect val5 = new Rect(val4.x + val4.width + 5f, val4.y, 45f, 30f);
		if (Widgets.ButtonImageWithBG(val5, ContentFinder<Texture2D>.Get("Icon/Btn_Search", true), null))
		{
			UpdateApparelList(ref apparels, searchApparelText);
		}
		Rect val6 = new Rect(val.x + val.width + 10f, 100f, 80f, 40f);
		if (Widgets.ButtonText(val6, ">>", true, true, true, null) && selectedApparel != null)
		{
			if (GenList.NullOrEmpty<StructDress>((IList<StructDress>)core.dresses))
			{
				core.dresses = new List<StructDress>();
			}
			if (core.dresses.Where((StructDress d) => d.dressName == ((Def)selectedApparel).defName).Count() <= 0)
			{
				List<BodyTypeDef> list = new List<BodyTypeDef>();
				for (int num4 = 0; num4 < DefDatabase<BodyTypeDef>.AllDefsListForReading.Count; num4++)
				{
					BodyTypeDef val7 = bodyList[num4];
					Texture2D val8 = ContentFinder<Texture2D>.Get(selectedApparel.apparel.wornGraphicPath + "_" + ((object)val7).ToString() + "_south", false);
					if ((UnityEngine.Object)(object)val8 != (UnityEngine.Object)null)
					{
						list.Add(val7);
					}
				}
				BodyTypeDef val9 = (selectedBody = ((list.Count == 0) ? ((ITab)this).SelPawn.story.bodyType : ((list.IndexOf(((ITab)this).SelPawn.story.bodyType) < 0) ? list.First() : ((ITab)this).SelPawn.story.bodyType)));
				core.dresses.Add(new StructDress(((Def)selectedApparel).defName, Color.white, Gen.ToStringSafe<BodyTypeDef>(val9)));
			}
		}
		Rect val10 = new Rect(val.x + val.width + 10f, 160f, 80f, 40f);
		if (Widgets.ButtonText(val10, "<<", true, true, true, null) && selectedDress != null && !GenList.NullOrEmpty<StructDress>((IList<StructDress>)core.dresses))
		{
			core.dresses.RemoveAll((StructDress structDress) => structDress.dressName == ((Def)selectedDress).defName);
			selectedDress = null;
		}
		Rect val11 = new Rect(rectDispMain.x + val.width + 100f, val.y, 250f, 200f);
		Widgets.DrawBoxSolid(val11, new Color(0.024f, 0.026f, 0.028f));
		if (!GenList.NullOrEmpty<StructDress>((IList<StructDress>)core.dresses))
		{
			Rect val12 = new Rect(0f, 0f, val11.width - 16f, (float)Enumerable.Count(core.dresses) * 46f);
			Vector2 val13 = default;
			Widgets.BeginScrollView(val11, ref scrollPosition_DressAvailable, val12, true);
			num = 0f;
			for (int num5 = 0; num5 < core.dresses.Count; num5++)
			{
				ThingDef dress = ThingDef.Named(core.dresses[num5].dressName);
				Color color = core.dresses[num5].color;
				float num6 = val13.y - 46f;
				float num7 = val13.y + val12.height;
				if (num > num6 && num < num7)
				{
					DoDressRow(new Rect(0f, num, val12.width - 4f, 46f), dress, color);
				}
				num += 46f;
			}
			Widgets.EndScrollView();
			Rect val14 = new Rect(val11.x + val11.width + 5f, val11.y, 30f, 60f);
			if (Widgets.ButtonText(val14, "▲", true, true, true, null) && selectedDress != null && !GenList.NullOrEmpty<StructDress>((IList<StructDress>)core.dresses))
			{
				int num8 = core.dresses.FindIndex((StructDress item) => item.dressName == ((Def)selectedDress).defName);
				if (num8 > 0)
				{
					MoveElement(core.dresses, num8, num8 - 1);
				}
			}
			Rect val15 = new Rect(val11.x + val11.width + 5f, val11.y + 130f, 30f, 60f);
			if (Widgets.ButtonText(val15, "▼", true, true, true, null) && selectedDress != null && !GenList.NullOrEmpty<StructDress>((IList<StructDress>)core.dresses))
			{
				int num9 = core.dresses.FindIndex((StructDress item) => item.dressName == ((Def)selectedDress).defName);
				if (num9 < Enumerable.Count(core.dresses) - 1)
				{
					MoveElement(core.dresses, num9, num9 + 1);
				}
			}
			if (selectedDress != null)
			{
				Color color2 = core.dresses.Where((StructDress d) => d.dressName == ((Def)selectedDress).defName).First().color;
				float r = color2.r;
				float g = color2.g;
				float b = color2.b;
				Rect val16 = new Rect(val11.x, val11.y + val11.height + 20f, val11.width, 30f);
				Widgets.HorizontalSlider(val16, ref r, new FloatRange(0f, 1f), string.Format("Red : {0}", r.ToString("F2")), -1f);
				Rect val17 = new Rect(val11.x, val16.y + 35f, val11.width, 30f);
				Widgets.HorizontalSlider(val17, ref g, new FloatRange(0f, 1f), string.Format("Green : {0}", g.ToString("F2")), -1f);
				Rect val18 = new Rect(val11.x, val17.y + 35f, val11.width, 30f);
				Widgets.HorizontalSlider(val18, ref b, new FloatRange(0f, 1f), string.Format("Blue : {0}", b.ToString("F2")), -1f);
				core.dresses.Where((StructDress d) => d.dressName == ((Def)selectedDress).defName).First().color = new Color(r, g, b);
				Rect val19 = new Rect(val11.x, val11.y + val11.height + 120f, val11.width, 155f);
				Widgets.DrawBoxSolid(val19, new Color(0.024f, 0.026f, 0.028f));
				Rect val20 = new Rect(0f, 0f, val19.width - 16f, (float)Enumerable.Count(DefDatabase<BodyTypeDef>.AllDefsListForReading) * 32f);
				Vector2 val21 = default;
				Widgets.BeginScrollView(val19, ref scrollPosition_BodyAvailable, val20, true);
				num = 0f;
				for (int num10 = 0; num10 < DefDatabase<BodyTypeDef>.AllDefsListForReading.Count; num10++)
				{
					BodyTypeDef val22 = bodyList[num10];
					Texture2D val23 = ContentFinder<Texture2D>.Get(selectedDress.apparel.wornGraphicPath + "_" + ((object)val22).ToString() + "_south", false);
					if ((UnityEngine.Object)(object)val23 != (UnityEngine.Object)null)
					{
						float num11 = val21.y - 32f;
						float num12 = val21.y + val20.height;
						if (num > num11 && num < num12)
						{
							DoBodyRow(new Rect(0f, num, val20.width - 4f, 32f), val22, core, val23);
						}
						num += 32f;
					}
				}
				Widgets.EndScrollView();
			}
		}
		Rect val24 = new Rect(rectDispMain.x + val.width + 120f, rectDispMain.height - 20f, 80f, 30f);
		if (Widgets.ButtonImageWithBG(val24, ContentFinder<Texture2D>.Get("Icon/Btn_DressUp", true), null))
		{
			core.dressUpEnable = true;
			((ITab)this).SelPawn.Drawer.renderer.SetAllGraphicsDirty();
		}
		Rect val25 = new Rect(rectDispMain.x + val.width + 220f, rectDispMain.height - 20f, 80f, 30f);
		if (Widgets.ButtonImageWithBG(val25, ContentFinder<Texture2D>.Get("Icon/Btn_DressOff", true), null))
		{
			core.dressUpEnable = false;
			((ITab)this).SelPawn.Drawer.renderer.SetAllGraphicsDirty();
		}
		if (core.loadedExtend)
		{
			return;
		}
		DuCEx_Comp_DressOffset duCEx_Comp_DressOffset = ThingCompUtility.TryGetComp<DuCEx_Comp_DressOffset>((Thing)(object)((ITab)this).SelPawn);
		if (GenList.NullOrEmpty<StructOffset>((IList<StructOffset>)duCEx_Comp_DressOffset.offsets))
		{
			duCEx_Comp_DressOffset.offsets = new List<StructOffset>();
		}
		if (selectedDress == null || GenList.NullOrEmpty<StructDress>((IList<StructDress>)core.dresses))
		{
			((InspectTabBase)this).size.x = originSize.x;
		}
		else
		{
			((InspectTabBase)this).size.x = originSize.x + guiWidth;
			StructOffset structOffset = GenCollection.FirstOrDefault<StructOffset>(duCEx_Comp_DressOffset.offsets, (Predicate<StructOffset>)((StructOffset d) => d.dressName == ((Def)selectedDress).defName));
			if (structOffset == null)
			{
				structOffset = new StructOffset(((Def)selectedDress).defName, Vector3.zero, Vector3.one, 0f, 0f, 0f, 0f);
				duCEx_Comp_DressOffset.offsets.Add(structOffset);
			}
			Vector3 scale = structOffset.scale;
			float value = scale.x;
			float value2 = scale.y;
			float value3 = scale.z;
			Vector3 offset = structOffset.offset;
			float value4 = offset.x;
			float value5 = offset.y;
			float value6 = offset.z;
			float value7 = structOffset.northLayerOffset;
			float value8 = structOffset.southLayerOffset;
			float value9 = structOffset.eastLayerOffset;
			float value10 = structOffset.westLayerOffset;
			float num13 = 10f;
			float num14 = 40f;
			float num15 = 5f;
			float num16 = 20f;
			Rect val26 = new Rect(0f, 0f, ((InspectTabBase)this).size.x, ((InspectTabBase)this).size.y);
			val26 = GenUI.ExpandedBy(GenUI.RightPartPixels(GenUI.BottomPartPixels(val26, val26.height - num14), guiWidth), 0f - num13);
			Rect workRect = val26;
			drawSlider(string.Format("Width : {0}", value.ToString("F2")), ref value, ref workRect, new FloatRange(0f, 2f));
			Rect val27 = default;
			GenUI.SplitHorizontally(workRect, num15, out val27, out workRect);
			drawSlider(string.Format("Height : {0}", value3.ToString("F2")), ref value3, ref workRect, new FloatRange(0f, 2f));
			GenUI.SplitHorizontally(workRect, num15, out val27, out workRect);
			drawSlider(string.Format("Thickness : {0}", value2.ToString("F2")), ref value2, ref workRect, new FloatRange(0f, 2f));
			GenUI.SplitHorizontally(workRect, num15, out val27, out workRect);
			GenUI.SplitHorizontally(workRect, num16, out val27, out workRect);
			drawSlider(string.Format("Horizontal : {0}", value4.ToString("F2")), ref value4, ref workRect, new FloatRange(-1f, 1f));
			GenUI.SplitHorizontally(workRect, num15, out val27, out workRect);
			drawSlider(string.Format("Vertical : {0}", value6.ToString("F2")), ref value6, ref workRect, new FloatRange(-1f, 1f));
			GenUI.SplitHorizontally(workRect, num15, out val27, out workRect);
			drawSlider(string.Format("Depth : {0}", value5.ToString("F2")), ref value5, ref workRect, new FloatRange(-1f, 1f));
			GenUI.SplitHorizontally(workRect, num15, out val27, out workRect);
			GenUI.SplitHorizontally(workRect, num16, out val27, out workRect);
			drawSlider(string.Format("OffsetLayer North : {0}", value7.ToString("F0")), ref value7, ref workRect, new FloatRange(-100f, 100f));
			GenUI.SplitHorizontally(workRect, num16, out val27, out workRect);
			drawSlider(string.Format("OffsetLayer South : {0}", value8.ToString("F0")), ref value8, ref workRect, new FloatRange(-100f, 100f));
			GenUI.SplitHorizontally(workRect, num16, out val27, out workRect);
			drawSlider(string.Format("OffsetLayer East : {0}", value9.ToString("F0")), ref value9, ref workRect, new FloatRange(-100f, 100f));
			GenUI.SplitHorizontally(workRect, num16, out val27, out workRect);
			drawSlider(string.Format("OffsetLayer West : {0}", value10.ToString("F0")), ref value10, ref workRect, new FloatRange(-100f, 100f));
			structOffset.offset = new Vector3(value4, value5, value6);
			structOffset.scale = new Vector3(value, value2, value3);
			if (structOffset.northLayerOffset != value7 || structOffset.southLayerOffset != value8 || structOffset.eastLayerOffset != value9 || structOffset.westLayerOffset != value10)
			{
				structOffset.northLayerOffset = value7;
				structOffset.southLayerOffset = value8;
				structOffset.eastLayerOffset = value9;
				structOffset.westLayerOffset = value10;
				if (core.dressUpEnable)
				{
					((ITab)this).SelPawn.Drawer.renderer.SetAllGraphicsDirty();
				}
			}
			GenUI.SplitHorizontally(workRect, num16, out val27, out workRect);
			float num17 = 30f;
			double num18 = 3.0;
			float num19 = (float)Math.Floor((double)workRect.width / num18);
			Rect val28 = default;
			GenUI.SplitHorizontally(workRect, num17, out val28, out workRect);
			Rect val29 = default;
			GenUI.SplitVertically(val28, num19, out val29, out val28);
			Rect val30 = default;
			Rect val31 = default;
			GenUI.SplitVertically(val28, num19, out val30, out val31);
			if (Widgets.ButtonText(val29, "Copy", true, true, true, null))
			{
				clipboard = new StructOffset("cp", structOffset.offset, structOffset.scale, structOffset.northLayerOffset, structOffset.southLayerOffset, structOffset.eastLayerOffset, structOffset.westLayerOffset);
			}
			if (Widgets.ButtonText(val30, "Paste", true, true, true, null))
			{
				structOffset.offset = clipboard.offset;
				structOffset.scale = clipboard.scale;
				structOffset.northLayerOffset = clipboard.northLayerOffset;
				structOffset.southLayerOffset = clipboard.southLayerOffset;
				structOffset.eastLayerOffset = clipboard.eastLayerOffset;
				structOffset.westLayerOffset = clipboard.westLayerOffset;
				if (core.dressUpEnable)
				{
					((ITab)this).SelPawn.Drawer.renderer.SetAllGraphicsDirty();
				}
			}
			if (Widgets.ButtonText(val31, "Reset", true, true, true, null))
			{
				structOffset.offset = Vector3.zero;
				structOffset.scale = Vector3.one;
				structOffset.northLayerOffset = 0f;
				structOffset.southLayerOffset = 0f;
				structOffset.eastLayerOffset = 0f;
				structOffset.westLayerOffset = 0f;
				if (core.dressUpEnable)
				{
					((ITab)this).SelPawn.Drawer.renderer.SetAllGraphicsDirty();
				}
			}
		}
		if (GenList.NullOrEmpty<StructOffset>((IList<StructOffset>)duCEx_Comp_DressOffset.offsets))
		{
			return;
		}
		if (GenList.NullOrEmpty<StructDress>((IList<StructDress>)core.dresses))
		{
			duCEx_Comp_DressOffset.offsets.Clear();
			return;
		}
		IEnumerable<StructOffset> source = from d in core.dresses
			join o in duCEx_Comp_DressOffset.offsets on d.dressName equals o.dressName
			select o;
		StructOffset[] array = source.ToArray();
		if (array.Count() != Enumerable.Count(core.dresses))
		{
			duCEx_Comp_DressOffset.offsets.Clear();
			if (0 < array.Count())
			{
				duCEx_Comp_DressOffset.offsets.AddRange(array);
			}
		}
	}

	private void DrawBodyTab(Rect rectDispMain, float baseHeight)
	{
		Rect val = new Rect(rectDispMain.x + 4f, rectDispMain.y, 280f, rectDispMain.height - 35f - baseHeight);
		Widgets.DrawBoxSolid(val, new Color(0.024f, 0.026f, 0.028f));
		Rect val2 = new Rect(0f, 0f, val.width - 16f, 364f);
		Vector2 val3 = default;
		Widgets.BeginScrollView(val, ref scrollPosition_ElementAreaAvailable, val2, true);
		float num = 0f;
		Array values = Enum.GetValues(typeof(BodyChangeElement));
		for (int i = 0; i < 3; i++)
		{
			float num2 = val3.y - 52f;
			float num3 = val3.y + val2.height;
			if (num > num2 && num < num3)
			{
				DoElementAreaRow(new Rect(0f, num, val2.width - 4f, 52f), (BodyChangeElement)values.GetValue(i), ref core.bodyTabEnableList[i]);
			}
			num += 52f;
		}
		Widgets.EndScrollView();
		Rect val4 = new Rect(rectDispMain.x + 300f, rectDispMain.y, 300f, 300f);
		Widgets.DrawBoxSolid(val4, new Color(0.024f, 0.026f, 0.028f));
		int num4 = 0;
		switch (selectedElement)
		{
		case 0:
		{
			num4 = Enumerable.Count(DefDatabase<BodyTypeDef>.AllDefsListForReading);
			Rect val15 = new Rect(0f, 0f, val4.width - 16f, (float)num4 * 32f);
			Vector2 val16 = default;
			Widgets.BeginScrollView(val4, ref scrollPosition_PropertiesAvailable, val15, true);
			num = 0f;
			for (int l = 0; l < num4; l++)
			{
				BodyTypeDef body = bodyList[l];
				float num9 = val16.y - 32f;
				float num10 = val16.y + val15.height;
				if (num > num9 && num < num10)
				{
					DoBodyTypeRow(new Rect(0f, num, val15.width - 4f, 32f), body);
				}
				num += 32f;
			}
			Widgets.EndScrollView();
			Color bodyColor = core.bodyColor;
			float r2 = bodyColor.r;
			float g2 = bodyColor.g;
			float b2 = bodyColor.b;
			Rect val17 = new Rect(val4.x, val4.yMax + 20f, val4.width, 30f);
			Widgets.HorizontalSlider(val17, ref r2, new FloatRange(0f, 1f), "Red", -1f);
			Rect val18 = new Rect(val4.x, val17.yMax + 5f, val4.width, 30f);
			Widgets.HorizontalSlider(val18, ref g2, new FloatRange(0f, 1f), "Green", -1f);
			Rect val19 = new Rect(val4.x, val18.yMax + 5f, val4.width, 30f);
			Widgets.HorizontalSlider(val19, ref b2, new FloatRange(0f, 1f), "Blue", -1f);
			core.bodyColor = new Color(r2, g2, b2);
			Rect val20 = new Rect(val4.x, val19.yMax + 10f, val4.width, 30f);
			Widgets.CheckboxLabeled(val20, "Invisible body mode", ref core.bodyInvisible, false, (Texture2D)null, (Texture2D)null, false, false);
			break;
		}
		case 1:
		{
			num4 = Enumerable.Count(DefDatabase<HeadTypeDef>.AllDefsListForReading);
			Rect val10 = new Rect(0f, 0f, val4.width - 16f, (float)num4 * 32f);
			Vector2 val11 = default;
			Widgets.BeginScrollView(val4, ref scrollPosition_PropertiesAvailable, val10, true);
			num = 0f;
			for (int k = 0; k < num4; k++)
			{
				HeadTypeDef head = headList[k];
				float num7 = val11.y - 32f;
				float num8 = val11.y + val10.height;
				if (num > num7 && num < num8)
				{
					DoHeadTypeRow(new Rect(0f, num, val10.width - 4f, 32f), head);
				}
				num += 32f;
			}
			Widgets.EndScrollView();
			Vector3 headOffset = core.headOffset;
			Rect val12 = new Rect(val4.x, val4.y + val4.height + 20f, val4.width, 30f);
			Widgets.HorizontalSlider(val12, ref headOffset.x, new FloatRange(-2f, 2f), "Offset x", -1f);
			Rect val13 = new Rect(val4.x, val12.y + 35f, val4.width, 30f);
			Widgets.HorizontalSlider(val13, ref headOffset.z, new FloatRange(-2f, 2f), "Offset z", -1f);
			core.headOffset = headOffset;
			Rect val14 = new Rect(val4.x, val13.yMax + 10f, val4.width, 30f);
			Widgets.CheckboxLabeled(val14, "Invisible head mode", ref core.headInvisible, false, (Texture2D)null, (Texture2D)null, false, false);
			break;
		}
		case 2:
		{
			num4 = Enumerable.Count(DefDatabase<HairDef>.AllDefsListForReading);
			Rect val21 = new Rect(0f, 0f, val4.width - 16f, (float)num4 * 32f);
			Vector2 val22 = default;
			Widgets.BeginScrollView(val4, ref scrollPosition_PropertiesAvailable, val21, true);
			num = 0f;
			for (int m = 0; m < num4; m++)
			{
				HairDef hair = hairList[m];
				float num11 = val22.y - 32f;
				float num12 = val22.y + val21.height;
				if (num > num11 && num < num12)
				{
					DoHairRow(new Rect(0f, num, val21.width - 4f, 32f), hair);
				}
				num += 32f;
			}
			Widgets.EndScrollView();
			Color hairColor = core.hairColor;
			float r3 = hairColor.r;
			float g3 = hairColor.g;
			float b3 = hairColor.b;
			Rect val23 = new Rect(val4.x, val4.yMax + 20f, val4.width, 30f);
			Widgets.HorizontalSlider(val23, ref r3, new FloatRange(0f, 1f), "Red", -1f);
			Rect val24 = new Rect(val4.x, val23.yMax + 5f, val4.width, 30f);
			Widgets.HorizontalSlider(val24, ref g3, new FloatRange(0f, 1f), "Green", -1f);
			Rect val25 = new Rect(val4.x, val24.yMax + 5f, val4.width, 30f);
			Widgets.HorizontalSlider(val25, ref b3, new FloatRange(0f, 1f), "Blue", -1f);
			core.hairColor = new Color(r3, g3, b3);
			break;
		}
		case 3:
		{
			num4 = Enumerable.Count(DefDatabase<BeardDef>.AllDefsListForReading);
			Rect val5 = new Rect(0f, 0f, val4.width - 16f, (float)num4 * 32f);
			Vector2 val6 = default;
			Widgets.BeginScrollView(val4, ref scrollPosition_PropertiesAvailable, val5, true);
			num = 0f;
			for (int j = 0; j < num4; j++)
			{
				BeardDef beard = beardList[j];
				float num5 = val6.y - 32f;
				float num6 = val6.y + val5.height;
				if (num > num5 && num < num6)
				{
					DoBeardRow(new Rect(0f, num, val5.width - 4f, 32f), beard);
				}
				num += 32f;
			}
			Widgets.EndScrollView();
			Color beardColor = core.beardColor;
			float r = beardColor.r;
			float g = beardColor.g;
			float b = beardColor.b;
			Rect val7 = new Rect(val4.x, val4.yMax + 20f, val4.width, 30f);
			Widgets.HorizontalSlider(val7, ref r, new FloatRange(0f, 1f), "Red", -1f);
			Rect val8 = new Rect(val4.x, val7.yMax + 5f, val4.width, 30f);
			Widgets.HorizontalSlider(val8, ref g, new FloatRange(0f, 1f), "Green", -1f);
			Rect val9 = new Rect(val4.x, val8.yMax + 5f, val4.width, 30f);
			Widgets.HorizontalSlider(val9, ref b, new FloatRange(0f, 1f), "Blue", -1f);
			core.beardColor = new Color(r, g, b);
			break;
		}
		}
		Rect val26 = new Rect(rectDispMain.x + val.width + 90f, rectDispMain.height - 20f, 80f, 30f);
		if (Widgets.ButtonImageWithBG(val26, ContentFinder<Texture2D>.Get("Icon/Btn_ShapeUp", true), null))
		{
			core.bodyTypeName = Gen.ToStringSafe<BodyTypeDef>(selectedBodyType);
			core.headTypeName = Gen.ToStringSafe<HeadTypeDef>(selectedHeadType);
			core.hairName = Gen.ToStringSafe<HairDef>(selectedHair);
			core.beardName = Gen.ToStringSafe<BeardDef>(selectedBeard);
			core.animalEnable = false;
			core.bodyChangeEnable = true;
			((ITab)this).SelPawn.Drawer.renderer.SetAllGraphicsDirty();
		}
		Rect val27 = new Rect(rectDispMain.x + val.width + 190f, rectDispMain.height - 20f, 80f, 30f);
		if (Widgets.ButtonImageWithBG(val27, ContentFinder<Texture2D>.Get("Icon/Btn_ShapeDown", true), null))
		{
			core.bodyChangeEnable = false;
			((ITab)this).SelPawn.Drawer.renderer.SetAllGraphicsDirty();
		}
	}

	private void DoApparelRow(Rect rect, ThingDef apparel)
	{
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
		}
		Rect val = rect;
		val.width -= 5f;
		Rect val2 = rect;
		val2.xMax -= 4f;
		val2.xMin = val2.xMax - 35f;
		if (selectedApparel == apparel)
		{
			Widgets.DrawHighlightSelected(rect);
		}
		Text.Anchor = (TextAnchor)3;
		Rect val3 = new Rect(val.x + 4f + val.height, val.y, val.width - 4f - val.height, val.height);
		Widgets.Label(val3, GenText.Truncate(((Def)apparel).label, val3.width, (Dictionary<string, string>)null));
		GUI.contentColor = Color.white;
		Rect val4 = new Rect(val.x + 4f, val.y, val.height, val.height);
		Widgets.DefIcon(val4, (Def)(object)apparel, (ThingDef)null, 1f, (ThingStyleDef)null, false, Color.white, (Material)null, (int?)null);
		GenUI.ResetLabelAlign();
		if (Widgets.ButtonInvisible(rect, true))
		{
			SelectApparel(apparel);
			SoundStarter.PlayOneShotOnCamera(SoundDefOf.Click, (Map)null);
		}
	}

	private void DoDressRow(Rect rect, ThingDef dress, Color color)
	{
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
		}
		Rect val = rect;
		val.width -= 5f;
		Rect val2 = rect;
		val2.xMax -= 4f;
		val2.xMin = val2.xMax - 35f;
		if (selectedDress == dress)
		{
			Widgets.DrawHighlightSelected(rect);
		}
		Text.Anchor = (TextAnchor)3;
		Rect val3 = new Rect(val.x + 4f + val.height, val.y, val.width - 4f - val.height, val.height);
		Widgets.Label(val3, GenText.Truncate(((Def)dress).label, val3.width, (Dictionary<string, string>)null));
		GUI.contentColor = Color.white;
		Rect val4 = new Rect(val.x + 4f, val.y, val.height, val.height);
		Widgets.DefIcon(val4, (Def)(object)dress, (ThingDef)null, 1f, (ThingStyleDef)null, false, color, (Material)null, (int?)null);
		GenUI.ResetLabelAlign();
		if (Widgets.ButtonInvisible(rect, true))
		{
			SelectDress(dress);
			SoundStarter.PlayOneShotOnCamera(SoundDefOf.Click, (Map)null);
		}
	}

	private void DoBodyRow(Rect rect, BodyTypeDef body, DuC_Comp_DressUpCore core, Texture2D texture)
	{
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
		}
		Rect val = rect;
		val.width -= 5f;
		Rect val2 = rect;
		val2.xMax -= 4f;
		val2.xMin = val2.xMax - 35f;
		if (selectedBody == body)
		{
			Widgets.DrawHighlightSelected(rect);
		}
		Text.Anchor = (TextAnchor)3;
		Rect val3 = new Rect(val.x + 4f + val.height, val.y, val.width - 4f - val.height, val.height);
		Widgets.Label(val3, GenText.Truncate(Gen.ToStringSafe<BodyTypeDef>(body), val3.width, (Dictionary<string, string>)null));
		GUI.contentColor = Color.white;
		Rect val4 = new Rect(val.x + 4f, val.y, val.height, val.height);
		Widgets.DrawTextureFitted(val4, (Texture)(object)texture, 1.4f);
		GenUI.ResetLabelAlign();
		if (Widgets.ButtonInvisible(rect, true))
		{
			SelectBody(body, ref core);
			SoundStarter.PlayOneShotOnCamera(SoundDefOf.Click, (Map)null);
		}
	}

	private void DoElementAreaRow(Rect rect, BodyChangeElement element, ref bool enable)
	{
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
		}
		Rect val = rect;
		val.width -= 5f;
		Rect val2 = rect;
		val2.xMax -= 4f;
		val2.xMin = val2.xMax - 35f + 52f;
		if (selectedElement == (int)element)
		{
			Widgets.DrawHighlightSelected(rect);
		}
		Widgets.Checkbox(4f, rect.yMin + 16f, ref enable, 24f, false, false, (Texture2D)null, (Texture2D)null);
		Rect val3 = new Rect(30f, rect.yMin, 52f, 52f);
		Texture2D val4 = null;
		switch (element)
		{
		case BodyChangeElement.Body:
			val4 = ((selectedBodyType != null) ? ContentFinder<Texture2D>.Get(selectedBodyType.bodyNakedGraphicPath + "_south", false) : ContentFinder<Texture2D>.Get(((ITab)this).SelPawn.story.bodyType.bodyNakedGraphicPath + "_south", false));
			break;
		case BodyChangeElement.Head:
			val4 = ((selectedHeadType != null) ? ContentFinder<Texture2D>.Get(selectedHeadType.graphicPath + "_south", false) : ContentFinder<Texture2D>.Get(((ITab)this).SelPawn.story.headType.graphicPath + "_south", false));
			break;
		case BodyChangeElement.Hair:
		{
			string text2 = ((selectedHair != null) ? ((StyleItemDef)selectedHair).texPath : ((StyleItemDef)((ITab)this).SelPawn.story.hairDef).texPath);
			val4 = ((text2 == null || !(text2 != "")) ? null : ContentFinder<Texture2D>.Get(text2 + "_south", false));
			break;
		}
		case BodyChangeElement.Beard:
		{
			string text = ((selectedBeard != null) ? ((StyleItemDef)selectedBeard).texPath : ((StyleItemDef)((ITab)this).SelPawn.style.beardDef).texPath);
			val4 = ((text == null || !(text != "")) ? null : ContentFinder<Texture2D>.Get(text + "_south", false));
			break;
		}
		}
		if ((UnityEngine.Object)(object)val4 != (UnityEngine.Object)null)
		{
			Widgets.DrawTextureFitted(val3, (Texture)(object)val4, 1.5f);
		}
		Text.Anchor = (TextAnchor)3;
		Rect val5 = new Rect(val3.xMax + 4f, val.y, val.width - 4f - val.height, val.height);
		Widgets.Label(val5, GenText.Truncate(Gen.ToStringSafe<BodyChangeElement>(element), val5.width, (Dictionary<string, string>)null));
		GUI.contentColor = Color.white;
		GenUI.ResetLabelAlign();
		if (Widgets.ButtonInvisible(rect, true))
		{
			SelectElement(element);
			SoundStarter.PlayOneShotOnCamera(SoundDefOf.Click, (Map)null);
		}
	}

	private void DoBodyTypeRow(Rect rect, BodyTypeDef body)
	{
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
		}
		Rect val = rect;
		val.width -= 5f;
		if (selectedBodyType == body)
		{
			Widgets.DrawHighlightSelected(rect);
		}
		Text.Anchor = (TextAnchor)3;
		Rect val2 = new Rect(val.x + 4f + val.height, val.y, val.width - 4f - val.height, val.height);
		Widgets.Label(val2, GenText.Truncate(Gen.ToStringSafe<BodyTypeDef>(body), val2.width, (Dictionary<string, string>)null));
		GUI.contentColor = Color.white;
		Rect val3 = new Rect(val.x + 4f, val.y, val.height, val.height);
		Widgets.DrawTextureFitted(val3, (Texture)(object)ContentFinder<Texture2D>.Get(body.bodyNakedGraphicPath + "_south", true), 1f);
		GenUI.ResetLabelAlign();
		if (Widgets.ButtonInvisible(rect, true))
		{
			SelectBodyType(body);
			SoundStarter.PlayOneShotOnCamera(SoundDefOf.Click, (Map)null);
		}
	}

	private void DoHeadTypeRow(Rect rect, HeadTypeDef head)
	{
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
		}
		Rect val = rect;
		val.width -= 5f;
		if (selectedHeadType == head)
		{
			Widgets.DrawHighlightSelected(rect);
		}
		Text.Anchor = (TextAnchor)3;
		Rect val2 = new Rect(val.x + 4f + val.height, val.y, val.width - 4f - val.height, val.height);
		Widgets.Label(val2, GenText.Truncate(Gen.ToStringSafe<HeadTypeDef>(head), val2.width, (Dictionary<string, string>)null));
		GUI.contentColor = Color.white;
		Rect val3 = new Rect(val.x + 4f, val.y, val.height, val.height);
		Widgets.DrawTextureFitted(val3, (Texture)(object)ContentFinder<Texture2D>.Get(head.graphicPath + "_south", true), 1f);
		GenUI.ResetLabelAlign();
		if (Widgets.ButtonInvisible(rect, true))
		{
			SelectHeadType(head);
			SoundStarter.PlayOneShotOnCamera(SoundDefOf.Click, (Map)null);
		}
	}

	private void DoHairRow(Rect rect, HairDef hair)
	{
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
		}
		Rect val = rect;
		val.width -= 5f;
		if (selectedHair == hair)
		{
			Widgets.DrawHighlightSelected(rect);
		}
		Text.Anchor = (TextAnchor)3;
		Rect val2 = new Rect(val.x + 4f + val.height, val.y, val.width - 4f - val.height, val.height);
		Widgets.Label(val2, GenText.Truncate(Gen.ToStringSafe<string>(((Def)hair).label), val2.width, (Dictionary<string, string>)null));
		GUI.contentColor = Color.white;
		Rect val3 = new Rect(val.x + 4f, val.y, val.height, val.height);
		if (((StyleItemDef)hair).texPath != null && ((StyleItemDef)hair).texPath != "")
		{
			Texture2D val4 = ContentFinder<Texture2D>.Get(((StyleItemDef)hair).texPath + "_south", true);
			if ((UnityEngine.Object)(object)val4 != (UnityEngine.Object)null)
			{
				Widgets.DrawTextureFitted(val3, (Texture)(object)val4, 1f);
			}
		}
		GenUI.ResetLabelAlign();
		if (Widgets.ButtonInvisible(rect, true))
		{
			SelectHairType(hair);
			SoundStarter.PlayOneShotOnCamera(SoundDefOf.Click, (Map)null);
		}
	}

	private void DoBeardRow(Rect rect, BeardDef beard)
	{
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
		}
		Rect val = rect;
		val.width -= 5f;
		if (selectedBeard == beard)
		{
			Widgets.DrawHighlightSelected(rect);
		}
		Text.Anchor = (TextAnchor)3;
		Rect val2 = new Rect(val.x + 4f + val.height, val.y, val.width - 4f - val.height, val.height);
		Widgets.Label(val2, GenText.Truncate(Gen.ToStringSafe<string>(((Def)beard).label), val2.width, (Dictionary<string, string>)null));
		GUI.contentColor = Color.white;
		Rect val3 = new Rect(val.x + 4f, val.y, val.height, val.height);
		if (((StyleItemDef)beard).texPath != null && ((StyleItemDef)beard).texPath != "")
		{
			Texture2D val4 = ContentFinder<Texture2D>.Get(((StyleItemDef)beard).texPath + "_south", true);
			if ((UnityEngine.Object)(object)val4 != (UnityEngine.Object)null)
			{
				Widgets.DrawTextureFitted(val3, (Texture)(object)val4, 1f);
			}
		}
		GenUI.ResetLabelAlign();
		if (Widgets.ButtonInvisible(rect, true))
		{
			SelectBeardType(beard);
			SoundStarter.PlayOneShotOnCamera(SoundDefOf.Click, (Map)null);
		}
	}

	private void DoAnimalRow(Rect rect, PawnKindDef animal)
	{
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
		}
		Rect val = rect;
		val.width -= 5f;
		Rect val2 = rect;
		val2.xMax -= 4f;
		val2.xMin = val2.xMax - 35f;
		if (selectedAnimal == animal)
		{
			Widgets.DrawHighlightSelected(rect);
		}
		Text.Anchor = (TextAnchor)3;
		Rect val3 = new Rect(val.x + 4f, val.y, val.width - 4f, val.height);
		Widgets.Label(val3, GenText.Truncate(((Def)animal).label, val3.width, (Dictionary<string, string>)null));
		GUI.contentColor = Color.white;
		GenUI.ResetLabelAlign();
		if (Widgets.ButtonInvisible(rect, true))
		{
			SelectAnimal(animal);
			SoundStarter.PlayOneShotOnCamera(SoundDefOf.Click, (Map)null);
		}
	}

	private void SelectApparel(ThingDef apparel)
	{
		selectedApparel = apparel;
	}

	private void SelectDress(ThingDef dress)
	{
		selectedDress = dress;
	}

	private void SelectBody(BodyTypeDef body, ref DuC_Comp_DressUpCore core)
	{
		selectedBody = body;
		core.dresses.Where((StructDress d) => d.dressName == ((Def)selectedDress).defName).First().bodyName = ((Def)selectedBody).defName;
	}

	private void SelectElement(BodyChangeElement element)
	{
		selectedElement = (int)element;
	}

	private void SelectBodyType(BodyTypeDef body)
	{
		selectedBodyType = body;
	}

	private void SelectHeadType(HeadTypeDef head)
	{
		selectedHeadType = head;
	}

	private void SelectHairType(HairDef hair)
	{
		selectedHair = hair;
	}

	private void SelectBeardType(BeardDef beard)
	{
		selectedBeard = beard;
	}

	private void SelectAnimal(PawnKindDef animal)
	{
		selectedAnimal = animal;
		curStage = animal.lifeStages.Count - 1;
		curSize.x = Math.Min(animal.lifeStages[curStage].bodyGraphicData.drawSize.x, 2f);
		curSize.y = Math.Min(animal.lifeStages[curStage].bodyGraphicData.drawSize.y, 2f);
		curColor = animal.lifeStages[curStage].bodyGraphicData.color;
	}

	private void MoveElement<T>(List<T> list, int curIndex, int newIndex)
	{
		T item = list[curIndex];
		list.RemoveAt(curIndex);
		list.Insert(newIndex, item);
	}

	private static void drawSlider(string title, ref float value, ref Rect workRect, FloatRange range)
	{
		float num = 30f;
		Rect val = default;
		GenUI.SplitHorizontally(workRect, num, out val, out workRect);
		Widgets.HorizontalSlider(val, ref value, range, title, -1f);
	}

	static DuC_ITab_Pawn_DressUpColonist()
	{
	}
}
