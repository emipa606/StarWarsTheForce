using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace ProjectJedi;

[StaticConstructorOnStartup]
public static class HarmonyPatching
{
    static HarmonyPatching()
    {
        new Harmony("rimworld.jecrell.jedi").PatchAll(Assembly.GetExecutingAssembly());
    }

    //Force Shield Gizmos Getter
    public static IEnumerable<Gizmo> GizmoGetter(HediffComp_Shield compHediffShield)
    {
        if (compHediffShield.GetWornGizmos() == null)
        {
            yield break;
        }

        using var enumerator = compHediffShield.GetWornGizmos().GetEnumerator();
        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;
            yield return current;
        }
    }

    public static bool IsSWSaber(ThingDef weaponDef)
    {
        return weaponDef is { IsMeleeWeapon: true } && weaponDef.defName.Contains("SWSaber_");
    }
}