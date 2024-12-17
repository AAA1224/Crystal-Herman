using Herman;
using KoboldTools.Logging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using KoboldTools;
using static Cinemachine.DocumentationSortingAttribute;

public class Building : MonoBehaviour, IInteractive
{

    public float State = 1.0f;
    public float Luminance = 0.0f;
    public float BaseLuminance = 0.1f;
    public Marketplace Marketplace;
    public UnityEvent OnBuildingStateChanged = new UnityEvent();
    public UnityEvent OnBuildingRepaired = new UnityEvent();
    public UnityEvent OnBuildingRepair = new UnityEvent();
    public UnityEvent OnBuildingBroken = new UnityEvent();
    public UnityEvent OnLuminanceChanged = new UnityEvent();
    public UnityEvent OnLuminanceHalf = new UnityEvent();
    public UnityEvent OnLuminanceFull = new UnityEvent();
    private UnityEvent _interacted = new UnityEvent();
    public uint netId;

    private BuildingDisplayState _buildingStateDisplay;
    private BuildingDisplayLuminance _luminanceDisplay;
    private BuildingLinkWithIncident _linkWithIncident;

    public void IncrementLuminance()
    {
        this.Luminance += 0.1f;
    }

    public bool MayBreak
    {
        get
        {
            return this._buildingStateDisplay != null;
        }
    }

    public bool DisplaysLuminance
    {
        get
        {
            return this._luminanceDisplay != null;
        }
    }
    public void Awake()
    {
        this._buildingStateDisplay = GetComponent<BuildingDisplayState>();
        this._luminanceDisplay = GetComponent<BuildingDisplayLuminance>();
        this._linkWithIncident = GetComponent<BuildingLinkWithIncident>();
    }

    public bool IsLinkedWith(Incident incident)
    {
        if (this._linkWithIncident != null)
        {
            return this._linkWithIncident.IsLinkedWith(incident);
        }
        else
        {
            return false;
        }
    }

    public bool IncursInfrastructureCosts
    {
        get
        {
            return !(this.Marketplace != null && this.Marketplace.offers.Count > 0);
        }
    }

    public UnityEvent interacted
    {
        get
        {
            return _interacted;
        }
    }


    // Start is called before the first frame update
    public IEnumerator Start()
    {
        while (MainGameManager.Instance == null)
        {
            yield return null;
        }

        MainGameManager.Instance.AddBuilding(this);
        this.Luminance = this.BaseLuminance;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void onPointerDown()
    {
        //do nothing
    }

    public void onPointerUp()
    {
        Debug.Log("interacted");
        //raise interacted event
        if (Marketplace.seller != null)
        {
            if (Marketplace.offers.Count > 0 || Marketplace.seller == MainGameManager.Instance.localPlayer)
            {
                MainGameManager.Instance.localPlayer.WatchedMarket = Marketplace;
            }
        }
        else
        {
            RootLogger.Warning(this, "Cannot open the marketplace, because no seller is set");
        }
    }
}
