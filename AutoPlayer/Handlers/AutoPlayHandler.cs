
using AutoPlayer.Utils;
using IPA.Utilities;
using JoshaParity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using Zenject;
using static BeatmapSaveDataVersion2_6_0AndEarlier.BeatmapSaveData;

namespace AutoPlayer.Handlers
{
    public class AutoPlayHandler : MonoBehaviour
    {

        public static AutoPlayHandler Instance = new AutoPlayHandler();

        //Zenject
        public IAudioTimeSource _iAudioTimeSource;
        public IGamePause? _pause;
        public BeatmapObjectSpawnController? _beatmapObjectSpawnController;
        bool init = false;

        //Pause
        public bool isPaused = false;
        public bool isPauseAnimation = false;

        public static bool isNoodleExtensions = false;
        public static bool levelIsRotational = false;
        public static Transform playerRoot;

        public static Vector3 zSaberOffset = new Vector3(0, 0, 0.1f);





        public struct ControllerPositions
        {
            public ControllerPositions(Vector3 _position, Quaternion _rotation)
            {
                position = _position;
                rotation = _rotation;
            }
            public Vector3 position;
            public Quaternion rotation;
        }


        public static ControllerPositions headset = new ControllerPositions(new Vector3(0, 1.7f, 0f), Quaternion.identity);


        public static SaberMovement leftSaber = new SaberMovement();
        public static SaberMovement rightSaber = new SaberMovement();
        public static ControllerPositions leftController = new ControllerPositions(Vector3.zero, Quaternion.identity);
        public static ControllerPositions rightController = new ControllerPositions(Vector3.zero, Quaternion.identity);


        [Inject]
        public void Construct(IGamePause pause, IAudioTimeSource iAudioTimeSource, BeatmapObjectSpawnController beatmapObjectSpawnController)
        {
            _beatmapObjectSpawnController = beatmapObjectSpawnController;
            _pause = pause;
            _iAudioTimeSource = iAudioTimeSource;
            init = true;
        }



        public void Start()
        {
            Instance = this;

            ResetData();
            if (_pause == null) { Debug.Log("Pause is Null, Not Controllable Zenject failed."); return; }
            _pause.didPauseEvent += delegate
            {
                isPaused = true;
                isPauseAnimation = true;
            };
            _pause.didResumeEvent += delegate { isPaused = false; };
            _pause.willResumeEvent += delegate { isPauseAnimation = false; };
            isPaused = false;
            isPauseAnimation = false;


        }





        static List<SwingData> swingDataLeft = new List<SwingData>();
        static List<SwingData> swingDataRight = new List<SwingData>();
        static float bpm;
        public void ResetData()
        {
            BS_Utils.Gameplay.ScoreSubmission.DisableSubmission("AutoPlayer");
            NoteUtils.ResetAllListData();


            var beatmap = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.difficultyBeatmap;
            var beatmapData = SongCore.Collections.RetrieveDifficultyData(beatmap);
            if (beatmap != null)
            {
                levelIsRotational = beatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.name.Contains("Degree");
                if (beatmapData != null)
                {
                    isNoodleExtensions = beatmapData.additionalDifficultyData._requirements.Contains("Noodle Extensions") || beatmapData.additionalDifficultyData._suggestions.Contains("Noodle Extensions");
                    if (isNoodleExtensions) playerRoot = GameObject.Find("LocalPlayerGameCore").transform;
                }
                else
                {
                    isNoodleExtensions = false;
                }
            }
            else
            {
                isNoodleExtensions = false;
            }

            var level = (CustomPreviewBeatmapLevel)beatmap.level;
            MapAnalyser ana = new MapAnalyser(level.customLevelPath);
            Debug.Log(beatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName);
            leftSaber.ResetData();
            rightSaber.ResetData();

            SortSwingData(ana.GetSwingData((BeatmapDifficultyRank)beatmap.difficultyRank, beatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName.ToLower()), out swingDataLeft, out swingDataRight);
            bpm = level.beatsPerMinute;

            
            //Debug.Log(swingData[0].ToString());
        }


        public void SortSwingData(List<SwingData>? swingData, out List<SwingData> left, out List<SwingData> right)
        {
            left = new List<SwingData>();
            right = new List<SwingData>();
            if (swingData == null) return;
            foreach (var swing in swingData)
            {
                if (swing.rightHand)
                {
                    right.Add(swing);
                }
                else { 
                    left.Add(swing); 
                }
            }
        }


        public void Update()
        {
            Debug.Log(swingDataLeft.Count);
            Quaternion leftRot; Vector3 leftPos;
            Quaternion rightRot; Vector3 rightPos;
            leftSaber.UpdateMovement(leftController, ColorType.ColorA, swingDataLeft, bpm, _iAudioTimeSource, _beatmapObjectSpawnController, out leftRot, out leftPos);
            rightSaber.UpdateMovement(rightController, ColorType.ColorB, swingDataRight, bpm, _iAudioTimeSource, _beatmapObjectSpawnController, out rightRot, out rightPos);

            leftController.position = leftPos;
            leftController.rotation = leftRot;
            rightController.position = rightPos;
            rightController.rotation = rightRot;

        }

    }
}
