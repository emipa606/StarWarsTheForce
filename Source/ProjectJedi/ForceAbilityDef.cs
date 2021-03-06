﻿using System.Text;
using Verse;

namespace ProjectJedi
{
    public class ForceAbilityDef : AbilityUser.AbilityDef
    {
        public float forcePoolCost = 0.0f;

        public int abilityPoints = 1;

        public int darksideTreePointsRequired = 0;

        public int lightsideTreePointsRequired = 0;

        public ForceAlignmentType requiredAlignmentType = ForceAlignmentType.None;

        public ForceAlignmentType changedAlignmentType = ForceAlignmentType.None;

        public float changedAlignmentRate = 0.0f;


        public string GetPointDesc()
        {
            StringBuilder s = new StringBuilder();
            s.AppendLine("PJ_PointsRequired".Translate(new object[]
                {
                abilityPoints
                }));
            if (darksideTreePointsRequired > 0)
            {
                s.AppendLine("PJ_DarkPointsRequired".Translate(new object[]
                {
                    darksideTreePointsRequired
                }));
            }
            if (lightsideTreePointsRequired > 0)
            {
                s.AppendLine("PJ_LightPointsRequired".Translate(new object[]
                {
                    lightsideTreePointsRequired
                }));
            }
            string result = s.ToString();
            return result;
        }
    }

}
