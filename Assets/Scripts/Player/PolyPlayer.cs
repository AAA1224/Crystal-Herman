using Herman;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PolyPlayer
{
    private List<Talent> _talents = new List<Talent>();
    private List<Incident> _incidents = new List<Incident>();
    private Pocket _pocket = new Pocket();
    private UnityEvent _playerStateChanged = new UnityEvent();
    private UnityEvent _characterChanged = new UnityEvent();

    private Character _loadedCharacter = null;
    private bool _runsForMayor = false;
    private bool _mayor = false;
    private Job _job = null;
    public Person _person = null;
    private Home _home = null;
    public float _foodHealthStatus = 0.0f;
    public int _goodFoodNumber = 0;
    public int _badFoodNumber = 0;

    public Home Home
    {
        get
        {
            return this._home;
        }
        set
        {
            this._home = value;
            this._playerStateChanged.Invoke();
        }
    }
    public List<Talent> Talents
    {
        get
        {
            return this._talents;
        }
        set
        {
            this._talents = value;
            this._playerStateChanged.Invoke();
        }
    }

    public Pocket Pocket
    {
        get
        {
            return this._pocket;
        }
        set
        {
            this._pocket = value;
            this._playerStateChanged.Invoke();
        }
    }

    public Person Person
    {
        get
        {
            return this._person;
        }
        set
        {
            this._person = value;
            this._playerStateChanged.Invoke();
        }
    }

    public Job Job
    {
        get
        {
            return this._job;
        }
        set
        {
            this._job = value;
            this._playerStateChanged.Invoke();
        }
    }

    public UnityEvent PlayerStateChanged
    {
        get
        {
            return this._playerStateChanged;
        }
    }
    public UnityEvent OnTurnCompleted
    {
        get
        {
            return this._onTurnCompleted;
        }
    }

    public UnityEvent OnWaitingForTurnCompletion
    {
        get
        {
            return this._onWaitingForTurnCompletion;
        }
    }


    private UnityEvent _onTurnCompleted = new UnityEvent();

    private UnityEvent _onWaitingForTurnCompletion = new UnityEvent();

    //The number of WC offers made
    public int numWCOffers;
    //The total value of the WC offers
    public int valueWCOffers;
    //The number of WC offers bought
    public int numWCBought;
    //The total value of the WC offers bought
    public int valueWCBought;
    //The number of WC offers sold
    public int numWCSold;
    //The total value of the WC offers sold
    public int valueWCSold;
    //The number of Gold offers made
    public int numGoldOffers;
    //The total value of Gold offers
    public int valueGoldOffers;
    //The number of Gold offers bought
    public int numGoldBought;
    //The total value of Gold offers bought
    public int valueGoldBought;
    //The number of Gold offers sold
    public int numGoldSold;
    //The total value of Gold offers sold
    public int valueGoldSold;

    public Character LoadedCharacter
    {
        get
        {
            return this._loadedCharacter;
        }

        set
        {
            if (this._loadedCharacter != value)
            {
                this._loadedCharacter = value;
                this._characterChanged.Invoke();
            }
        }
    }
    public bool Mayor
    {
        get
        {
            return this._mayor;
        }
        set
        {
            this._mayor = value;
        }
    }

    public List<Incident> Incidents
    {
        get
        {
            return this._incidents;
        }
        set
        {
            this._incidents = value;
            this._playerStateChanged.Invoke();
        }
    }
}
