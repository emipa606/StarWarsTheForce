using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace ProjectJedi;

[HarmonyPatch(typeof(AttackTargetsCache), nameof(AttackTargetsCache.Notify_FactionHostilityChanged))]
public static class AttackTargetsCache_Notify_FactionHostilityChanged
{
    public static void Postfix(AttackTargetsCache __instance, Faction f1, Faction f2, Map ___map)
    {
        var ghost = (PawnGhost)___map?.mapPawns.AllPawnsSpawned.FirstOrDefault(x => x is PawnGhost);
        ghost?.FactionSetup();
    }
}