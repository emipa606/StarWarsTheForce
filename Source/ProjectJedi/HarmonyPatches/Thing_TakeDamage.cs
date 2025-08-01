using HarmonyLib;
using Verse;

namespace ProjectJedi;

[HarmonyPatch(typeof(Thing), nameof(Thing.TakeDamage))]
public static class Thing_TakeDamage
{
    public static void Prefix(ref DamageInfo dinfo)
    {
        if (dinfo.Instigator is not Pawn attacker)
        {
            return;
        }

        if (!HarmonyPatching.IsSWSaber(dinfo.Weapon))
        {
            return;
        }

        var compForce = attacker.GetComp<CompForceUser>();
        if (compForce is not { IsForceUser: true })
        {
            dinfo.SetAmount(10);
        }
        else
        {
            var newDamage = (int)(dinfo.Amount / 2);

            var offensePoints = compForce.ForceSkillLevel("PJ_LightsaberOffense");
            if (offensePoints > 0)
            {
                for (var i = 0; i < offensePoints; i++)
                {
                    newDamage += (int)(dinfo.Amount / 5);
                }
            }

            dinfo.SetAmount(newDamage);
        }
    }
}