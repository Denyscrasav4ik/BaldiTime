using HarmonyLib;

namespace BaldiTime;

[HarmonyPatch(typeof(BaseGameManager), "PrepareToLoad")]
internal class PrepareToLoad
{
    private static void Prefix(BaseGameManager __instance)
    {
        BaldiTimePlugin.Instance.Reset();
    }
}
