using RimWorld;
using Verse;
using Verse.AI;

namespace ProjectJedi;

public class ThinkNode_ModNeedPercentageAbove : ThinkNode_Conditional
{
    private NeedDef need;

    private float threshold;

    public override ThinkNode DeepCopy(bool resolve = true)
    {
        var thinkNodeModNeedPercentageAbove = (ThinkNode_ModNeedPercentageAbove)base.DeepCopy(resolve);
        thinkNodeModNeedPercentageAbove.need = need;
        thinkNodeModNeedPercentageAbove.threshold = threshold;
        return thinkNodeModNeedPercentageAbove;
    }

    protected override bool Satisfied(Pawn pawn)
    {
        if (need == null)
        {
            return false;
        }

        if (pawn?.needs?.TryGetNeed(need) == null)
        {
            return false;
        }

        return pawn.needs.TryGetNeed(need).CurLevelPercentage > threshold;
    }
}