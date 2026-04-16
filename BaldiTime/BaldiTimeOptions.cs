using MTM101BaldAPI.OptionsAPI;
using MTM101BaldAPI.UI;
using TMPro;
using UnityEngine;

namespace BaldiTime;

internal class BaldiTimeOptions : CustomOptionsCategory
{
    private TextMeshProUGUI musicText;
    private TextMeshProUGUI modeText;

    public override void Build()
    {
        MenuToggle menuToggle = CreateToggle("enabled", "Escape Timer", BaldiTimePlugin.Instance.ReworkEnabled, Vector3.up * 40f, 300f);
        AddTooltip(menuToggle, "When enabled, the Timer will start in the escape. Hurry Up!!");
        BaldiTimePlugin.Instance.enabledToggle = menuToggle;

        CreateText("EscapeMusicText", "Escape Music", Vector3.up * 10f, BaldiFonts.BoldComicSans12, TextAlignmentOptions.Center, new Vector2(200f, 70f), Color.black);
        CreateButton(MusicSelectionLeft, base.menuArrowLeft, base.menuArrowLeftHighlight, "MusicSelectionLeftBtn", new Vector3(-165f, -15f));
        CreateButton(MusicSelectionRight, base.menuArrowRight, base.menuArrowRightHighlight, "MusicSelectionRightBtn", new Vector3(165f, -15f));

        musicText = CreateText("MusicText", "iseeyou", Vector3.up * -15f, BaldiFonts.ComicSans24, TextAlignmentOptions.Center, new Vector2(1000f, 32f), Color.black);
        BaldiTimePlugin.Instance.musicText = musicText;

        CreateText("DifficultyText", "Difficulty", Vector3.up * -45f, BaldiFonts.BoldComicSans12, TextAlignmentOptions.Center, new Vector2(200f, 70f), Color.black);
        CreateButton(ModeSelectionLeft, base.menuArrowLeft, base.menuArrowLeftHighlight, "ModeSelectionLeftBtn", new Vector3(-60f, -70f));
        CreateButton(ModeSelectionRight, base.menuArrowRight, base.menuArrowRightHighlight, "ModeSelectionRightBtn", new Vector3(60f, -70f));

        modeText = CreateText("ModeText", BaldiTimePlugin.Instance.difficulties[BaldiTimePlugin.Instance.difficulty], Vector3.up * -70f, BaldiFonts.ComicSans24, TextAlignmentOptions.Center, new Vector2(300f, 32f), Color.black);

        CreateText("EffectTexts", "Escape Effects", Vector3.up * -100f, BaldiFonts.BoldComicSans12, TextAlignmentOptions.Center, new Vector2(200f, 70f), Color.black);

        MenuToggle menuToggle2 = CreateToggle("lights", "Red Lights", BaldiTimePlugin.Instance.LightFadeEnabled, Vector3.down * 125f, 300f);
        AddTooltip(menuToggle2, "If enabled, red lights will slowly pulse during the escape");
        BaldiTimePlugin.Instance.lightsToggle = menuToggle2;

        MenuToggle menuToggle3 = CreateToggle("shake", "Shake", BaldiTimePlugin.Instance.ShakeEnabled, Vector3.down * 160f, 300f);
        AddTooltip(menuToggle3, "If enabled, the level will shake during the escape");
        BaldiTimePlugin.Instance.shakeToggle = menuToggle3;

        BaldiTimePlugin.Instance.optionsMenuBuilt = true;
    }

    private void MusicSelectionLeft()
    {
        BaldiTimePlugin.Instance.musicIndex--;
        if (BaldiTimePlugin.Instance.musicIndex < 0)
        {
            BaldiTimePlugin.Instance.musicIndex = Mathf.Max(0, BaldiTimePlugin.Instance.midis.Count - 1);
        }
    }

    private void MusicSelectionRight()
    {
        BaldiTimePlugin.Instance.musicIndex++;
        if (BaldiTimePlugin.Instance.musicIndex > BaldiTimePlugin.Instance.midis.Count - 1)
        {
            BaldiTimePlugin.Instance.musicIndex = 0;
        }
    }

    private void ModeSelectionLeft()
    {
        BaldiTimePlugin.Instance.difficulty--;
        if (BaldiTimePlugin.Instance.difficulty < 0)
        {
            BaldiTimePlugin.Instance.difficulty = 2;
        }
        modeText.text = BaldiTimePlugin.Instance.difficulties[BaldiTimePlugin.Instance.difficulty];
    }

    private void ModeSelectionRight()
    {
        BaldiTimePlugin.Instance.difficulty++;
        if (BaldiTimePlugin.Instance.difficulty > 2)
        {
            BaldiTimePlugin.Instance.difficulty = 0;
        }
        modeText.text = BaldiTimePlugin.Instance.difficulties[BaldiTimePlugin.Instance.difficulty];
    }
}
