using UnityEngine;
using UnityEditor;

namespace RagdollRealms.Editor
{
public class FixAnimImport
{
    public static void Execute()
    {
        string[] animPaths = {
            "Assets/Models/Animations/stuwalk.fbx",
            "Assets/Models/Animations/goofyinplace.fbx",
            "Assets/Models/Animations/happy_idle.fbx"
        };

        // Get the source avatar from the main model
        var blobImporter = AssetImporter.GetAtPath("Assets/Models/blob.fbx") as ModelImporter;
        if (blobImporter == null)
        {
            Debug.LogError("[FixImport] blob.fbx not found");
            return;
        }
        Debug.Log($"[FixImport] blob.fbx: animationType={blobImporter.animationType}, avatar={blobImporter.sourceAvatar?.name}");

        foreach (var path in animPaths)
        {
            var importer = AssetImporter.GetAtPath(path) as ModelImporter;
            if (importer == null)
            {
                Debug.LogWarning($"[FixImport] Not found: {path}");
                continue;
            }

            Debug.Log($"[FixImport] {path}: animationType={importer.animationType}, sourceAvatar={importer.sourceAvatar?.name}");

            bool changed = false;

            // Must be Humanoid
            if (importer.animationType != ModelImporterAnimationType.Human)
            {
                importer.animationType = ModelImporterAnimationType.Human;
                changed = true;
                Debug.Log($"[FixImport]   → Set to Humanoid");
            }

            // Use the blob model's avatar for retargeting
            if (importer.sourceAvatar != blobImporter.sourceAvatar)
            {
                importer.sourceAvatar = blobImporter.sourceAvatar;
                changed = true;
                Debug.Log($"[FixImport]   → Set sourceAvatar to {blobImporter.sourceAvatar?.name}");
            }

            if (changed)
            {
                importer.SaveAndReimport();
                Debug.Log($"[FixImport]   → Reimported {path}");
            }
            else
            {
                Debug.Log($"[FixImport]   → Already correct");
            }
        }
    }
}
}
