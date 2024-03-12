﻿using Bark.GUI;
using Bark.Patches;
using BepInEx.Configuration;
using GorillaLocomotion;
using UnityEngine;

namespace Bark.Modules.Physics
{
    public class NoSlip: BarkModule
    {
        public static readonly string DisplayName = "No Slip";
        public static NoSlip Instance;

        void Awake() { Instance = this; }

        protected override void OnEnable()
        {
            if (!MenuController.Instance.Built) return;
            base.OnEnable();
            if (SlipperyHands.Instance)
                SlipperyHands.Instance.enabled = false;
        }

        protected override void Cleanup() 
        {
            string s = $"The functionality for this module is in {nameof(SlidePatch) }";
        }

        public override string GetDisplayName()
        {
            return DisplayName;
        }

        public override string Tutorial()
        {
            return "Effect: You no longer slide on slippery surfaces.";
        }

    }
}
