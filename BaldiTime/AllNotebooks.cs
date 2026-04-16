using System.Collections;
using System.Reflection;
using HarmonyLib;
using MTM101BaldAPI.Reflection;
using UnityEngine;

namespace BaldiTime;

[HarmonyPatch(typeof(MainGameManager), "AllNotebooks")]
internal class AllNotebooks
{
    private static IEnumerator WaitForSpoopMode()
    {
        if (Singleton<CoreGameManager>.Instance.timeLimitChallenge && Singleton<CoreGameManager>.Instance.currentMode == Mode.Main)
        {
            yield break;
        }
        while (!BaldiTimePlugin.Instance.spoopMode)
        {
            yield return new WaitForEndOfFrame();
        }
        BaldiTimePlugin.Instance.panic = true;
        if (BaldiTimePlugin.Instance.midis[BaldiTimePlugin.Instance.EscapeMusicIndex] != "NULL")
        {
            if (BaldiTimePlugin.Instance.midis[BaldiTimePlugin.Instance.EscapeMusicIndex] == "SHUFFLE")
            {
                Singleton<MusicManager>.Instance.PlayMidi(BaldiTimePlugin.Instance.midis[Random.Range(0, BaldiTimePlugin.Instance.midis.Count - 2)], loop: true);
            }
            else
            {
                Singleton<MusicManager>.Instance.PlayMidi(BaldiTimePlugin.Instance.midis[BaldiTimePlugin.Instance.EscapeMusicIndex], loop: true);
            }
        }
        if (BaldiTimePlugin.Instance.IsTimerConditions())
        {
            EnvironmentController ec = Singleton<BaseGameManager>.Instance.Ec;
            RandomEvent timeOut = Object.FindObjectOfType<TimeOut>();
            IEnumerator startEv = (IEnumerator)ec.GetType()
    .GetMethod("EventTimer", BindingFlags.Instance | BindingFlags.NonPublic)
    .Invoke(ec, new object[3]
    {
        timeOut,
        BaldiTimePlugin.Instance.hurryUpTime + 2f,
        false
    });
            ec.StartCoroutine(startEv);
            ec.SetTimeLimit(ec.SurpassedGameTime + BaldiTimePlugin.Instance.hurryUpTime);
            BaldiTV btv = Object.FindObjectOfType<BaldiTV>();
            Animator btv_anim = (Animator)btv.ReflectionGetVariable("baldiTvAnimator");
            if (!btv_anim.GetBool("Active"))
            {
                btv_anim.SetBool("Active", value: true);
            }
        }
    }

    private static void Postfix(MainGameManager __instance)
    {
        __instance.StartCoroutine(WaitForSpoopMode());
        BaldiTimePlugin.Instance.StartCoroutine(BaldiTimePlugin.Instance.BaldiTime());
    }
}
