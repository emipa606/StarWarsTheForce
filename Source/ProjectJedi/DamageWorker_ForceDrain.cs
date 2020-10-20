﻿using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;

namespace ProjectJedi
{
    public class DamageWorker_ForceDrain : DamageWorker
    {
        public override DamageResult Apply(DamageInfo dinfo, Thing thing)
        {
            DamageResult result = new DamageResult
            {
                totalDamageDealt = 0f
            };
            if (thing is PawnGhost)
            {
                Messages.Message("PJ_ForceGhostResisted".Translate(), MessageTypeDefOf.NegativeEvent);
                return result;
            }

            if (thing is Pawn pawn)
            {
                if (dinfo.Instigator != null)
                {
                    if (dinfo.Instigator is Pawn caster)
                    {
                        CompForceUser victimForce = pawn.GetComp<CompForceUser>();
                        int maxInjuries = 2;
                        int maxHeals = 0;
                        int maxPoolDamage = 30;


                        if (victimForce != null)
                        {
                            if (victimForce.IsForceUser)
                            {
                                Need_ForcePool victimForcePool = victimForce.ForcePool;
                                if (victimForcePool != null)
                                {
                                    if (victimForcePool.CurLevel > 0.1f)
                                    {
                                        //Turn 0.01f into 1, or 1.0 into 100.
                                        int victimForceInt = System.Convert.ToInt32(victimForcePool.CurLevel * 100);
                                        //Log.Message("Victim Force Pool = " + victimForceInt.ToString());
                                        Need_ForcePool casterPool = caster.needs.TryGetNeed<Need_ForcePool>();
                                        if (casterPool != null)
                                        {
                                            Messages.Message("PJ_ForceDrainOne".Translate(new object[]
                                                {
                                                    caster.Label,
                                                    pawn.Label
                                                }), MessageTypeDefOf.SilentInput);
                                            for (int i = 0; i < Mathf.Min(victimForceInt, maxPoolDamage); i++)
                                            {
                                                if (casterPool.CurLevel >= 0.99f) break;
                                                casterPool.CurLevel += 0.01f;
                                                victimForcePool.CurLevel -= 0.05f;
                                            }
                                            return result;
                                        }
                                    }
                                }
                            }
                        }

                        Messages.Message("PJ_ForceDrainTwo".Translate(new object[]
                            {
                               caster.Label,
                               pawn.Label
                            }), MessageTypeDefOf.SilentInput);

                        foreach (BodyPartRecord rec in pawn.health.hediffSet.GetNotMissingParts().InRandomOrder())
                        {
                            if (maxInjuries > 0)
                            {
                                pawn.TakeDamage(new DamageInfo(DamageDefOf.Burn, new IntRange(5, 10).RandomInRange, 1f, -1, caster, rec));
                                maxInjuries--;
                                maxHeals++;
                            }
                        }

                        int maxInjuriesPerBodypart;
                        foreach (BodyPartRecord rec in caster.health.hediffSet.GetInjuredParts())
                        {
                            if (maxHeals > 0)
                            {
                                maxInjuriesPerBodypart = 2;
                                foreach (Hediff_Injury current in from injury in caster.health.hediffSet.GetHediffs<Hediff_Injury>() where injury.Part == rec select injury)
                                {
                                    if (maxInjuriesPerBodypart > 0)
                                    {
                                        if (current.CanHealNaturally() && !current.IsPermanent()) // basically check for scars and old wounds
                                        {
                                            current.Heal((int)current.Severity + 1);
                                            maxHeals--;
                                            maxInjuriesPerBodypart--;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }

            return result;

        }
    }
}