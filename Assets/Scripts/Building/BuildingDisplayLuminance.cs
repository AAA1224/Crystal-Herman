using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KoboldTools;

[RequireComponent(typeof(Building))]
public class BuildingDisplayLuminance : MonoBehaviour
{
    public Light lightSource;
    public SpriteRenderer scaleSource;
    public Color fromColor = Color.white;
    public Color toColor = Color.white;
    public float luminanceMultiplier = 50;
    public MeshRenderer[] associatedRenderers;
    public float minEmission = 1.5f;
    public float maxEmission = 3.5f;
    private Building building;
    private Color[] baseEmissionColors;

    private float lastLuminance = 0f;

    public void Awake()
    {
        if (this.lightSource == null)
        {
            this.lightSource = GetComponentInChildren<Light>();
        }

        this.building = GetComponent<Building>();
        //this.building.OnLuminanceChanged.AddListener(this.onLuminanceChanged);
        this.building.OnBuildingRepair.AddListener(this.onLuminanceChanged);
        this.building.OnBuildingBroken.AddListener(this.onLuminanceChanged);

        this.lastLuminance = building.Luminance;
        this.onLuminanceChanged();
        baseEmissionColors = new Color[associatedRenderers.Length];
        for (int i = 0; i < associatedRenderers.Length; i++)
        {
            baseEmissionColors[i] = associatedRenderers[i].material.GetColor("_EmissionColor");
        }
    }

    private void onLuminanceChanged()
    {
        if (this.lightSource != null)
            this.lightSource.intensity = this.luminanceMultiplier * this.building.Luminance;

        if (this.building.Luminance != 0f)
        {
            //animate if luminance is not zero
            StartCoroutine(increaseLuminance());
        }
        else
        {
            //set to zero instantly
            setLuminanceInstant();
        }
    }

    private IEnumerator increaseLuminance()
    {
        float prog = 0f;
        float duration = 1f;
        float oldEmission = Mathf.Lerp(minEmission, maxEmission, lastLuminance);
        float newEmission = Mathf.Lerp(minEmission, maxEmission, this.building.Luminance);
        Color oldColor = Color.Lerp(fromColor, toColor, lastLuminance);
        Color newColor = Color.Lerp(fromColor, toColor, this.building.Luminance);

        while (prog < 1f)
        {
            /*if (this.scaleSource != null)
                this.scaleSource.color = Color.LerpUnclamped(oldColor,newColor,Easing.ease(EasingType.BackOut,prog));*/



            if (prog < 0.5f)
            {
                if (this.scaleSource != null)
                    this.scaleSource.color = Color.Lerp(oldColor, toColor, Easing.ease(EasingType.QuadraticIn, prog * 2f));

                for (int i = 0; i < associatedRenderers.Length; i++)
                {
                    associatedRenderers[i].material.SetColor("_EmissionColor", baseEmissionColors[i] * Mathf.LinearToGammaSpace(Mathf.Lerp(oldEmission, maxEmission, Easing.ease(EasingType.QuadraticIn, prog * 2f))));
                }
            }
            else
            {
                if (this.scaleSource != null)
                    this.scaleSource.color = Color.Lerp(toColor, newColor, Easing.ease(EasingType.QuadraticIn, prog * 2f - 1f));

                for (int i = 0; i < associatedRenderers.Length; i++)
                {
                    associatedRenderers[i].material.SetColor("_EmissionColor", baseEmissionColors[i] * Mathf.LinearToGammaSpace(Mathf.Lerp(oldEmission, maxEmission, Easing.ease(EasingType.QuadraticIn, prog * 2f - 1f))));
                }
            }



            /*for (int i = 0; i < associatedRenderers.Length; i++)
            {
                associatedRenderers[i].material.SetColor("_EmissionColor", baseEmissionColors[i] * Mathf.LinearToGammaSpace(Mathf.LerpUnclamped(oldEmission, newEmission, Easing.ease(EasingType.BackOut, prog))));
            }*/

            prog += Time.deltaTime / duration;
            yield return null;
        }

        setLuminanceInstant();
        lastLuminance = building.Luminance;
    }

    private void setLuminanceInstant()
    {
        if (this.scaleSource != null)
            this.scaleSource.color = Color.Lerp(fromColor, toColor, building.Luminance);

        for (int i = 0; i < associatedRenderers.Length; i++)
        {
            associatedRenderers[i].material.SetColor("_EmissionColor", baseEmissionColors[i] * Mathf.LinearToGammaSpace(Mathf.Lerp(minEmission, maxEmission, building.Luminance)));
        }
    }

}

