using Herman;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Cinemachine.DocumentationSortingAttribute;

public class Building : MonoBehaviour
{

    public float State = 1.0f;
    public float Luminance = 0.0f;
    public float BaseLuminance = 0.1f;
    public UnityEvent OnBuildingStateChanged = new UnityEvent();
    public UnityEvent OnBuildingRepaired = new UnityEvent();
    public UnityEvent OnBuildingRepair = new UnityEvent();
    public UnityEvent OnBuildingBroken = new UnityEvent();
    public UnityEvent OnLuminanceChanged = new UnityEvent();
    public UnityEvent OnLuminanceHalf = new UnityEvent();
    public UnityEvent OnLuminanceFull = new UnityEvent();
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
}
