using Verse;

namespace ProjectJedi;

public class Settings : ModSettings
{
    public static int nonForceUserLightsaberDamage = 10;
    public float forceWielderDifficultyModifier = 5;
    public float forceXPDelayFactor = 1;
    public string forceXPDelayStringBuffer;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref forceWielderDifficultyModifier, "forceWielderDifficultyModifier");
        Scribe_Values.Look(ref forceXPDelayFactor, "forceXPDelayFactor");
    }
}