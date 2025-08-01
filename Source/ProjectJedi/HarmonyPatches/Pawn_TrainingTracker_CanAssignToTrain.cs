using HarmonyLib;
using RimWorld;
using Verse;

namespace ProjectJedi;

[HarmonyPatch(typeof(Pawn_TrainingTracker), nameof(Pawn_TrainingTracker.CanAssignToTrain),
    [typeof(TrainableDef), typeof(ThingDef), typeof(bool), typeof(Pawn)],
    [ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Normal])]
public static class Pawn_TrainingTracker_CanAssignToTrain
{
    public static bool Prefix(ThingDef pawnDef, ref AcceptanceReport __result)
    {
        if (pawnDef.defName != "PJ_ForceGhostR")
        {
            return true;
        }

        __result = false;
        return false;
    }
}