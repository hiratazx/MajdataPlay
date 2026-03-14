using MajdataPlay.Scenes.Game.Notes.Slide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MajdataPlay.Game.Buffers;
public struct SlideMetadata
{
    public string SlideType { get; init; }
    public int StartPos { get; init; }
    public bool IsMirror { get; init; }

    public Vector3 SlideOkPosition { get; init; }
    public Quaternion SlideOkRotation { get; init; }

    public SlideTable SlideTable { get; init; }

    public ReadOnlySpan<Vector3> SlideBarPositions => _slideBarPositions ?? ReadOnlySpan<Vector3>.Empty;
    public ReadOnlySpan<Quaternion> SlideBarRotations => _slideBarRotations ?? ReadOnlySpan<Quaternion>.Empty;
    public ReadOnlySpan<Vector3> SlideStarPositions => _slideStarPositions ?? ReadOnlySpan<Vector3>.Empty;
    public ReadOnlySpan<Quaternion> SlideStarRotations => _slideStarRotations ?? ReadOnlySpan<Quaternion>.Empty;

    readonly Vector3[] _slideBarPositions;
    readonly Quaternion[] _slideBarRotations;
    readonly Vector3[] _slideStarPositions;
    readonly Quaternion[] _slideStarRotations;

    public SlideMetadata(Vector3[] a, Quaternion[] b, Vector3[] c, Quaternion[] d)
    {
        _slideBarPositions = a;
        _slideBarRotations = b;
        _slideStarPositions = c;
        _slideStarRotations = d;
    }
}
