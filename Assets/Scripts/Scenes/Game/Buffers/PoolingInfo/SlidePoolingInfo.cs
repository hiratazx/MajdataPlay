using MajdataPlay.Game.Buffers;
using MajdataPlay.Scenes.Game.Notes.Slide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MajdataPlay.Scenes.Game.Buffers;
internal class SlidePoolingInfo : NotePoolingInfo
{
    public string SlideType { get; init; }
    public bool IsMirror { get; init; }
    public bool IsJustR { get; init; }
    public int EndPos { get; init; }
    public float StartTiming { get; init; }
    public float Length { get; init; }
    public bool IsSlideNoHead { get; init; }
    public bool IsSlideNoTrack { get; init; }
    public int Multiple { get; init; }
    public SlideMetadata Metadata { get; init; }
    public ConnSlideInfo ConnSlideInfo { get; init; }
}
