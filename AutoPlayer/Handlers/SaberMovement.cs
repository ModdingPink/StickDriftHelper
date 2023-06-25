using AutoPlayer.Utils;
using JoshaParity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static AutoPlayer.Handlers.AutoPlayHandler;

namespace AutoPlayer.Handlers
{
    public class SaberMovement
    {
        public ControllerPositions saber = new ControllerPositions(Vector3.zero, Quaternion.identity);

        public NoteUtils.AutoPlayNoteInfo? prevData;

        public Quaternion noteRotationAngle = Quaternion.identity;
        public Quaternion noteHitAngle = Quaternion.identity;
        public Quaternion lookingAngle = Quaternion.identity;
        public Vector3 lookingNoteAxis = Vector3.zero;

        public void ResetData()
        {
            swingIndex = 0;
            prevData = null;
            noteRotationAngle = Quaternion.identity;
            noteHitAngle = Quaternion.identity;
            firstFrame = true;
        }

        int swingIndex = 0;
        float _beatTimeToReachSabers = 0.2f;
        float timeOffset = 0.15f;
        bool firstFrame = false;

        float angleLerp = 0;

        public void UpdateMovement(ControllerPositions controller, ColorType type, List<SwingData> swingData, float bpm, IAudioTimeSource _iAudioTimeSource, BeatmapObjectSpawnController _beatmapObjectSpawnController,  out Quaternion rotation, out Vector3 position)
        {
            if (swingData == null) {
                rotation = Quaternion.identity;
                position = Vector3.zero;

                return;

            }
            float currentTime = _iAudioTimeSource.songTime;


            SwingData currentSwing = swingData[swingIndex];
            SwingData? nextSwing = (swingData.Count > swingIndex) ? swingData[swingIndex + 1] : null;
            SwingData? prevSwing = (0 < swingIndex) ? swingData[swingIndex - 1] : null;


            float swingStart = (currentSwing.swingStartBeat * (60.0f / bpm)) - timeOffset;
            float swingEnd = (currentSwing.swingEndBeat * (60.0f / bpm)) + (timeOffset/2);
            float lastSwingEnd = swingStart;
            float nextSwingStart = swingEnd;
            if (prevSwing != null)
            {
                lastSwingEnd = (prevSwing.Value.swingEndBeat * (60.0f / bpm)) + timeOffset;
                if (swingStart < lastSwingEnd) { swingStart = (swingStart + lastSwingEnd) / 2; }
            }
            if (nextSwing != null)
            {
                nextSwingStart = (nextSwing.Value.swingStartBeat * (60.0f / bpm)) - timeOffset;
                if (swingEnd > nextSwingStart) { swingEnd = (swingEnd + nextSwingStart) / 2; }
            }

            if (swingData.Count > swingIndex)
            {
               
                if (currentTime > swingEnd)
                {
                    firstFrame = true;
                    swingIndex++;
                    currentSwing = swingData[swingIndex];
                }
                else
                {
                    firstFrame = false;
                }
            }

            var AllNotes = NoteUtils.GetListFromColour(type);

            var newNote = AllNotes.Where(note => note.note.noteData.colorType == type && note.noteTime >= swingStart && note.noteTime <= swingEnd && note.itemWasCut == false);

            /*
            if (note == null) return;

            var _jump = note.Value.note.gameObject.GetComponent<NoteJump>();
            Vector3 _startPos = _beatmapObjectSpawnController.Get2DNoteOffset((int)currentSwing.startPos.x, (NoteLineLayer)currentSwing.startPos.y);
            var _startVerticalVelocity = _jump.GetField<float, NoteJump>("_startVerticalVelocity");
            var _gravity = _jump.GetField<float, NoteJump>("_gravity");
            var _jumpDuration = _jump.GetField<float, NoteJump>("_jumpDuration");
            var num = _jumpDuration * 0.5f;
            */



            float time = Mathf.InverseLerp(swingStart, swingEnd, currentTime);
            float betweenTime = Mathf.InverseLerp(lastSwingEnd, swingStart, currentTime);

            //Debug.Log(time);

            float angle = (currentSwing.swingParity == Parity.Forehand) ? 120 : -120;

            float rot = currentSwing.startPos.rotation * (currentSwing.rightHand ? 1 : -1); 

            //Vector2 rotationAxis =  Quaternion.Euler(0, 0, rot) * new Vector2(0, (time * ());
            Vector2 rotationAxis =  Quaternion.Euler(0, 0, rot) * new Vector2(0, (currentSwing.swingParity == Parity.Forehand) ? -1 : 1);


            float zPivot = (1 - rotationAxis.magnitude);

            //var RotationAngle = Quaternion.LookRotation(new Vector3(rotationAxis.x, rotationAxis.y, zPivot) * 100, Vector2.right);
            
            //rotation = RotationAngle * Quaternion.AngleAxis(((testAngle * 2) - 1) - 20, Vector3.up);

            var lerpedRotation = Mathf.Lerp(currentSwing.startPos.rotation, currentSwing.endPos.rotation, time);
            var lerpedSwingAngle = Mathf.Lerp(angle * -1, angle, time);
            if (!currentSwing.rightHand) {
                lerpedRotation *= -1;
            }

            float lastLerpedRotation = lerpedRotation;
            if (prevSwing.HasValue)
            {
                lastLerpedRotation = prevSwing.HasValue ? Mathf.Lerp(prevSwing.Value.startPos.rotation, prevSwing.Value.endPos.rotation, time) : lerpedRotation;
                if (!prevSwing.Value.rightHand)
                {
                    lastLerpedRotation *= -1;
                }
            }
            

            angleLerp = Mathf.Lerp(lastLerpedRotation, lerpedRotation, betweenTime);

            var lerpRot = Quaternion.AngleAxis(angleLerp, Vector3.forward); //spin left/right

            //noteRotationAngle = Quaternion.Lerp(lerpRot, noteRotationAngle, time);
            //lerpRot = noteRotationAngle;

            var lerpAngle = Quaternion.AngleAxis(lerpedSwingAngle, Vector3.right); //swing up/down

            rotation = lerpRot * lerpAngle;

            var note = newNote.Count() > 0 ? newNote.First().note : null;
            if (note == null)
            {
                position = controller.position;
                return;
            }

            var lookRotation = Quaternion.LookRotation(note.transform.position - controller.position, Vector3.up);


            float distanceFromNote;
            distanceFromNote = NoteUtils.GetRotatedForwardFacingPositionFromNote(note).z;

            float addedSpeed = Mathf.Clamp((distanceFromNote / -50) + 1, 0, 1);
            addedSpeed = Mathf.Pow(addedSpeed + 0.05f, 9f);

            var notePos = Vector3.Lerp(new Vector3(_beatmapObjectSpawnController.Get2DNoteOffset((int)currentSwing.startPos.x, NoteLineLayer.Base).x, note.transform.position.y, 0), new Vector3(_beatmapObjectSpawnController.Get2DNoteOffset((int)currentSwing.endPos.x, NoteLineLayer.Base).x, note.transform.position.y, 0), time);
            position = Vector3.Lerp(controller.position, notePos, Time.deltaTime * addedSpeed * 8);



            //position = notePos;

            //rotation = lookRotation;

            //rotation *= Quaternion.Lerp(Quaternion.Euler(rightController.rotation.eulerAngles.x, 0, 0), Quaternion.Euler(angle, 0, 0), time);
            //position = Vector2.Lerp(_beatmapObjectSpawnController.Get2DNoteOffset((int)currentSwing.startPos.x, (NoteLineLayer)currentSwing.startPos.y), _beatmapObjectSpawnController.Get2DNoteOffset((int)currentSwing.endPos.x, (NoteLineLayer)currentSwing.endPos.y), time);

            //leftSaber.UpdateMovement(ColorType.ColorA, _iAudioTimeSource, out leftController.rotation, out leftController.position);
            //rightSaber.UpdateMovement(ColorType.ColorB, _iAudioTimeSource, out rightController.rotation, out rightController.position);

        }


    }

}
