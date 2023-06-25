using AutoPlayer.Handlers;
using AutoPlayer.Utils;
using HarmonyLib;
using HMUI;
using IPA.Config.Data;
using IPA.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.XR;

namespace AutoPlayer.Patches
{
    internal class NotePatches
    {


        //pink cute

        #region Note Removal
        
        [HarmonyPatch(typeof(NoteController))]
        [HarmonyPatch("Dissolve")]
        internal class NoteControllerDissolvePatch
        {
            static void Prefix(NoteController __instance)
            {
                //NoteUtils.RemoveNote(__instance);
            }
        }        
        
        [HarmonyPatch(typeof(NoteController))]
        [HarmonyPatch("HandleNoteDidPassMissedMarkerEvent")]
        internal class NoteControllerHandleNoteDidPassMissedMarkerEventPatch
        {
            static void Prefix(NoteController __instance)
            {
                //NoteUtils.RemoveNote(__instance);
            }
        }

        //pink cute

        [HarmonyPatch(typeof(NoteController))]
        [HarmonyPatch("SendNoteWasCutEvent")]
        internal class NoteControllerCutPatch
        {
            static void Prefix(NoteController __instance)
            {
                NoteUtils.SetWasHit(__instance);
                //NoteUtils.RemoveNote(__instance);
            }
        }

        #endregion

        #region Add Note To List On Spawn And Order Notes

        [HarmonyPatch(typeof(NoteController))]
        [HarmonyPatch("Init")]
        internal class NoteControllerSpawnPatch
        {

            static void Postfix(NoteController __instance)
            {
                if (__instance is MultiplayerConnectedPlayerNoteController) return;

                List<NoteUtils.AutoPlayNoteInfo> noteConlist = NoteUtils.GetListFromColour(__instance.noteData.colorType);

                //code below ensures that Stacks/Windows make the saber spawn at the head
                bool ignoreForceSwing = false;
                var endRot = __instance.GetComponent<NoteJump>().GetField<Quaternion, NoteJump>("_endRotation");

                if (__instance.noteData.cutDirection == NoteCutDirection.Any && endRot == NoteCutDirectionExtensions.Rotation(NoteCutDirection.Down))
                {
                    if (noteConlist.Count > 0) {
                        endRot = Quaternion.Inverse(noteConlist[noteConlist.Count - 1].rotation);
                    }
                    ignoreForceSwing= true;
                    //var lookPos = new Vector3(notePos.x, notePos.y, zSaberOffset.z);
                    //swingAngle = Quaternion.LookRotation(lookPos - controller.position, Vector3.down);
                    //swingAngle = Quaternion.Inverse(prevQ);
                }

                float time = __instance.noteData.time;
               

                NoteUtils.AutoPlayNoteInfo toAdd = new NoteUtils.AutoPlayNoteInfo(__instance, endRot, time, __instance.noteData.timeToPrevColorNote, __instance.noteData.timeToNextColorNote, ignoreForceSwing);
                
                if (noteConlist.Count > 0) //is there another note in the list?
                {
                    var prevNote = noteConlist[noteConlist.Count - 1];

                    if (__instance.noteData.gameplayType != NoteData.GameplayType.Bomb)
                    {
                        if (__instance.noteData.time == prevNote.note.noteData.time) //are they on the same "Beat" (same point in time)
                        {
                            
                            Vector3 notePosA = new Vector3(__instance.beatPos.x, __instance.transform.position.y, __instance.beatPos.z);
                            Vector3 notePosB = new Vector3(prevNote.note.beatPos.x, prevNote.note.transform.position.y, prevNote.note.beatPos.z); //get note positions
                            var angle = endRot.eulerAngles.z;
                            Vector3 directionToOrigin = Vector3.Normalize(notePosA - notePosB);
                            Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.up;//get note direction

                            float dotProduct = Vector3.Dot(directionToOrigin, direction); //essennnttialllllyyyyy  is note A looking towards note B?

                            if (dotProduct > 0)
                            { //yes, this is a stack which has spawned its notes in the wrong order, swap them so the top of the stack is first.
                                noteConlist[noteConlist.Count - 1] = toAdd;
                                toAdd = prevNote;


                            }
                            else
                            {
                                //note correct!
                                toAdd.ignoreForceSwing= true;
                            }
                        }
                    }
                }

                noteConlist.Add(toAdd);

            }
        }
        #endregion


    }
}
