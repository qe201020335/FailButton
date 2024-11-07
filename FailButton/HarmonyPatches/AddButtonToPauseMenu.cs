using System.Linq;
using HarmonyLib;
using HMUI;
using Polyglot;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using IPA.Utilities;
using System.Collections;

namespace FailButton.HarmonyPatches
{
    [HarmonyPatch(typeof(PauseMenuManager), "Start")]
    static class AddButtonToPauseMenu
    {
        static Transform _b = null;

        static void Postfix(PauseMenuManager __instance)
        {
            SharedCoroutineStarter.instance.StartCoroutine(DoTheFunny(__instance));
        }

        static IEnumerator DoTheFunny(PauseMenuManager __instance)
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
            r.anchorMin = new Vector2(0.53f, 0);
            r.anchorMax = new Vector2(0.64f, 0.5f);

            _b.localPosition = new Vector3(48.69f, -2.6f, 0);

            var t = _b.GetComponentInChildren<TextMeshProUGUI>();
            t.text = "Fail";

            GameObject.DestroyImmediate(t.transform.parent.GetComponent<LayoutElement>());
            GameObject.DestroyImmediate(t.transform.parent.GetComponent<StackLayoutGroup>());

            var b = _b.GetComponent<NoTransitionsButton>();
            b.onClick.RemoveAllListeners();

            b.onClick.AddListener(() =>
            {
                var s = Resources.FindObjectsOfTypeAll<StandardLevelGameplayManager>().FirstOrDefault();

                if (s == null)
                    return;

                ReflectionUtil.GetField<StandardLevelGameplayManager.InitData, StandardLevelGameplayManager>(s, "_initData")
                    .SetField("failOn0Energy", true);

                AccessTools.Method(typeof(StandardLevelGameplayManager), "HandleGameEnergyDidReach0").Invoke(s, null);
            });

            yield return null;

            r = t.GetComponentInParent<RectTransform>();

            r.anchorMin = new Vector2(1, 0);
            r.anchorMax = new Vector2(0, 1);

            r.offsetMin = r.offsetMax = Vector2.zero;
        }
    }
}