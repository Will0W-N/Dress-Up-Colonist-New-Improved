using UnityEngine;
using Verse;

namespace DressUpColonist;

public class StructOffset : IExposable
{
	public string dressName;

	public Vector3 offset;

	public Vector3 scale;

	public float northLayerOffset;

	public float southLayerOffset;

	public float eastLayerOffset;

	public float westLayerOffset;

	public StructOffset()
	{
	}

	public StructOffset(string dress, Vector3 offset, Vector3 scale, float north, float south, float east, float west)
	{
		dressName = dress;
		this.offset = offset;
		this.scale = scale;
		northLayerOffset = north;
		southLayerOffset = south;
		eastLayerOffset = east;
		westLayerOffset = west;
	}

	public void ExposeData()
	{
		Scribe_Values.Look<string>(ref dressName, "DuCEx_StructOffset_DressName", "", false);
		Scribe_Values.Look<Vector3>(ref offset, "DuCEx_StructOffset_Offset", Vector3.zero, false);
		Scribe_Values.Look<Vector3>(ref scale, "DuCEx_StructOffset_Scale", Vector3.one, false);
		Scribe_Values.Look<float>(ref northLayerOffset, "DuCEx_StructOffset_NorthLayerOffset", 0f, false);
		Scribe_Values.Look<float>(ref southLayerOffset, "DuCEx_StructOffset_SouthLayerOffset", 0f, false);
		Scribe_Values.Look<float>(ref eastLayerOffset, "DuCEx_StructOffset_EastLayerOffset", 0f, false);
		Scribe_Values.Look<float>(ref westLayerOffset, "DuCEx_StructOffset_WestLayerOffset", 0f, false);
	}
}
