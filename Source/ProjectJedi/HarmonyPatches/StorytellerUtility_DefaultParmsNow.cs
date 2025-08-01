using System;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace ProjectJedi;

[HarmonyPatch(typeof(StorytellerUtility), nameof(StorytellerUtility.DefaultParmsNow))]
public static class StorytellerUtility_DefaultParmsNow
{
    // RimWorld.StorytellerUtility
    public static void Postfix(ref IncidentParms __result, IncidentCategoryDef incCat,
        IIncidentTarget target)
    {
        if (target is not Map map)
        {
            return;
        }

        if (!(__result.points > 0))
        {
            return;
        }

        try
        {
            var forceUsers = map.mapPawns.FreeColonistsSpawned.ToList()
                .FindAll(p => p.GetComp<CompForceUser>() != null);

            foreach (var pawn in forceUsers)
            {
                var compForce = pawn.GetComp<CompForceUser>();
                if (compForce.ForceUserLevel > 0)
                {
                    __result.points += 5 * compForce.ForceUserLevel;
                }
            }
        }
        catch (NullReferenceException)
        {
        }
    }
}