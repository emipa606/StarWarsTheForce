using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ProjectJedi
{
    public class PawnGhost : AbilityUser.PawnSummoned
    {
        public override void PostSummonSetup()
        {
            base.PostSummonSetup();
            if (Spawner?.Faction == Faction.OfPlayerSilentFail)
                FactionSetup();
            PowersSetup();
        }
        public void PowersSetup()
        {
            CompForceUser forcePowers = GetComp<CompForceUser>();
            if (forcePowers == null)
            {
                ThingComp thingComp = (ThingComp)Activator.CreateInstance(typeof(CompForceUser));
                thingComp.parent = this;
                var comps = AccessTools.Field(typeof(ThingWithComps), "comps").GetValue(this);
                if (comps != null)
                {
                    ((List<ThingComp>)comps).Add(thingComp);
                }
                thingComp.Initialize(null);
            }
            forcePowers = GetComp<CompForceUser>();
            if (forcePowers != null)
            {
                forcePowers.AlignmentValue = 0.99f;
                for (int o = 0; o < 10; o++)
                {
                    forcePowers.ForceUserLevel += 1;
                    forcePowers.ForceData.Skills.InRandomOrder().First((ForceSkill x) => x.level < 4).level++;
                    forcePowers.ForceData.AbilityPoints -= 1;
                }
                for (int i = 0; i < 8; i++)
                {
                    forcePowers.ForceUserLevel += 1;
                    forcePowers.LevelUpPower(forcePowers.ForceData.PowersLight.InRandomOrder().First((ForcePower x) => x.level < 2));
                    forcePowers.ForceData.AbilityPoints -= 1;
                }
            }
        }
                
        public void FactionSetup()
        {
            Faction ghostFaction = Faction;
            if (Faction?.def != FactionDef.Named("PJ_GhostFaction")) return;
            if (ghostFaction != null && ghostFaction != Faction.OfPlayerSilentFail)
            {
                foreach (Faction fac in Find.FactionManager.AllFactions)
                {
                    bool hostile = fac.HostileTo(Faction.OfPlayerSilentFail);
                    ghostFaction.TrySetRelationKind(fac, FactionRelationKind.Hostile);
                }
            }
        }
    }
}
