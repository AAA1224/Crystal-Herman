using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using System.Reflection;
using UnityEngine;
[CreateAssetMenu(fileName = "LightingSettings",menuName = "New LightingSettings")]
public class LightingSettings : ScriptableObject {

    [SerializeField]
    private SerializedObject s_sourceLightmapSettings;
    [SerializeField]
    private SerializedObject s_sourceRenderSettings;

    [ContextMenu("Fetch Settings from Current")]
    public void CopySettings()
    {
        UnityEngine.Object lightmapSettings;
        if (!TryGetSettings<LightmapSettings>("GetLightmapSettings", out lightmapSettings))
            return;

        UnityEngine.Object renderSettings;
        if (!TryGetSettings<RenderSettings>("GetRenderSettings", out renderSettings))
            return;

        s_sourceLightmapSettings = new SerializedObject(lightmapSettings);
        s_sourceRenderSettings = new SerializedObject(renderSettings);
    }

    [ContextMenu("Apply Settings to Current")]
    public void PasteSettings()
    {
        if (s_sourceLightmapSettings != null && s_sourceRenderSettings != null)
        {
            UnityEngine.Object lightmapSettings;
            if (!TryGetSettings<LightmapSettings>("GetLightmapSettings", out lightmapSettings))
                return;

            UnityEngine.Object renderSettings;
            if (!TryGetSettings<RenderSettings>("GetRenderSettings", out renderSettings))
                return;

            CopyInternal(s_sourceLightmapSettings, new SerializedObject(lightmapSettings));
            CopyInternal(s_sourceRenderSettings, new SerializedObject(renderSettings));

            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }
    }

    private void CopyInternal(SerializedObject source, SerializedObject dest)
    {
        var prop = source.GetIterator();
        while (prop.Next(true))
        {
            var copyProperty = true;
            foreach (var propertyName in new[] { "m_Sun", "m_FileID", "m_PathID", "m_ObjectHideFlags" })
            {
                if (string.Equals(prop.name, propertyName, System.StringComparison.Ordinal))
                {
                    copyProperty = false;
                    break;
                }
            }

            if (copyProperty)
                dest.CopyFromSerializedProperty(prop);
        }

        dest.ApplyModifiedProperties();
    }

    private bool TryGetSettings<T>(string methodName, out UnityEngine.Object settings)
    {
        settings = null;

        var method = typeof(T).GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
        if (method == null)
        {
            Debug.LogErrorFormat("CopyLightingSettings: Could not find {0}.{1}", typeof(T).Name, methodName);
            return false;
        }

        var value = method.Invoke(null, null) as UnityEngine.Object;
        if (value == null)
        {
            Debug.LogErrorFormat("CopyLightingSettings: Could get data from {0}.{1}", typeof(T).Name, methodName);
            return false;
        }

        settings = value;
        return true;
    }

}
