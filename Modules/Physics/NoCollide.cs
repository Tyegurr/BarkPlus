﻿using System;
using System.Collections;
using Bark.GUI;
using Bark.Patches;
using Bark.Tools;
using GorillaLocomotion;
using UnityEngine;
using Bark.Modules.Multiplayer;
using Bark.Modules.Movement;
using BepInEx.Configuration;

namespace Bark.Modules.Physics
{
    public class NoCollide : BarkModule
    {
        public static readonly string DisplayName = "No Collide";
        public static NoCollide Instance;
        private LayerMask baseMask;
        private bool baseHeadIsTrigger, baseBodyIsTrigger;
        public static bool active;
        public static int layer = 29, layerMask = 1 << layer;
        private Vector3 activationLocation;
        private float activationAngle;
        bool flyWasEnabled;

        private struct GorillaTriggerInfo
        {
            public Collider collider;
            public bool wasEnabled;
        }

        void Awake() { Instance = this; }

        protected override void OnEnable()
        {
            try
            {
                if (!MenuController.Instance.Built) return;
                base.OnEnable();
                activationLocation = Player.Instance.bodyCollider.transform.position;
                activationAngle = Player.Instance.bodyCollider.transform.eulerAngles.y;
                if (!Piggyback.mounted)
                {
                    try
                    {
                        var fly = Plugin.menuController.GetComponent<Fly>();
                        flyWasEnabled = fly.enabled;
                        fly.enabled = true;
                    }
                    catch
                    {
                        Logging.Debug("Failed to enable fly for noclip.");
                    }
                }

                Logging.Debug("Disabling triggers");
                TriggerBoxPatches.triggersEnabled = false;
                baseMask = Player.Instance.locomotionEnabledLayers;
                Player.Instance.locomotionEnabledLayers = layerMask;

                baseBodyIsTrigger = Player.Instance.bodyCollider.isTrigger;
                Player.Instance.bodyCollider.isTrigger = true;

                baseHeadIsTrigger = Player.Instance.headCollider.isTrigger;
                Player.Instance.headCollider.isTrigger = true;
                active = true;
            }
            catch (Exception e) { Logging.Exception(e); }
        }

        protected override void Cleanup() 
        {
            StartCoroutine(CleanupRoutine());
        }

        IEnumerator CleanupRoutine()
        {
            Logging.Debug("Cleaning up noclip");

            if (!active) yield break;
            Player.Instance.locomotionEnabledLayers = baseMask;
            Player.Instance.bodyCollider.isTrigger = baseBodyIsTrigger;
            Player.Instance.headCollider.isTrigger = baseHeadIsTrigger;
            TeleportPatch.TeleportPlayer(activationLocation, activationAngle);
            active = false;
            // Wait for the telport to complete
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            TriggerBoxPatches.triggersEnabled = true;
            Plugin.menuController.GetComponent<Fly>().enabled = flyWasEnabled;
            Logging.Debug("Enabling triggers");
        }

        public override string GetDisplayName()
        {
            return DisplayName;
        }

        public override string Tutorial()
        {
            return "Effect: Disables collisions. Automatically enables Fly (Use the sticks to move).";
        }

    }
}
