using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using AbilityUser;

namespace ProjectJedi
{
    public class ForcePower : IExposable
    {
        public List<AbilityDef> abilityDefs;
        public int level;
        public int ticksUntilNextCast = -1;
        public AbilityDef GetAbilityDef(int index)
        {
            AbilityDef result = null;
            if (abilityDefs != null && abilityDefs.Count > 0)
            {
                result = abilityDefs[0];
                
                if (index > -1 && index < abilityDefs.Count) result = abilityDefs[index];
                else if (index >= abilityDefs.Count)
                {
                    result = abilityDefs[abilityDefs.Count - 1];
                }
            }
            return result;
        }
        public AbilityDef AbilityDef
        {
            get
            {
                AbilityDef result = null;
                if (abilityDefs != null && abilityDefs.Count > 0)
                {
                    result = abilityDefs[0];

                    int index = level - 1;
                    if (index > -1 && index < abilityDefs.Count) result = abilityDefs[index];
                    else if (index >= abilityDefs.Count)
                    {
                        result = abilityDefs[abilityDefs.Count - 1];
                    }
                }
                return result;
            }
        }
        public AbilityDef NextLevelAbilityDef
        {
            get
            {
                AbilityDef result = null;
                if (abilityDefs != null && abilityDefs.Count > 0)
                {
                    result = abilityDefs[0];

                    int index = level;
                    if (index > -1 && index <= abilityDefs.Count) result = abilityDefs[index];
                    else if (index >= abilityDefs.Count)
                    {
                        result = abilityDefs[abilityDefs.Count - 1];
                    }
                }
                return result;
            }
        }



        public AbilityDef HasAbilityDef(AbilityDef defToFind)
        {
            return abilityDefs.FirstOrDefault((AbilityDef x) => x == defToFind);
        }

        public ForcePower()
        {

        }

        public Texture2D Icon
        {
            get
            {
                return AbilityDef.uiIcon;
            }
        }

        public ForcePower(List<AbilityDef> newAbilityDefs)
        {
            level = 0;
            abilityDefs = newAbilityDefs;
        }



        public void ExposeData()
        {
            Scribe_Values.Look(ref level, "level", 0);
            Scribe_Values.Look(ref ticksUntilNextCast, "ticksUntilNextCast", -1);
            Scribe_Collections.Look(ref abilityDefs, "abilityDefs", LookMode.Def, null);
        }
    }
}
