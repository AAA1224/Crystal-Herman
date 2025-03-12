using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Herman;
using Photon.Realtime;
using static Cinemachine.DocumentationSortingAttribute;
using KoboldTools.Logging;

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
    private PolyPlayer player;
    private Incident linkedIncident = null;

    public void Awake()
    {
        this.building = GetComponent<Building>();
    }

    public IEnumerator Start()
    {
        while (MainGameManager.Instance == null)
        {
            yield return null;
        }

        MainGameManager.Instance.onAuthoritativePlayerChanged.AddListener(this.onAuthoritativePlayerChanged);
        this.onAuthoritativePlayerChanged();

        //add a listener to the offer creation of the associated marketplace
        //addMarketplaceResolveListener();
    }

    public bool IsLinkedWith(Incident incident)
    {
        uint buildingNetId = this.building.netId;
        return this.filterTags.Any(f => incident.EquivalentTags(f.tags)) && incident.State == IncidentState.UNTOUCHED && incident.IgnoranceCost.BreakBuilding != buildingNetId && incident.ApplicationBenefit.RepairBuilding != buildingNetId;
    }
    private void onAuthoritativePlayerChanged()
    {
        if (this.player != null)
        {
            this.player.PlayerStateChanged.RemoveListener(this.onPlayerStateChanged);
        }
        if (MainGameManager.Instance.localPlayer != null)
        {
            this.player = MainGameManager.Instance.localPlayer;
            this.player.PlayerStateChanged.AddListener(this.onPlayerStateChanged);
            this.onPlayerStateChanged();
        }
    }

    private void onPlayerStateChanged()
    {
        uint buildingNetId = this.building.netId;
        Incident newLinkedIncident = this.player.Incidents.Find(e => this.IsLinkedWith(e));
        if (newLinkedIncident != null)
        {
            this.linkedIncident = newLinkedIncident;
            this.linkedIncident.IgnoranceCost.BreakBuilding = buildingNetId;
            this.linkedIncident.ApplicationBenefit.RepairBuilding = buildingNetId;
            RootLogger.Debug(this, "Linking the incident {0} to the building", linkedIncident);
            MainGameManager.Instance.ClientUpdateIncident(player, linkedIncident);
        }
    }

}