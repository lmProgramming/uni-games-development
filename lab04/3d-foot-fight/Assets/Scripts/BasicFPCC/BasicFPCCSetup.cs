// ------------------------------------------ 
// BasicFPCC.cs
// a basic first person character controller
// with jump, crouch, run, slide
// 2020-10-04 Alucard Jay Kay 
// ------------------------------------------ 

// source : 
// https://discussions.unity.com/t/855344
// Brackeys FPS controller base : 
// https://www.youtube.com/watch?v=_QajrabyTJc
// smooth mouse look : 
// https://discussions.unity.com/t/710168/2
// ground check : (added isGrounded)
// https://gist.github.com/jawinn/f466b237c0cdc5f92d96
// run, crouch, slide : (added check for headroom before un-crouching)
// https://answers.unity.com/questions/374157/character-controller-slide-action-script.html
// interact with rigidbodies : 
// https://docs.unity3d.com/2018.4/Documentation/ScriptReference/CharacterController.OnControllerColliderHit.html

// this section adds create BasicFPCC object to the menu : New -> GameObject -> 3D Object
// then configures the game object
// demo layer used : Ignore Raycast
// also finds the main camera, attaches and sets position
// and creates capsule gfx object (for visual while editing)

using UnityEditor;
using UnityEngine;

namespace BasicFPCC
{
    public class BasicFPCCSetup : MonoBehaviour
    {
#if UNITY_EDITOR

        private const int PlayerLayer = 2;

        [MenuItem("GameObject/3D Object/BasicFPCC", false, 0)]
        public static void CreateBasicFpcc()
        {
            var go = new GameObject("Player");

            var controller = go.AddComponent<CharacterController>();
            controller.center = new Vector3(0, 1, 0);

            var basicFpcc = go.AddComponent<BasicFPCC>();

            // Layer Mask
            go.layer = PlayerLayer;
            basicFpcc.castingMask = ~(1 << PlayerLayer);
            Debug.LogError("** SET the LAYER of the PLAYER Object, and the LAYER MASK of the BasicFPCC castingMask **");
            Debug.LogWarning(
                "Assign the BasicFPCC Player object to its own Layer, then assign the Layer Mask to ignore the BasicFPCC Player object Layer. Currently using layer "
                + PlayerLayer + ": " + LayerMask.LayerToName(PlayerLayer)
            );

            // Main Camera
            var mainCamObject = GameObject.Find("Main Camera");
            if (mainCamObject)
            {
                mainCamObject.transform.parent = go.transform;
                mainCamObject.transform.localPosition = new Vector3(0, 1.7f, 0);
                mainCamObject.transform.localRotation = Quaternion.identity;

                basicFpcc.cameraTx = mainCamObject.transform;
            }
            else // create example camera
            {
                Debug.LogError(
                    "** Main Camera NOT FOUND ** \nA new Camera has been created and assigned. Please replace this with the Main Camera (and associated AudioListener).");

                var camGo = new GameObject("BasicFPCC Camera");
                camGo.AddComponent<Camera>();

                camGo.transform.parent = go.transform;
                camGo.transform.localPosition = new Vector3(0, 1.7f, 0);
                camGo.transform.localRotation = Quaternion.identity;

                basicFpcc.cameraTx = camGo.transform;
            }

            // GFX
            var gfx = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            var cc = gfx.GetComponent<Collider>();
            DestroyImmediate(cc);
            gfx.transform.parent = go.transform;
            gfx.transform.localPosition = new Vector3(0, 1, 0);
            gfx.name = "GFX";
            gfx.layer = PlayerLayer;
            basicFpcc.playerGfx = gfx.transform;
        }
#endif
    }
}