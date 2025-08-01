using HarmonyLib;
using Verse;

namespace ProjectJedi;

[HarmonyPatch(typeof(Pawn_HealthTracker), nameof(Pawn_HealthTracker.PreApplyDamage))]
public static class Pawn_HealthTracker_PreApplyDamage
{
    // Use the Force Shield to prevent damage
    public static bool Prefix(Pawn_HealthTracker __instance, DamageInfo dinfo, out bool absorbed)
    {
        var pawn = (Pawn)AccessTools.Field(typeof(Pawn_HealthTracker), "pawn").GetValue(__instance);
        if (pawn?.health.hediffSet.hediffs is { Count: > 0 })
        {
            var shieldHediff =
                pawn.health.hediffSet.hediffs.FirstOrDefault(x =>
                    x.TryGetComp<HediffComp_Shield>() != null);
            var shield = shieldHediff?.TryGetComp<HediffComp_Shield>();
            if (shield != null)
            {
                if (shield.CheckPreAbsorbDamage(dinfo))
                {
                    absorbed = true;
                    return false;
                }
            }
        }

        absorbed = false;
        return true;
    }
}