using HarmonyLib;
using UnityEngine;
using Verse;

namespace ProjectJedi;

[HarmonyPatch(typeof(PawnRenderUtility), nameof(PawnRenderUtility.DrawEquipmentAndApparelExtras))]
public static class PawnRenderUtility_DrawEquipmentAndApparelExtras
{
    // Draw the Force Shield
    public static void Postfix
        (Pawn pawn, Vector3 drawPos, Rot4 facing, PawnRenderFlags flags)
    {
        if (pawn?.health?.hediffSet?.hediffs is not { Count: > 0 })
        {
            return;
        }

        var shieldHediff =
            pawn.health.hediffSet.hediffs.FirstOrDefault(x =>
                x.TryGetComp<HediffComp_Shield>() != null);
        var shield = shieldHediff?.TryGetComp<HediffComp_Shield>();
        shield?.DrawWornExtras();
    }
}