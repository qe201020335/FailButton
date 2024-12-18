﻿using System.Linq;
using HarmonyLib;
using HMUI;
using BGLib.Polyglot;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using IPA.Utilities;
using System.Collections;

namespace FailButton.HarmonyPatches
{
    [HarmonyPatch(typeof(PauseMenuManager), nameof(PauseMenuManager.Start))]
    static class AddButtonToPauseMenu
    {
        static Transform _b = null;

        static void Postfix(PauseMenuManager __instance)
        {
            var gameplayManager = Resources.FindObjectsOfTypeAll<StandardLevelGameplayManager>().FirstOrDefault();
            if (gameplayManager != null) __instance.StartCoroutine(DoTheFunny(__instance, gameplayManager));
        }

        static IEnumerator DoTheFunny(PauseMenuManager __instance, StandardLevelGameplayManager gameplayManager)
        {
            if (_b != null)
                yield break;

            yield return null;

            var c = __instance.transform.Find("Wrapper/MenuWrapper/Canvas/MainBar/Buttons");

            if (c == null || c.parent == null || c.childCount == 0 || !c.GetChild(0).gameObject.activeInHierarchy)
                yield break;

            _b = GameObject.Instantiate(c.GetChild(0), c.parent.GetChild(0));
            GameObject.Destroy(_b.GetComponentInChildren<LocalizedTextMeshProUGUI>());

            var r = _b.GetComponent<RectTransform>();
            r.anchorMin = new Vector2(0.22f, 0.25f);
            r.anchorMax = new Vector2(0.33f, 0.75f);

            _b.localPosition = new Vector3(48.69f, -2.6f, 0);

            var t = _b.GetComponentInChildren<TextMeshProUGUI>();
            t.text = "Fail";

            GameObject.DestroyImmediate(t.transform.parent.GetComponent<LayoutElement>());
            GameObject.DestroyImmediate(t.transform.parent.GetComponent<StackLayoutGroup>());

            var b = _b.GetComponent<NoTransitionsButton>();
            b.onClick.RemoveAllListeners();

            b.onClick.AddListener(() =>
            {
                gameplayManager._initData.SetField(nameof(gameplayManager._initData.continueGameplayWith0Energy), false);
                gameplayManager.HandleGameEnergyDidReach0();
            });

            yield return null;

            r = t.GetComponentInParent<RectTransform>();

            r.anchorMin = new Vector2(0, 0);
            r.anchorMax = new Vector2(1, 1);

            r.offsetMin = r.offsetMax = Vector2.zero;
        }
    }
}