using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using RagdollRealms.UI.HUD;

namespace RagdollRealms.Editor
{
    public static class WireCoreHpBar
    {
        private static Transform FindChild(string rootPath, string childPath)
        {
            var root = GameObject.Find(rootPath);
            if (root == null) return null;
            return string.IsNullOrEmpty(childPath) ? root.transform : root.transform.Find(childPath);
        }

        public static void Execute()
        {
            // Position CoreHpBar panel at top-center
            var panelTransform = FindChild("Canvas/CoreHpBar", "");
            if (panelTransform == null) { Debug.LogError("CoreHpBar not found"); return; }
            var panel = panelTransform.gameObject;

            var panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.3f, 0.92f);
            panelRect.anchorMax = new Vector2(0.7f, 0.98f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            // Set panel color to dark semi-transparent
            var panelImage = panel.GetComponent<Image>();
            if (panelImage != null)
                panelImage.color = new Color(0.1f, 0.1f, 0.15f, 0.85f);

            // HP Bar Background
            var hpBgTransform = panelTransform.Find("HpBarBg");
            if (hpBgTransform == null) { Debug.LogError("HpBarBg not found"); return; }
            var hpBg = hpBgTransform.gameObject;
            var hpBgRect = hpBg.GetComponent<RectTransform>();
            hpBgRect.anchorMin = new Vector2(0.05f, 0.2f);
            hpBgRect.anchorMax = new Vector2(0.75f, 0.8f);
            hpBgRect.offsetMin = Vector2.zero;
            hpBgRect.offsetMax = Vector2.zero;
            hpBg.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);

            // HP Bar Fill
            var hpFillTransform = panelTransform.Find("HpBarFill");
            if (hpFillTransform == null) { Debug.LogError("HpBarFill not found"); return; }
            var hpFill = hpFillTransform.gameObject;
            var hpFillRect = hpFill.GetComponent<RectTransform>();
            hpFillRect.anchorMin = new Vector2(0.05f, 0.2f);
            hpFillRect.anchorMax = new Vector2(0.75f, 0.8f);
            hpFillRect.offsetMin = Vector2.zero;
            hpFillRect.offsetMax = Vector2.zero;
            var hpFillImage = hpFill.GetComponent<Image>();
            hpFillImage.color = new Color(0.2f, 0.8f, 0.3f, 1f);
            hpFillImage.type = Image.Type.Filled;
            hpFillImage.fillMethod = Image.FillMethod.Horizontal;
            hpFillImage.fillAmount = 1f;

            // Shield Bar Fill (overlay, slightly smaller)
            var shieldFillTransform = panelTransform.Find("ShieldBarFill");
            if (shieldFillTransform == null) { Debug.LogError("ShieldBarFill not found"); return; }
            var shieldFill = shieldFillTransform.gameObject;
            var shieldRect = shieldFill.GetComponent<RectTransform>();
            shieldRect.anchorMin = new Vector2(0.05f, 0.2f);
            shieldRect.anchorMax = new Vector2(0.75f, 0.8f);
            shieldRect.offsetMin = Vector2.zero;
            shieldRect.offsetMax = Vector2.zero;
            var shieldImage = shieldFill.GetComponent<Image>();
            shieldImage.color = new Color(0.3f, 0.6f, 1f, 0.5f);
            shieldImage.type = Image.Type.Filled;
            shieldImage.fillMethod = Image.FillMethod.Horizontal;
            shieldImage.fillAmount = 0f;

            // HP Label
            var hpLabelTransform = panelTransform.Find("HpLabel");
            if (hpLabelTransform == null) { Debug.LogError("HpLabel not found"); return; }
            var hpLabel = hpLabelTransform.gameObject;
            var hpLabelRect = hpLabel.GetComponent<RectTransform>();
            hpLabelRect.anchorMin = new Vector2(0.05f, 0.2f);
            hpLabelRect.anchorMax = new Vector2(0.75f, 0.8f);
            hpLabelRect.offsetMin = Vector2.zero;
            hpLabelRect.offsetMax = Vector2.zero;
            var hpText = hpLabel.GetComponent<Text>();
            hpText.text = "1000/1000";
            hpText.alignment = TextAnchor.MiddleCenter;
            hpText.fontSize = 14;
            hpText.color = Color.white;
            hpText.fontStyle = FontStyle.Bold;

            // Percent Label
            var percentLabelTransform = panelTransform.Find("PercentLabel");
            if (percentLabelTransform == null) { Debug.LogError("PercentLabel not found"); return; }
            var percentLabel = percentLabelTransform.gameObject;
            var percentRect = percentLabel.GetComponent<RectTransform>();
            percentRect.anchorMin = new Vector2(0.78f, 0.2f);
            percentRect.anchorMax = new Vector2(0.95f, 0.8f);
            percentRect.offsetMin = Vector2.zero;
            percentRect.offsetMax = Vector2.zero;
            var percentText = percentLabel.GetComponent<Text>();
            percentText.text = "100%";
            percentText.alignment = TextAnchor.MiddleCenter;
            percentText.fontSize = 16;
            percentText.color = Color.white;
            percentText.fontStyle = FontStyle.Bold;

            // Wire CoreHpView references
            var view = panel.GetComponent<CoreHpView>();
            if (view != null)
            {
                var so = new SerializedObject(view);
                so.FindProperty("_hpBarFill").objectReferenceValue = hpFillImage;
                so.FindProperty("_shieldBarFill").objectReferenceValue = shieldImage;
                so.FindProperty("_hpLabel").objectReferenceValue = hpText;
                so.FindProperty("_percentLabel").objectReferenceValue = percentText;
                so.FindProperty("_healthyColor").colorValue = new Color(0.2f, 0.8f, 0.3f, 1f);
                so.FindProperty("_warningColor").colorValue = new Color(1f, 0.85f, 0.2f, 1f);
                so.FindProperty("_criticalColor").colorValue = new Color(1f, 0.2f, 0.2f, 1f);
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            Debug.Log("[WireCoreHpBar] CoreHpBar UI wired successfully");
        }
    }
}
