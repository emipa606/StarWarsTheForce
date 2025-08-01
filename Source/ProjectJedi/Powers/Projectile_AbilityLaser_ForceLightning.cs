using AbilityUser;
using RimWorld;
using Verse;

namespace ProjectJedi;

public class Projectile_AbilityLaser_ForceLightning : Projectile_AbilityLaser
{
    public override void Impact_Override(Thing hitThing)
    {
        base.Impact_Override(hitThing);
        if (hitThing == null)
        {
            return;
        }

        //Throw effects along the way
        var path = Map.pathFinder.FindPathNow(Caster.PositionHeld, hitThing.PositionHeld,
            TraverseParms.For(
                TraverseMode.PassAllDestroyableThings, canBashDoors: true,
                canBashFences: true
            ));
        if (path == null)
        {
            return;
        }

        while (path.NodesLeftCount > 0)
        {
            var nodeCurrent = path.ConsumeNextNode();

            FleckMaker.ThrowSmoke(nodeCurrent.ToVector3(), hitThing.MapHeld, new FloatRange(0.05f, 0.5f).RandomInRange);
            if (new FloatRange(0f, 1f).RandomInRange > 0.2f)
            {
                FleckMaker.ThrowMicroSparks(nodeCurrent.ToVector3(), hitThing.MapHeld);
            }

            FleckMaker.ThrowLightningGlow(nodeCurrent.ToVector3(), hitThing.MapHeld,
                new FloatRange(1f, 1.5f).RandomInRange);
        }

        //Effect at end location
        var loc = hitThing.Position.ToVector3Shifted();
        for (var i = 0; i < 4; i++)
        {
            FleckMaker.ThrowSmoke(loc, hitThing.MapHeld, 2.5f);
            FleckMaker.ThrowMicroSparks(loc, hitThing.MapHeld);
            FleckMaker.ThrowLightningGlow(loc, hitThing.MapHeld, 2.5f);
        }
    }
}