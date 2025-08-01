using HarmonyLib;
using RimWorld;
using Verse;

namespace ProjectJedi;

[HarmonyPatch(typeof(Verb_MeleeAttack), "GetNonMissChance")]
public static class Verb_MeleeAttack_GetNonMissChance
{
    public static void Postfix(Verb_MeleeAttack __instance, ref float __result)
    {
        if (__instance.Caster is not Pawn attacker)
        {
            return;
        }

        var weapon = __instance.EquipmentSource;
        if (weapon == null || !HarmonyPatching.IsSWSaber(weapon.def))
        {
            return;
        }

        var compForce = attacker.GetComp<CompForceUser>();
        if (compForce is not { IsForceUser: true })
        {
            __result = 0.5f;
        }
        else
        {
            var newAccuracy = __result / 2;

            var accuracyPoints = compForce.ForceSkillLevel("PJ_LightsaberAccuracy");
            if (accuracyPoints > 0)
            {
                for (var i = 0; i < accuracyPoints; i++)
                {
                    newAccuracy += 0.2f;
                }
            }

            __result = newAccuracy;
        }
    }
}