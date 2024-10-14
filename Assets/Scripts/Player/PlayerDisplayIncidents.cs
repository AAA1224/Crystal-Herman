using ExitGames.Client.Photon;
using Herman;
using KoboldTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PoolPopEvent : MonoBehaviour
{
    public UnityEvent popEvent = new UnityEvent();
    public void onPop()
    {
        popEvent.Invoke();
    }
}

public class Pool<T> where T : Component
{
    public Pool(T template, bool checkPoolComponents = false)
    {
        _template = template;
        _template.gameObject.SetActive(false);
        _checkPoolComponents = checkPoolComponents;
    }

    private bool _checkPoolComponents = false;
    private T _template;
    private Stack<T> _used = new Stack<T>();
    private Stack<T> _unused = new Stack<T>();

    public T pop()
    {
        if (_unused.Count > 0)
        {
            _used.Push(_unused.Pop());
        }
        else
        {
            T newObject = Object.Instantiate(_template) as T;
            newObject.transform.SetParent(_template.transform.parent, false);
            _used.Push(newObject);
        }

        if (_checkPoolComponents)
        {
            PoolPopEvent ppe = _used.Peek().GetComponent<PoolPopEvent>();
            if (ppe != null)
            {
                ppe.onPop();
            }
        }

        return _used.Peek();
    }

    public void releaseAll()
    {
        while (_used.Count > 0)
        {
            T element = _used.Pop();
            if (element != null && element.gameObject != null)
            {
                element.gameObject.SetActive(false);
                _unused.Push(element);
            }
        }
    }

    public Stack<T> getUsed()
    {
        return _used;
    }

    public T[] getUsedAsArray()
    {
        return _used.ToArray();
    }

}

public class PlayerDisplayIncidents : MonoBehaviour
{
    [Tooltip("If set to true, also shows ignored incidents.")]
    public bool showIgnored = false;
    [Tooltip("If set to true, also shows resolved incidents.")]
    public bool showResolved = false;
    [Tooltip("If set to true, also shows applied incidents.")]
    public bool showApplied = false;
    [Tooltip("If set to true, also shows non-influenceable incidents.")]
    public bool showNonInfluenceable = false;
    [Tooltip("Selects those incident types thate are not to be shown.")]
    public List<string> excludeIncidentTypes = new List<string>();
    [Tooltip("The template game object to use to display each incident.")]
    public Transform incidentUiTemplate;
    private Pool<Transform> incidentUiPool;
    private PolyPlayer model;

    public void Awake()
    {
        this.incidentUiPool = new Pool<Transform>(this.incidentUiTemplate);
    }

    public IEnumerator Start()
    {
        while (MainGameManager.Instance == null)
        {
            yield return null;
        }

        while (MainGameManager.Instance.localPlayer == null)
        {
            yield return null;
        }

        this.model = MainGameManager.Instance.localPlayer;
        this.onModelChanged();
    }

    public void onModelChanged()
    {
        this.model.PlayerStateChanged.AddListener(this.onPlayerStateChanged);
    }
    public void onModelRemoved()
    {
        this.model.PlayerStateChanged.RemoveListener(this.onPlayerStateChanged);
    }
    private void onPlayerStateChanged()
    {
        // Ignore those incidents that will be removed by other incidents.
        List<Incident> currentIncidents = new List<Incident>(this.model.Incidents);

        this.incidentUiPool.releaseAll();
        foreach (Incident incident in currentIncidents)
        {
            if ((incident.State == IncidentState.UNTOUCHED) || (incident.State == IncidentState.APPLIED && this.showApplied) || (incident.State == IncidentState.RESOLVED && this.showResolved) || (incident.State == IncidentState.IGNORED && this.showIgnored))
            {
                if (incident.Influenceable || (this.showNonInfluenceable && !incident.Influenceable))
                {
                    if (!this.excludeIncidentTypes.Contains(incident.Type))
                    {
                        Transform incidentUi = this.incidentUiPool.pop();
                        incidentUi.gameObject.SetActive(true);
                        incidentUi.gameObject.GetComponent<IncidentUI>().model = incident;
                        VC<Incident>.addModelToAllControllers(incident, incidentUi.gameObject);
                    }
                }
            }
        }
    }
}
