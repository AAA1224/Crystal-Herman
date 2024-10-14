using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Building))]
public class BuildingDisplayState : MonoBehaviour
{
    public Color repairedColor;
    public Color brokenColor;
    public MeshRenderer[] associatedRenderers;
    private Building building;

    public void Awake()
    {
        this.building = GetComponent<Building>();
        this.building.OnBuildingBroken.AddListener(this.onBuildingBroken);
        this.building.OnBuildingRepaired.AddListener(this.onBuildingRepaired);
    }

    private void onBuildingBroken()
    {
        foreach (MeshRenderer mr in associatedRenderers)
        {
            mr.material.color = this.brokenColor;
        }
    }

    private void onBuildingRepaired()
    {
        foreach (MeshRenderer mr in associatedRenderers)
        {
            mr.material.color = this.repairedColor;
        }
    }
}