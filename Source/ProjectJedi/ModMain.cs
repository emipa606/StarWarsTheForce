using Mlie;
using UnityEngine;
using Verse;

namespace ProjectJedi;

public class ModMain : Mod
{
    private static string currentVersion;
    private readonly Settings settings;

    public ModMain(ModContentPack content) : base(content)
    {
        settings = GetSettings<Settings>();
        ModInfo.forceXPDelayFactor = settings.forceXPDelayFactor;
        currentVersion = VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
    }

    public override string SettingsCategory()
    {
        return "Star Wars - The Force";
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        var lister = new Listing_Standard();
        lister.Begin(inRect);
        settings.forceXPDelayFactor =
            lister.SliderLabeled(
                "PJ_SettingsForceXPDelay_Num".Translate(settings.forceXPDelayFactor.ToStringDecimalIfSmall()),
                settings.forceXPDelayFactor, 0.1f, 10f);
        if (currentVersion != null)
        {
            lister.Gap();
            GUI.contentColor = Color.gray;
            lister.Label("PJ_CurrentModVersion".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        lister.End();
        WriteSettings();
    }

    public override void WriteSettings()
    {
        base.WriteSettings();
        ModInfo.forceXPDelayFactor = settings.forceXPDelayFactor;
    }
}