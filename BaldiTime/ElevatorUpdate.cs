using HarmonyLib;
using MTM101BaldAPI.Reflection;
using UnityEngine;

namespace BaldiTime;

[HarmonyPatch(typeof(Elevator), "Update")]
internal class ElevatorUpdate
{
    private static bool Prefix(Elevator __instance)
    {
        DigitalClock[] array = (DigitalClock[])__instance.ReflectionGetVariable("clock");
        if (!BaldiTimePlugin.Instance.panic)
        {
            array[0].SetColor(Color.clear);
            array[1].SetColor(Color.clear);
            return false;
        }
        array[0].UpdateDisplay(Singleton<BaseGameManager>.Instance.Ec.RemainingTime);
        array[1].UpdateDisplay(Singleton<BaseGameManager>.Instance.Ec.RemainingTime);
        if ((float)Singleton<BaseGameManager>.Instance.Ec.RemainingTime <= 10f)
        {
            array[0].SetColor(Color.red);
            array[1].SetColor(Color.red);
            return false;
        }
        if ((float)Singleton<BaseGameManager>.Instance.Ec.RemainingTime <= 30f)
        {
            array[0].SetColor(Color.yellow);
            array[1].SetColor(Color.yellow);
            return false;
        }
        array[0].SetColor(Color.white);
        array[1].SetColor(Color.white);
        return false;
    }
}
