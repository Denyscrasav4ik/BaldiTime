using HarmonyLib;

namespace BaldiTime;

[HarmonyPatch(typeof(BaldiTV), "QueueCheck")]
internal class QueueCheck
{
    private static bool Prefix(BaldiTV __instance)
    {
        if (BaldiTimePlugin.Instance.IsEscapeConditions() && BaldiTimePlugin.Instance.IsTimerConditions() && !BaldiTimePlugin.Instance.timesUp)
        {
            return false;
        }
        return true;
    }
}
