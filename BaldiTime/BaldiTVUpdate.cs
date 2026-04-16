using HarmonyLib;
using MTM101BaldAPI.Reflection;
using TMPro;
using UnityEngine;

namespace BaldiTime;

[HarmonyPatch(typeof(BaldiTV), "Update")]
internal class BaldiTVUpdate
{
    private static bool ticking;

    private static void Postfix(BaldiTV __instance)
    {
        if (!(Object.FindObjectOfType<EnvironmentController>() != null))
        {
            return;
        }
        EnvironmentController ec = Singleton<BaseGameManager>.Instance.Ec;
        AudioSource audioSource = __instance.gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = __instance.gameObject.AddComponent<AudioSource>();
        }
        if (BaldiTimePlugin.Instance.IsEscapeConditions() && BaldiTimePlugin.Instance.IsTimerConditions())
        {
            TMP_Text tMP_Text = (TMP_Text)__instance.ReflectionGetVariable("timeTmp");
            _ = (GameObject)__instance.ReflectionGetVariable("exclamationObject");
            SoundObject soundObject = (SoundObject)__instance.ReflectionGetVariable("timeLimitTicking");
            bool flag = (bool)__instance.ReflectionGetVariable("busy");
            tMP_Text.text = Singleton<BaseGameManager>.Instance.Ec.GetDisplayTime();
            tMP_Text.gameObject.SetActive(!flag);
            if (ec.RemainingTime <= 0)
            {
                if (ticking)
                {
                    ticking = false;
                    BaldiTimePlugin.Instance.timesUp = true;
                }
            }
            else if (ec.RemainingTime <= 10)
            {
                tMP_Text.color = Color.red;
                if (!ticking)
                {
                    ticking = true;
                    audioSource.clip = soundObject.soundClip;
                    audioSource.loop = true;
                    audioSource.pitch = 0.5f;
                    audioSource.Play();
                }
                audioSource.pitch += 0.1f * Time.deltaTime;
            }
            else if (ec.RemainingTime <= 30)
            {
                tMP_Text.color = Color.yellow;
            }
            else
            {
                tMP_Text.color = Color.white;
            }
        }
        if (!ticking || !BaldiTimePlugin.Instance.IsEscapeConditions())
        {
            audioSource.Stop();
        }
        if (!BaldiTimePlugin.Instance.IsEscapeConditions())
        {
            return;
        }
        if (BaldiTimePlugin.Instance.LightFadeEnabled)
        {
            Cell[,] cells = ec.cells;
            foreach (Cell cell in cells)
            {
                float time = Time.time;
                float num = Mathf.Sin(time) * 0.35f + 0.35f;
                Color.RGBToHSV(cell.lightColor, out var H, out var _, out var V);
                cell.lightColor = Color.HSVToRGB(H, Mathf.Round(num * 10f) / 10f, V);
                ec.UpdateLightingAtCell(cell);
            }
        }
        else
        {
            Cell[,] cells2 = ec.cells;
            foreach (Cell cell2 in cells2)
            {
                Color.RGBToHSV(cell2.lightColor, out var H2, out var _, out var V2);
                cell2.lightColor = Color.HSVToRGB(H2, 0f, V2);
                ec.UpdateLightingAtCell(cell2);
            }
        }
        if (BaldiTimePlugin.Instance.ShakeEnabled)
        {
            Vector3 localPosition = Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.Find("CameraBase").localPosition;
            localPosition = new Vector3(Mathf.Sin(Time.time * 100f) * 0.05f, localPosition.y, localPosition.z);
            Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.Find("CameraBase").localPosition = localPosition;
        }
    }
}
