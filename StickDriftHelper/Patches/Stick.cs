using HarmonyLib;
using HMUI;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.XR;

namespace StickDriftHelper.Patches
{
    internal class Stick
    {


        public static Vector2 ApplyDeadzone(Vector2 input, float deadzone)
        {
            float magnitude = input.magnitude;
            if (magnitude > deadzone)
            {
                float normalizedMagnitude = Mathf.InverseLerp(deadzone, 1.0f, magnitude);
                return input.normalized * normalizedMagnitude;
            }
            return Vector2.zero;
        }


        [HarmonyPatch(typeof(ScrollView))]
        [HarmonyPatch("HandleJoystickWasNotCenteredThisFrame")]
        internal class ScrollViewStickDrift
        {
            static bool Prefix(ScrollView __instance, ref Vector2 deltaPos)
            {
                if (!Config.Instance.Enabled) return true;
                deltaPos = ApplyDeadzone(deltaPos, Config.Instance.deadzone);

                if(deltaPos.sqrMagnitude < 0.01f)
                {
                    __instance.HandleJoystickWasCenteredThisFrame();
                    return false;
                }
                return true;

            }
        }
               


    }
}
