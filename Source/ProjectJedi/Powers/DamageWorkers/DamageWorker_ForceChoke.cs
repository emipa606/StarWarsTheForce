using System.Collections.Generic;
using Verse;

namespace ProjectJedi;

public class DamageWorker_ForceChoke : DamageWorker_AddInjury
{
    protected override BodyPartRecord ChooseHitPart(DamageInfo dinfo, Pawn pawn)
    {
        BodyPartRecord rec = null;
        var neckRecord = pawn.def.race.body.AllParts.FirstOrDefault(x => x.def.label == "neck");

        var neckMissingPart = new List<Hediff_MissingPart>();
        pawn.health.hediffSet.GetHediffs(ref neckMissingPart, x => x.Part == neckRecord);
        if (neckMissingPart?.NullOrEmpty() == true)
        {
            rec = neckRecord;
        }
        else
        {
            var lungRecord = pawn.def.race.body.AllParts.FirstOrDefault(x =>
                x.def.tags.Any(s => s.defName is "BreathingSource" or "BreathingPathway"));

            var lungsMissingPart = new List<Hediff_MissingPart>();
            pawn.health.hediffSet.GetHediffs(ref lungsMissingPart, x => x.Part == lungRecord);
            if (lungsMissingPart.NullOrEmpty())
            {
                rec = lungRecord;
            }
        }

        return rec;
    }

    protected override void ApplySpecialEffectsToPart(Pawn pawn, float totalDamage, DamageInfo dinfo,
        DamageResult result)
    {
        FinalizeAndAddInjury(pawn, totalDamage, dinfo, result);
    }
}