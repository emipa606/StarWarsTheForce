using System.Text;
using AbilityUser;
using RimWorld;
using UnityEngine;
using Verse;

namespace ProjectJedi;

public sealed class ForceAbility : PawnAbility
{
    public ForceAbility()
    {
    }

    public ForceAbility(CompAbilityUser AbilityUser) : base(AbilityUser)
    {
        this.AbilityUser = AbilityUser as CompForceUser;
    }


    public ForceAbility(AbilityData abilityData) : base(abilityData)
    {
        AbilityUser =
            abilityData.Pawn.AllComps.FirstOrDefault(x => x.GetType() == abilityData.AbilityClass) as CompForceUser;
    }

    private CompForceUser ForceUser => ForceUtility.GetForceUser(Pawn);
    private ForceAbilityDef ForceDef => Def as ForceAbilityDef;

    private float ActualForceCost => ForceDef.forcePoolCost -
                                     (ForceDef.forcePoolCost * (0.15f * ForceUser.ForceSkillLevel("PJ_ForcePool")));

    public override void PostAbilityAttempt()
    {
        //Log.Message("ForceAbility :: PostAbilityAttempt Called");
        base.PostAbilityAttempt();
        if (ForceDef?.changedAlignmentType != ForceAlignmentType.None)
        {
            if (ForceDef != null)
            {
                ForceUser.AlignmentValue += ForceDef.changedAlignmentRate;
            }

            ForceUser.UpdateAlignment();
        }

        if (Pawn.needs.TryGetNeed<Need_ForcePool>() is { } fp)
        {
            fp.UseForcePower(ActualForceCost);
        }
    }

    /// <summary>
    ///     Shows the required alignment (optional),
    ///     alignment change (optional),
    ///     and the force pool usage
    /// </summary>
    /// <returns></returns>
    public override string PostAbilityVerbCompDesc(VerbProperties_Ability verbDef)
    {
        var result = "";
        if (verbDef is not { abilityDef: ForceAbilityDef forceDef })
        {
            return result;
        }

        var postDesc = new StringBuilder();
        var alignDesc = "";
        var changeDesc = "";

        if (forceDef.changedAlignmentType != ForceAlignmentType.None)
        {
            alignDesc = "ForceAbilityDescAlign".Translate(forceDef.requiredAlignmentType.ToString());
        }

        if (forceDef.changedAlignmentType != ForceAlignmentType.None)
        {
            changeDesc = "ForceAbilityDescChange".Translate(forceDef.changedAlignmentType.ToString(),
                Mathf.Abs(forceDef.changedAlignmentRate).ToString("0.##"));
        }

        string pointsDesc;

        if (ForceUser?.ForceSkillLevel("PJ_ForcePool") > 0)
        {
            var poolCost = forceDef.forcePoolCost -
                           (forceDef.forcePoolCost * (0.15f * ForceUser.ForceSkillLevel("PJ_ForcePool")));
            pointsDesc =
                "ForceAbilityDescOriginPoints".Translate(Mathf.Abs(forceDef.forcePoolCost).ToString("0.##"))
                + "\n" +
                "ForceAbilityDescNewPoints".Translate(poolCost.ToString("0.##"))
                ;
        }
        else
        {
            pointsDesc = "ForceAbilityDescPoints".Translate(Mathf.Abs(forceDef.forcePoolCost).ToString("0.##"));
        }

        if (alignDesc != "")
        {
            postDesc.AppendLine(alignDesc);
        }

        if (changeDesc != "")
        {
            postDesc.AppendLine(changeDesc);
        }

        if (pointsDesc != "")
        {
            postDesc.AppendLine(pointsDesc);
        }

        result = postDesc.ToString();

        return result;
    }

    public override bool CanCastPowerCheck(AbilityContext context, out string reason)
    {
        if (!base.CanCastPowerCheck(context, out reason))
        {
            return false;
        }

        reason = "";
        if (Def is not ForceAbilityDef forceDef)
        {
            return true;
        }

        if (forceDef.requiredAlignmentType != ForceAlignmentType.None)
        {
            if (forceDef.requiredAlignmentType != ForceUtility.GetForceAlignmentType(Pawn))
            {
                reason = "PJ_WrongAlignment".Translate(Pawn.LabelShort);
                return false;
            }
        }

        if (ForceUser?.ForcePool != null)
        {
            if (forceDef.forcePoolCost > 0 &&
                ActualForceCost > Pawn.GetForcePool()?.CurLevel)
            {
                reason = "PJ_DrainedForcePool".Translate(Pawn.LabelShort);
                return false;
            }
        }

        if (AbilityUser?.Pawn?.apparel?.WornApparel == null ||
            !(AbilityUser?.Pawn?.apparel?.WornApparelCount > 0))
        {
            return true;
        }

        if (AbilityUser?.Pawn?.apparel?.WornApparel?.FirstOrDefault(x =>
                x.def == ThingDefOf.Apparel_ShieldBelt) == null)
        {
            return true;
        }

        reason = "PJ_UsingShieldBelt".Translate(Pawn.LabelShort);
        return false;
    }

    public override bool CanOverpowerTarget(AbilityContext context, LocalTargetInfo target, out string reason)
    {
        reason = "";
        if (target.Thing is not PawnGhost)
        {
            return base.CanOverpowerTarget(context, target, out reason);
        }

        Messages.Message(
            "PJ_ForceResisted".Translate(target.Thing.LabelShort, AbilityUser.Pawn.LabelShort, Def.label),
            MessageTypeDefOf.NegativeEvent);
        return false;
    }
}