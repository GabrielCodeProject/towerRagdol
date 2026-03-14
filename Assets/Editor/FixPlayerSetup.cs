using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using RagdollRealms.Systems.Player;

public static class FixPlayerSetup
{
    public static string Execute()
    {
        var result = "";
        var inputAssetPath = "Assets/Resources/PlayerInputActions.inputactions";

        // === Step 1: Ensure input actions file is proper JSON ===
        var existingObj = AssetDatabase.LoadMainAssetAtPath(inputAssetPath);
        InputActionAsset inputAsset = existingObj as InputActionAsset;

        if (existingObj != null && inputAsset == null)
        {
            AssetDatabase.DeleteAsset(inputAssetPath);
            AssetDatabase.Refresh();
            existingObj = null;
            result += "Deleted broken YAML input actions. ";
        }

        if (inputAsset == null)
        {
            var tempAsset = InputActionAsset.FromJson(@"{
                ""name"": ""PlayerInputActions"",
                ""maps"": [{
                    ""name"": ""Player"",
                    ""actions"": [
                        { ""name"": ""Move"", ""type"": ""Value"", ""expectedControlType"": ""Vector2"" },
                        { ""name"": ""Jump"", ""type"": ""Button"" },
                        { ""name"": ""Sprint"", ""type"": ""Button"" },
                        { ""name"": ""Look"", ""type"": ""Value"", ""expectedControlType"": ""Vector2"" }
                    ],
                    ""bindings"": [
                        { ""path"": ""<Gamepad>/leftStick"", ""action"": ""Move"" },
                        { ""name"": ""WASD"", ""path"": ""2DVector"", ""action"": ""Move"", ""isComposite"": true },
                        { ""name"": ""up"", ""path"": ""<Keyboard>/w"", ""action"": ""Move"", ""isPartOfComposite"": true },
                        { ""name"": ""down"", ""path"": ""<Keyboard>/s"", ""action"": ""Move"", ""isPartOfComposite"": true },
                        { ""name"": ""left"", ""path"": ""<Keyboard>/a"", ""action"": ""Move"", ""isPartOfComposite"": true },
                        { ""name"": ""right"", ""path"": ""<Keyboard>/d"", ""action"": ""Move"", ""isPartOfComposite"": true },
                        { ""name"": ""Arrows"", ""path"": ""2DVector"", ""action"": ""Move"", ""isComposite"": true },
                        { ""name"": ""up"", ""path"": ""<Keyboard>/upArrow"", ""action"": ""Move"", ""isPartOfComposite"": true },
                        { ""name"": ""down"", ""path"": ""<Keyboard>/downArrow"", ""action"": ""Move"", ""isPartOfComposite"": true },
                        { ""name"": ""left"", ""path"": ""<Keyboard>/leftArrow"", ""action"": ""Move"", ""isPartOfComposite"": true },
                        { ""name"": ""right"", ""path"": ""<Keyboard>/rightArrow"", ""action"": ""Move"", ""isPartOfComposite"": true },
                        { ""path"": ""<Keyboard>/space"", ""action"": ""Jump"" },
                        { ""path"": ""<Gamepad>/buttonSouth"", ""action"": ""Jump"" },
                        { ""path"": ""<Keyboard>/leftShift"", ""action"": ""Sprint"" },
                        { ""path"": ""<Gamepad>/leftStickPress"", ""action"": ""Sprint"" },
                        { ""path"": ""<Mouse>/delta"", ""action"": ""Look"" },
                        { ""path"": ""<Gamepad>/rightStick"", ""action"": ""Look"" }
                    ]
                }]
            }");

            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");

            System.IO.File.WriteAllText(inputAssetPath, tempAsset.ToJson());
            AssetDatabase.Refresh();

            inputAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(inputAssetPath);
            if (inputAsset == null)
                return "ERROR: Failed to create input actions as JSON";

            result += "Recreated input actions as JSON. ";
        }

        // === Step 2: Find InputActionReference sub-assets from the imported file ===
        var allSubAssets = AssetDatabase.LoadAllAssetsAtPath(inputAssetPath);
        InputActionReference moveRef = null, jumpRef = null, sprintRef = null, lookRef = null;

        foreach (var sub in allSubAssets)
        {
            var actionRef = sub as InputActionReference;
            if (actionRef == null) continue;

            var actionName = actionRef.action?.name;
            if (actionName == "Move") moveRef = actionRef;
            else if (actionName == "Jump") jumpRef = actionRef;
            else if (actionName == "Sprint") sprintRef = actionRef;
            else if (actionName == "Look") lookRef = actionRef;
        }

        // If no sub-asset references exist, list what we found for debugging
        if (moveRef == null && jumpRef == null)
        {
            var debugMsg = $"Sub-assets in {inputAssetPath}: ";
            foreach (var sub in allSubAssets)
                debugMsg += $"[{sub.GetType().Name}: {sub.name}] ";
            return result + debugMsg;
        }

        // === Step 3: Assign references to PlayerInputController ===
        var player = GameObject.FindWithTag("Player");
        if (player == null)
            return result + "ERROR: No Player found in scene";

        var inputController = player.GetComponent<PlayerInputController>();
        if (inputController == null)
            return result + "ERROR: No PlayerInputController on Player";

        var so = new SerializedObject(inputController);
        if (moveRef != null) so.FindProperty("_moveAction").objectReferenceValue = moveRef;
        if (jumpRef != null) so.FindProperty("_jumpAction").objectReferenceValue = jumpRef;
        if (sprintRef != null) so.FindProperty("_sprintAction").objectReferenceValue = sprintRef;
        if (lookRef != null) so.FindProperty("_lookAction").objectReferenceValue = lookRef;
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(inputController);

        result += $"Assigned: Move={moveRef != null} Jump={jumpRef != null} Sprint={sprintRef != null} Look={lookRef != null}. ";

        // === Step 4: Regenerate Animator Controller ===
        EditorApplication.ExecuteMenuItem("Ragdoll Realms/Create Player Prefab");
        result += "Triggered animator controller regeneration.";

        return result;
    }
}
