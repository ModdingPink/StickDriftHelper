using AutoPlayer.Handlers;
using IPA.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoPlayer.Utils
{
    public static class NoteUtils
    {


        public struct AutoPlayNoteInfo
        {
            public AutoPlayNoteInfo(NoteController _note, Quaternion _rotation, float _noteTime, float _prevNoteTime, float _nextNoteTime, bool _ignoreForceSwing)
            {
                rotation = _rotation;
                note = _note;
                timeToNextNote = _nextNoteTime;
                timeToPrevNote = _prevNoteTime;

                noteTime = _noteTime;

                ignoreForceSwing = _ignoreForceSwing;
                itemWasCut = false;
            }
            public NoteController note;
            public Quaternion rotation;
            public float timeToNextNote;
            public float timeToPrevNote;            
            public float noteTime;
            public bool ignoreForceSwing;
            public bool itemWasCut;

        }


        static List<ObstacleController> obstacleList = new List<ObstacleController>();
        static List<AutoPlayNoteInfo> bombList = new List<AutoPlayNoteInfo>();
        static List<AutoPlayNoteInfo> noteListLeft = new List<AutoPlayNoteInfo>();
        static List<AutoPlayNoteInfo> noteListRight = new List<AutoPlayNoteInfo>();


        public static IEnumerator RemoveNote(NoteController __instance, float time)
        {
            yield return new WaitForSeconds(time);
            List<NoteUtils.AutoPlayNoteInfo> noteConlist;

            if (__instance.noteData.gameplayType == NoteData.GameplayType.Bomb)
            {
                noteConlist = NoteUtils.GetListFromColour(ColorType.None);
            }
            else
            {
                noteConlist = NoteUtils.GetListFromColour(__instance.noteData.colorType);
            }

            foreach (var item in noteConlist)
            {
                if (item.note == __instance && item.noteTime == __instance.noteData.time)
                {
                    noteConlist.Remove(item);
                    break;
                }
            }


        }

        public static void RemoveNote(NoteController __instance)
        {
            List<NoteUtils.AutoPlayNoteInfo> noteConlist;

            if (__instance.noteData.gameplayType == NoteData.GameplayType.Bomb)
            {
                noteConlist = NoteUtils.GetListFromColour(ColorType.None);
            }
            else
            {
                noteConlist = NoteUtils.GetListFromColour(__instance.noteData.colorType);
            }

            foreach (var item in noteConlist)
            {
                if (item.note == __instance)
                {
                    noteConlist.Remove(item);
                    break;
                }
            }

        }

        public static void SetWasHit(NoteController __instance)
        {
            List<NoteUtils.AutoPlayNoteInfo> noteConlist;

            if (__instance.noteData.gameplayType == NoteData.GameplayType.Bomb)
            {
                noteConlist = NoteUtils.GetListFromColour(ColorType.None);
            }
            else
            {
                noteConlist = NoteUtils.GetListFromColour(__instance.noteData.colorType);
            }

            for (int i = 0; i < noteConlist.Count; i++)
            {
                if (noteConlist[i].note == __instance)
                {
                    var modifiedItem = noteConlist[i];
                    modifiedItem.itemWasCut = true;
                    noteConlist[i] = modifiedItem;
                    break;
                }
            }

        }

        public static void RemoveNote(NoteUtils.AutoPlayNoteInfo noteInfo)
        {
            List<NoteUtils.AutoPlayNoteInfo> noteConlist;

            if (noteInfo.note.noteData.gameplayType == NoteData.GameplayType.Bomb)
            {
                noteConlist = NoteUtils.GetListFromColour(ColorType.None);
            }
            else
            {
                noteConlist = NoteUtils.GetListFromColour(noteInfo.note.noteData.colorType);
            }

            if (noteConlist.Contains(noteInfo))
            {
                noteConlist.Remove(noteInfo);
            }

        }



        public static bool ContainsNote(List<AutoPlayNoteInfo> list, NoteController _note)
        {
            foreach (var item in list)
            {
                if (item.note == _note)
                {
                    return true;
                }
            }

            return false;

        }


        public static void ResetAllListData()
        {
            obstacleList.Clear();
            bombList.Clear();
            noteListLeft.Clear();
            noteListRight.Clear();
        }



        public static ObstacleController GetCurrentWall(int noteWanted = 0)
        {
            if (obstacleList.Count > noteWanted)
            {
                if (obstacleList[noteWanted])
                {
                    return obstacleList[noteWanted];
                }
                else
                {
                    obstacleList.RemoveAt(noteWanted);
                    return GetCurrentWall(noteWanted);
                }
            }
            return null;
        }

        public static AutoPlayNoteInfo? GetBombData(int noteWanted = 0)
        {
            return GetDataFromList(bombList, noteWanted);
        }

        public static AutoPlayNoteInfo? GetDataFromColour(ColorType colorType, int noteWanted = 0)
        {
            var note = GetDataFromList(GetListFromColour(colorType), noteWanted);
            return note;
        }

        public static List<AutoPlayNoteInfo> GetListFromColour(ColorType colorType)
        {
            if (colorType == ColorType.None) return bombList;
            return (colorType == ColorType.ColorA) ? noteListLeft : noteListRight;
        }

        public static bool IsNotePreMissLine(NoteController note)
        {
            var distance = note.transform.position.z;
            return distance <= (AutoPlayHandler.zSaberOffset.z - 0.05f);
        }


        public static AutoPlayNoteInfo? GetDataFromList(List<AutoPlayNoteInfo> noteList, int noteWanted = 0)
        {
            if (noteList.Count > noteWanted)
            {
                if (noteList[noteWanted].note.noteData.scoringType == NoteData.ScoringType.Ignore || noteList[noteWanted].note.noteData.scoringType == NoteData.ScoringType.NoScore)
                {
                    noteList.RemoveAt(noteWanted);
                    return GetDataFromList(noteList, noteWanted);
                }
                return noteList[noteWanted];
            }
            return null;
        }




        public static float GetTimeFromZPosition(NoteController note, float zPosition)
        {
            var _jump = note.gameObject.GetComponent<NoteJump>();
            var _startPos = _jump.GetField<Vector3, NoteJump>("_startPos");
            var _endPos = _jump.GetField<Vector3, NoteJump>("_endPos");
            var _beatTime = _jump.GetField<float, NoteJump>("_beatTime");
            var _jumpDuration = _jump.GetField<float, NoteJump>("_jumpDuration");
            var _inverseWorldRotation = _jump.GetField<Quaternion, NoteJump>("_inverseWorldRotation");
            var _playerTransforms = _jump.GetField<PlayerTransforms, NoteJump>("_playerTransforms");
            float num2 = Mathf.InverseLerp(_startPos.z, _endPos.z, zPosition);
            float num = _beatTime - _jumpDuration * 0.5f + num2 * _jumpDuration;
            return num;
        }

        public static float GetTimeOffsetFromNotePositioningSystem(NoteController note, float zPosition)
        {
            var _jump = note.gameObject.GetComponent<NoteJump>();
            var _atsc = _jump.GetField<IAudioTimeSource, NoteJump>("_audioTimeSyncController");
            return _atsc.songTime - GetTimeFromZPosition(note, zPosition);
        }

        public static float GetZPositionFromTime(NoteController note, float songTime)
        {
            var _jump = note.gameObject.GetComponent<NoteJump>();
            var _startPos = _jump.GetField<Vector3, NoteJump>("_startPos");
            var _endPos = _jump.GetField<Vector3, NoteJump>("_endPos");
            var _beatTime = _jump.GetField<float, NoteJump>("_beatTime");
            var _jumpDuration = _jump.GetField<float, NoteJump>("_jumpDuration");
            var _inverseWorldRotation = _jump.GetField<Quaternion, NoteJump>("_inverseWorldRotation");
            var _playerTransforms = _jump.GetField<PlayerTransforms, NoteJump>("_playerTransforms");
            float num = songTime - (_beatTime - _jumpDuration * 0.5f);
            float num2 = num / _jumpDuration;
            float num3 = _playerTransforms.HeadOffsetZ(_inverseWorldRotation);
            return Mathf.LerpUnclamped(_startPos.z + num3 * Mathf.Min(1f, num2 * 2f), _endPos.z + num3, num2);

        }

        public static Vector2 GetPosFromNote(NoteController currentItem)
        {
            if (AutoPlayHandler.isNoodleExtensions) return Quaternion.Inverse(currentItem.transform.rotation) * currentItem.transform.position;
            var _jump = currentItem.gameObject.GetComponent<NoteJump>(); //thank you goobieeeee stolen from accdot
            var _startPos = _jump.GetField<Vector3, NoteJump>("_startPos");
            var _endPos = _jump.GetField<Vector3, NoteJump>("_endPos");
            var _startVerticalVelocity = _jump.GetField<float, NoteJump>("_startVerticalVelocity");
            var _gravity = _jump.GetField<float, NoteJump>("_gravity");
            var _jumpDuration = _jump.GetField<float, NoteJump>("_jumpDuration");
            var num = _jumpDuration * 0.5f;

            var pos = new Vector2(_endPos.x, (_startPos.y + _startVerticalVelocity * num - _gravity * num * num * 0.5f));

            //var pos = new Vector2(((_startPos - currentItem.beatPos).normalized + currentItem.beatPos).x, (_startPos.y + _startVerticalVelocity * num - _gravity * num * num * 0.5f));
            return pos;
            //return pos + (NoteCutDirectionExtensions.Direction(currentItem.noteData.cutDirection) * directionOffset);
        }

        public static Vector2 AddExtraPositionFromNote(Vector2 originalPos, NoteData currentItem, float offset)
        {
            if (!NoteCutDirectionExtensions.IsMainDirection(currentItem.cutDirection)) offset *= 0.70710678118f;
            return originalPos + (NoteCutDirectionExtensions.Direction(currentItem.cutDirection) * -offset);
        }
        public static Vector3 GetRotatedForwardFacingPositionFromNote(NoteController note)
        {
            return Quaternion.Inverse(note.transform.rotation) * note.transform.position;
        }


        public static float GetDistanceFromSaber(NoteController note, bool calculateDotProduct = true)
        {
            Vector3 centerPoint = AutoPlayHandler.zSaberOffset;
            if (AutoPlayHandler.isNoodleExtensions)
            {
                centerPoint += AutoPlayHandler.playerRoot.transform.position;
            }
            int direction = 1;
            if (calculateDotProduct)
            {
                Vector3 directionToOrigin = Vector3.Normalize(note.transform.position - centerPoint);
                float dotProduct = Vector3.Dot(directionToOrigin, AutoPlayHandler.headset.rotation * Vector3.forward);
                if (dotProduct < 0) direction = -1;
            }
            float distance = Vector3.Distance(NoteUtils.GetRotatedForwardFacingPositionFromNote(note), centerPoint) * direction;

            return distance;
        }


    }
}

