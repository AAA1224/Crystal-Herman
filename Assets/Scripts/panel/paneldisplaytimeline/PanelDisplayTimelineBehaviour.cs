using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using KoboldTools;

[Serializable]
public class PanelDisplayTimelineBehaviour : PlayableBehaviour
{
    public bool shouldOpen = false;
    public bool shouldClose = false;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (info.evaluationType == FrameData.EvaluationType.Playback)
        {
            this.shouldOpen = true;
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (info.evaluationType == FrameData.EvaluationType.Playback)
        {
            this.shouldClose = true;
        }
    }
}
