using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class SetupRagdollTestScene
{
    public static void Execute()
    {
        // Create and open a new test scene
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        // ========== 1. MANAGERS ==========
        var managersRoot = new GameObject("--- Managers ---");

        // ServiceLocator
        var slGO = new GameObject("ServiceLocator");
        slGO.transform.SetParent(managersRoot.transform);
        slGO.AddComponent<RagdollRealms.Core.ServiceLocator>();

        // EventBus
        var ebGO = new GameObject("EventBus");
        ebGO.transform.SetParent(managersRoot.transform);
        ebGO.AddComponent<RagdollRealms.Core.EventBus>();

        // PoolManager
        var pmGO = new GameObject("PoolManager");
        pmGO.transform.SetParent(managersRoot.transform);
        pmGO.AddComponent<RagdollRealms.Core.PoolManager>();

        // PerformanceBudgetManager
        var pbGO = new GameObject("PerformanceBudgetManager");
        pbGO.transform.SetParent(managersRoot.transform);
        pbGO.AddComponent<RagdollRealms.Core.PerformanceBudgetManager>();

        // ContentRegistryManager
        var crGO = new GameObject("ContentRegistryManager");
        crGO.transform.SetParent(managersRoot.transform);
        crGO.AddComponent<RagdollRealms.Systems.ContentRegistryManager>();

        // ========== 2. CREATE RAGDOLL CONFIG SO ==========
        var configPath = "Assets/Resources/RagdollConfigs";
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");
        if (!AssetDatabase.IsValidFolder(configPath))
            AssetDatabase.CreateFolder("Assets/Resources", "RagdollConfigs");

        var config = ScriptableObject.CreateInstance<RagdollRealms.Content.RagdollConfigDefinition>();
        AssetDatabase.CreateAsset(config, configPath + "/DefaultRagdollConfig.asset");

        // Set the ID via SerializedObject
        var so = new SerializedObject(config);
        so.FindProperty("_id").stringValue = "default_ragdoll";
        so.FindProperty("_displayName").stringValue = "Default Ragdoll Config";
        so.FindProperty("_description").stringValue = "Default ragdoll physics configuration for testing";
        so.ApplyModifiedPropertiesWithoutUndo();

        // ========== 3. GROUND PLANE ==========
        var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(5, 1, 5);
        var groundMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        groundMat.color = new Color(0.3f, 0.5f, 0.3f);
        ground.GetComponent<MeshRenderer>().sharedMaterial = groundMat;

        // ========== 4. RAGDOLL CHARACTER ==========
        // Build a simple humanoid ragdoll from primitives
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

        // --- Right Upper Arm ---
        var rUpperArm = CreateBodyPart("RightUpperArm", torso.transform, new Vector3(0.4f, 0.2f, 0), new Vector3(0.3f, 0.1f, 0.1f), 1f);
        CreateJoint(rUpperArm, torso.GetComponent<Rigidbody>());

        // --- Right Lower Arm ---
        var rLowerArm = CreateBodyPart("RightLowerArm", rUpperArm.transform, new Vector3(0.3f, 0, 0), new Vector3(0.25f, 0.08f, 0.08f), 0.8f);
        CreateJoint(rLowerArm, rUpperArm.GetComponent<Rigidbody>());

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

        // ========== 5. ADD RAGDOLL SYSTEM COMPONENTS ==========
        // RagdollController
        var rc = ragdollRoot.AddComponent<RagdollRealms.Systems.Ragdoll.RagdollController>();
        var rcSO = new SerializedObject(rc);
        rcSO.FindProperty("_config").objectReferenceValue = config;
        rcSO.FindProperty("_hipRigidbody").objectReferenceValue = hip.GetComponent<Rigidbody>();
        rcSO.ApplyModifiedPropertiesWithoutUndo();

        // AnimationFollower (no animator for this test — joints will just hold pose)
        ragdollRoot.AddComponent<RagdollRealms.Systems.Ragdoll.AnimationFollower>();

        // RagdollForceReceiver
        ragdollRoot.AddComponent<RagdollRealms.Systems.Ragdoll.RagdollForceReceiver>();

        // RagdollRecoveryController
        ragdollRoot.AddComponent<RagdollRealms.Systems.Ragdoll.RagdollRecoveryController>();

        // RagdollLODController
        ragdollRoot.AddComponent<RagdollRealms.Systems.Ragdoll.RagdollLODController>();

        // MeleeCombatController on both lower arms (fists)
        lLowerArm.AddComponent<RagdollRealms.Systems.Combat.MeleeCombatController>();
        rLowerArm.AddComponent<RagdollRealms.Systems.Combat.MeleeCombatController>();

        // ========== 6. FORCE TEST SCRIPT ==========
        ragdollRoot.AddComponent<RagdollForceTestHelper>();

        // ========== 7. TARGET DUMMY (second ragdoll to hit) ==========
        var dummy = Object.Instantiate(ragdollRoot);
        dummy.name = "TargetDummy";
        dummy.transform.position = new Vector3(3f, 3f, 0);

        // ========== 8. POSITION CAMERA ==========
        var cam = Camera.main;
        if (cam != null)
        {
            cam.transform.position = new Vector3(0, 4f, -8f);
            cam.transform.LookAt(new Vector3(1.5f, 2f, 0));
        }

        // ========== 9. SAVE ==========
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/RagdollTestScene.unity");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("[SetupRagdollTestScene] Test scene created at Assets/Scenes/RagdollTestScene.unity");
        Debug.Log("[SetupRagdollTestScene] Press SPACE=push, F=explosion, R=reset position, G=grapple test");
    }

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

        // Color body parts
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

        // Lock linear motion (bones don't stretch)
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;

        // Allow angular motion with limits
        joint.angularXMotion = ConfigurableJointMotion.Limited;
        joint.angularYMotion = ConfigurableJointMotion.Limited;
        joint.angularZMotion = ConfigurableJointMotion.Limited;

        // Set angular limits
        var highLimit = new SoftJointLimit { limit = 45f };
        var lowLimit = new SoftJointLimit { limit = -45f };
        joint.highAngularXLimit = highLimit;
        joint.lowAngularXLimit = lowLimit;

        var yLimit = new SoftJointLimit { limit = 30f };
        joint.angularYLimit = yLimit;
        joint.angularZLimit = yLimit;

        // Set spring drives (active ragdoll)
        var drive = new JointDrive
        {
            positionSpring = 500f,
            positionDamper = 50f,
            maximumForce = 1000f
        };
        joint.angularXDrive = drive;
        joint.angularYZDrive = drive;
        joint.rotationDriveMode = RotationDriveMode.Slerp;
        joint.slerpDrive = drive;
    }
}
