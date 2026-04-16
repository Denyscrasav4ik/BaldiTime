using System.Collections;
using System.Reflection;
using HarmonyLib;
using MTM101BaldAPI.Reflection;
using UnityEngine;

namespace BaldiTime;

[HarmonyPatch(typeof(MainGameManager), "BeginSpoopMode")]
internal class BeginSpoopMode
{
    private static void Postfix()
    {
        BaldiTimePlugin.Instance.spoopMode = true;
        if (!Singleton<CoreGameManager>.Instance.timeLimitChallenge || Singleton<CoreGameManager>.Instance.currentMode != Mode.Main)
        {
            return;
        }
        BaldiTimePlugin.Instance.hurryUpTime = 60f;
        if (BaldiTimePlugin.Instance.difficultyThisGame == 2)
        {
            BaldiTimePlugin.Instance.hurryUpTime = 15f;
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
        EnvironmentController ec = Singleton<BaseGameManager>.Instance.Ec;
        RandomEvent randomEvent = Object.FindObjectOfType<TimeOut>();
        IEnumerator routine = (IEnumerator)ec.GetType().GetMethod("EventTimer", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(ec, new object[2]
        {
            randomEvent,
            BaldiTimePlugin.Instance.hurryUpTime + 2f
        });
        ec.StartCoroutine(routine);
        ec.SetTimeLimit(ec.SurpassedGameTime + BaldiTimePlugin.Instance.hurryUpTime);
        if (BaldiTimePlugin.Instance.IsTimerConditions())
        {
            BaldiTV me = Object.FindObjectOfType<BaldiTV>();
            Animator animator = (Animator)me.ReflectionGetVariable("baldiTvAnimator");
            if (!animator.GetBool("Active"))
            {
                animator.SetBool("Active", value: true);
            }
        }
    }
}
