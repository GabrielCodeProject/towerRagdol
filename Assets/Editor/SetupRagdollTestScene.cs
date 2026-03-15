using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using RagdollRealms.Systems.Ragdoll;

namespace RagdollRealms.Editor
{
    public static class SetupRagdollTestScene
    {
        // Mixamo bone suffix -> mapped name (works with any prefix: mixamorig:, mixamorig1:, etc.)
        private static readonly Dictionary<string, string> MixamoBoneSuffixMap = new Dictionary<string, string>
        {
            { "Hips", "Hips" },
            { "Spine", "Spine" },
            { "Spine1", "Spine1" },
            { "Spine2", "Spine2" },
            { "Head", "Head" },
            { "Neck", "Neck" },
            { "LeftShoulder", "LeftShoulder" },
            { "LeftArm", "LeftUpperArm" },
            { "LeftForeArm", "LeftLowerArm" },
            { "LeftHand", "LeftHand" },
            { "RightShoulder", "RightShoulder" },
            { "RightArm", "RightUpperArm" },
            { "RightForeArm", "RightLowerArm" },
            { "RightHand", "RightHand" },
            { "LeftUpLeg", "LeftUpperLeg" },
            { "LeftLeg", "LeftLowerLeg" },
            { "LeftFoot", "LeftFoot" },
            { "RightUpLeg", "RightUpperLeg" },
            { "RightLeg", "RightLowerLeg" },
            { "RightFoot", "RightFoot" },
        };

        // Bones that get ragdoll physics (skip shoulders, hands, feet for simpler ragdoll)
        private static readonly HashSet<string> RagdollBones = new HashSet<string>
        {
            "Hips", "Spine1", "Head",
            "LeftUpperArm", "LeftLowerArm",
            "RightUpperArm", "RightLowerArm",
            "LeftUpperLeg", "LeftLowerLeg", "LeftFoot",
            "RightUpperLeg", "RightLowerLeg", "RightFoot",
        };

        // Parent mapping for joints (child -> parent)
        private static readonly Dictionary<string, string> JointParentMap = new Dictionary<string, string>
        {
            { "Spine1", "Hips" },
            { "Head", "Spine1" },
            { "LeftUpperArm", "Spine1" },
            { "LeftLowerArm", "LeftUpperArm" },
            { "RightUpperArm", "Spine1" },
            { "RightLowerArm", "RightUpperArm" },
            { "LeftUpperLeg", "Hips" },
            { "LeftLowerLeg", "LeftUpperLeg" },
            { "LeftFoot", "LeftLowerLeg" },
            { "RightUpperLeg", "Hips" },
            { "RightLowerLeg", "RightUpperLeg" },
            { "RightFoot", "RightLowerLeg" },
        };

        // Collider sizes per bone (half-extents for capsules: radius, height)
        private static readonly Dictionary<string, Vector2> BoneColliderSizes = new Dictionary<string, Vector2>
        {
            { "Hips", new Vector2(0.12f, 0.2f) },
            { "Spine1", new Vector2(0.12f, 0.25f) },
            { "Head", new Vector2(0.1f, 0.16f) },
            { "LeftUpperArm", new Vector2(0.05f, 0.25f) },
            { "LeftLowerArm", new Vector2(0.04f, 0.22f) },
            { "RightUpperArm", new Vector2(0.05f, 0.25f) },
            { "RightLowerArm", new Vector2(0.04f, 0.22f) },
            { "LeftUpperLeg", new Vector2(0.06f, 0.35f) },
            { "LeftLowerLeg", new Vector2(0.05f, 0.35f) },
            { "RightUpperLeg", new Vector2(0.06f, 0.35f) },
            { "RightLowerLeg", new Vector2(0.05f, 0.35f) },
            { "LeftFoot", new Vector2(0.04f, 0.18f) },
            { "RightFoot", new Vector2(0.04f, 0.18f) },
        };

        // Mass per bone
        private static readonly Dictionary<string, float> BoneMasses = new Dictionary<string, float>
        {
            { "Hips", 5f },
            { "Spine1", 3f },
            { "Head", 1.5f },
            { "LeftUpperArm", 1f },
            { "LeftLowerArm", 0.8f },
            { "RightUpperArm", 1f },
            { "RightLowerArm", 0.8f },
            { "LeftUpperLeg", 1.5f },
            { "LeftLowerLeg", 1f },
            { "RightUpperLeg", 1.5f },
            { "RightLowerLeg", 1f },
            { "LeftFoot", 0.8f },
            { "RightFoot", 0.8f },
        };

        // Capsule direction per bone (0=X, 1=Y, 2=Z)
        private static readonly Dictionary<string, int> BoneCapsuleDirection = new Dictionary<string, int>
        {
            { "Hips", 0 },        // wide
            { "Spine1", 0 },      // wide
            { "Head", 1 },        // tall
            { "LeftUpperArm", 0 },
            { "LeftLowerArm", 0 },
            { "RightUpperArm", 0 },
            { "RightLowerArm", 0 },
            { "LeftUpperLeg", 1 },
            { "LeftLowerLeg", 1 },
            { "RightUpperLeg", 1 },
            { "RightLowerLeg", 1 },
            { "LeftFoot", 2 },    // along Z (forward)
            { "RightFoot", 2 },   // along Z (forward)
        };

        // Joint spring drive constants
        private const float JointPositionSpring = 1500f;
        private const float JointPositionDamper = 100f;
        private const float JointMaximumForce = 3000f;

        [MenuItem("Ragdoll Realms/Setup Ragdoll Test Scene")]
        public static void Execute()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            // ========== 1. MANAGERS ==========
            CreateManagers();

            // ========== 2. CREATE RAGDOLL CONFIG SO ==========
            var config = CreateOrLoadConfig();

            // ========== 3. GROUND PLANE ==========
            CreateGround();

            // ========== 4. RAGDOLL CHARACTER ==========
            GameObject ragdollRoot;
            GameObject fbxModel = FindMixamoModel();

            if (fbxModel != null)
            {
                Debug.Log($"[SetupRagdollTestScene] Found Mixamo model: {AssetDatabase.GetAssetPath(fbxModel)}");
                ragdollRoot = BuildRagdollFromModel(fbxModel, config);
            }
            else
            {
                Debug.Log("[SetupRagdollTestScene] No FBX model found in Assets/Models/. Using primitive cubes.");
                Debug.Log("[SetupRagdollTestScene] To use your Mixamo character, place the .fbx in Assets/Models/");
                ragdollRoot = BuildPrimitiveRagdoll(config);
            }

            // ========== 5. ADD RAGDOLL SYSTEM COMPONENTS ==========
            AttachRagdollComponents(ragdollRoot, config);

            // ========== 6. TARGET DUMMY ==========
            var dummy = UnityEngine.Object.Instantiate(ragdollRoot);
            dummy.name = "TargetDummy";
            dummy.transform.position = new Vector3(3f, 1.2f, 0);

            // ========== 7. POSITION CAMERA ==========
            var cam = Camera.main;
            if (cam != null)
            {
                cam.transform.position = new Vector3(1.5f, 2f, -5f);
                cam.transform.LookAt(new Vector3(1.5f, 1f, 0));
            }

            // ========== 8. SAVE ==========
            EditorSceneManager.SaveScene(scene, "Assets/Scenes/RagdollTestScene.unity");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[SetupRagdollTestScene] Test scene created at Assets/Scenes/RagdollTestScene.unity");
            Debug.Log("[SetupRagdollTestScene] Press SPACE=push, F=explosion, R=reset position, G=grapple test");
        }

        // ==================== MODEL DETECTION ====================

        private static GameObject FindMixamoModel()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Models"))
                return null;

            var guids = AssetDatabase.FindAssets("t:Model", new[] { "Assets/Models" });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (!path.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase)) continue;

                // Skip animation files in subfolders — only use root-level models
                if (path.Replace("Assets/Models/", "").Contains("/")) continue;

                var model = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (model == null) continue;

                var hips = FindChildRecursive(model.transform, name => MatchesBoneSuffix(name, "Hips"));
                if (hips != null)
                    return model;

                Debug.LogWarning($"[SetupRagdollTestScene] Found FBX at {path} but no 'Hips' bone detected. " +
                                 "Make sure the model uses Mixamo naming (mixamorig:Hips).");
            }

            return null;
        }

        private static Transform FindChildRecursive(Transform parent, Func<string, bool> predicate)
        {
            if (predicate(parent.name))
                return parent;

            for (int i = 0; i < parent.childCount; i++)
            {
                var result = FindChildRecursive(parent.GetChild(i), predicate);
                if (result != null) return result;
            }
            return null;
        }

        private static bool MatchesBoneSuffix(string name, string suffix)
        {
            return name == suffix || (name.Contains(":") && name.Substring(name.LastIndexOf(':') + 1) == suffix);
        }

        private static bool MatchesExactName(string name, string target)
        {
            return name == target;
        }

        // ==================== MODEL-BASED RAGDOLL ====================

        private static GameObject BuildRagdollFromModel(GameObject fbxPrefab, RagdollRealms.Content.RagdollConfigDefinition config)
        {
            fbxPrefab = EnsureHumanoidRig(fbxPrefab);

            var ragdollRoot = (GameObject)PrefabUtility.InstantiatePrefab(fbxPrefab);
            ragdollRoot.name = "TestRagdoll";
            ragdollRoot.transform.position = new Vector3(0, 1.2f, 0);

            var boneTransforms = new Dictionary<string, Transform>();
            BuildBoneLookup(ragdollRoot.transform, boneTransforms);

            if (!boneTransforms.ContainsKey("Hips"))
            {
                Debug.LogError("[SetupRagdollTestScene] Could not find Hips bone in model!");
                return ragdollRoot;
            }

            var boneRigidbodies = AddRagdollPhysics(boneTransforms);

            // Add MeleeCombatController on lower arms
            if (boneTransforms.ContainsKey("LeftLowerArm"))
                boneTransforms["LeftLowerArm"].gameObject.AddComponent<RagdollRealms.Systems.Combat.MeleeCombatController>();
            if (boneTransforms.ContainsKey("RightLowerArm"))
                boneTransforms["RightLowerArm"].gameObject.AddComponent<RagdollRealms.Systems.Combat.MeleeCombatController>();

            SetupAnimator(ragdollRoot);

            Debug.Log($"[SetupRagdollTestScene] Rigged {boneRigidbodies.Count} bones with ragdoll physics.");
            return ragdollRoot;
        }

        private static GameObject EnsureHumanoidRig(GameObject fbxPrefab)
        {
            var path = AssetDatabase.GetAssetPath(fbxPrefab);
            var importer = AssetImporter.GetAtPath(path) as ModelImporter;
            if (importer != null && importer.animationType != ModelImporterAnimationType.Human)
            {
                importer.animationType = ModelImporterAnimationType.Human;
                importer.SaveAndReimport();
                fbxPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            return fbxPrefab;
        }

        private static Dictionary<string, Rigidbody> AddRagdollPhysics(Dictionary<string, Transform> boneTransforms)
        {
            var boneRigidbodies = new Dictionary<string, Rigidbody>();

            foreach (var boneName in RagdollBones)
            {
                if (!boneTransforms.ContainsKey(boneName))
                {
                    Debug.LogWarning($"[SetupRagdollTestScene] Bone '{boneName}' not found in model, skipping.");
                    continue;
                }

                var go = boneTransforms[boneName].gameObject;

                var rb = go.AddComponent<Rigidbody>();
                rb.mass = BoneMasses.GetValueOrDefault(boneName, 1f);
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                boneRigidbodies[boneName] = rb;

                var capsule = go.AddComponent<CapsuleCollider>();
                var size = BoneColliderSizes.GetValueOrDefault(boneName, new Vector2(0.05f, 0.2f));
                capsule.radius = size.x;
                capsule.height = size.y;
                capsule.direction = BoneCapsuleDirection.GetValueOrDefault(boneName, 1);
            }

            foreach (var kvp in JointParentMap)
            {
                if (!boneRigidbodies.ContainsKey(kvp.Key) || !boneRigidbodies.ContainsKey(kvp.Value))
                    continue;

                CreateJoint(boneTransforms[kvp.Key].gameObject, boneRigidbodies[kvp.Value]);
            }

            return boneRigidbodies;
        }

        // ==================== ANIMATOR SETUP ====================

        private static void SetupAnimator(GameObject ragdollRoot)
        {
            var idleClip = FindIdleClip();
            if (idleClip == null)
            {
                Debug.LogWarning("[SetupRagdollTestScene] No animation clip found in Assets/Models/Animations/. Ragdoll will hold T-pose.");
                return;
            }

            idleClip = EnsureClipIsHumanoidAndLooping(idleClip);
            CreateAndAssignAnimatorController(ragdollRoot, idleClip);
            Debug.Log($"[SetupRagdollTestScene] Animator set up with idle clip: {idleClip.name}");
        }

        private static AnimationClip FindIdleClip()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Models/Animations"))
                return null;

            var guids = AssetDatabase.FindAssets("t:AnimationClip", new[] { "Assets/Models/Animations" });
            foreach (var guid in guids)
            {
                var clipPath = AssetDatabase.GUIDToAssetPath(guid);
                var clip = LoadFirstAnimationClip(clipPath);
                if (clip != null) return clip;
            }
            return null;
        }

        private static AnimationClip EnsureClipIsHumanoidAndLooping(AnimationClip clip)
        {
            var clipPath = AssetDatabase.GetAssetPath(clip);
            var importer = AssetImporter.GetAtPath(clipPath) as ModelImporter;
            if (importer == null) return clip;

            bool reimport = false;

            if (importer.animationType != ModelImporterAnimationType.Human)
            {
                importer.animationType = ModelImporterAnimationType.Human;
                reimport = true;
            }

            var clipAnimations = importer.clipAnimations;
            if (clipAnimations.Length == 0)
                clipAnimations = importer.defaultClipAnimations;

            foreach (var c in clipAnimations)
            {
                if (!c.loopTime)
                {
                    c.loopTime = true;
                    reimport = true;
                }
            }

            if (reimport)
            {
                importer.clipAnimations = clipAnimations;
                importer.SaveAndReimport();
                clip = LoadFirstAnimationClip(clipPath) ?? clip;
            }

            return clip;
        }

        private static void CreateAndAssignAnimatorController(GameObject ragdollRoot, AnimationClip idleClip)
        {
            var controllerPath = "Assets/Resources/RagdollAnimator.controller";
            var controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
            var rootStateMachine = controller.layers[0].stateMachine;
            var idleState = rootStateMachine.AddState("Idle");
            idleState.motion = idleClip;
            rootStateMachine.defaultState = idleState;

            var animator = ragdollRoot.GetComponent<Animator>();
            if (animator == null)
                animator = ragdollRoot.AddComponent<Animator>();
            animator.runtimeAnimatorController = controller;
            animator.applyRootMotion = false;
        }

        private static AnimationClip LoadFirstAnimationClip(string assetPath)
        {
            var assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            foreach (var asset in assets)
            {
                if (asset is AnimationClip clip && !clip.name.StartsWith("__preview__"))
                    return clip;
            }
            return null;
        }

        // ==================== BONE LOOKUP ====================

        private static void BuildBoneLookup(Transform root, Dictionary<string, Transform> lookup)
        {
            var name = root.name;
            var suffix = name.Contains(":") ? name.Substring(name.LastIndexOf(':') + 1) : name;

            if (MixamoBoneSuffixMap.TryGetValue(suffix, out var mappedName))
            {
                lookup[mappedName] = root;
            }
            else if (RagdollBones.Contains(name) && !lookup.ContainsKey(name))
            {
                lookup[name] = root;
            }

            for (int i = 0; i < root.childCount; i++)
            {
                BuildBoneLookup(root.GetChild(i), lookup);
            }
        }

        // ==================== PRIMITIVE RAGDOLL (FALLBACK) ====================

        private static GameObject BuildPrimitiveRagdoll(RagdollRealms.Content.RagdollConfigDefinition config)
        {
            var ragdollRoot = new GameObject("TestRagdoll");
            ragdollRoot.transform.position = new Vector3(0, 3f, 0);

            // --- Hip (root body) ---
            var hip = CreateBodyPart("Hip", ragdollRoot.transform, Vector3.zero, new Vector3(0.4f, 0.2f, 0.3f), 5f);

            // --- Torso ---
            var torso = CreateBodyPart("Torso", hip.transform, new Vector3(0, 0.4f, 0), new Vector3(0.4f, 0.4f, 0.25f), 3f);
            CreateJoint(torso, hip.GetComponent<Rigidbody>());

            // --- Head ---
            var head = CreateBodyPartSphere("Head", torso.transform, new Vector3(0, 0.4f, 0), 0.15f, 1.5f);
            CreateJoint(head, torso.GetComponent<Rigidbody>());

            // --- Left Upper Arm ---
            var lUpperArm = CreateBodyPart("LeftUpperArm", torso.transform, new Vector3(-0.4f, 0.2f, 0), new Vector3(0.3f, 0.1f, 0.1f), 1f);
            CreateJoint(lUpperArm, torso.GetComponent<Rigidbody>());

            // --- Left Lower Arm ---
            var lLowerArm = CreateBodyPart("LeftLowerArm", lUpperArm.transform, new Vector3(-0.3f, 0, 0), new Vector3(0.25f, 0.08f, 0.08f), 0.8f);
            CreateJoint(lLowerArm, lUpperArm.GetComponent<Rigidbody>());
            lLowerArm.AddComponent<RagdollRealms.Systems.Combat.MeleeCombatController>();

            // --- Right Upper Arm ---
            var rUpperArm = CreateBodyPart("RightUpperArm", torso.transform, new Vector3(0.4f, 0.2f, 0), new Vector3(0.3f, 0.1f, 0.1f), 1f);
            CreateJoint(rUpperArm, torso.GetComponent<Rigidbody>());

            // --- Right Lower Arm ---
            var rLowerArm = CreateBodyPart("RightLowerArm", rUpperArm.transform, new Vector3(0.3f, 0, 0), new Vector3(0.25f, 0.08f, 0.08f), 0.8f);
            CreateJoint(rLowerArm, rUpperArm.GetComponent<Rigidbody>());
            rLowerArm.AddComponent<RagdollRealms.Systems.Combat.MeleeCombatController>();

            // --- Left Upper Leg ---
            var lUpperLeg = CreateBodyPart("LeftUpperLeg", hip.transform, new Vector3(-0.15f, -0.35f, 0), new Vector3(0.12f, 0.3f, 0.12f), 1.5f);
            CreateJoint(lUpperLeg, hip.GetComponent<Rigidbody>());

            // --- Left Lower Leg ---
            var lLowerLeg = CreateBodyPart("LeftLowerLeg", lUpperLeg.transform, new Vector3(0, -0.35f, 0), new Vector3(0.1f, 0.3f, 0.1f), 1f);
            CreateJoint(lLowerLeg, lUpperLeg.GetComponent<Rigidbody>());

            // --- Right Upper Leg ---
            var rUpperLeg = CreateBodyPart("RightUpperLeg", hip.transform, new Vector3(0.15f, -0.35f, 0), new Vector3(0.12f, 0.3f, 0.12f), 1.5f);
            CreateJoint(rUpperLeg, hip.GetComponent<Rigidbody>());

            // --- Right Lower Leg ---
            var rLowerLeg = CreateBodyPart("RightLowerLeg", rUpperLeg.transform, new Vector3(0, -0.35f, 0), new Vector3(0.1f, 0.3f, 0.1f), 1f);
            CreateJoint(rLowerLeg, rUpperLeg.GetComponent<Rigidbody>());

            return ragdollRoot;
        }

        // ==================== SHARED SETUP ====================

        private static void CreateManagers()
        {
            var managersRoot = new GameObject("--- Managers ---");

            var slGO = new GameObject("ServiceLocator");
            slGO.transform.SetParent(managersRoot.transform);
            slGO.AddComponent<RagdollRealms.Core.ServiceLocator>();

            var ebGO = new GameObject("EventBus");
            ebGO.transform.SetParent(managersRoot.transform);
            ebGO.AddComponent<RagdollRealms.Core.EventBus>();

            var pmGO = new GameObject("PoolManager");
            pmGO.transform.SetParent(managersRoot.transform);
            pmGO.AddComponent<RagdollRealms.Core.PoolManager>();

            var pbGO = new GameObject("PerformanceBudgetManager");
            pbGO.transform.SetParent(managersRoot.transform);
            pbGO.AddComponent<RagdollRealms.Core.PerformanceBudgetManager>();

            var crGO = new GameObject("ContentRegistryManager");
            crGO.transform.SetParent(managersRoot.transform);
            crGO.AddComponent<RagdollRealms.Systems.ContentRegistryManager>();
        }

        private static RagdollRealms.Content.RagdollConfigDefinition CreateOrLoadConfig()
        {
            var configPath = "Assets/Resources/RagdollConfigs";
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");
            if (!AssetDatabase.IsValidFolder(configPath))
                AssetDatabase.CreateFolder("Assets/Resources", "RagdollConfigs");

            var existing = AssetDatabase.LoadAssetAtPath<RagdollRealms.Content.RagdollConfigDefinition>(
                configPath + "/DefaultRagdollConfig.asset");
            if (existing != null) return existing;

            var config = ScriptableObject.CreateInstance<RagdollRealms.Content.RagdollConfigDefinition>();
            AssetDatabase.CreateAsset(config, configPath + "/DefaultRagdollConfig.asset");

            var so = new SerializedObject(config);
            so.FindProperty("_id").stringValue = "default_ragdoll";
            so.FindProperty("_displayName").stringValue = "Default Ragdoll Config";
            so.FindProperty("_description").stringValue = "Default ragdoll physics configuration for testing";
            so.ApplyModifiedPropertiesWithoutUndo();

            return config;
        }

        private static void CreateGround()
        {
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(5, 1, 5);
            var groundMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            groundMat.color = new Color(0.3f, 0.5f, 0.3f);
            ground.GetComponent<MeshRenderer>().sharedMaterial = groundMat;
        }

        private static void AttachRagdollComponents(GameObject ragdollRoot, RagdollRealms.Content.RagdollConfigDefinition config)
        {
            var hipRb = FindHipRigidbody(ragdollRoot);

            var rc = ragdollRoot.AddComponent<RagdollRealms.Systems.Ragdoll.RagdollController>();
            var rcSO = new SerializedObject(rc);
            rcSO.FindProperty("_config").objectReferenceValue = config;
            rcSO.FindProperty("_hipRigidbody").objectReferenceValue = hipRb;
            rcSO.ApplyModifiedPropertiesWithoutUndo();

            ragdollRoot.AddComponent<RagdollRealms.Systems.Ragdoll.AnimationFollower>();
            ragdollRoot.AddComponent<RagdollRealms.Systems.Ragdoll.RagdollForceReceiver>();
            ragdollRoot.AddComponent<RagdollRealms.Systems.Ragdoll.RagdollRecoveryController>();
            ragdollRoot.AddComponent<RagdollRealms.Systems.Ragdoll.RagdollLODController>();

            var collisionHandler = ragdollRoot.AddComponent<RagdollCollisionHandler>();
            var joints = ragdollRoot.GetComponentsInChildren<ConfigurableJoint>();
            for (int i = 0; i < joints.Length; i++)
            {
                var reporter = joints[i].gameObject.AddComponent<BoneCollisionReporter>();
                reporter.Initialize(collisionHandler, i, ragdollRoot.transform);
            }

            ragdollRoot.AddComponent<RagdollForceTestHelper>();
        }

        private static Rigidbody FindHipRigidbody(GameObject root)
        {
            var hipNames = new[] { "Hip", "Hips" };
            foreach (var name in hipNames)
            {
                var found = FindChildRecursive(root.transform, n => MatchesBoneSuffix(n, name));
                if (found != null)
                {
                    var rb = found.GetComponent<Rigidbody>();
                    if (rb != null) return rb;
                }
            }
            return root.GetComponentInChildren<Rigidbody>();
        }

        // ==================== PRIMITIVE HELPERS ====================

        private static GameObject CreateBodyPart(string name, Transform parent, Vector3 localPos, Vector3 scale, float mass)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent);
            go.transform.localPosition = localPos;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = scale;

            var rb = go.GetComponent<Rigidbody>();
            if (rb == null) rb = go.AddComponent<Rigidbody>();
            rb.mass = mass;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = new Color(0.8f, 0.6f, 0.4f);
            go.GetComponent<MeshRenderer>().sharedMaterial = mat;

            return go;
        }

        private static GameObject CreateBodyPartSphere(string name, Transform parent, Vector3 localPos, float radius, float mass)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = name;
            go.transform.SetParent(parent);
            go.transform.localPosition = localPos;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one * radius * 2f;

            var rb = go.GetComponent<Rigidbody>();
            if (rb == null) rb = go.AddComponent<Rigidbody>();
            rb.mass = mass;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = new Color(0.9f, 0.7f, 0.5f);
            go.GetComponent<MeshRenderer>().sharedMaterial = mat;

            return go;
        }

        private static void CreateJoint(GameObject child, Rigidbody connectedBody)
        {
            var joint = child.AddComponent<ConfigurableJoint>();
            joint.connectedBody = connectedBody;

            joint.xMotion = ConfigurableJointMotion.Locked;
            joint.yMotion = ConfigurableJointMotion.Locked;
            joint.zMotion = ConfigurableJointMotion.Locked;

            joint.angularXMotion = ConfigurableJointMotion.Limited;
            joint.angularYMotion = ConfigurableJointMotion.Limited;
            joint.angularZMotion = ConfigurableJointMotion.Limited;

            var highLimit = new SoftJointLimit { limit = 45f };
            var lowLimit = new SoftJointLimit { limit = -45f };
            joint.highAngularXLimit = highLimit;
            joint.lowAngularXLimit = lowLimit;

            var yLimit = new SoftJointLimit { limit = 30f };
            joint.angularYLimit = yLimit;
            joint.angularZLimit = yLimit;

            var drive = new JointDrive
            {
                positionSpring = JointPositionSpring,
                positionDamper = JointPositionDamper,
                maximumForce = JointMaximumForce
            };
            joint.angularXDrive = drive;
            joint.angularYZDrive = drive;
            joint.rotationDriveMode = RotationDriveMode.Slerp;
            joint.slerpDrive = drive;
        }
    }
}
