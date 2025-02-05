using System;
using HarmonyLib;
using BepInEx;
using UnityEngine;

namespace Platform_Mod
{
    [BepInPlugin("com.Kameron.Mod", "Platform", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private GameObject rightPlat;
        private GameObject leftPlat;

        private bool rightCreatePlat = false;
        private bool leftCreatePlat = false;

        void Awake()
        {
            Logger.LogInfo("Mod Loaded Successfully!");
            Harmony harmony = new Harmony("com.Kameron.Mod");
            harmony.PatchAll();
        }

        void Update()
        {
            if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.GameModeString.Contains("MODDED"))
            {
                GorillaLocomotion.Player player = GorillaLocomotion.Player.Instance;
                if (player == null)
                {
                    Logger.LogError("Player instance is null!");
                    return;
                }

                bool rightGrip = ControllerInputPoller.instance.rightGrab;
                bool leftGrip = ControllerInputPoller.instance.leftGrab;

                Vector3 rightHandPos = new Vector3(player.rightControllerTransform.position.x, player.rightControllerTransform.position.y - 0.1f, player.rightControllerTransform.position.z);
                Vector3 leftHandPos = new Vector3(player.leftControllerTransform.position.x, player.leftControllerTransform.position.y - 0.1f, player.leftControllerTransform.position.z);

                // Right Hand Platform
                if (rightGrip && !rightCreatePlat)
                {
                    rightCreatePlat = true;
                    rightPlat = CreatePlatform(rightHandPos);
                }
                else if (!rightGrip && rightCreatePlat)
                {
                    rightCreatePlat = false;
                    Destroy(rightPlat, 2.0f); // Allow platform to exist for 2 seconds
                }

                // Left Hand Platform
                if (leftGrip && !leftCreatePlat)
                {
                    leftCreatePlat = true;
                    leftPlat = CreatePlatform(leftHandPos);
                }
                else if (!leftGrip && leftCreatePlat)
                {
                    leftCreatePlat = false;
                    Destroy(leftPlat, 2.0f);
                }
            }
        }

        private GameObject CreatePlatform(Vector3 position)
        {
            Logger.LogInfo("Creating Platform");
            GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
            platform.transform.rotation = Quaternion.identity;
            platform.transform.position = position;
            platform.transform.localScale = new Vector3(0.3f, 0.01f, 0.3f);
            Destroy(platform.GetComponent<Rigidbody>()); // Remove physics if not needed

            Renderer rend = platform.GetComponent<Renderer>();
            rend.material.shader = Shader.Find("GorillaTag/UberShader"); // Use a valid shader
            rend.material.color = Color.white;

            return platform;
        }
    }
}
