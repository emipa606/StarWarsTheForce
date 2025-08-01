using System.Collections.Generic;
using System.Diagnostics;
using RimWorld;
using Verse;
using Verse.AI;

namespace ProjectJedi;

public class JobDriver_ForceMeditation : JobDriver
{
    private Rot4 faceDir;
    private int totalTicks;

    public override bool TryMakePreToilReservations(bool somethin)
    {
        return true;
    }

    [DebuggerHidden]
    protected override IEnumerable<Toil> MakeNewToils()
    {
        yield return Toils_Reserve.Reserve(TargetIndex.A);
        yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
        yield return new Toil
        {
            initAction = delegate { faceDir = !job.def.faceDir.IsValid ? Rot4.Random : job.def.faceDir; },
            tickIntervalAction = delegate(int delta)
            {
                totalTicks += delta;
                pawn.rotationTracker.FaceCell(pawn.Position + faceDir.FacingCell);
                pawn.GainComfortFromCellIfPossible(delta);
                if (pawn.TryGetComp<CompForceUser>() == null)
                {
                    return;
                }

                var forceComp = pawn.GetComp<CompForceUser>();
                // Check total ticks, add xp for each 60 ticks and remove those from totalTicks
                if (totalTicks > 60)
                {
                    var xpToGain = totalTicks / 60;
                    totalTicks -= xpToGain * 60;
                    forceComp.ForceUserXP += xpToGain;
                }

                var poolForce = pawn.needs.TryGetNeed<Need_ForcePool>();
                if (poolForce == null)
                {
                    return;
                }

                if (poolForce.CurLevel < 0.99f)
                {
                    poolForce.CurLevel += 0.0005f * delta;
                }
                else
                {
                    EndJobWith(JobCondition.Succeeded);
                }
            },
            defaultCompleteMode = ToilCompleteMode.Delay,
            defaultDuration = 1800
        };
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref faceDir, "faceDir");
    }
}