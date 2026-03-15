using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class SetupSpellSlots
{
    public static void Execute()
    {
        for (int i = 0; i < 4; i++)
        {
            var slot = GameObject.Find($"Canvas/SpellToolbar/Slot{i}");
            if (slot == null) { Debug.LogError($"Slot{i} not found"); continue; }

            var icon = slot.transform.Find("Icon");
            var cooldown = slot.transform.Find("CooldownFill");
            var nameLabel = slot.transform.Find("NameLabel");
            var manaCost = slot.transform.Find("ManaCostLabel");
            var readyGlow = slot.transform.Find("ReadyGlow");

            // Icon - square, left side
            if (icon != null)
            {
                var rt = icon.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.05f, 0.15f);
                rt.anchorMax = new Vector2(0.45f, 0.85f);
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = Vector2.zero;
                var img = icon.GetComponent<Image>();
                img.color = new Color(0.4f, 0.4f, 0.4f, 1f);
            }

            // CooldownFill - overlays icon
            if (cooldown != null)
            {
                var rt = cooldown.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.05f, 0.15f);
                rt.anchorMax = new Vector2(0.45f, 0.85f);
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = Vector2.zero;
                var img = cooldown.GetComponent<Image>();
                img.color = new Color(0f, 0f, 0f, 0.6f);
                img.type = Image.Type.Filled;
                img.fillMethod = Image.FillMethod.Vertical;
                img.fillAmount = 0f;
            }

            // NameLabel - right side, top
            if (nameLabel != null)
            {
                var rt = nameLabel.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.98f, 0.9f);
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = Vector2.zero;
                var txt = nameLabel.GetComponent<Text>();
                txt.text = "";
                txt.fontSize = 10;
                txt.alignment = TextAnchor.MiddleCenter;
                txt.color = Color.white;
            }

            // ManaCostLabel - right side, bottom
            if (manaCost != null)
            {
                var rt = manaCost.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.1f);
                rt.anchorMax = new Vector2(0.98f, 0.5f);
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = Vector2.zero;
                var txt = manaCost.GetComponent<Text>();
                txt.text = "";
                txt.fontSize = 10;
                txt.alignment = TextAnchor.MiddleCenter;
                txt.color = new Color(0.2f, 0.4f, 0.9f, 1f);
            }

            // ReadyGlow - border overlay, starts inactive
            if (readyGlow != null)
            {
                var rt = readyGlow.GetComponent<RectTransform>();
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = Vector2.zero;
                var img = readyGlow.GetComponent<Image>();
                img.color = new Color(0.2f, 0.9f, 0.2f, 0.3f);
                img.raycastTarget = false;
                readyGlow.gameObject.SetActive(false);
            }
        }

        Debug.Log("Spell slots setup complete!");
        EditorUtility.SetDirty(GameObject.Find("Canvas/SpellToolbar"));
    }
}
