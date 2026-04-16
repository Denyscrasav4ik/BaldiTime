using System;
using HarmonyLib;

namespace BaldiTime;

[HarmonyPatch(typeof(MusicManager), "PlayMidi", new Type[]
{
    typeof(string),
    typeof(float),
    typeof(bool)
})]
internal class PlayMidi
{
    private static bool Prefix(MusicManager __instance, string song, float volume, bool loop)
    {
        if (BaldiTimePlugin.Instance.timesUp)
        {
            return false;
        }
        return true;
    }
}
