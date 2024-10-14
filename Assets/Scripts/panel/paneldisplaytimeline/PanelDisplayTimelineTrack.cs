using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using KoboldTools;

[TrackColor(0.152952f, 0.5186608f, 0.9044118f)]
[TrackClipType(typeof(PanelDisplayTimelineClip))]
[TrackBindingType(typeof(Panel))]
public class PanelDisplayTimelineTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<PanelDisplayTimelineMixerBehaviour>.Create (graph, inputCount);
    }

    public override void GatherProperties (PlayableDirector director, IPropertyCollector driver)
    {
#if UNITY_EDITOR
        Panel trackBinding = director.GetGenericBinding(this) as Panel;

        if (trackBinding == null)
        {
            return;
        }

        var serializedObject = new UnityEditor.SerializedObject (trackBinding);
        var iterator = serializedObject.GetIterator();
        while (iterator.NextVisible(true))
        {
            if (iterator.hasVisibleChildren)
            {
                continue;
            }

            driver.AddFromName<Panel>(trackBinding.gameObject, iterator.propertyPath);
        }
#endif
        base.GatherProperties(director, driver);
    }
}
