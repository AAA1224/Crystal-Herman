using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using KoboldTools;

[Serializable]
public class PanelDisplayTimelineClip : PlayableAsset, ITimelineClipAsset
{
    public PanelDisplayTimelineBehaviour template = new PanelDisplayTimelineBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.Extrapolation; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<PanelDisplayTimelineBehaviour>.Create (graph, template);
        return playable;
    }
}
