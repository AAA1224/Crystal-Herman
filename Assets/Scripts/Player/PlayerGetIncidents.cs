using Herman;
using KoboldTools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;
using UnityEngine.UIElements.Experimental;
using System;


public class Roulette<T> where T : IEquatable<T>
{
    /// <summary>
    /// The internal set of pockets.
    /// </summary>
    private List<Pocket> _pockets = new List<Pocket>();
    /// <summary>
    /// The current sum of all pocket sizes.
    /// </summary>
    private int _maxSize = 0;
    /// <summary>
    /// The random generator instance.
    /// </summary>
    private System.Random _rng = new System.Random();

    /// <summary>
    /// A pocket stores a generic object and the relative probability of
    /// it's occurrence in the urn.
    /// </summary>
    private struct Pocket
    {
        public T pocketObject;
        public int pocketSize;
    }
    /// <summary>
    /// Adds a new pocket to the roulette.
    /// </summary>
    /// <param name="pocketObject">Pocket object instance.</param>
    /// <param name="pocketSize">Pocket size (relative probability of occurrence).</param>
    public void addPocket(T pocketObject, int pocketSize)
    {
        Pocket newPocket = new Pocket();
        newPocket.pocketObject = pocketObject;
        newPocket.pocketSize = pocketSize;
        this._pockets.Add(newPocket);
        this.calcMaxSize();
    }
    /// <summary>
    /// Removes the specified pocket from the roulette.
    /// </summary>
    /// <param name="pocketObject">Pocket object instance.</param>
    public void removePocket(T pocketObject)
    {
        for (int i = this._pockets.Count - 1; i >= 0; i--)
        {
            if (this._pockets[i].pocketObject.Equals(pocketObject))
            {
                this._pockets.RemoveAt(i);
            }
        }
        this.calcMaxSize();
    }
    /// <summary>
    /// Refresh the internal state of the roulette.
    /// </summary>
    public void refresh()
    {
        this.calcMaxSize();
    }
    /// <summary>
    /// Randomly select an element from the pool.
    /// </summary>
    /// <returns>The selected element.</returns>
    public T spinRoulette()
    {
        int targetSize = this._rng.Next(0, this._maxSize + 1);
        int currentSize = 0;
        foreach (Pocket pck in this._pockets)
        {
            currentSize += pck.pocketSize;
            if (currentSize >= targetSize)
            {
                return pck.pocketObject;
            }
        }
        return default(T);
    }
    /// <summary>
    /// Calculates the sum of pocket sizes over all pockets.
    /// </summary>
    private void calcMaxSize()
    {
        this._maxSize = this._pockets.Sum(p => p.pocketSize);
    }

}

public class PlayerGetIncidents : MonoBehaviour
{
    [Header("Fortune Wheel")]
    [Tooltip("The number of times the wheel is spun in one session.")]
    public int maxWheelDraws = 2;
    [Tooltip("The lower bound on the random target angle.")]
    public float minTargetAngle = 720.0f;
    [Tooltip("The upper bound on the random target angle.")]
    public float maxTargetAngle = 2880.0f;
    [Tooltip("The rotation speed of the wheel hand in Hz.")]
    public float rotationSpeed = 1.0f;
    [Tooltip("Points to the transform of the wheel hand.")]
    public Transform wheelHand;
    [Tooltip("Each wheel segment corresponds to a specific type of incident.")]
    public string[] wheelSegmentTypes;
    [Range(0f, 1f)]
    public float matchProbability = 0.8f;
    public GameObject waitingNotification;
    //public UiResource Resource;

    /// <summary>
    /// Counts the number of times, the wheel was spun.
    /// </summary>
    private int numWheelDraws = 0;
    /// <summary>
    /// Stores the initial rotation of the wheel hand.
    /// </summary>
    private Quaternion initialRotation;
    /// <summary>
    /// Holds a set of lucky incidents for random selection.
    /// </summary>
    public Roulette<Incident> luckRoulette = new Roulette<Incident>();
    /// <summary>
    /// Holds a set of disasters for random selection.
    /// </summary>
    public Roulette<Incident> disasterRoulette = new Roulette<Incident>();
    /// <summary>
    /// Holds a set of talent-acquisition incidents for random selection.
    /// </summary>
    public Roulette<Incident> talentRoulette = new Roulette<Incident>();
    /// <summary>
    /// Holds the set of incidents reserved for the city.
    /// </summary>
    public List<Incident> cityIncidents = new List<Incident>();
    /// <summary>
    /// Holds a reference to the selected wheel segment.
    /// </summary>
    private string selectedSegmentType = null;

    //The Chance a city disaster will occur modified by the options menu
    private int disaterChance;

    /// <summary>
    /// Waits for the <see cref="Level"/> instance to appear, then calls
    /// the parent class Start method.
    /// </summary>
    void Start()
    {
        // Save the initial local rotation of the wheel hand.
        this.initialRotation = this.wheelHand.localRotation;
    }

    public void startWheelSpinning()
    {
        //get random angle
        float angle = this.getRandomTargetAngle();

        //get a new talent in 1/12 of all draws
        float discriminant = UnityEngine.Random.Range(0.0f, 1.0f);
        if (discriminant < (1.0f / 12.0f))
        {
            angle = this.maxTargetAngle;
        }

        StartCoroutine(this.spinWheel(angle));
    }

    private float getRandomTargetAngle()
    {
        return UnityEngine.Random.Range(this.minTargetAngle, this.maxTargetAngle);
    }

    private string getSelectedSegmentType(float angle)
    {
        int idx = Mathf.FloorToInt(this.wheelSegmentTypes.Length * (angle / 360.0f));
        string type = this.wheelSegmentTypes[idx];

        return type;
    }

    private IEnumerator spinWheel(float targetAngle)
    {
        // Increment the wheel turn counter.
        this.numWheelDraws += 1;

        // Reset the wheel hand back to its original rotation state.
        this.wheelHand.localRotation = this.initialRotation;

        float angle = 0.0f;
        for (float progress = 0.0f; progress <= 1.0f; progress += Time.deltaTime * this.rotationSpeed)
        {
            // Interpolate the angle.
            angle = Mathf.Lerp(0.0f, targetAngle, KoboldTools.Easing.ease(EasingType.QuarticOut, progress)) % 360.0f;

            // Set the wheel hand rotation
            this.wheelHand.localRotation = Quaternion.Euler(0.0f, 0.0f, angle - 15.0f) * this.initialRotation;
            yield return null;
        }

        this.selectedSegmentType = this.getSelectedSegmentType(angle);
        addEventFromSelectedSegment();

    }

    private void waitingForTurnCompletion()
    {
        //this.waitingNotification.SetActive(true);
    }

    private void turnCompleted()
    {
        //this.waitingNotification.SetActive(false);
    }

    private void commitIncident(Incident incident)
    {
        // Add the selected incident to the player.
        //this.model.ClientAddIncident(incident);

        Alert.show(false, incident.LocalisedTitle, incident.LocalisedDescription, null, Localisation.instance.getLocalisedText("btnOk"));
    }

    private void displayToughLuck(Incident incident)
    {
        Alert.show(false, incident.LocalisedTitle, Localisation.instance.getLocalisedText("toughLuck"), null, Localisation.instance.getLocalisedText("btnOk"));
    }

    private void addEventFromSelectedSegment()
    {
        //select incident and add to player
        if (this.selectedSegmentType == "Luck")
        {
            Incident incident = this.luckRoulette.spinRoulette().Clone();
            this.commitIncident(incident);
        }
        else if (this.selectedSegmentType == "Disaster")
        {
            Incident selectedIncident = null;

            //get a sure match when probability matches
            if (UnityEngine.Random.Range(0f, 1f) < matchProbability)
            {
                //we need a sure match
                int loop = 0; //loop counter to exit loop when unsuccessful after 100 draws
                while (selectedIncident == null || loop < 100) // || !Level.instance.allPlayers.Any(p => p.Talents.Any(t => selectedIncident.Tags.SequenceEqual(t.Tags))))
                {
                    selectedIncident = this.disasterRoulette.spinRoulette();
                    loop++;
                }
            }
            else
            {
                //just get incident randomly
                selectedIncident = this.disasterRoulette.spinRoulette();
            }
            this.commitIncident(selectedIncident.Clone());
        }
        else if (this.selectedSegmentType == "Talent")
        {
            Incident incident = this.talentRoulette.spinRoulette().Clone();
            //if (incident.ApplicationBenefit.WouldAddTalent(this.model))
            //{
                this.commitIncident(incident);
            //}
            //else
            //{
            //    this.commitIncident(this.luckRoulette.spinRoulette().Clone());
            //}
        }
    }

    private void addCityEvent()
    {

        Incident currentIncident = this.cityIncidents.FirstOrDefault(e => e.Month == 1);

        ////Check to see if the frequency option is modified
        //if (Options_Controller.frequencyFactor > 0)
        //{
        //    //If changed, select a random incident
        //    currentIncident = cityIncidents[UnityEngine.Random.Range(0, cityIncidents.Count)];
        //}

        //Modify the cost of the incident based on severity option
        if (currentIncident != null)
        {
            int currentCost = 0;
            currentIncident.ApplicationCost.TryGetExpenses(Currency.FIAT, out currentCost);
            currentIncident.ApplicationCost.SetExpenses(Currency.FIAT, (int)(currentCost));
            currentIncident.Description = Localisation.instance.getLocalisedText(currentIncident.Description);
            currentIncident.Description += "\nThe city has to pay " + (currentCost) + " in repair costs";
        }

        if (currentIncident != null)
        {
            this.commitIncident(currentIncident);
        }
        else
        {
            //StartCoroutine(this.delayedEndTurn());
        }
    }
}
