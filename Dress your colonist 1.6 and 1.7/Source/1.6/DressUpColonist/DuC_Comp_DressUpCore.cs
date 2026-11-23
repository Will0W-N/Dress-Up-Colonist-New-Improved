using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace DressUpColonist;

[StaticConstructorOnStartup]
public class DuC_Comp_DressUpCore : ThingComp
{
	public bool loadedExtend = false;

	public List<StructDress> dresses;

	public Apparel dummy = null;

	public bool dressUpEnable = false;

	public bool bodyChangeEnable = false;

	public bool animalEnable = false;

	public bool[] bodyTabEnableList = new bool[7];

	public string bodyTypeName = "";

	public Color bodyColor = Color.white;

	public bool bodyInvisible = false;

	public string headTypeName = "";

	public Vector3 headOffset = Vector3.zero;

	public bool headInvisible = false;

	public string hairName = "";

	public Color hairColor = Color.white;

	public string beardName = "";

	public Color beardColor = Color.white;

	public string animalKindDefName = "";

	public Vector2 animalSize = new Vector2(1f, 1f);

	public Color animalColor = Color.white;

	public int animalStage = 0;

	public bool reverseWest = false;

	public DuC_CompProperties_DressUpCore Props => (DuC_CompProperties_DressUpCore)(object)base.props;

	private Pawn pawn
	{
		get
		{
			ThingWithComps parent = base.parent;
			return (Pawn)(object)((parent is Pawn) ? parent : null);
		}
	}

	public override void PostSpawnSetup(bool respawningAfterLoad)
	{
		((ThingComp)this).PostSpawnSetup(respawningAfterLoad);
		foreach (ModContentPack item in LoadedModManager.RunningMods.Where((ModContentPack pack) => !pack.IsCoreMod))
		{
			if (item.Name == "DressUpColonistExtend")
			{
				loadedExtend = true;
			}
		}
	}

	public override void CompTick()
	{
		((ThingComp)this).CompTick();
		if (pawn.RaceProps.Humanlike && bodyTypeName == "")
		{
			bodyTypeName = Gen.ToStringSafe<BodyTypeDef>(pawn.story.bodyType);
			bodyColor = pawn.story.SkinColor;
			headTypeName = Gen.ToStringSafe<HeadTypeDef>(pawn.story.headType);
			hairName = Gen.ToStringSafe<HairDef>(pawn.story.hairDef);
			hairColor = pawn.story.HairColor;
			beardName = Gen.ToStringSafe<BeardDef>(pawn.style.beardDef);
			beardColor = pawn.story.HairColor;
		}
	}

	public override void PostExposeData()
	{
		((ThingComp)this).PostExposeData();
		if (pawn.RaceProps.Humanlike)
		{
			Scribe_Values.Look<bool>(ref dressUpEnable, "DuC_CoreComp_DressUpEnable", false, false);
			Scribe_Values.Look<bool>(ref bodyChangeEnable, "DuC_CoreComp_BodyChangeEnable", false, false);
			Scribe_Values.Look<bool>(ref animalEnable, "DuC_CoreComp_AnimalShapeShiftEnable", false, false);
			Scribe_Values.Look<string>(ref bodyTypeName, "DuC_CoreComp_BodyTypeName", "", false);
			Scribe_Values.Look<Color>(ref bodyColor, "DuC_CoreComp_BodyColor", Color.white, false);
			Scribe_Values.Look<bool>(ref bodyInvisible, "DuC_CoreComp_BodyInvisible", false, false);
			Scribe_Values.Look<string>(ref headTypeName, "DuC_CoreComp_HeadTypeName", "", false);
			Scribe_Values.Look<Vector3>(ref headOffset, "DuC_CoreComp_HeadOffset", Vector3.zero, false);
			Scribe_Values.Look<bool>(ref headInvisible, "DuC_CoreComp_HeadInvisible", false, false);
			Scribe_Values.Look<string>(ref hairName, "DuC_CoreComp_HairName", "", false);
			Scribe_Values.Look<Color>(ref hairColor, "DuC_CoreComp_HairColor", Color.white, false);
			Scribe_Values.Look<string>(ref beardName, "DuC_CoreComp_BeardName", "", false);
			Scribe_Values.Look<Color>(ref beardColor, "DuC_CoreComp_BeardColor", Color.white, false);
			Scribe_Deep.Look<Apparel>(ref dummy, "DuC_CoreComp_DummyApparel", Array.Empty<object>());
			Scribe_Collections.Look<StructDress>(ref dresses, "DuC_CoreComp_Dresses", (LookMode)0, Array.Empty<object>());
			Scribe_Values.Look<bool>(ref reverseWest, "DuC_CoreComp_ReverseWest", false, false);
			Scribe_Values.Look<string>(ref animalKindDefName, "DuC_CoreComp_AnimalKindDefName", "", false);
			Scribe_Values.Look<Vector2>(ref animalSize, "DuC_CoreComp_AnimalSize", new Vector2(1f, 1f), false);
			Scribe_Values.Look<Color>(ref animalColor, "DuC_CoreComp_AnimalColor", Color.white, false);
			Scribe_Values.Look<int>(ref animalStage, "DuC_CoreComp_AnimalStage", 0, false);
			Scribe_Values.Look<bool>(ref bodyTabEnableList[0], "DuC_CoreComp_BodyTabEnableList_Body", false, false);
			Scribe_Values.Look<bool>(ref bodyTabEnableList[1], "DuC_CoreComp_BodyTabEnableList_Head", false, false);
			Scribe_Values.Look<bool>(ref bodyTabEnableList[2], "DuC_CoreComp_BodyTabEnableList_Hair", false, false);
			Scribe_Values.Look<bool>(ref bodyTabEnableList[3], "DuC_CoreComp_BodyTabEnableList_Beard", false, false);
			Scribe_Values.Look<bool>(ref bodyTabEnableList[4], "DuC_CoreComp_BodyTabEnableList_BodyTatoo", false, false);
			Scribe_Values.Look<bool>(ref bodyTabEnableList[5], "DuC_CoreComp_BodyTabEnableList_HeadTatoo", false, false);
			Scribe_Values.Look<bool>(ref bodyTabEnableList[6], "DuC_CoreComp_BodyTabEnableList_Fur", false, false);
		}
	}

	public DuC_Comp_DressUpCore()
	{
	}
}
