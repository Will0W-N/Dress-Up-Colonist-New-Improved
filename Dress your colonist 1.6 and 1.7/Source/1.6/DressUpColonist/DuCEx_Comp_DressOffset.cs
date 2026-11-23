using System;
using System.Collections.Generic;
using Verse;

namespace DressUpColonist;

[StaticConstructorOnStartup]
public class DuCEx_Comp_DressOffset : ThingComp
{
	public List<StructOffset> offsets;

	public DuCEx_CompProperties_DressOffset Props => (DuCEx_CompProperties_DressOffset)(object)base.props;

	public override void PostExposeData()
	{
		((ThingComp)this).PostExposeData();
		Scribe_Collections.Look<StructOffset>(ref offsets, "DuCEx_ExtendComp_Offsets", (LookMode)0, Array.Empty<object>());
	}
}
