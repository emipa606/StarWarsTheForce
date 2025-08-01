using Verse;

namespace ProjectJedi;

internal static class ForceUtility
{
    public static bool IsForceUser(this Pawn p)
    {
        switch (p)
        {
            case null:
                return false;
            case PawnGhost:
                return true;
        }

        if (p.story?.traits is not { } t)
        {
            return false;
        }

        return t.HasTrait(ProjectJediDefOf.PJ_JediTrait) ||
               t.HasTrait(ProjectJediDefOf.PJ_SithTrait) ||
               t.HasTrait(ProjectJediDefOf.PJ_GrayTrait) ||
               t.HasTrait(ProjectJediDefOf.PJ_ForceSensitive);
    }

    public static ForceAlignmentType GetForceAlignmentType(Pawn pawn)
    {
        if (pawn?.GetComp<CompForceUser>() is { } forceUser)
        {
            return forceUser.ForceAlignmentType;
        }

        return ForceAlignmentType.None;
    }

    public static Need_ForcePool GetForcePool(this Pawn pawn)
    {
        if (pawn?.GetComp<CompForceUser>() is { } forceUser)
        {
            return forceUser.ForcePool;
        }

        return null;
    }

    public static CompForceUser GetForceUser(Pawn pawn)
    {
        if (pawn?.GetComp<CompForceUser>() is { } forceUser)
        {
            return forceUser;
        }

        return null;
    }
}