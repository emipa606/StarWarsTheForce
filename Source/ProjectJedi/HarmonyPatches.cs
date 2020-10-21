﻿using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using UnityEngine;

namespace ProjectJedi
{
    [StaticConstructorOnStartup]
    static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            var harmony = new Harmony("rimworld.jecrell.jedi");
            harmony.Patch(AccessTools.Method(typeof(Thing), nameof(Thing.TakeDamage)),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(TakeDamage_PreFix)), null);
            harmony.Patch(AccessTools.Method(typeof(Verb_MeleeAttack), "GetNonMissChance"), null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(GetNonMissChance_PostFix)));
            harmony.Patch(AccessTools.Method(typeof(Pawn_HealthTracker), nameof(Pawn_HealthTracker.PreApplyDamage)),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(PreApplyDamage_PreFix)), null);
            harmony.Patch(AccessTools.Method(typeof(PawnRenderer), "DrawEquipment"), null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(DrawEquipment_PostFix)));
            harmony.Patch(AccessTools.Method(typeof(Pawn), nameof(Pawn.GetGizmos)), null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(GetGizmos_PostFix)));
            harmony.Patch(AccessTools.Method(typeof(SkillRecord), nameof(SkillRecord.Learn)), null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(Learn_PostFix)));
            harmony.Patch(AccessTools.Method(typeof(StorytellerUtility), nameof(StorytellerUtility.DefaultParmsNow)), null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(DefaultParmsNow_PostFix)));
            harmony.Patch(AccessTools.Method(typeof(AttackTargetsCache), nameof(AttackTargetsCache.Notify_FactionHostilityChanged)), null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(Notify_FactionHostilityChanged_PostFix)));
        }

        // RimWorld.StorytellerUtility
        public static void DefaultParmsNow_PostFix(ref IncidentParms __result, IncidentCategoryDef incCat, IIncidentTarget target)
        {
            if (!(target is Map map)) return;
            if (!(__result.points > 0)) return;
            try
            {
                List<Pawn> forceUsers = map.mapPawns.FreeColonistsSpawned.ToList()
                    .FindAll(p => p.GetComp<CompForceUser>() != null);
                if (forceUsers == null) return;
                foreach (Pawn pawn in forceUsers)
                {
                    CompForceUser compForce = pawn.GetComp<CompForceUser>();
                    if (compForce.ForceUserLevel > 0)
                    {
                        __result.points += (5 * compForce.ForceUserLevel);
                    }
                }
            }
            catch (NullReferenceException)
            {
            }
        }

        /// Add Force XP every time a pawn learns a skill.
        public static void Learn_PostFix(SkillRecord __instance, float xp, bool direct = false)
        {
            Pawn pawn = (Pawn) AccessTools.Field(typeof(SkillRecord), "pawn").GetValue(__instance);
            if (xp > 0 && pawn?.TryGetComp<CompForceUser>() is CompForceUser compForce &&
                Find.TickManager.TicksGame > compForce?.ForceData?.TicksUntilXPGain)
            {
                int delay = (int)(130 * ModInfo.forceXPDelayFactor);
                if (__instance.def == SkillDefOf.Intellectual || __instance.def == SkillDefOf.Plants) delay += (int)(50 * ModInfo.forceXPDelayFactor);
                compForce.ForceData.TicksUntilXPGain = Find.TickManager.TicksGame + delay;
                compForce.ForceUserXP++;
            }
        }

        public static IEnumerable<Gizmo> GizmoGetter(HediffComp_Shield compHediffShield)
        {
            if (compHediffShield.GetWornGizmos() != null)
            {
                IEnumerator<Gizmo> enumerator = compHediffShield.GetWornGizmos().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Gizmo current = enumerator.Current;
                    yield return current;
                }
            }
        }

        public static void GetGizmos_PostFix(Pawn __instance, ref IEnumerable<Gizmo> __result)
        {
            Pawn pawn = __instance;
            if (pawn.health?.hediffSet?.hediffs != null && pawn.health.hediffSet.hediffs.Count > 0)
            {
                Hediff shieldHediff =
                    pawn.health.hediffSet.hediffs.FirstOrDefault((Hediff x) =>
                        x.TryGetComp<HediffComp_Shield>() != null);
                HediffComp_Shield shield = shieldHediff?.TryGetComp<HediffComp_Shield>();
                if (shield != null)
                {
                    __result = __result.Concat(GizmoGetter(shield));
                }
            }
        }

        // Verse.PawnRenderer
        public static void DrawEquipment_PostFix(PawnRenderer __instance, Vector3 rootLoc)
        {
            Pawn pawn = (Pawn) AccessTools.Field(typeof(PawnRenderer), "pawn").GetValue(__instance);
            if (pawn.health?.hediffSet?.hediffs != null && pawn.health.hediffSet.hediffs.Count > 0)
            {
                Hediff shieldHediff =
                    pawn.health.hediffSet.hediffs.FirstOrDefault((Hediff x) =>
                        x.TryGetComp<HediffComp_Shield>() != null);
                HediffComp_Shield shield = shieldHediff?.TryGetComp<HediffComp_Shield>();
                shield?.DrawWornExtras();
            }
        }


        // Verse.Pawn_HealthTracker
        public static bool PreApplyDamage_PreFix(Pawn_HealthTracker __instance, DamageInfo dinfo, out bool absorbed)
        {
            Pawn pawn = (Pawn) AccessTools.Field(typeof(Pawn_HealthTracker), "pawn").GetValue(__instance);
            if (pawn != null)
            {
                if (pawn.health.hediffSet.hediffs != null && pawn.health.hediffSet.hediffs.Count > 0)
                {
                    Hediff shieldHediff =
                        pawn.health.hediffSet.hediffs.FirstOrDefault((Hediff x) =>
                            x.TryGetComp<HediffComp_Shield>() != null);
                    if (shieldHediff != null)
                    {
                        HediffComp_Shield shield = shieldHediff.TryGetComp<HediffComp_Shield>();
                        if (shield != null)
                        {
                            if (shield.CheckPreAbsorbDamage(dinfo))
                            {
                                absorbed = true;
                                return false;
                            }
                        }
                    }
                }
            }
            absorbed = false;
            return true;
        }

        public static int nonForceUserLightsaberDamage = 10;

        public static void GetNonMissChance_PostFix(Verb_MeleeAttack __instance, ref float __result)
        {
            if (!(__instance.Caster is Pawn attacker)) return;
            ThingWithComps weapon = __instance.EquipmentSource;
            if (weapon == null || !IsSWSaber(weapon.def)) return;
            CompForceUser compForce = attacker.GetComp<CompForceUser>();
            if (compForce == null || !compForce.IsForceUser)
            {
                __result = 0.5f;
            }
            else
            {
                float newAccuracy = (float) (__result / 2);

                int accuracyPoints = compForce.ForceSkillLevel("PJ_LightsaberAccuracy");
                if (accuracyPoints > 0)
                {
                    for (int i = 0; i < accuracyPoints; i++)
                    {
                        newAccuracy += 0.2f;
                    }
                }
                __result = newAccuracy;
            }
        }

        public static void TakeDamage_PreFix(ref DamageInfo dinfo)
        {
            if (!(dinfo.Instigator is Pawn attacker)) return;
            if (!IsSWSaber(dinfo.Weapon)) return;
            CompForceUser compForce = attacker.GetComp<CompForceUser>();
            if (compForce == null || !compForce.IsForceUser)
            {
                dinfo.SetAmount(10);
            }
            else
            {
                int newDamage = (int) (dinfo.Amount / 2);

                int offensePoints = compForce.ForceSkillLevel("PJ_LightsaberOffense");
                if (offensePoints > 0)
                {
                    for (int i = 0; i < offensePoints; i++)
                    {
                        newDamage += (int) (dinfo.Amount / 5);
                    }
                }
                dinfo.SetAmount(newDamage);
            }
        }

        private static bool IsSWSaber(ThingDef weaponDef)
        {
            return weaponDef != null && weaponDef.IsMeleeWeapon && weaponDef.defName.Contains("SWSaber_");
        }

        public static void Notify_FactionHostilityChanged_PostFix(AttackTargetsCache __instance, Faction f1, Faction f2)
        {
            Map map = (Map) AccessTools.Field(typeof(AttackTargetsCache), "map").GetValue(__instance);
            PawnGhost ghost = (PawnGhost) map?.mapPawns.AllPawnsSpawned.FirstOrDefault((Pawn x) => x is PawnGhost);
            ghost?.FactionSetup();
        }
    }
}