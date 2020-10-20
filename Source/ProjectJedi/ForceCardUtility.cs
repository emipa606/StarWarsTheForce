﻿using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace ProjectJedi
{
    public class ForceCardUtility
    {
        // RimWorld.CharacterCardUtility
        public static Vector2 forceCardSize = default;
        public static Vector2 ForceCardSize
        {
            get
            {
                if (forceCardSize == default)
                {
                    forceCardSize = new Vector2(395f, 536f);
                    if (LanguageDatabase.activeLanguage == LanguageDatabase.AllLoadedLanguages.FirstOrDefault(x => x.folderName == "German"))
                    {
                        forceCardSize = new Vector2(470f, 536f);
                    }
                }
                return forceCardSize;
            }
        }
        //public static Vector2 ForceCardSize = new Vector2(470f, 536f);  // Ideal for German version

        public static float ButtonSize = 40f;

        public static float ForceButtonSize = 46f;

        public static float ForceButtonPointSize = 24f;

        public static float HeaderSize = 32f;

        public static float TextSize = 22f;

        public static float Padding = 3f;

        public static float SpacingOffset = 15f;

        public static float SectionOffset = 8f;

        public static float ColumnSize = 245f;

        public static float SkillsColumnHeight = 113f;

        public static float SkillsColumnDivider = 114f;
        //public static float SkillsColumnDivider = 170f; // Ideal for German version

        public static float SkillsTextWidth = 138f;
        //public static float SkillsTextWidth = 170f; // Ideal for German version

        public static float SkillsBoxSize = 18f;

        public static float PowersColumnHeight = 195f;

        public static float PowersColumnWidth = 123f;

        public static bool adjustedForLanguage = false;

        public static void AdjustForLanguage()
        { 
            if (!adjustedForLanguage)
            {
                adjustedForLanguage = true;
                if (LanguageDatabase.activeLanguage == LanguageDatabase.AllLoadedLanguages.FirstOrDefault(x => x.folderName == "German"))
                {
                    SkillsColumnDivider = 170f;
                    SkillsTextWidth = 170f;
                }
            }
        }

        // RimWorld.CharacterCardUtility
        public static void DrawForceCard(Rect rect, Pawn pawn)
        {
            AdjustForLanguage();

            GUI.BeginGroup(rect);

            CompForceUser compForce = pawn.GetComp<CompForceUser>();
            if (compForce != null)
            {
                if (compForce.ForceUserLevel > 0)
                {
                    float alignmentTextSize = Text.CalcSize("PJ_Alignment".Translate()).x;
                    Rect rect2 = new Rect(((rect.width / 2) - alignmentTextSize) + SpacingOffset, rect.y, rect.width, HeaderSize);
                    Text.Font = GameFont.Medium;
                    Widgets.Label(rect2, "PJ_Alignment".Translate().CapitalizeFirst());
                    Text.Font = GameFont.Small;
                    //                             Alignment

                    Widgets.DrawLineHorizontal(rect.x - 10, rect2.yMax, rect.width - 15f);
                    //---------------------------------------------------------------------


                    float grayTextSize = Text.CalcSize("PJ_Gray".Translate()).x;
                    float lightTextSize = Text.CalcSize("PJ_Light".Translate()).x;
                    Rect rectAlignmentLabels = new Rect(0 + SpacingOffset, 0 + rect2.yMax + 2, ForceCardSize.x, ForceCardUtility.ButtonSize * 1.15f);
                    Rect rectAlignmentDark = new Rect(rectAlignmentLabels.x, rectAlignmentLabels.y, rectAlignmentLabels.width / 3, rectAlignmentLabels.height);
                    Rect rectAlignmentGray = new Rect((rectAlignmentLabels.x + (rectAlignmentLabels.width / 2)) - grayTextSize, rectAlignmentLabels.y, rectAlignmentLabels.width / 3, rectAlignmentLabels.height);
                    Rect rectAlignmentLight = new Rect(rectAlignmentLabels.width - (lightTextSize * 2), rectAlignmentLabels.y, rectAlignmentLabels.width / 3, rectAlignmentLabels.height);
                    Widgets.Label(rectAlignmentDark, "PJ_Dark".Translate().CapitalizeFirst());
                    Widgets.Label(rectAlignmentGray, "PJ_Gray".Translate().CapitalizeFirst());
                    Widgets.Label(rectAlignmentLight, "PJ_Light".Translate().CapitalizeFirst());

                    //Dark                        Gray                        Light
                    Rect rectAlignment = new Rect(rect.x, rectAlignmentLabels.yMax / 1.5f, rectAlignmentLabels.width - 20f, TextSize);

                    AlignmentOnGUI(rectAlignment, pawn.GetComp<CompForceUser>());
                    // |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||

                    float skillsTextSize = Text.CalcSize("PJ_Skills".Translate()).x;
                    Rect rectSkillsLabel = new Rect((rectAlignmentLabels.width / 2) - skillsTextSize, rectAlignment.yMax + SectionOffset, rect.width, HeaderSize);
                    Text.Font = GameFont.Medium;
                    Widgets.Label(rectSkillsLabel, "PJ_Skills".Translate().CapitalizeFirst());
                    Text.Font = GameFont.Small;

                    //                               Skills

                    Widgets.DrawLineHorizontal(rect.x - 10, rectSkillsLabel.yMax + Padding, rect.width - 15f);
                    //---------------------------------------------------------------------

                    Rect rectSkills = new Rect(rect.x, rectSkillsLabel.yMax + Padding, rectSkillsLabel.width, SkillsColumnHeight);
                    Rect rectInfoPane = new Rect(rectSkills.x, rectSkills.y + Padding, SkillsColumnDivider, SkillsColumnHeight);
                    Rect rectSkillsPane = new Rect(rectSkills.x + SkillsColumnDivider, rectSkills.y + Padding, rectSkills.width - SkillsColumnDivider, SkillsColumnHeight);

                    InfoPane(rectInfoPane, pawn.GetComp<CompForceUser>());
                    SkillsPane(rectSkillsPane, pawn.GetComp<CompForceUser>());

                    // LEVEL ________________             |       Lightsaber Offense  [X][X][+][_][_]
                    // ||||||||||||||||||||||             |       Lightsaber Defense  [+][_][_][_][_]
                    // Points Available 1                 |       Lightsaber Accuracy [X][+][_][_][_]
                    //

                    float powersTextSize = Text.CalcSize("PJ_Powers".Translate()).x;
                    Rect rectPowersLabel = new Rect((rect.width / 2) - (powersTextSize / 2), rectSkills.yMax + SectionOffset, rect.width, HeaderSize);
                    Text.Font = GameFont.Medium;
                    Widgets.Label(rectPowersLabel, "PJ_Powers".Translate().CapitalizeFirst());
                    Text.Font = GameFont.Small;

                    //Powers

                    Widgets.DrawLineHorizontal(rect.x - 10, rectPowersLabel.yMax, rect.width - 15f);
                    //---------------------------------------------------------------------

                    Rect rectPowers = new Rect(rect.x, rectPowersLabel.yMax + SectionOffset, rectPowersLabel.width, PowersColumnHeight);
                    Rect rectPowersDark = new Rect(rectPowers.x, rectPowers.y, PowersColumnWidth, PowersColumnHeight);
                    Rect rectPowersGray = new Rect(rectPowers.x + PowersColumnWidth, rectPowers.y, PowersColumnWidth, PowersColumnHeight);
                    Rect rectPowersLight = new Rect(rectPowers.x + PowersColumnWidth + PowersColumnWidth, rectPowers.y, PowersColumnWidth, PowersColumnHeight);

                    PowersGUIHandler(rectPowersDark, pawn.GetComp<CompForceUser>(), pawn.GetComp<CompForceUser>().ForceData.PowersDark, TexButton.PJTex_ForcePointDark);
                    PowersGUIHandler(rectPowersGray, pawn.GetComp<CompForceUser>(), pawn.GetComp<CompForceUser>().ForceData.PowersGray, TexButton.PJTex_ForcePointGray);
                    PowersGUIHandler(rectPowersLight, pawn.GetComp<CompForceUser>(), pawn.GetComp<CompForceUser>().ForceData.PowersLight, TexButton.PJTex_ForcePointLight);
                }
                else
                {
                    Rect rectInfoPane = new Rect(rect.x, rect.y, rect.width, rect.height);
                    InfoPaneSensitive(rectInfoPane, pawn.GetComp<CompForceUser>());
                }
            }

            GUI.EndGroup();
        }

        #region AlignmentGUI
        public static string AlignmentTipString(CompForceUser compForce, bool sensitive)
        {
            return "PJ_AlignmentDesc".Translate();

        }

        public static string ForceXPTipString(CompForceUser compForce, bool sensitive)
        {
            if (!sensitive) return compForce.ForceUserXP.ToString() + " / " + compForce.ForceUserXPTillNextLevel.ToString() + "\n" + "PJ_ForceXPDesc".Translate();
            return "PJ_ForceSensitiveDesc".Translate(new object[] {
                compForce.AbilityUser.LabelShort
            });

        }

        public static void AlignmentOnGUI(Rect rect, CompForceUser compForce)
        {

            ////base.DrawOnGUI(rect, maxThresholdMarkers, customMargin, drawArrows, doTooltip);
            if (rect.height > 70f)
            {
                float num = (rect.height - 70f) / 2f;
                rect.height = 70f;
                rect.y += num;
            }
            float num2 = 14f;
            if (rect.height < 50f)
            {
                num2 *= Mathf.InverseLerp(0f, 50f, rect.height);
            }
            Text.Anchor = TextAnchor.UpperLeft;
            Rect rect3 = new Rect(rect.x, rect.y + rect.height / 2f, rect.width - 10f, rect.height);
            rect3 = new Rect(rect3.x + 20, rect3.y, rect3.width - 35, rect3.height - num2);
            if (Mouse.IsOver(rect3))
            {
                Widgets.DrawHighlight(rect3);
            }
            TooltipHandler.TipRegion(rect3, new TipSignal(() => AlignmentTipString(compForce, false), rect.GetHashCode()));
            Widgets.FillableBar(rect3, 1.0f, TexButton.PJTex_AlignmentBar);

            float curInstantLevelPercentage = compForce.AlignmentValue;
            if (curInstantLevelPercentage >= 0f)
            {
                DrawBarInstantMarkerAt(rect3, curInstantLevelPercentage);
            }
            
            if (DebugSettings.godMode)
            {
                Rect rectDebugPlus = new Rect(rect3.xMax + 10, rect3.y, 10, 10);
                if (Widgets.ButtonText(rectDebugPlus, "+", true, false, true))
                {
                    compForce.AlignmentValue += 0.025f;
                }
                Rect rectDebugMinus = new Rect(rect3.xMin - 15, rect3.y, 10, 10);
                if (Widgets.ButtonText(rectDebugMinus, "-", true, false, true))
                {
                    compForce.AlignmentValue -= 0.025f;
                }
            }
        }

        public static void DrawBarInstantMarkerAt(Rect barRect, float pct)
        {
            float num = 12f;
            if (barRect.width < 150f)
            {
                num /= 2f;
            }
            Vector2 vector = new Vector2(barRect.x + barRect.width * pct, barRect.y + barRect.height);
            Rect position = new Rect(vector.x - num / 2f, vector.y, num, num);
            GUI.DrawTexture(position, TexButton.PJTex_AlignmentBarMarker);
        }
        #endregion AlignmentGUI

        #region InfoPane
        public static void InfoPane(Rect inRect, CompForceUser compForce)
        {
            Rect rectLevel = new Rect(inRect.x, inRect.y, inRect.width * 0.7f, TextSize);
            Text.Font = GameFont.Small;
            Widgets.Label(rectLevel, "PJ_Level".Translate().CapitalizeFirst() + " " + compForce.ForceUserLevel.ToString());
            Text.Font = GameFont.Small;

            if (DebugSettings.godMode)
            {
                Rect rectDebugPlus = new Rect(rectLevel.xMax, inRect.y, inRect.width * 0.3f, TextSize);
                if (Widgets.ButtonText(rectDebugPlus, "+", true, false, true))
                {
                    compForce.LevelUp(true);
                }
                if (compForce.ForceUserLevel > 0)
                {
                    Rect rectDebugReset = new Rect(rectDebugPlus.x, rectDebugPlus.yMax + 1, rectDebugPlus.width, TextSize);
                    if (Widgets.ButtonText(rectDebugReset, "~", true, false, true))
                    {
                        compForce.ResetPowers();
                    }
                }
            }
            
            //Level 0

            Rect rectPointsAvail = new Rect(inRect.x, rectLevel.yMax, inRect.width, TextSize);
            Text.Font = GameFont.Tiny;
            Widgets.Label(rectPointsAvail, compForce.ForceData.AbilityPoints + " " + "PJ_PointsAvail".Translate());
            Text.Font = GameFont.Small;

            //0 points available

            Rect rectLevelBar = new Rect(rectPointsAvail.x, rectPointsAvail.yMax + 3f, inRect.width - 10f, HeaderSize * 0.6f);
            DrawLevelBar(rectLevelBar, compForce);

            //[|||||||||||||]

            //Rect rectAffiliation = new Rect(rectPointsAvail.x, rectLevelBar.yMax + 3f, inRect.width - 10f, HeaderSize + 5f);
            //Text.Font = GameFont.Small;
            //string affiliation = "None";
            //if (compForce.affiliation != null) affiliation = compForce.affiliation.Name; 
            //Widgets.Label(rectAffiliation, "PJ_Affiliation".Translate().CapitalizeFirst() + ": " + affiliation);
            //Text.Font = GameFont.Small;

        }

        public static void InfoPaneSensitive(Rect inRect, CompForceUser compForce)
        {
            Rect rectLevel = new Rect(inRect.x, inRect.y, inRect.width * 0.7f, TextSize);
            Text.Font = GameFont.Small;
            Widgets.Label(rectLevel, "PJ_SensitiveMessage".Translate(new object[] {
                compForce.AbilityUser.LabelShort
            }
            ));
            Text.Font = GameFont.Small;

            if (DebugSettings.godMode)
            {
                Rect rectDebugPlus = new Rect(rectLevel.xMax, inRect.y, inRect.width * 0.3f, TextSize);
                if (Widgets.ButtonText(rectDebugPlus, "+", true, false, true))
                {
                    compForce.LevelUp(true);
                }
            }
            
            //Something is awakening...

            Rect rectPointsAvail = new Rect(inRect.x, rectLevel.yMax, inRect.width, TextSize);
            Rect rectLevelBar = new Rect(rectPointsAvail.x, rectPointsAvail.yMax + 3f, inRect.width - 10f, HeaderSize * 0.6f);
            DrawLevelBar(rectLevelBar, compForce, true);

        }
        public static void DrawLevelBar(Rect rect, CompForceUser compForce, bool sensitive = false)
        {
            ////base.DrawOnGUI(rect, maxThresholdMarkers, customMargin, drawArrows, doTooltip);
            if (rect.height > 70f)
            {
                float num = (rect.height - 70f) / 2f;
                rect.height = 70f;
                rect.y += num;
            }
            if (Mouse.IsOver(rect))
            {
                Widgets.DrawHighlight(rect);
            }
            TooltipHandler.TipRegion(rect, new TipSignal(() => ForceXPTipString(compForce, sensitive), rect.GetHashCode()));
            float num2 = 14f;
            if (rect.height < 50f)
            {
                num2 *= Mathf.InverseLerp(0f, 50f, rect.height);
            }
            Text.Anchor = TextAnchor.UpperLeft;
            Rect rect3 = new Rect(rect.x, rect.y + rect.height / 2f, rect.width, rect.height);
            rect3 = new Rect(rect3.x, rect3.y, rect3.width, rect3.height - num2);
            Widgets.FillableBar(rect3, compForce.XPTillNextLevelPercent, (Texture2D)AccessTools.Field(typeof(Widgets), "BarFullTexHor").GetValue(null), BaseContent.GreyTex, false);
        }
        #endregion InfoPane

        #region SkillsPane
        public static void SkillsPane(Rect inRect, CompForceUser compForce)
        {
            float currentYOffset = inRect.y;
            
            if (!compForce?.ForceData.Skills.NullOrEmpty() ?? false)
            {
                foreach (ForceSkill skill in compForce.ForceData.Skills)
                {
                    Rect lightsaberOffense = new Rect(inRect.x, currentYOffset, inRect.width, TextSize);
                    Rect lightsaberOffenseLabel = new Rect(inRect.x, currentYOffset, SkillsTextWidth, TextSize);
                    Widgets.Label(lightsaberOffenseLabel, skill.label.Translate());

                    TooltipHandler.TipRegion(lightsaberOffenseLabel, new TipSignal(() => skill.desc.Translate(), lightsaberOffenseLabel.GetHashCode()));
                    Rect lightsaberOffensiveBoxes = new Rect(lightsaberOffenseLabel.xMax, currentYOffset, inRect.width - SkillsTextWidth, TextSize);

                    for (int i = 1; i <= 5; i++)
                    {
                        Rect lightsaberCheckbox = new Rect(lightsaberOffensiveBoxes.x + (SkillsBoxSize * i), lightsaberOffensiveBoxes.y, SkillsBoxSize, TextSize);
                        if (skill.level >= i)
                        {
                            Widgets.DrawTextureFitted(new Rect(lightsaberCheckbox.x, lightsaberCheckbox.y, lightsaberCheckbox.width - 2, TextSize), TexButton.PJTex_SkillBoxFull, 1f);
                            continue;
                        }
                        else if ((i - skill.level == 1 && compForce.ForceData.AbilityPoints > 0 && skill.level < 5) && (compForce.AbilityUser.Faction == Faction.OfPlayer))
                        {
                            //TooltipHandler.TipRegion(rectRename, "RenameTemple".Translate());
                            if (Widgets.ButtonImage(new Rect(lightsaberCheckbox.x, lightsaberCheckbox.y, lightsaberCheckbox.width - 2, TextSize - 4), TexButton.PJTex_SkillBoxAdd))
                            {
                                compForce.ForceData.AbilityPoints--;
                                skill.level++;
                            }
                            //Widgets.DrawTextureFitted(new Rect(lightsaberCheckbox.x, lightsaberCheckbox.y, lightsaberCheckbox.width - 2, TextSize), TexButton.PJTex_SkillBoxAdd, 1f);
                            continue;
                        }
                        else
                        {
                            Widgets.DrawTextureFitted(new Rect(lightsaberCheckbox.x, lightsaberCheckbox.y, lightsaberCheckbox.width - 2, TextSize), TexButton.PJTex_SkillBox, 1f);
                            continue;
                        }
                    }

                    currentYOffset += TextSize;
                }
            }
            
        }
        #endregion SkillsPane

        #region PowersGUI
        public static void PowersGUIHandler(Rect inRect, CompForceUser compForce, List<ForcePower> forcePowers, Texture2D pointTexture)
        {
            float buttonYOffset = inRect.y;
            foreach (ForcePower power in forcePowers)
            {
                
                Rect buttonRect = new Rect(inRect.x, buttonYOffset, ForceButtonSize, ForceButtonSize);
                TooltipHandler.TipRegion(buttonRect, () => power.AbilityDef.label + "\n\n" + power.AbilityDef.description + "\n\n" + "PJ_CheckStarsForMoreInfo".Translate(), 398462);
                if (compForce.ForceData.AbilityPoints == 0 || power.level >= 3)
                {
                    Widgets.DrawTextureFitted(buttonRect, power.Icon, 1.0f);
                }
                else if(Widgets.ButtonImage(buttonRect, power.Icon) && (compForce.AbilityUser.Faction == Faction.OfPlayer))
                {
                    ForceAbilityDef powerDef = power.NextLevelAbilityDef as ForceAbilityDef;
                    if (powerDef.requiredAlignmentType != ForceAlignmentType.None &&
                        powerDef.requiredAlignmentType != compForce.ForceAlignmentType)
                    {
                        Messages.Message("PJ_NextLevelAlignmentMismatch".Translate(new object[]
                        {
                            powerDef.requiredAlignmentType.ToString(),
                            compForce.ForceAlignmentType.ToString()
                        }),  MessageTypeDefOf.RejectInput);
                        return;
                    }
                    if (compForce.LightsidePoints < powerDef.lightsideTreePointsRequired)
                    {
                        Messages.Message("PJ_LightsidePointsRequired".Translate(new object[]
                        {
                            powerDef.lightsideTreePointsRequired
                        }), MessageTypeDefOf.RejectInput);
                        return;
                    }
                    if (compForce.DarksidePoints < powerDef.darksideTreePointsRequired)
                    {
                        Messages.Message("PJ_DarksidePointsRequired".Translate(new object[]
                        {  
                            powerDef.darksideTreePointsRequired
                        }), MessageTypeDefOf.RejectInput);
                        return;
                    }
                    if (compForce.ForceData.AbilityPoints < powerDef.abilityPoints)
                    {
                        Messages.Message("PJ_NotEnoughAbilityPoints".Translate(new object[]
                        {
                            compForce.ForceData.AbilityPoints,
                            powerDef.abilityPoints
                        }), MessageTypeDefOf.RejectInput);
                        return;
                    }
                    if (compForce.AbilityUser.story != null && (compForce.AbilityUser.WorkTagIsDisabled(WorkTags.Violent) && power.AbilityDef.MainVerb.isViolent))
                    {
                        Messages.Message("IsIncapableOfViolenceLower".Translate(new object[]
                        {
                            compForce.parent.LabelShort
                        }), MessageTypeDefOf.RejectInput);
                        return;
                    }
                    compForce.LevelUpPower(power);
                    compForce.ForceData.AbilityPoints -= powerDef.abilityPoints;
                }
                for (int i = 0; i < 3; i++)
                {
                    float drawXOffset = ForceButtonSize + 1f;
                    if (i != 0) drawXOffset += (ForceButtonPointSize * i);

                    float drawYOffset = buttonYOffset + (ForceButtonSize / 3f);
                    Rect powerRegion = new Rect(inRect.x + drawXOffset, drawYOffset, ForceButtonPointSize, ForceButtonPointSize);

                    if (power.level > i)
                    {
                        Widgets.DrawTextureFitted(powerRegion, pointTexture, 1.0f);
                    }
                    else
                    {
                        Widgets.DrawTextureFitted(powerRegion, TexButton.PJTex_ForcePointDim, 1.0f);
                    }
                    if (power.GetAbilityDef(i) is ForceAbilityDef powerDef)
                    {
                        TooltipHandler.TipRegion(powerRegion, () => powerDef.GetDescription() + "\n" + compForce.PostAbilityVerbCompDesc(powerDef.MainVerb) + "\n" + powerDef.GetPointDesc(), 398462);
                    }

                }
                buttonYOffset += ForceButtonSize + 1;
            }
        }
        #endregion PowersGUI
    }
}
