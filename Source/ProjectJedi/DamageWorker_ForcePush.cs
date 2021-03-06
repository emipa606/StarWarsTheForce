﻿using RimWorld;
using Verse;
using UnityEngine;

namespace ProjectJedi
{
    public class DamageWorker_ForcePush : DamageWorker_ForceLeveled
    {
        public Vector3 PushResult(Thing thingToPush, int pushDist, out bool collision)
        {
            Vector3 origin = thingToPush.TrueCenter();
            Vector3 result = origin;
            bool collisionResult = false;
            for (int i = 1; i <= pushDist; i++)
            {
                int pushDistX = i;
                int pushDistZ = i;
                if (origin.x < Caster.TrueCenter().x) pushDistX = -pushDistX;
                if (origin.z < Caster.TrueCenter().z) pushDistZ = -pushDistZ;
                Vector3 tempNewLoc = new Vector3(origin.x + pushDistX, 0f, origin.z + pushDistZ);
                if (GenGrid.Standable(tempNewLoc.ToIntVec3(), Caster.Map))
                {
                    result = tempNewLoc;
                }
                else
                {
                    if (thingToPush is Pawn)
                    {
                        //target.TakeDamage(new DamageInfo(DamageDefOf.Blunt, Rand.Range(3, 6), -1, null, null, null));
                        collisionResult = true;
                        break;
                    }
                }
            }
            collision = collisionResult;
            return result;
        }

        public void PushEffect(Thing target, int distance, bool damageOnCollision = false)
        {
            if (target != null && target is Pawn pawn)
            {
                Vector3 loc = PushResult(target, distance, out bool applyDamage);
                if (pawn.RaceProps.Humanlike) pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDef.Named("PJ_ThoughtPush"), null);
                FlyingObject flyingObject = (FlyingObject)GenSpawn.Spawn(ThingDef.Named("PJ_PFlyingObject"), target.Position, target.Map);
                if (applyDamage && damageOnCollision) flyingObject.Launch(Caster, new LocalTargetInfo(loc.ToIntVec3()), target, new DamageInfo(DamageDefOf.Blunt, Rand.Range(8,10)));
                else flyingObject.Launch(Caster, new LocalTargetInfo(loc.ToIntVec3()), target);
            }
        }

        public override void ApprenticeEffect(Thing target)
        {
            PushEffect(target, 8);
        }
        public override void AdeptEffect(Thing target)
        {
            PushEffect(target, 10);
        }
        public override void MasterEffect(Thing target)
        {
            PushEffect(target, 12, true);
        }
    }
}
