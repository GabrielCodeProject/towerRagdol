using UnityEngine;
using UnityEditor;

namespace RagdollRealms.Editor
{
public class FixAnimationLooping
{
    public static void Execute()
    {
        string[] paths = {
            "Assets/Models/Animations/happy_idle.fbx",
            "Assets/Models/Animations/stuwalk.fbx",
            "Assets/Models/Animations/goofyinplace.fbx",
            "Assets/Models/blob.fbx"
        };

        foreach (var path in paths)
        {
            var importer = AssetImporter.GetAtPath(path) as ModelImporter;
            if (importer == null)
            {
                Debug.LogWarning($"[FixAnim] No importer for {path}");
                continue;
            }

            var clips = importer.clipAnimations;
            if (clips.Length == 0)
            {
                // Use default clip animations as base
                clips = importer.defaultClipAnimations;
            }

            bool changed = false;
            foreach (var clip in clips)
            {
                if (!clip.loopTime)
                {
                    clip.loopTime = true;
                    clip.loopPose = true;
                    changed = true;
                    Debug.Log($"[FixAnim] Set looping on '{clip.name}' in {path}");
                }
                else
                {
                    Debug.Log($"[FixAnim] '{clip.name}' in {path} already loops");
                }
            }

            if (changed)
            {
                importer.clipAnimations = clips;
                importer.SaveAndReimport();
                Debug.Log($"[FixAnim] Reimported {path}");
            }
        }
    }
}
}
