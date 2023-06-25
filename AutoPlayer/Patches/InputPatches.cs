using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.XR;
using UnityEngine;
using AutoPlayer.Handlers;

namespace AutoPlayer.Patches
{
    internal class InputPatches
    {

        #region Controller Input Patch
        [HarmonyPatch(typeof(DevicelessVRHelper))]
        [HarmonyPatch(nameof(IVRPlatformHelper.GetNodePose))]
        internal class ControllerGetNodePosePatch
        {
            static bool Prefix(DevicelessVRHelper __instance, ref bool __result, XRNode nodeType, int idx, out Vector3 pos, out Quaternion rot)
            {
                rot = Quaternion.identity;
                pos = Vector3.one;

                if (nodeType == XRNode.LeftHand)
                {
                    pos = AutoPlayHandler.leftController.position;
                    rot = AutoPlayHandler.leftController.rotation;
                    __result = true;
                }
                else if (nodeType == XRNode.RightHand)
                {
                    pos = AutoPlayHandler.rightController.position;
                    rot = AutoPlayHandler.rightController.rotation;
                    __result = true;
                }
                return false;
            }
        }
        #endregion

    }
}
