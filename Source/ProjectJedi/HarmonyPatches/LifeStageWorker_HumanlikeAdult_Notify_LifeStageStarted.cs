using HarmonyLib;
using RimWorld;
using Verse;

namespace ProjectJedi;

[HarmonyPatch(typeof(LifeStageWorker_HumanlikeAdult),
    nameof(LifeStageWorker_HumanlikeAdult.Notify_LifeStageStarted))]
public static class LifeStageWorker_HumanlikeAdult_Notify_LifeStageStarted
{
    /// <summary>
    ///     Removes lifestage start functions from force ghosts
    /// </summary>
    /// <param name="pawn"></param>
    // RimWorld.LifeStageWorker_HumanlikeAdult
    public static bool Prefix(Pawn pawn)
    {
        return pawn.def.defName != "PJ_ForceGhostR";
    }
}