using HarmonyLib;
using RimWorld;
using Verse;

namespace ProjectJedi;

[HarmonyPatch(typeof(SkillRecord), nameof(SkillRecord.Learn))]
public static class SkillRecord_Learn
{
    /// Add Force XP every time a pawn learns a skill.
    public static void Postfix(SkillRecord __instance, float xp, Pawn ___pawn, bool direct = false)
    {
        if (!(xp > 0) || ___pawn?.TryGetComp<CompForceUser>() is not { } compForce ||
            !(Find.TickManager.TicksGame > compForce.ForceData?.TicksUntilXPGain))
        {
            return;
        }

        var delay = (int)(130 * ModInfo.forceXPDelayFactor);
        if (__instance.def == SkillDefOf.Intellectual || __instance.def == SkillDefOf.Plants)
        {
            delay += (int)(50 * ModInfo.forceXPDelayFactor);
        }

        compForce.ForceData.TicksUntilXPGain = Find.TickManager.TicksGame + delay;
        compForce.ForceUserXP++;
    }
}