﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace ProjectJedi
{
    /// <summary>
    /// A special version of a projectile.
    /// This one "stores" a base object and "delivers" it.
    /// </summary>
    public class FlyingObject : ThingWithComps
    {
        protected Vector3 origin;
        protected Vector3 destination;
        protected float speed = 30.0f;
        protected int ticksToImpact;
        protected Thing launcher;
        protected Thing assignedTarget;
        protected Thing flyingThing;
        public DamageInfo? impactDamage;

        protected int StartingTicksToImpact
        {
            get
            {
                int num = Mathf.RoundToInt((this.origin - this.destination).magnitude / (this.speed / 100f));
                if (num < 1)
                {
                    num = 1;
                }
                return num;
            }
        }


        protected IntVec3 DestinationCell
        {
            get
            {
                return new IntVec3(this.destination);
            }
        }

        public virtual Vector3 ExactPosition
        {
            get
            {
                Vector3 b = (this.destination - this.origin) * (1f - (float)this.ticksToImpact / (float)this.StartingTicksToImpact);
                return this.origin + b + Vector3.up * this.def.Altitude;
            }
        }

        public virtual Quaternion ExactRotation
        {
            get
            {
                return Quaternion.LookRotation(this.destination - this.origin);
            }
        }

        public override Vector3 DrawPos
        {
            get
            {
                return this.ExactPosition;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.LookValue<Vector3>(ref this.origin, "origin", default(Vector3), false);
            Scribe_Values.LookValue<Vector3>(ref this.destination, "destination", default(Vector3), false);
            Scribe_Values.LookValue<int>(ref this.ticksToImpact, "ticksToImpact", 0, false);
            Scribe_References.LookReference<Thing>(ref this.assignedTarget, "assignedTarget", false);
            Scribe_References.LookReference<Thing>(ref this.launcher, "launcher", false);
            Scribe_References.LookReference<Thing>(ref this.flyingThing, "flyingThing");
        }

        public void Launch(Thing launcher, LocalTargetInfo targ, Thing flyingThing, DamageInfo? impactDamage)
        {
            this.Launch(launcher, base.Position.ToVector3Shifted(), targ, flyingThing, impactDamage);
        }

        public void Launch(Thing launcher, LocalTargetInfo targ, Thing flyingThing)
        {
            this.Launch(launcher, base.Position.ToVector3Shifted(), targ, flyingThing);
        }

        public void Launch(Thing launcher, Vector3 origin, LocalTargetInfo targ, Thing flyingThing, DamageInfo? newDamageInfo = null)
        {
            //Despawn the object to fly
            if (flyingThing.Spawned) flyingThing.DeSpawn();

            this.launcher = launcher;
            this.origin = origin;
            this.impactDamage = newDamageInfo;
            this.flyingThing = flyingThing;
            if (targ.Thing != null)
            {
                this.assignedTarget = targ.Thing;
            }
            this.destination = targ.Cell.ToVector3Shifted() + new Vector3(Rand.Range(-0.3f, 0.3f), 0f, Rand.Range(-0.3f, 0.3f));
            this.ticksToImpact = this.StartingTicksToImpact;
        }

        public override void Tick()
        {
            base.Tick();
            Vector3 exactPosition = this.ExactPosition;
            this.ticksToImpact--;
            if (!this.ExactPosition.InBounds(base.Map))
            {
                this.ticksToImpact++;
                base.Position = this.ExactPosition.ToIntVec3();
                this.Destroy(DestroyMode.Vanish);
                return;
            }
            
            base.Position = this.ExactPosition.ToIntVec3();
            if (this.ticksToImpact <= 0)
            {
                if (this.DestinationCell.InBounds(base.Map))
                {
                    base.Position = this.DestinationCell;
                }
                this.ImpactSomething();
                return;
            }

        }

        public override void Draw()
        {
            if (flyingThing != null)
            {
                if (flyingThing is Pawn)
                {
                    Pawn pawn = flyingThing as Pawn;
                    pawn.Drawer.DrawAt(this.DrawPos);
                    //Graphics.DrawMesh(MeshPool.plane10, this.DrawPos, this.ExactRotation, this.flyingThing.def.graphic.MatFront, 0);
                }
                else
                {
                    Graphics.DrawMesh(MeshPool.plane10, this.DrawPos, this.ExactRotation, this.flyingThing.def.DrawMatSingle, 0);
                }
                base.Comps_PostDraw();
            }
        }

        private void ImpactSomething()
        {
            if (this.assignedTarget != null)
            {
                Pawn pawn = this.assignedTarget as Pawn;
                if (pawn != null && pawn.GetPosture() != PawnPosture.Standing && (this.origin - this.destination).MagnitudeHorizontalSquared() >= 20.25f && Rand.Value > 0.2f)
                {
                    this.Impact(null);
                    return;
                }
                this.Impact(this.assignedTarget);
                return;
            }
            else
            {
                this.Impact(null);
                return;
            }
        }

        protected virtual void Impact(Thing hitThing)
        {
            GenSpawn.Spawn(flyingThing, this.Position, this.Map);
            if (impactDamage != null)
            {
                for (int i = 0; i < 3; i++) flyingThing.TakeDamage(impactDamage.Value);
            }
            this.Destroy(DestroyMode.Vanish);
        }


    }
}
