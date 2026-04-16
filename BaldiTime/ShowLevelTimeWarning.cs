using HarmonyLib;

namespace BaldiTime;

[HarmonyPatch(typeof(BaldiTV), "ShowLevelTimeWarning")]
internal class ShowLevelTimeWarning
{
    private static bool Prefix()
    {
        return !BaldiTimePlugin.Instance.reworkEnabledThisLevel;
    }
}
