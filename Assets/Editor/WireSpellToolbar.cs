using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using RagdollRealms.UI.HUD;

public class WireSpellToolbar
{
    public static void Execute()
    {
        var toolbar = GameObject.Find("Canvas/SpellToolbar");
        if (toolbar == null) { Debug.LogError("SpellToolbar not found"); return; }

        var view = toolbar.GetComponent<SpellToolbarView>();
        if (view == null) { Debug.LogError("SpellToolbarView not found"); return; }

        var so = new SerializedObject(view);
        var slotsProp = so.FindProperty("_slots");
        slotsProp.arraySize = 4;

        for (int i = 0; i < 4; i++)
        {
            var slot = toolbar.transform.Find($"Slot{i}");
            if (slot == null) { Debug.LogError($"Slot{i} not found"); continue; }

            var element = slotsProp.GetArrayElementAtIndex(i);

            var iconProp = element.FindPropertyRelative("Icon");
            var cooldownProp = element.FindPropertyRelative("CooldownFill");
            var nameProp = element.FindPropertyRelative("NameLabel");
            var manaProp = element.FindPropertyRelative("ManaCostLabel");
            var glowProp = element.FindPropertyRelative("ReadyGlow");

            iconProp.objectReferenceValue = slot.Find("Icon")?.GetComponent<Image>();
            cooldownProp.objectReferenceValue = slot.Find("CooldownFill")?.GetComponent<Image>();
            nameProp.objectReferenceValue = slot.Find("NameLabel")?.GetComponent<Text>();
            manaProp.objectReferenceValue = slot.Find("ManaCostLabel")?.GetComponent<Text>();
            glowProp.objectReferenceValue = slot.Find("ReadyGlow")?.gameObject;
        }

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(view);
        Debug.Log("SpellToolbarView wired successfully!");
    }
}
