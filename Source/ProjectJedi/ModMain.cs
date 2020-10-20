using UnityEngine;
using Verse;

namespace ProjectJedi
{
    public class ModMain : Mod
    {
        readonly Settings settings;

        public ModMain(ModContentPack content) : base(content)
        {
            settings = GetSettings<Settings>();
            ModInfo.forceXPDelayFactor = settings.forceXPDelayFactor;
        }

        public override string SettingsCategory() => "Star Wars - The Force";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            var label = "PJ_SettingsForceXPDelay_Num".Translate(settings.forceXPDelayFactor);
            settings.forceXPDelayFactor = Widgets.HorizontalSlider(inRect.TopHalf().TopHalf().TopHalf(), settings.forceXPDelayFactor, 0.1f, 10f, false, label, null, null, 0.1f);

            WriteSettings();

        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            ModInfo.forceXPDelayFactor = settings.forceXPDelayFactor;
        }

    }

    public class Settings : ModSettings
    {
        public float forceXPDelayFactor = 1;
        public string forceXPDelayStringBuffer;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref forceXPDelayFactor, "forceXPDelayFactor", 0);
        }
    }
}
