using System.Collections;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.OptionsAPI;
using MTM101BaldAPI.Registers;
using MTM101BaldAPI.SaveSystem;
using TMPro;
using UnityEngine;

namespace BaldiTime;

[BepInDependency("mtm101.rulerp.bbplus.baldidevapi", BepInDependency.DependencyFlags.HardDependency)]
[BepInPlugin("denyscrasav4ik.thedumbfactory.balditime", "Baldi Time", "1.0.0")]
public class BaldiTimePlugin : BaseUnityPlugin
{
    public ConfigEntry<bool> configReworkEnabled;
    public ConfigEntry<int> configEscapeMusicIndex;
    public ConfigEntry<int> configGameMode;
    public ConfigEntry<bool> configLightFadeEnabled;
    public ConfigEntry<bool> configShakeEnabled;

    public Sprite[] animationSprites;
    public GameObject animationOverlay;

    public bool panic = false;
    public bool spoopMode = false;
    public bool timesUp = false;
    public float hurryUpTime = 120f;
    public bool reworkEnabledThisLevel = false;
    public int difficultyThisGame = 0;

    public MenuToggle enabledToggle;
    public MenuToggle lightsToggle;
    public MenuToggle shakeToggle;
    public TextMeshProUGUI musicText;

    public List<string> midis = new List<string>();
    public List<string> midiNames = new List<string>();
    public string[] difficulties = new string[3] { "Normal", "Hard", "S-Hard" };

    public bool optionsMenuBuilt = false;
    public int musicIndex;
    public int difficulty;

    public static BaldiTimePlugin Instance { get; private set; }

    public bool ReworkEnabled => configReworkEnabled.Value;
    public int EscapeMusicIndex => configEscapeMusicIndex.Value;
    public int GameMode => configGameMode.Value;
    public bool LightFadeEnabled => configLightFadeEnabled.Value;
    public bool ShakeEnabled => configShakeEnabled.Value;

    public void Awake()
    {
        Instance = this;

        configReworkEnabled = Config.Bind("General", "Rework Enabled", true, "When enabled, the Timer will start in the escape.");
        configEscapeMusicIndex = Config.Bind("Music", "Music Index", 0, "The index of the selected escape music.");
        configGameMode = Config.Bind("General", "Difficulty", 0, "0 = Normal, 1 = Hard, 2 = S-Hard");
        configLightFadeEnabled = Config.Bind("Effects", "Red Lights", true, "If enabled, red lights will slowly pulse during the escape.");
        configShakeEnabled = Config.Bind("Effects", "Shake", true, "If enabled, the level will shake during the escape.");

        Harmony harmony = new Harmony("benjagamess.plusmods.hurryup");
        harmony.PatchAllConditionals();

        LoadingEvents.RegisterOnAssetsLoaded(base.Info, Preload(), LoadingEventOrder.Post);
        ModdedSaveGame.AddSaveHandler(base.Info);

        musicIndex = EscapeMusicIndex;
        difficulty = GameMode;
        CustomOptionsCore.OnMenuInitialize += OnMen;
    }

    private void Update()
    {
        if (optionsMenuBuilt && enabledToggle != null && lightsToggle != null && shakeToggle != null)
        {
            configReworkEnabled.Value = enabledToggle.Value;
            configEscapeMusicIndex.Value = musicIndex;

            if (musicIndex >= midis.Count)
            {
                musicIndex = 0;
            }

            configGameMode.Value = difficulty;
            configLightFadeEnabled.Value = lightsToggle.Value;
            configShakeEnabled.Value = shakeToggle.Value;

            if (midiNames.Count > 0)
            {
                musicText.text = midiNames[musicIndex];
            }
        }
    }

    private void OnMen(OptionsMenu __instance, CustomOptionsHandler handler)
    {
        handler.AddCategory<BaldiTimeOptions>("Escape\nOptions");
    }

    public bool IsTimerConditions()
    {
        return reworkEnabledThisLevel && Singleton<CoreGameManager>.Instance.currentMode == Mode.Main && !(Object.FindObjectOfType<TimeOut>() == null);
    }

    public bool IsEscapeConditions()
    {
        return panic && spoopMode;
    }

    public void Reset()
    {
        panic = false;
        spoopMode = false;
        timesUp = false;
    }

    private IEnumerator Preload()
    {
        yield return 2;
        yield return "Loading the Custom Escape Themes";
        AddMidisFromModFolder();
        yield return "Loading Animation Assets";
        LoadAnimationAssets();
    }

    private void AddMidisFromModFolder()
    {
        string midiPath = Path.Combine(AssetLoader.GetModPath(this), "Midis");
        if (!Directory.Exists(midiPath)) Directory.CreateDirectory(midiPath);

        string[] files = Directory.GetFiles(midiPath, "*.mid");
        string[] files2 = Directory.GetFiles(midiPath, "*.txt");
        for (int i = 0; i < files.Length; i++)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(files[i]);
            midis.Add(AssetLoader.MidiFromFile(files[i], fileNameWithoutExtension));
            bool flag = false;
            foreach (string path in files2)
            {
                if (Path.GetFileNameWithoutExtension(path) == fileNameWithoutExtension)
                {
                    midiNames.Add(File.ReadAllText(path));
                    flag = true;
                }
            }
            if (!flag)
            {
                midiNames.Add(fileNameWithoutExtension);
            }
        }
        midis.Add("SHUFFLE");
        midiNames.Add("Shuffle");
        midis.Add("NULL");
        midiNames.Add("None");
    }
    private void LoadAnimationAssets()
    {
        string imagePath = Path.Combine(AssetLoader.GetModPath(this), "Textures", "BaldiTime.png");
        if (File.Exists(imagePath))
        {
            Texture2D tex = AssetLoader.TextureFromFile(imagePath);
            animationSprites = AssetLoader.SpritesFromSpritesheet(2, 1, 1, new Vector2(0.5f, 0.5f), tex);
        }
    }

    public IEnumerator BaldiTime()
    {
        HudManager hud = Object.FindObjectOfType<HudManager>();
        if (hud == null) yield break;

        GameObject flash = new GameObject("Flash");
        flash.transform.SetParent(hud.Canvas().transform, false);
        var raw = flash.AddComponent<UnityEngine.UI.RawImage>();
        raw.color = Color.white;
        RectTransform flashRect = raw.rectTransform;
        flashRect.anchorMin = Vector2.zero;
        flashRect.anchorMax = Vector2.one;
        flashRect.sizeDelta = Vector2.zero;

        float timer = 0.5f;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            raw.color = new Color(1, 1, 1, timer / 0.5f);
            yield return null;
        }
        Destroy(flash);

        GameObject animObj = new GameObject("BaldiTime");
        animObj.transform.SetParent(hud.Canvas().transform, false);

        UnityEngine.UI.Image img = animObj.AddComponent<UnityEngine.UI.Image>();
        img.sprite = animationSprites[0];

        RectTransform rect = img.rectTransform;
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        rect.sizeDelta = new Vector2(img.sprite.rect.width, img.sprite.rect.height);

        float canvasHeight = hud.Canvas().GetComponent<RectTransform>().rect.height;
        float spriteHeight = rect.sizeDelta.y;

        float startY = -(spriteHeight / 2);
        float endY = canvasHeight + (spriteHeight / 2);

        float elapsed = 0f;
        float duration = 2.5f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            rect.anchoredPosition = new Vector2(0, Mathf.Lerp(startY, endY, progress));

            int frame = (int)(elapsed * 12) % animationSprites.Length;
            img.sprite = animationSprites[frame];

            yield return null;
        }
        Destroy(animObj);
    }
}
