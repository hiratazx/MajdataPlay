using MajdataPlay.Buffers;
using MajdataPlay.IO;
using MajdataPlay.Scenes.Game.Notes.Behaviours;
using MajdataPlay.Scenes.Game.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MajdataPlay.Game.Utils;
internal static class SlideHelper
{
    readonly static Dictionary<string, Vector3[][]> _slideBarPositions = new();
    readonly static Dictionary<string, Vector3[][]> _slideStarPositions = new();
    readonly static Dictionary<string, Vector3[]> _slideOkPositions = new();

    readonly static Dictionary<string, Vector3[][]> _slideBarMirrorPositions = new();
    readonly static Dictionary<string, Vector3[][]> _slideStarMirrorPositions = new();
    readonly static Dictionary<string, Vector3[]> _slideOkMirrorPositions = new();

    readonly static Dictionary<string, Quaternion[][]> _slideBarRotations = new();
    readonly static Dictionary<string, Quaternion[][]> _slideStarRotations = new();
    readonly static Dictionary<string, Quaternion[]> _slideOkRotations = new();

    readonly static Dictionary<string, Quaternion[][]> _slideBarMirrorRotations = new();
    readonly static Dictionary<string, Quaternion[][]> _slideStarMirrorRotations = new();
    readonly static Dictionary<string, Quaternion[]> _slideOkMirrorRotations = new();

    static bool _isInited = false;
    public static void Init(GameObject[] prefabs, 
        IReadOnlyDictionary<string, int> prefabMappingTable,
        Transform parent)
    {
        if(_isInited)
        {
            return;
        }
        for (var sb = 0; sb < prefabs.Length; sb++)
        {
            var prefab = prefabs[sb];
            var isWifi = prefab.name == "Slide_Wifi";
            using var totalBarPositions = new RentedList<Vector3[]>();
            using var totalBarRotations = new RentedList<Quaternion[]>();

            using var totalStarPositions = new RentedList<Vector3[]>();
            using var totalStarRotations = new RentedList<Quaternion[]>();

            using var slideOkPositions = new RentedList<Vector3>();
            using var slideOkRotations = new RentedList<Quaternion>();

            using var barPositions = new RentedList<Vector3>();
            using var barRotations = new RentedList<Quaternion>();
            using var starPositions = new RentedList<Vector3>();
            using var starRotations = new RentedList<Quaternion>();
            using var slideBars = new RentedList<GameObject>();
            if (isWifi)
            {
                continue;
            }
            else
            {
                var slideType = string.Empty;
                var isMirror = false;
            MIRROR_START:
                for (var j = 1; j < 9; j++)
                {
                    var instance = GameObject.Instantiate(prefab, parent);
                    var slideDrop = instance.GetComponent<SlideDrop>();
                    slideType = slideDrop.SlideType;
                    var slideEndPos = slideType switch
                    {
                        "line3" => 3,
                        "line4" => 4,
                        "line5" => 5,
                        "line6" => 6,
                        "line7" => 7,
                        "circle1" => 2,
                        "circle2" => 3,
                        "circle3" => 4,
                        "circle4" => 5,
                        "circle5" => 6,
                        "circle6" => 7,
                        "circle7" => 8,
                        "circle8" => 1,
                        "v1" => 1,
                        "v2" => 2,
                        "v3" => 3,
                        "v4" => 4,
                        "v6" => 6,
                        "v7" => 7,
                        "v8" => 8,
                        "ppqq1" => 1,
                        "ppqq2" => 2,
                        "ppqq3" => 3,
                        "ppqq4" => 4,
                        "ppqq5" => 5,
                        "ppqq6" => 6,
                        "ppqq7" => 7,
                        "ppqq8" => 8,
                        "pq1" => 1,
                        "pq2" => 2,
                        "pq3" => 3,
                        "pq4" => 4,
                        "pq5" => 5,
                        "pq6" => 6,
                        "pq7" => 7,
                        "pq8" => 8,
                        "s" => 5,
                        "L2" => 2,
                        "L3" => 3,
                        "L4" => 4,
                        "L5" => 5,
                        _ => 1
                    };
                    var posDiff =  - slideDrop.StartPos;
                    slideDrop.StartPos = j;
                    var endPosArea = ((SensorArea)(j - 1)).Diff(posDiff);
                    if (isMirror)
                    {
                        slideEndPos = (int)endPosArea.Mirror(SensorArea.A1);
                    }
                    else
                    {
                        slideEndPos = (int)endPosArea;
                    }

                    slideEndPos += 1;
                    slideDrop.EndPos = slideEndPos;
                    if (isMirror)
                    {
                        slideDrop.Transform.localScale = new Vector3(-1f, 1f, 1f);
                        slideDrop.Transform.rotation = Quaternion.Euler(0f, 0f, -45f * j);
                    }
                    else
                    {
                        slideDrop.Transform.rotation = Quaternion.Euler(0f, 0f, -45f * (j - 1));
                    }
                    for (var i = 0; i < slideDrop.Transform.childCount - 1; i++)
                    {
                        var bar = slideDrop.Transform.GetChild(i).gameObject;
                        slideBars.Add(bar);
                        barPositions.Add(bar.transform.position);
                        barRotations.Add(bar.transform.rotation);
                    }
                    var slideOk = slideDrop.Transform.GetChild(slideDrop.Transform.childCount - 1);
                    slideOkPositions.Add(slideOk.position);
                    slideOkRotations.Add(slideOk.rotation);
                    starPositions.Add(NoteHelper.GetTapPosition(j, 4.8f));
                    for (var i = 0; i < slideBars.Count; i++)
                    {
                        var bar = slideBars[i];
                        starPositions.Add(bar.transform.position);

                        starRotations.Add(Quaternion.Euler(bar.transform.rotation.normalized.eulerAngles + new Vector3(0f, 0f, 18f)));
                        if (i == slideBars.Count - 1)
                        {
                            var a = slideBars[i - 1].transform.rotation.normalized.eulerAngles;
                            var b = bar.transform.rotation.normalized.eulerAngles;
                            var diff = a - b;
                            var newEulerAugle = b - diff;
                            starRotations.Add(Quaternion.Euler(newEulerAugle + new Vector3(0f, 0f, 18f)));
                        }
                    }
                    var endPos = NoteHelper.GetTapPosition(slideEndPos, 4.8f);
                    starPositions.Add(endPos);
                    totalBarPositions.Add(barPositions.ToArray());
                    totalBarRotations.Add(barRotations.ToArray());
                    totalStarPositions.Add(starPositions.ToArray());
                    totalStarRotations.Add(starRotations.ToArray());

                    barPositions.Clear();
                    barRotations.Clear();
                    starPositions.Clear();
                    starRotations.Clear();
                    slideBars.Clear();
                    GameObject.DestroyImmediate(instance);
                }

                if (!isMirror)
                {
                    isMirror = true;
                    _slideBarPositions.Add(slideType, totalBarPositions.ToArray());
                    _slideBarRotations.Add(slideType, totalBarRotations.ToArray());
                    _slideStarPositions.Add(slideType, totalStarPositions.ToArray());
                    _slideStarRotations.Add(slideType, totalStarRotations.ToArray());

                    _slideOkPositions.Add(slideType, slideOkPositions.ToArray());
                    _slideOkRotations.Add(slideType, slideOkRotations.ToArray());

                    totalBarPositions.Clear();
                    totalBarRotations.Clear();
                    totalStarPositions.Clear();
                    totalStarRotations.Clear();
                    slideOkPositions.Clear();
                    slideOkRotations.Clear();
                    goto MIRROR_START;
                }
                _slideBarMirrorPositions.Add(slideType, totalBarPositions.ToArray());
                _slideBarMirrorRotations.Add(slideType, totalBarRotations.ToArray());
                _slideStarMirrorPositions.Add(slideType, totalStarPositions.ToArray());
                _slideStarMirrorRotations.Add(slideType, totalStarRotations.ToArray());

                _slideOkMirrorPositions.Add(slideType, slideOkPositions.ToArray());
                _slideOkMirrorRotations.Add(slideType, slideOkRotations.ToArray());
            }
        }
        _isInited = true;
    }
    public static SlidePosMetadata GetSlidePosMetadata(string slideType, int startPos, bool isMirror)
    {
        if(isMirror)
        {
            return new()
            {
                SlideType = slideType,
                StartPos = startPos,
                IsMirror = true,
                SlideOkPosition = _slideOkMirrorPositions[slideType][startPos],
                SlideOkRotation = _slideOkMirrorRotations[slideType][startPos],

                SlideBarPositions = _slideBarMirrorPositions[slideType][startPos],
                SlideBarRotations = _slideBarMirrorRotations[slideType][startPos],

                SlideStarPositions = _slideStarMirrorPositions[slideType][startPos],
                SlideStarRotations = _slideStarMirrorRotations[slideType][startPos]
            };
        }
        else
        {
            return new()
            {
                SlideType = slideType,
                StartPos = startPos,
                IsMirror = false,
                SlideOkPosition = _slideOkPositions[slideType][startPos],
                SlideOkRotation = _slideOkRotations[slideType][startPos],

                SlideBarPositions = _slideBarPositions[slideType][startPos],
                SlideBarRotations = _slideBarRotations[slideType][startPos],

                SlideStarPositions = _slideStarPositions[slideType][startPos],
                SlideStarRotations = _slideStarRotations[slideType][startPos]
            };
        }
    }

    public readonly struct SlidePosMetadata
    {
        public string SlideType { get; init; }
        public int StartPos { get; init; }
        public bool IsMirror { get; init; }

        public Vector3 SlideOkPosition { get; init; }
        public Quaternion SlideOkRotation { get; init; }

        public Vector3[] SlideBarPositions { get; init; }
        public Quaternion[] SlideBarRotations { get; init; }
        public Vector3[] SlideStarPositions { get; init; }
        public Quaternion[] SlideStarRotations { get; init; }
    }
}
