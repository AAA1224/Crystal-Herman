using Herman;
using KoboldTools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UI;

public interface ICharacter
{
    float Luminance { get; set; }
    Vector3 steeringTarget { get; set; }
    UnityEvent stateChanged { get; }
    UnityEvent OnLuminanceChanged { get; }
    UnityEvent OnLuminanceHalf { get; }
    UnityEvent OnLuminanceFull { get; }
}

public class Character : MonoBehaviour, ICharacter
{
    public NavMeshAgent navMeshAgent;
    private Animator animator;
    private Vector3 currentDestination = Vector3.zero;

    private float _luminance = 0.0f;
    /// <summary>
    /// The target point for the navigation. Character moves towards this point when set.
    /// </summary>
    private Vector3 _steeringTarget = Vector3.zero;
    /// <summary>
    /// State change event of the character. Is called when state of the character changes.
    /// </summary>
    private UnityEvent _stateChanged = new UnityEvent();
    private UnityEvent _onLuminanceChanged = new UnityEvent();
    private UnityEvent _onLuminanceHalf = new UnityEvent();
    private UnityEvent _onLuminanceFull = new UnityEvent();
    private PolyPlayer _model;

    public PolyPlayer model
    {
        get { return _model; }
        set
        {
            this._model = value;
            this.onModelChanged();
        }
    }

    public void onModelChanged()
    {
        this._model.PlayerStateChanged.AddListener(onPlayerStateChanged);
        onPlayerStateChanged();
    }

    public void onPlayerStateChanged()
    {
        if (this._model.Person != null)
        {
            this.gameObject.transform.Find("Canvas").Find("Name Tag").GetComponent<Text>().text = Localisation.instance.getLocalisedText(this._model.Person.Title);
        }
    }

    public float Luminance
    {
        get
        {
            return this._luminance;
        }

        set
        {
            if (Math.Abs(this._luminance - value) > float.Epsilon)
            {
                if (value > 1.0f)
                {
                    this._luminance = 1.0f;
                }
                else if (value < 0.0f)
                {
                    this._luminance = 0.0f;
                }
                else
                {
                    this._luminance = value;
                }

                // Fire the appropriate events.
                if (Math.Abs(this._luminance - 0.5f) <= float.Epsilon)
                {
                    this._onLuminanceHalf.Invoke();
                }
                else if (Math.Abs(this._luminance - 1.0f) <= float.Epsilon)
                {
                    this._onLuminanceFull.Invoke();
                }
                this._onLuminanceChanged.Invoke();
            }
        }
    }

    public Vector3 steeringTarget
    {
        get
        {
            return this._steeringTarget;
        }

        set
        {
            if (this._steeringTarget != value)
            {
                //set new target and invoke state changed event
                this._steeringTarget = value;
                this.stateChanged.Invoke();
            }
        }
    }

    public UnityEvent stateChanged
    {
        get
        {
            return this._stateChanged;
        }
    }

    public UnityEvent OnLuminanceChanged
    {
        get
        {
            return this._onLuminanceChanged;
        }
    }

    public UnityEvent OnLuminanceHalf
    {
        get
        {
            return this._onLuminanceHalf;
        }
    }

    public UnityEvent OnLuminanceFull
    {
        get
        {
            return this._onLuminanceFull;
        }
    }


    public void Start()
    {
        animator = GetComponentInChildren<Animator>();
        this.stateChanged.AddListener(onStateChanged);
    }

    public void onStateChanged()
    {
        if (!navMeshAgent.isOnNavMesh)
        {
            return;
        }
        StopCoroutine(navigate());
        //set the navmesh target
        navMeshAgent.destination = steeringTarget;
        currentDestination = steeringTarget;
        StartCoroutine(navigate());
    }

    private IEnumerator navigate()
    {
        if (animator != null)
            animator.SetBool("walking", true);
        yield return null; 
        while (!navMeshAgent.isOnNavMesh)
        {
            yield return null;
        }
        while (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
        {
            yield return null;
        }

        if (animator != null)
            animator.SetBool("walking", false);
    }
}
