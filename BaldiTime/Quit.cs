using HarmonyLib;

namespace BaldiTime;

[HarmonyPatch(typeof(CoreGameManager), "Quit")]
internal class Quit
{
    private static void Prefix()
    {
        BaldiTimePlugin.Instance.Reset();
    }
}
