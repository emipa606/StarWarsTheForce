using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Verse;

namespace ProjectJedi;

[HarmonyPatch(typeof(Pawn), nameof(Pawn.GetGizmos))]
public static class Pawn_GetGizmos
{
    //Force Shield Gizmos Patch 2
    public static void Postfix(Pawn __instance, ref IEnumerable<Gizmo> __result)
    {
        if (__instance.health?.hediffSet?.hediffs is not { Count: > 0 })
        {
            return;
        }

        var shieldHediff =
            __instance.health.hediffSet.hediffs.FirstOrDefault(x =>
                x.TryGetComp<HediffComp_Shield>() != null);
        var shield = shieldHediff?.TryGetComp<HediffComp_Shield>();
        if (shield != null)
        {
            __result = __result.Concat(HarmonyPatching.GizmoGetter(shield));
        }
    }
}