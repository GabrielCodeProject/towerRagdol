using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using RagdollRealms.UI.HUD;

namespace RagdollRealms.Editor
{
    public static class WireCoreAlarm
    {
        private static Transform FindChild(string rootPath, string childPath)
        {
            var root = GameObject.Find(rootPath);
            if (root == null) return null;
            return string.IsNullOrEmpty(childPath) ? root.transform : root.transform.Find(childPath);
        }

        public static void Execute()
        {
            // Position alarm panel at center of screen (overlay)
            var panelTransform = FindChild("Canvas/CoreAlarm", "");
            if (panelTransform == null) { Debug.LogError("CoreAlarm not found"); return; }
            var panel = panelTransform.gameObject;

            var panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.35f, 0.35f);
            panelRect.anchorMax = new Vector2(0.65f, 0.65f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            // Make panel transparent (just a container)
            var panelImage = panel.GetComponent<Image>();
            if (panelImage != null)
                panelImage.color = new Color(0, 0, 0, 0);

            // Direction indicator — arrow-like triangle
            var indicatorTransform = panelTransform.Find("DirectionIndicator");
            if (indicatorTransform == null) { Debug.LogError("DirectionIndicator not found"); return; }
            var indicator = indicatorTransform.gameObject;
            var indicatorRect = indicator.GetComponent<RectTransform>();
            indicatorRect.anchorMin = new Vector2(0.4f, 0.8f);
            indicatorRect.anchorMax = new Vector2(0.6f, 1f);
            indicatorRect.offsetMin = Vector2.zero;
            indicatorRect.offsetMax = Vector2.zero;
            indicatorRect.pivot = new Vector2(0.5f, 0f); // Pivot at bottom center for rotation

            var indicatorImage = indicator.GetComponent<Image>();
            indicatorImage.color = new Color(1f, 0.85f, 0.2f, 0.9f);

            // Start hidden
            indicator.SetActive(false);

            // Wire CoreAlarmView references
            var view = panel.GetComponent<CoreAlarmView>();
            if (view != null)
            {
                var so = new SerializedObject(view);
                so.FindProperty("_directionIndicator").objectReferenceValue = indicatorRect;
                so.FindProperty("_indicatorImage").objectReferenceValue = indicatorImage;
                so.FindProperty("_normalColor").colorValue = new Color(1f, 0.85f, 0.2f, 0.9f);
                so.FindProperty("_priorityColor").colorValue = new Color(1f, 0.2f, 0.2f, 0.9f);
                so.FindProperty("_displayDuration").floatValue = 3f;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            Debug.Log("[WireCoreAlarm] CoreAlarm UI wired successfully");
        }
    }
}
