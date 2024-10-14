using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Herman;
using Photon.Realtime;
using static Cinemachine.DocumentationSortingAttribute;

[RequireComponent(typeof(Building))]
public class BuildingLinkWithIncident : MonoBehaviour
{
    [Serializable]
    public class Tags
    {
        public List<string> tags;
    }

    public List<Tags> filterTags;
    private Building building;
    private Level level;
    private Player player;
    private Incident linkedIncident = null;

    public void Awake()
    {
        this.building = GetComponent<Building>();
    }

    public bool IsLinkedWith(Incident incident)
    {
        uint buildingNetId = this.building.netId;
        return this.filterTags.Any(f => incident.EquivalentTags(f.tags)) && incident.State == IncidentState.UNTOUCHED && incident.IgnoranceCost.BreakBuilding != buildingNetId && incident.ApplicationBenefit.RepairBuilding != buildingNetId;
    }
}