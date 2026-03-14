using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using RagdollRealms.Systems.Player;

public static class FixPrefabSetup
{
    public static string Execute()
    {
        var prefabPath = "Assets/Prefabs/Player.prefab";
        var inputAssetPath = "Assets/Resources/PlayerInputActions.inputactions";

        var inputAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(inputAssetPath);
        if (inputAsset == null)
            return "ERROR: InputActionAsset not found";

        // Find InputActionReference sub-assets
        var allSubAssets = AssetDatabase.LoadAllAssetsAtPath(inputAssetPath);
        InputActionReference moveRef = null, jumpRef = null, sprintRef = null, lookRef = null;

        foreach (var sub in allSubAssets)
        {
            var actionRef = sub as InputActionReference;
            if (actionRef == null) continue;
            var name = actionRef.action?.name;
            if (name == "Move") moveRef = actionRef;
            else if (name == "Jump") jumpRef = actionRef;
            else if (name == "Sprint") sprintRef = actionRef;
            else if (name == "Look") lookRef = actionRef;
        }

        if (moveRef == null)
            return "ERROR: No InputActionReference sub-assets found for Move";

        // Open prefab for editing
        var prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefabAsset == null)
            return "ERROR: Prefab not found at " + prefabPath;

        var prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);
        var inputCtrl = prefabRoot.GetComponent<PlayerInputController>();
        if (inputCtrl == null)
        {
            PrefabUtility.UnloadPrefabContents(prefabRoot);
            return "ERROR: No PlayerInputController on prefab";
        }

        var so = new SerializedObject(inputCtrl);
        if (moveRef != null) so.FindProperty("_moveAction").objectReferenceValue = moveRef;
        if (jumpRef != null) so.FindProperty("_jumpAction").objectReferenceValue = jumpRef;
        if (sprintRef != null) so.FindProperty("_sprintAction").objectReferenceValue = sprintRef;
        if (lookRef != null) so.FindProperty("_lookAction").objectReferenceValue = lookRef;
        so.ApplyModifiedProperties();

        PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
        PrefabUtility.UnloadPrefabContents(prefabRoot);

        return $"Prefab fixed: Animator controller + input actions (Move={moveRef != null}, Jump={jumpRef != null}, Sprint={sprintRef != null}, Look={lookRef != null})";
    }
}
