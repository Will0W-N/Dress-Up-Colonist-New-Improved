using System.Reflection;
using HarmonyLib;
using Verse;

namespace DressUpColonist;

[StaticConstructorOnStartup]
internal class Main
{
	static Main()
	{
		Harmony val = new Harmony("DressUpColonist.HarmonyPatch");
		val.PatchAll(Assembly.GetExecutingAssembly());
	}
}
