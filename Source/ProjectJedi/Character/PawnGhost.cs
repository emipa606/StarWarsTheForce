using System;
using System.Collections.Generic;
using System.Linq;
using AbilityUser;
using HarmonyLib;
using RimWorld;
using SWSaber;
using Verse;
using Verse.Sound;

namespace ProjectJedi;

public class PawnGhost : PawnSummoned
{
    public override void PostSummonSetup()
    {
        base.PostSummonSetup();
        if (Spawner?.Faction == Faction.OfPlayerSilentFail)
        {
            FactionSetup();
        }

        powersSetup();
        weaponSetup();
    }

    private void weaponSetup()
    {
        // Only equip a lightsaber if SW Sabers is on
        ///////////////////////////////////////////////////////////////////////////////////
        {
            try
            {
                ((Action)(() =>
                {
                    if (AccessTools.Method(typeof(Utility), nameof(Utility.CrystalSlotter)) == null)
                    {
                        return;
                    }

                    if (DefDatabase<ThingDef>.GetNamedSilentFail("SWSaber_Lightsaber") is not { } saberDef)
                    {
                        return;
                    }

                    //Equip a standard lightsaber
                    var saberToEquip =
                        (ThingWithComps)GenSpawn.Spawn(ThingMaker.MakeThing(saberDef, ThingDefOf.Steel),
                            Position, MapHeld);
                    saberToEquip.DeSpawn();
                    equipment.MakeRoomFor(saberToEquip);
                    equipment.AddEquipment(saberToEquip);
                    saberToEquip.def.soundInteract?.PlayOneShot(new TargetInfo(Position, Map));

                    //Equip a random 'good jedi' crystals
                    var lightsaberEffect = saberToEquip.TryGetComp<CompLightsaberActivatableEffect>();
                    var crystalSlot = saberToEquip.GetComp<CompCrystalSlotLoadable>();
                    var randomCrystals = new List<string>
                    {
                        "PJ_KyberCrystal",
                        "PJ_KyberCrystalBlue",
                        "PJ_KyberCrystalCyan"
                    };
                    var crystalThing =
                        (ThingWithComps)ThingMaker.MakeThing(ThingDef.Named(randomCrystals.RandomElement()));
                    Utility.CrystalSlotter(crystalSlot, lightsaberEffect, crystalThing);
                })).Invoke();
            }
#pragma warning disable 168
            catch (TypeLoadException ex)
            {
                /*////Log.Message(ex.ToString());*/
            }
#pragma warning restore 168
        }
    }

    private void powersSetup()
    {
        var forcePowers = GetComp<CompForceUser>();
        if (forcePowers == null)
        {
            var thingComp = (ThingComp)Activator.CreateInstance(typeof(CompForceUser));
            thingComp.parent = this;
            var comps = AccessTools.Field(typeof(ThingWithComps), "comps").GetValue(this);
            ((List<ThingComp>)comps)?.Add(thingComp);

            thingComp.Initialize(null);
        }

        forcePowers = GetComp<CompForceUser>();
        if (forcePowers == null)
        {
            return;
        }

        forcePowers.AlignmentValue = 0.99f;
        for (var o = 0; o < 10; o++)
        {
            forcePowers.ForceUserLevel += 1;
            forcePowers.ForceData.Skills.InRandomOrder().First(x => x.level < 4).level++;
            forcePowers.ForceData.AbilityPoints -= 1;
        }

        for (var i = 0; i < 8; i++)
        {
            forcePowers.ForceUserLevel += 1;
            forcePowers.LevelUpPower(forcePowers.ForceData.PowersLight.InRandomOrder().First(x => x.level < 2));
            forcePowers.ForceData.AbilityPoints -= 1;
        }
    }

    public void FactionSetup()
    {
        var ghostFaction = Faction;
        if (Faction?.def != FactionDef.Named("PJ_GhostFaction"))
        {
            return;
        }

        if (ghostFaction == null || ghostFaction == Faction.OfPlayerSilentFail)
        {
            return;
        }

        foreach (var fac in Find.FactionManager.AllFactions)
        {
            var unused = fac.HostileTo(Faction.OfPlayerSilentFail);
            ghostFaction.SetRelationDirect(fac, FactionRelationKind.Hostile);
        }
    }
}