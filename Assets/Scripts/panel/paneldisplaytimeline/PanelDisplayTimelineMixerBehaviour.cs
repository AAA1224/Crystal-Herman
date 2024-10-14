using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using KoboldTools;

public class PanelDisplayTimelineMixerBehaviour : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        Panel trackBinding = playerData as Panel;

        if (trackBinding != null)
        {
            int inputCount = playable.GetInputCount();
            for (var i = 0; i < inputCount; i++)
            {
                ScriptPlayable<PanelDisplayTimelineBehaviour> inputPlayable = (ScriptPlayable<PanelDisplayTimelineBehaviour>)playable.GetInput(i);
                PanelDisplayTimelineBehaviour input = inputPlayable.GetBehaviour();

                if (input.shouldOpen)
                {
                    trackBinding.onOpen();
                    input.shouldOpen = false;
                }

                if (input.shouldClose)
                {
                    trackBinding.onClose();
                    input.shouldClose = false;
                }
            }
        }
    }
}
