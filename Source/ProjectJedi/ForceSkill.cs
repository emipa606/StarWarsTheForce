using System;
using Verse;

namespace ProjectJedi
{
    public class ForceSkill : IExposable
    {
        public string label;
        public string desc;
        public int level;

        public ForceSkill()
        {

        }

        public ForceSkill(String newLabel, String newDesc)
        {
            label = newLabel;
            desc = newDesc;
            level = 0;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref label, "label", "default");
            Scribe_Values.Look(ref desc, "desc", "default");
            Scribe_Values.Look(ref level, "level", 0);
        }
    }
}
