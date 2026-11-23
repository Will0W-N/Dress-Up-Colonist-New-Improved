using UnityEngine;
using Verse;

namespace DressUpColonist;

public class StructDress : IExposable
{
	public string dressName;

	public Color color;

	public string bodyName;

	public StructDress()
	{
	}

	public StructDress(string dress, Color color, string body)
	{
		dressName = dress;
		this.color = color;
		bodyName = body;
	}

	public void ExposeData()
	{
		Scribe_Values.Look<string>(ref dressName, "DuC_StructDress_DressName", "", false);
		Scribe_Values.Look<Color>(ref color, "DuC_StructDress_Color", Color.white, false);
		Scribe_Values.Look<string>(ref bodyName, "DuC_StructDress_BodyName", "", false);
	}
}
