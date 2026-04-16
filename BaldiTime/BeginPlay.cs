using System.Collections.Generic;
using HarmonyLib;
using MTM101BaldAPI.Reflection;
using UnityEngine;

namespace BaldiTime;

[HarmonyPatch(typeof(MainGameManager), "BeginPlay")]
internal class BeginPlay
{
    private static void Postfix(MainGameManager __instance)
    {
        BaldiTimePlugin.Instance.Reset();
        if (Singleton<BaseGameManager>.Instance.CurrentLevel == 0 && Singleton<CoreGameManager>.Instance.Lives == 2)
        {
            BaldiTimePlugin.Instance.difficultyThisGame = BaldiTimePlugin.Instance.GameMode;
        }
        BaldiTimePlugin.Instance.reworkEnabledThisLevel = BaldiTimePlugin.Instance.ReworkEnabled;
        if (BaldiTimePlugin.Instance.IsTimerConditions())
        {
            List<RandomEvent> list = (List<RandomEvent>)__instance.Ec.ReflectionGetVariable("events");
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Type == RandomEventType.TimeOut)
                {
                    list.Remove(list[i]);
                }
            }
            __instance.Ec.ReflectionSetVariable("events", list);
        }
        int currentLevel = Singleton<BaseGameManager>.Instance.CurrentLevel;
        if (BaldiTimePlugin.Instance.difficultyThisGame == 2)
        {
            switch (currentLevel)
            {
                case 0:
                    BaldiTimePlugin.Instance.hurryUpTime = 15f;
                    break;
                case 1:
                    BaldiTimePlugin.Instance.hurryUpTime = 30f;
                    break;
                case 2:
                    BaldiTimePlugin.Instance.hurryUpTime = 45f;
                    break;
                case 3:
                    BaldiTimePlugin.Instance.hurryUpTime = 30f;
                    break;
                case 4:
                    BaldiTimePlugin.Instance.hurryUpTime = 45f;
                    break;
            }
        }
        else if (BaldiTimePlugin.Instance.difficultyThisGame == 1)
        {
            switch (currentLevel)
            {
                case 0:
                    BaldiTimePlugin.Instance.hurryUpTime = 30f;
                    break;
                case 1:
                    BaldiTimePlugin.Instance.hurryUpTime = 60f;
                    break;
                case 2:
                    BaldiTimePlugin.Instance.hurryUpTime = 90f;
                    break;
                case 3:
                    BaldiTimePlugin.Instance.hurryUpTime = 70f;
                    break;
                case 4:
                    BaldiTimePlugin.Instance.hurryUpTime = 100f;
                    break;
            }
        }
        else
        {
            switch (currentLevel)
            {
                case 0:
                    BaldiTimePlugin.Instance.hurryUpTime = 60f;
                    break;
                case 1:
                    BaldiTimePlugin.Instance.hurryUpTime = 120f;
                    break;
                case 2:
                    BaldiTimePlugin.Instance.hurryUpTime = 180f;
                    break;
                case 3:
                    BaldiTimePlugin.Instance.hurryUpTime = 140f;
                    break;
                case 4:
                    BaldiTimePlugin.Instance.hurryUpTime = 200f;
                    break;
            }
        }
        if (BaldiTimePlugin.Instance.difficultyThisGame != 2)
        {
            MathMachine[] array = Object.FindObjectsOfType<MathMachine>();
            for (int j = 0; j < array.Length; j++)
            {
                _ = array[j];
                BaldiTimePlugin.Instance.hurryUpTime += 5f;
            }
        }
    }
}
