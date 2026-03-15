using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using RagdollRealms.Systems.Ragdoll;

namespace RagdollRealms.Editor
{
    public static class CreatePlayerPrefab
    {
        // Mixamo bone suffix -> mapped name
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

        private static readonly HashSet<string> RagdollBones = new HashSet<string>
        {
            "Hips", "Spine1", "Head",
            "LeftUpperArm", "LeftLowerArm",
            "RightUpperArm", "RightLowerArm",
            "LeftUpperLeg", "LeftLowerLeg", "LeftFoot",
            "RightUpperLeg", "RightLowerLeg", "RightFoot",
        };

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

        private static readonly Dictionary<string, int> BoneCapsuleDirection = new Dictionary<string, int>
        {
            { "Hips", 0 },
            { "Spine1", 0 },
            { "Head", 1 },
            { "LeftUpperArm", 0 },
            { "LeftLowerArm", 0 },
            { "RightUpperArm", 0 },
            { "RightLowerArm", 0 },
            { "LeftUpperLeg", 1 },
            { "LeftLowerLeg", 1 },
            { "RightUpperLeg", 1 },
            { "RightLowerLeg", 1 },
            { "LeftFoot", 2 },
            { "RightFoot", 2 },
        };

        // Joint limits per bone type for more realistic constraints
        private static readonly Dictionary<string, Vector3> BoneJointLimits = new Dictionary<string, Vector3>
        {
            // { boneName, (lowAngularX, highAngularX, angularYZ) }
            { "Spine1", new Vector3(-30f, 30f, 20f) },
            { "Head", new Vector3(-40f, 40f, 25f) },
            { "LeftUpperArm", new Vector3(-80f, 80f, 45f) },
            { "LeftLowerArm", new Vector3(-5f, 130f, 10f) },
            { "RightUpperArm", new Vector3(-80f, 80f, 45f) },
            { "RightLowerArm", new Vector3(-5f, 130f, 10f) },
            { "LeftUpperLeg", new Vector3(-80f, 40f, 25f) },
            { "LeftLowerLeg", new Vector3(-130f, 5f, 10f) },
            { "LeftFoot", new Vector3(-30f, 50f, 15f) },
            { "RightUpperLeg", new Vector3(-80f, 40f, 25f) },
            { "RightLowerLeg", new Vector3(-130f, 5f, 10f) },
            { "RightFoot", new Vector3(-30f, 50f, 15f) },
        };

        private const float JointPositionSpring = 1500f;
        private const float JointPositionDamper = 100f;
        private const float JointMaximumForce = 3000f;

        [MenuItem("Ragdoll Realms/Create Player Prefab")]
        public static void Execute()
        {
            Debug.Log("[CreatePlayerPrefab] Starting player prefab creation...");

            var fbxModel = FindAndPrepareModel();
            if (fbxModel == null) return;

            var ragdollConfig = CreateOrLoadRagdollConfig();
            var playerConfig = CreateOrLoadPlayerConfig();
            var animController = CreateLocomotionAnimatorController();
            var inputActions = CreateOrLoadInputActions();

            var playerRoot = InstantiatePlayerRoot(fbxModel);

            var boneTransforms = new Dictionary<string, Transform>();
            BuildBoneLookup(playerRoot.transform, boneTransforms);
            if (!boneTransforms.ContainsKey("Hips"))
            {
                Debug.LogError("[CreatePlayerPrefab] Could not find Hips bone! Aborting.");
                UnityEngine.Object.DestroyImmediate(playerRoot);
                return;
            }

            var boneRigidbodies = AddRagdollPhysics(boneTransforms);
            SetupAnimator(playerRoot, animController);
            AddCombatComponents(boneTransforms);
            AddRagdollComponents(playerRoot, ragdollConfig, boneRigidbodies);
            AddPlayerComponents(playerRoot, playerConfig, inputActions);

            playerRoot.AddComponent<RagdollForceTestHelper>();

            SaveAsPrefab(playerRoot, boneRigidbodies.Count);
        }

        private static GameObject FindAndPrepareModel()
        {
            var fbxModel = FindMixamoModel();
            if (fbxModel == null)
            {
                Debug.LogError("[CreatePlayerPrefab] No Mixamo FBX model found in Assets/Models/. Aborting.");
                return null;
            }
            Debug.Log($"[CreatePlayerPrefab] Using model: {AssetDatabase.GetAssetPath(fbxModel)}");
            return EnsureHumanoidRig(fbxModel);
        }

        private static GameObject InstantiatePlayerRoot(GameObject fbxModel)
        {
            var playerRoot = (GameObject)PrefabUtility.InstantiatePrefab(fbxModel);
            playerRoot.name = "Player";
            playerRoot.transform.position = Vector3.zero;
            playerRoot.tag = "Player";
            return playerRoot;
        }

        private static void SetupAnimator(GameObject playerRoot, AnimatorController animController)
        {
            var animator = playerRoot.GetComponent<Animator>();
            if (animator == null)
                animator = playerRoot.AddComponent<Animator>();
            animator.runtimeAnimatorController = animController;
            animator.applyRootMotion = false;
            animator.updateMode = AnimatorUpdateMode.Normal;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        }

        private static void AddCombatComponents(Dictionary<string, Transform> boneTransforms)
        {
            if (boneTransforms.ContainsKey("LeftLowerArm"))
                boneTransforms["LeftLowerArm"].gameObject.AddComponent<RagdollRealms.Systems.Combat.MeleeCombatController>();
            if (boneTransforms.ContainsKey("RightLowerArm"))
                boneTransforms["RightLowerArm"].gameObject.AddComponent<RagdollRealms.Systems.Combat.MeleeCombatController>();
        }

        private static void AddRagdollComponents(GameObject playerRoot,
            RagdollRealms.Content.RagdollConfigDefinition ragdollConfig,
            Dictionary<string, Rigidbody> boneRigidbodies)
        {
            Rigidbody hipRb = boneRigidbodies.GetValueOrDefault("Hips");

            var ragdollCtrl = playerRoot.AddComponent<RagdollRealms.Systems.Ragdoll.RagdollController>();
            var ragdollSO = new SerializedObject(ragdollCtrl);
            ragdollSO.FindProperty("_config").objectReferenceValue = ragdollConfig;
            ragdollSO.FindProperty("_hipRigidbody").objectReferenceValue = hipRb;
            ragdollSO.ApplyModifiedPropertiesWithoutUndo();

            playerRoot.AddComponent<RagdollRealms.Systems.Ragdoll.AnimationFollower>();
            playerRoot.AddComponent<RagdollRealms.Systems.Ragdoll.RagdollForceReceiver>();
            playerRoot.AddComponent<RagdollRealms.Systems.Ragdoll.RagdollRecoveryController>();
            playerRoot.AddComponent<RagdollRealms.Systems.Ragdoll.RagdollLODController>();

            var collisionHandler = playerRoot.AddComponent<RagdollCollisionHandler>();
            var joints = playerRoot.GetComponentsInChildren<ConfigurableJoint>();
            for (int i = 0; i < joints.Length; i++)
            {
                var reporter = joints[i].gameObject.AddComponent<BoneCollisionReporter>();
                reporter.Initialize(collisionHandler, i, playerRoot.transform);
            }

            Debug.Log($"[CreatePlayerPrefab] Rigged {boneRigidbodies.Count} bones with ragdoll physics.");
        }

        private static void AddPlayerComponents(GameObject playerRoot,
            RagdollRealms.Content.PlayerConfigDefinition playerConfig,
            InputActionAsset inputActions)
        {
            var inputCtrl = playerRoot.AddComponent<RagdollRealms.Systems.Player.PlayerInputController>();
            WireInputActions(inputCtrl, inputActions);

            var moveCtrl = playerRoot.AddComponent<RagdollRealms.Systems.Player.PlayerMovementController>();
            var moveSO = new SerializedObject(moveCtrl);
            moveSO.FindProperty("_config").objectReferenceValue = playerConfig;
            moveSO.FindProperty("_playerId").intValue = 0;
            moveSO.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SaveAsPrefab(GameObject playerRoot, int boneCount)
        {
            PrefabUtility.UnpackPrefabInstance(playerRoot, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
                AssetDatabase.CreateFolder("Assets", "Prefabs");

            string prefabPath = "Assets/Prefabs/Player.prefab";
            PrefabUtility.SaveAsPrefabAssetAndConnect(playerRoot, prefabPath, InteractionMode.AutomatedAction);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeGameObject = playerRoot;

            Debug.Log($"[CreatePlayerPrefab] Player prefab saved at {prefabPath}");
            Debug.Log($"[CreatePlayerPrefab] Bones rigged: {boneCount} | Joints: {playerRoot.GetComponentsInChildren<ConfigurableJoint>().Length}");
            Debug.Log("[CreatePlayerPrefab] Done! Player prefab is ready to play.");
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
                if (path.Replace("Assets/Models/", "").Contains("/")) continue;

                var model = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (model == null) continue;

                var hips = FindChildRecursive(model.transform, name => MatchesBoneSuffix(name, "Hips"));
                if (hips != null)
                    return model;
            }
            return null;
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

        // ==================== RAGDOLL PHYSICS ====================

        private static Dictionary<string, Rigidbody> AddRagdollPhysics(Dictionary<string, Transform> boneTransforms)
        {
            var boneRigidbodies = new Dictionary<string, Rigidbody>();

            foreach (var boneName in RagdollBones)
            {
                if (!boneTransforms.ContainsKey(boneName))
                {
                    Debug.LogWarning($"[CreatePlayerPrefab] Bone '{boneName}' not found, skipping.");
                    continue;
                }

                var go = boneTransforms[boneName].gameObject;

                // Remove existing components if any
                foreach (var existing in go.GetComponents<Rigidbody>())
                    UnityEngine.Object.DestroyImmediate(existing);
                foreach (var existing in go.GetComponents<Collider>())
                    UnityEngine.Object.DestroyImmediate(existing);
                foreach (var existing in go.GetComponents<ConfigurableJoint>())
                    UnityEngine.Object.DestroyImmediate(existing);

                var rb = go.AddComponent<Rigidbody>();
                rb.mass = BoneMasses.GetValueOrDefault(boneName, 1f);
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                boneRigidbodies[boneName] = rb;

                var capsule = go.AddComponent<CapsuleCollider>();
                var size = BoneColliderSizes.GetValueOrDefault(boneName, new Vector2(0.05f, 0.2f));
                capsule.radius = size.x;
                capsule.height = size.y;
                capsule.direction = BoneCapsuleDirection.GetValueOrDefault(boneName, 1);
            }

            // Create joints with proper limits
            foreach (var kvp in JointParentMap)
            {
                if (!boneRigidbodies.ContainsKey(kvp.Key) || !boneRigidbodies.ContainsKey(kvp.Value))
                    continue;

                var limits = BoneJointLimits.GetValueOrDefault(kvp.Key, new Vector3(-45f, 45f, 30f));
                CreateJoint(boneTransforms[kvp.Key].gameObject, boneRigidbodies[kvp.Value], limits);
            }

            return boneRigidbodies;
        }

        private static void CreateJoint(GameObject child, Rigidbody connectedBody, Vector3 limits)
        {
            var joint = child.AddComponent<ConfigurableJoint>();
            joint.connectedBody = connectedBody;

            joint.xMotion = ConfigurableJointMotion.Locked;
            joint.yMotion = ConfigurableJointMotion.Locked;
            joint.zMotion = ConfigurableJointMotion.Locked;

            joint.angularXMotion = ConfigurableJointMotion.Limited;
            joint.angularYMotion = ConfigurableJointMotion.Limited;
            joint.angularZMotion = ConfigurableJointMotion.Limited;

            joint.lowAngularXLimit = new SoftJointLimit { limit = limits.x };
            joint.highAngularXLimit = new SoftJointLimit { limit = limits.y };
            joint.angularYLimit = new SoftJointLimit { limit = limits.z };
            joint.angularZLimit = new SoftJointLimit { limit = limits.z };

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

            // Pre-processing for joint stability
            joint.enablePreprocessing = false;
        }

        // ==================== ANIMATOR CONTROLLER ====================

        private static AnimatorController CreateLocomotionAnimatorController()
        {
            var controllerPath = "Assets/Resources/RagdollAnimator.controller";

            // Load animation clips — two-pass to avoid race condition:
            // Pass 1: ensure all FBX files are imported as Humanoid with looping
            // Pass 2: load clips after reimport has completed
            AnimationClip idleClip = null;
            AnimationClip walkClip = null;
            AnimationClip goofyClip = null;

            if (AssetDatabase.IsValidFolder("Assets/Models/Animations"))
            {
                var guids = AssetDatabase.FindAssets("t:Model", new[] { "Assets/Models/Animations" });

                // Pass 1: Ensure import settings (may trigger reimport)
                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    EnsureAnimationImportSettings(path);
                }

                // Let reimported assets finish processing
                AssetDatabase.Refresh();

                // Pass 2: Load clips after all reimports are complete
                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    if (string.IsNullOrEmpty(path)) continue;
                    var fileName = System.IO.Path.GetFileNameWithoutExtension(path).ToLower();

                    var clip = LoadFirstAnimationClip(path);
                    if (clip == null)
                    {
                        Debug.LogWarning($"[CreatePlayerPrefab] No animation clip found in {path}");
                        continue;
                    }

                    if (fileName.Contains("idle") || fileName.Contains("happy"))
                        idleClip = clip;
                    else if (fileName.Contains("walk") || fileName.Contains("stuwalk"))
                        walkClip = clip;
                    else if (fileName.Contains("goofy"))
                        goofyClip = clip;
                }
            }

            // Use goofy as fallback idle if no dedicated idle found
            if (idleClip == null && goofyClip != null)
                idleClip = goofyClip;

            if (idleClip == null)
            {
                Debug.LogWarning("[CreatePlayerPrefab] No idle animation found. Animator will use empty controller.");
            }

            // Delete existing controller to recreate cleanly
            if (AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath) != null)
                AssetDatabase.DeleteAsset(controllerPath);

            var controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

            // Add Speed parameter for blend tree
            controller.AddParameter("Speed", AnimatorControllerParameterType.Float);

            var rootStateMachine = controller.layers[0].stateMachine;

            if (idleClip != null && walkClip != null)
            {
                // Create a 1D blend tree: Idle (0) -> Walk (0.5) -> Run (1.0)
                BlendTree blendTree;
                var locomotionState = controller.CreateBlendTreeInController("Locomotion", out blendTree, 0);

                blendTree.blendParameter = "Speed";
                blendTree.blendType = BlendTreeType.Simple1D;
                blendTree.AddChild(idleClip, 0f);
                blendTree.AddChild(walkClip, 0.5f);
                blendTree.AddChild(walkClip, 1.0f); // Reuse walk for run (faster playback via speed)

                // Set run threshold child to play faster
                var children = blendTree.children;
                if (children.Length >= 3)
                {
                    children[2].timeScale = 1.5f;
                    blendTree.children = children;
                }

                rootStateMachine.defaultState = locomotionState;
                Debug.Log("[CreatePlayerPrefab] Animator: Locomotion blend tree (Idle + Walk + Run)");
            }
            else if (idleClip != null)
            {
                var idleState = rootStateMachine.AddState("Idle");
                idleState.motion = idleClip;
                rootStateMachine.defaultState = idleState;
                Debug.Log("[CreatePlayerPrefab] Animator: Idle only (no walk clip found)");
            }
            else
            {
                Debug.LogWarning("[CreatePlayerPrefab] No animations found for controller.");
            }

            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();

            return controller;
        }

        private static void EnsureAnimationImportSettings(string path)
        {
            var importer = AssetImporter.GetAtPath(path) as ModelImporter;
            if (importer == null) return;

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
            }
        }

        // ==================== INPUT ACTIONS ====================

        private static InputActionAsset CreateOrLoadInputActions()
        {
            var path = "Assets/Resources/PlayerInputActions.inputactions";
            var existing = AssetDatabase.LoadAssetAtPath<InputActionAsset>(path);
            if (existing != null) return existing;

            var asset = InputActionAsset.FromJson(@"{
                ""name"": ""PlayerInputActions"",
                ""maps"": [
                    {
                        ""name"": ""Player"",
                        ""actions"": [
                            {
                                ""name"": ""Move"",
                                ""type"": ""Value"",
                                ""expectedControlType"": ""Vector2""
                            },
                            {
                                ""name"": ""Jump"",
                                ""type"": ""Button""
                            },
                            {
                                ""name"": ""Sprint"",
                                ""type"": ""Button""
                            },
                            {
                                ""name"": ""Look"",
                                ""type"": ""Value"",
                                ""expectedControlType"": ""Vector2""
                            }
                        ],
                        ""bindings"": [
                            {
                                ""path"": ""<Keyboard>/w"",
                                ""action"": ""Move"",
                                ""isComposite"": false,
                                ""isPartOfComposite"": false
                            },
                            {
                                ""path"": ""<Gamepad>/leftStick"",
                                ""action"": ""Move"",
                                ""isComposite"": false,
                                ""isPartOfComposite"": false
                            },
                            {
                                ""name"": ""WASD"",
                                ""path"": ""2DVector"",
                                ""action"": ""Move"",
                                ""isComposite"": true,
                                ""isPartOfComposite"": false
                            },
                            {
                                ""name"": ""up"",
                                ""path"": ""<Keyboard>/w"",
                                ""action"": ""Move"",
                                ""isComposite"": false,
                                ""isPartOfComposite"": true
                            },
                            {
                                ""name"": ""down"",
                                ""path"": ""<Keyboard>/s"",
                                ""action"": ""Move"",
                                ""isComposite"": false,
                                ""isPartOfComposite"": true
                            },
                            {
                                ""name"": ""left"",
                                ""path"": ""<Keyboard>/a"",
                                ""action"": ""Move"",
                                ""isComposite"": false,
                                ""isPartOfComposite"": true
                            },
                            {
                                ""name"": ""right"",
                                ""path"": ""<Keyboard>/d"",
                                ""action"": ""Move"",
                                ""isComposite"": false,
                                ""isPartOfComposite"": true
                            },
                            {
                                ""name"": ""Arrows"",
                                ""path"": ""2DVector"",
                                ""action"": ""Move"",
                                ""isComposite"": true,
                                ""isPartOfComposite"": false
                            },
                            {
                                ""name"": ""up"",
                                ""path"": ""<Keyboard>/upArrow"",
                                ""action"": ""Move"",
                                ""isComposite"": false,
                                ""isPartOfComposite"": true
                            },
                            {
                                ""name"": ""down"",
                                ""path"": ""<Keyboard>/downArrow"",
                                ""action"": ""Move"",
                                ""isComposite"": false,
                                ""isPartOfComposite"": true
                            },
                            {
                                ""name"": ""left"",
                                ""path"": ""<Keyboard>/leftArrow"",
                                ""action"": ""Move"",
                                ""isComposite"": false,
                                ""isPartOfComposite"": true
                            },
                            {
                                ""name"": ""right"",
                                ""path"": ""<Keyboard>/rightArrow"",
                                ""action"": ""Move"",
                                ""isComposite"": false,
                                ""isPartOfComposite"": true
                            },
                            {
                                ""path"": ""<Keyboard>/space"",
                                ""action"": ""Jump"",
                                ""isComposite"": false,
                                ""isPartOfComposite"": false
                            },
                            {
                                ""path"": ""<Gamepad>/buttonSouth"",
                                ""action"": ""Jump"",
                                ""isComposite"": false,
                                ""isPartOfComposite"": false
                            },
                            {
                                ""path"": ""<Keyboard>/leftShift"",
                                ""action"": ""Sprint"",
                                ""isComposite"": false,
                                ""isPartOfComposite"": false
                            },
                            {
                                ""path"": ""<Gamepad>/leftStickPress"",
                                ""action"": ""Sprint"",
                                ""isComposite"": false,
                                ""isPartOfComposite"": false
                            },
                            {
                                ""path"": ""<Mouse>/delta"",
                                ""action"": ""Look"",
                                ""isComposite"": false,
                                ""isPartOfComposite"": false
                            },
                            {
                                ""path"": ""<Gamepad>/rightStick"",
                                ""action"": ""Look"",
                                ""isComposite"": false,
                                ""isPartOfComposite"": false
                            }
                        ]
                    }
                ]
            }");

            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");

            // Write as JSON text file — Input System requires .inputactions to be JSON,
            // not Unity YAML (AssetDatabase.CreateAsset would produce YAML → DefaultAsset)
            System.IO.File.WriteAllText(path, asset.ToJson());
            AssetDatabase.Refresh();
            asset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(path);
            Debug.Log($"[CreatePlayerPrefab] Input Action Asset created at {path}");
            return asset;
        }

        private static void WireInputActions(RagdollRealms.Systems.Player.PlayerInputController inputCtrl, InputActionAsset inputActions)
        {
            var so = new SerializedObject(inputCtrl);
            var playerMap = inputActions.FindActionMap("Player");
            if (playerMap == null)
            {
                Debug.LogWarning("[CreatePlayerPrefab] Could not find 'Player' action map in input actions.");
                return;
            }

            // Create InputActionReferences for each action
            var moveAction = playerMap.FindAction("Move");
            var jumpAction = playerMap.FindAction("Jump");
            var sprintAction = playerMap.FindAction("Sprint");
            var lookAction = playerMap.FindAction("Look");

            if (moveAction != null)
                so.FindProperty("_moveAction").objectReferenceValue = InputActionReference.Create(moveAction);
            if (jumpAction != null)
                so.FindProperty("_jumpAction").objectReferenceValue = InputActionReference.Create(jumpAction);
            if (sprintAction != null)
                so.FindProperty("_sprintAction").objectReferenceValue = InputActionReference.Create(sprintAction);
            if (lookAction != null)
                so.FindProperty("_lookAction").objectReferenceValue = InputActionReference.Create(lookAction);

            so.ApplyModifiedPropertiesWithoutUndo();
            Debug.Log("[CreatePlayerPrefab] Input actions wired: Move (WASD/Arrows/Gamepad), Jump (Space/A), Sprint (Shift/L3), Look (Mouse/RStick)");
        }

        // ==================== CONFIG CREATION ====================

        private static RagdollRealms.Content.RagdollConfigDefinition CreateOrLoadRagdollConfig()
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
            so.FindProperty("_description").stringValue = "Default ragdoll physics configuration";
            so.ApplyModifiedPropertiesWithoutUndo();

            return config;
        }

        private static RagdollRealms.Content.PlayerConfigDefinition CreateOrLoadPlayerConfig()
        {
            var configPath = "Assets/Resources/PlayerConfigs";
            if (!AssetDatabase.IsValidFolder(configPath))
                AssetDatabase.CreateFolder("Assets/Resources", "PlayerConfigs");

            var existing = AssetDatabase.LoadAssetAtPath<RagdollRealms.Content.PlayerConfigDefinition>(
                configPath + "/DefaultPlayerConfig.asset");
            if (existing != null) return existing;

            var config = ScriptableObject.CreateInstance<RagdollRealms.Content.PlayerConfigDefinition>();
            AssetDatabase.CreateAsset(config, configPath + "/DefaultPlayerConfig.asset");

            var so = new SerializedObject(config);
            so.FindProperty("_id").stringValue = "default_player";
            so.FindProperty("_displayName").stringValue = "Default Player Config";
            so.FindProperty("_description").stringValue = "Default player movement and camera configuration";
            so.ApplyModifiedPropertiesWithoutUndo();

            return config;
        }

        // ==================== ANIMATION CLIP LOADING ====================

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
    }
}
