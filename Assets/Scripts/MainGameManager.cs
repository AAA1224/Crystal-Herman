using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using System;
using KoboldTools;
using System.Linq;
using UnityEngine.Playables;
using static Cinemachine.DocumentationSortingAttribute;
using Cinemachine;
using UnityEngine.Events;
using static UnityEngine.Rendering.VolumeComponent;
using KoboldTools.Logging;


namespace Herman
{
    [Serializable]
    public class LevelData
    {
        /// <summary>
        /// Contains the set of valid personal background info.
        /// </summary>
        public List<Person> Persons = new List<Person>();
        /// <summary>
        /// Contains the set of personal background info for the mayor.
        /// </summary>
        public List<Person> MayorPersons = new List<Person>();
        /// <summary>
        /// Contains the set of valid player homes.
        /// </summary>
        public List<Home> Homes = new List<Home>();
        /// <summary>
        /// Contains the set of homes for the mayor.
        /// </summary>
        public List<Home> MayorHomes = new List<Home>();
        /// <summary>
        /// Contains the set of jobs that signify unemployment.
        /// Can be used to provide multiple unemployment
        /// backgrounds.
        /// </summary>
        public List<Job> Unemployed = new List<Job>();
        /// <summary>
        /// Contains the set of valid player jobs.
        /// </summary>
        public List<Job> Jobs = new List<Job>();
        /// <summary>
        /// Contains the set of jobs for the mayor.
        /// </summary>
        public List<Job> MayorJobs = new List<Job>();
        /// <summary>
        /// Contains the set of valid player talents.
        /// </summary>
        public List<Talent> Talents = new List<Talent>();
        /// <summary>
        /// Contains the set of mayor talents.
        /// </summary>
        public List<Talent> MayorTalents = new List<Talent>();
        /// <summary>
        /// Contains the set of valid events.
        /// </summary>
        public List<Incident> Incidents = new List<Incident>();
    }


    public static class IListExtensions
    {
        /// <summary>
        /// Shuffles the element order of the specified list.
        /// </summary>
        /// <param name="list">List.</param>
        /// <typeparam name="T">Any type.</typeparam>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            System.Random rnd = new System.Random();
            while (n > 1)
            {
                int k = (rnd.Next(0, n) % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        /// <summary>
        /// Randomly selects the specified number of elements from the given
        /// list without placing back elements.
        /// </summary>
        /// <returns>The randomly chosen list elements.</returns>
        /// <param name="list">List.</param>
        /// <param name="number">Number.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static List<T> SelectRandom<T>(this IList<T> list, int number)
        {
            var output = new List<T>();

            if (number <= list.Count)
            {
                var indices = Enumerable.Range(0, list.Count).ToList();
                var rng = new System.Random();
                for (var i = 0; i < number; i++)
                {
                    var j = rng.Next(indices.Count);
                    output.Add(list[indices[j]]);
                    indices.RemoveAt(j);
                }
            }

            return output;
        }
        /// <summary>
        /// Creates a deep string representation of a generic list.
        /// </summary>
        /// <returns>The string representation of the list.</returns>
        /// <param name="list">List.</param>
        public static string ToVerboseString<T>(this IList<T> list)
        {
            return String.Format("[{0}]", String.Join(", ", list.Select(e => e.ToString()).ToArray()));
        }
    }

    public class MainGameManager : MonoBehaviourPunCallbacks
    {
        public static MainGameManager Instance = null;

        public int RegularStartingMoney = 3000;
        public int MayorBaseStartingMoney = 1500;
        public float MayorStartingMoneyFactor = 1.0f;
        public float InfrastructureCostFactor = 0.333333f;
        public float LuminancePerPoint = 0.01f;
        public int PolymoneyPerFreeTime = 100;
        public List<CurrencyValue> MaximumDebt = new List<CurrencyValue> {
            new CurrencyValue(Currency.FIAT, 100),
            new CurrencyValue(Currency.Q, 0),
        };

        private float _months = 1f;
        private int _maximumMonths = 12;
        public Button endTurnBtn;
        public Text overViewEndBtnText;
        private int defaultCamAliveCnt = 0;
        
        public Transform[] SpawnPoints;

        public GameObject characterPrefab;
        public GameObject cityCharacterPrefab;
        public TMP_Text personNameText;
        [SerializeField]
        private List<string> _taxTags = new List<string> { "Taxes" };
        [SerializeField]
        private List<string> _welfareTags = new List<string> { "Welfare" };
        [SerializeField]
        private List<string> _salaryTags = new List<string> { "Salary" };
        [SerializeField]
        private List<string> _rentTags = new List<string> { "Rent" };
        [SerializeField]
        private List<string> _infrastructureTags = new List<string> { "Recurrent", "City", "Infrastructure" };
        [SerializeField]
        private List<string> _foodTags = new List<string> { "Food" };

        private UnityEvent _onLevelStateChanged = new UnityEvent();
        private UnityEvent _onAuthoritativePlayerChanged = new UnityEvent();
        private List<GameObject> characters;
        private List<Building> _buildings = new List<Building>();
        private List<PolyPlayer> polyPlayers = new List<PolyPlayer>();
        public PolyPlayer _localPlayer = null;

        public Panel endMonthOverview = null;
        public Panel cityOverview = null;

        public PolyPlayer localPlayer
        {
            get
            {
                return _localPlayer;
            }

            set
            {
                if (_localPlayer != value)
                {
                    _localPlayer = value;
                    onAuthoritativePlayerChanged.Invoke();
                }
            }
        }

        public UnityEvent onAuthoritativePlayerChanged
        {
            get
            {
                return _onAuthoritativePlayerChanged;
            }
        }

        public List<string> taxTags
        {
            get
            {
                return this._taxTags;
            }
        }

        public List<string> welfareTags
        {
            get
            {
                return this._welfareTags;
            }
        }

        public List<string> salaryTags
        {
            get
            {
                return this._salaryTags;
            }
        }

        public List<string> rentTags
        {
            get
            {
                return this._rentTags;
            }
        }

        public List<string> infrastructureTags
        {
            get
            {
                return this._infrastructureTags;
            }
        }

        public List<string> foodTags
        {
            get
            {
                return this._foodTags;
            }
        }

        public List<Building> Buildings
        {
            get
            {
                return this._buildings;
            }

            set
            {
                this._buildings = value;
            }
        }

        public void AddBuilding(Building building)
        {
            Debug.Log("Building '{0}' (netid: {1}) was registered" + building.name + building.netId);
            this._buildings.Add(building);
            //this.handleLevelStateChange();
            //building.OnLuminanceChanged.AddListener(this.handleLevelStateChange);
            //building.OnBuildingStateChanged.AddListener(this.handleLevelStateChange);
        }

        public float CityState
        {
            get
            {
                List<Building> bldgs = this._buildings.FindAll(e => e.MayBreak).ToList();
                return bldgs.Sum(e => Mathf.Clamp01(e.State)) / bldgs.Count;
            }
        }

        public float TotalLuminance
        {
            get
            {
                List<Building> bldgs = this._buildings.FindAll(e => e.DisplaysLuminance).ToList();
                return bldgs.Sum(e => e.Luminance) / bldgs.Count;
            }
        }
        public int maximumMonths
        {
            get
            {
                return _maximumMonths;
            }
        }

        public float months
        {
            get
            {
                return _months;
            }

            set
            {
                if (value != _months)
                {
                    _months = value;
                    onLevelStateChanged.Invoke();
                }
            }
        }


        public UnityEvent onLevelStateChanged
        {
            get
            {
                return _onLevelStateChanged;
            }
        }

        public string levelDataJson = "leveldata.json";
        public LevelData loadedLevelData = null;
        public PlayableDirector intoIntroductionTimeline;
        public PlayableDirector intoGameTimeline;
        public CameraFollowPlayer cameraFollowPlayer;
        public CinemachineVirtualCamera spinWheelCamera;
        public GameObject walkArea;
        public double areaRadius = 20;

        #region UNITY
        public void Awake()
        {
            Instance = this;
        }

        public IEnumerator Start()
        {
            // Wait for the localisation singleton to appear.
            while (Localisation.instance == null)
            {
                yield return null;
            }
            while (Localisation.instance.languages.Count == 0)
            {
                yield return null;
            }
            yield return StartCoroutine(loadText(
                    levelDataJson,
                    (json) => { loadedLevelData = JsonUtility.FromJson<LevelData>(json); Debug.Log("Loaded level data from " + this.levelDataJson); SpawnPlayer(); }
                ));

            if (Localisation.instance == null)
            {
                Debug.Log("Localisation init");
            }
            //Debug.Log("localisation");
            //// Wait for the localisation singleton to appear.
            //while (Localisation.instance == null)
            //{
            //    yield return null;
            //}

            //Debug.Log("localisation loaded");

            //Localisation.instance.eLanguageChanged.AddListener(this.onLanguageChanged);
            //this.onLanguageChanged();
        }
        #endregion

        #region COROUTTINES
        private IEnumerable Spawn()
        {
            while (true)
            {

            }
        }
        #endregion

        public void onLanguageChanged()
        {
            defaultCamAliveCnt++;
            if (defaultCamAliveCnt == 2)
            {
                StartCoroutine(introRoutine());
            }
        }

        public IEnumerator introRoutine()
        {
            while (characters.Count < PhotonNetwork.PlayerList.Length)
                yield return null;
            Alert.show(true, "tutoMStoryIslandTitle", "tutoIntroQuest", null, "btnLetPlay");
            while (Alert.open)
                yield return null;
            Alert.show(true, "tutoMStoryIslandTitle", "tutoMStoryIsland", null, "btnOk");
            while (Alert.open)
                yield return null;
            GameObject playerCharacter = null;
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (PhotonNetwork.PlayerList[i] == PhotonNetwork.LocalPlayer)
                {
                    playerCharacter = characters[i];
                    cameraFollowPlayer._player = characters[i];
                    cameraFollowPlayer.characterChanged();
                    break;
                }
            }
            intoIntroductionTimeline.Play();
            bool isPlayerMayor = false;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("IsMayor", out object isMayorGetObj))
            {
                bool isMayor = (bool)isMayorGetObj;
                if (isMayor)
                {
                    isPlayerMayor = true;
                    Alert.show(true, "tutoMWelcomeTitle", "tutoMWelcome", null, "btnOk");
                }
                else
                {
                    Alert.show(true, "tutoPWelcomeTitle", "tutoPIntro1", null, "btnOk");
                }
            }
            else
            {
                Alert.show(true, "tutoPWelcomeTitle", "tutoPIntro1", null, "btnOk");
            }
            while (Alert.open)
                yield return null;
            cameraFollowPlayer.unfocus();
            intoGameTimeline.Play();

            if (PhotonNetwork.IsMasterClient)
            {
                ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable
                    {
                        { "flowstatus", "BEGIN_MONTH" },
                        { "months", _months }
                    };
                PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
            }
            if (isPlayerMayor)
            {
                walkArea.SetActive(true);
                Alert.show(true, null, "tutoMoveMajor", null, "tutoCloseAlertButton");
                while (Alert.open)
                    yield return null;

                bool completedMovement = false;
                while (!completedMovement)
                {
                    //check for tutorial completed
                    if (playerCharacter != null && Vector3.Distance(playerCharacter.transform.position, walkArea.transform.position) < areaRadius)
                    {
                        completedMovement = true;
                    }
                    yield return null;
                }
                Alert.show(true, null, "tutoMoveEndMajor", null, "tutoCloseAlertButton");
                while (Alert.open)
                    yield return null;
            }
            else
            {

                walkArea.SetActive(true);
                Alert.show(true, null, "tutoMoveCitizen", null, "tutoCloseAlertButton");
                while (Alert.open)
                    yield return null;

                bool completedMovement = false;
                while (!completedMovement)
                {
                    //check for tutorial completed
                    if (playerCharacter != null && Vector3.Distance(playerCharacter.transform.position, walkArea.transform.position) < areaRadius)
                    {
                        completedMovement = true;
                    }
                    yield return null;
                }
                Alert.show(true, null, "tutoMoveEndCitizen", null, "tutoCloseAlertButton");
                while (Alert.open)
                    yield return null;
                spinWheelCamera.Priority = 1000;
                PlayerGetIncidents playerGetIncidents = GetComponent<PlayerGetIncidents>();
                playerGetIncidents.minTargetAngle = 1185f;
                playerGetIncidents.maxTargetAngle = 1185f;
                playerGetIncidents.startWheelSpinning();
            }
        }

        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            base.OnRoomPropertiesUpdate(propertiesThatChanged);
            if (propertiesThatChanged.ContainsKey("flowstatus"))
            {
                if (propertiesThatChanged.TryGetValue("flowstatus", out object status))
                {
                    string flowStatus = status.ToString();
                    if (flowStatus.Equals("BEGIN_MONTH"))
                    {
                        onBeginMonth();
                    }
                    else
                    {
                        onEndMonth();
                    }
                }
            }
            if (propertiesThatChanged.ContainsKey("months"))
            {
                if (propertiesThatChanged.TryGetValue("months", out object monthObj))
                {
                    float month = (float)monthObj;
                    _months = month;
                    this._onLevelStateChanged.Invoke();
                }
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            Debug.Log("OnPlayerPropertiesUpdate for player: " + targetPlayer.NickName);
            if (changedProps != null)
            {
                if (changedProps.ContainsKey("steeringTarget"))
                {
                    if (targetPlayer.CustomProperties.TryGetValue("steeringTarget", out object steeringTargetObj))
                    {
                        Vector3 point = (Vector3)steeringTargetObj;

                        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                        {
                            if (PhotonNetwork.PlayerList[i] == targetPlayer)
                            {
                                characters[i].GetComponent<Character>().steeringTarget = point;
                            }
                        }
                    }
                }
                if (changedProps.ContainsKey("Person"))
                {
                    if (targetPlayer.CustomProperties.TryGetValue("Person", out object personObj))
                    {
                        string personObjStr = (string)personObj;
                        Person person = JsonUtility.FromJson<Person>(personObjStr);

                        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                        {
                            if (PhotonNetwork.PlayerList[i] == targetPlayer)
                            {
                                polyPlayers[i].Person = person;
                                if (targetPlayer == PhotonNetwork.LocalPlayer)
                                {
                                    personNameText.text = Localisation.instance.getLocalisedText(person.Title);
                                    localPlayer.Person = person;
                                }
                            }
                        }
                    }
                }
                if (changedProps.ContainsKey("Job"))
                {
                    if (targetPlayer.CustomProperties.TryGetValue("Job", out object jobObj))
                    {
                        string jobObjectStr = (string)jobObj;
                        Job job = JsonUtility.FromJson<Job>(jobObjectStr);

                        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                        {
                            if (PhotonNetwork.PlayerList[i] == targetPlayer)
                            {
                                polyPlayers[i].Job = job;
                                if (targetPlayer == PhotonNetwork.LocalPlayer)
                                {
                                    localPlayer.Job = job;
                                }
                            }
                        }
                    }
                }
                if (changedProps.ContainsKey("Home"))
                {
                    if (targetPlayer.CustomProperties.TryGetValue("Home", out object homeObj))
                    {
                        string homeObjectStr = (string)homeObj;
                        Home home = JsonUtility.FromJson<Home>(homeObjectStr);

                        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                        {
                            if (PhotonNetwork.PlayerList[i] == targetPlayer)
                            {
                                polyPlayers[i].Home = home;
                                if (targetPlayer == PhotonNetwork.LocalPlayer)
                                {
                                    localPlayer.Home = home;
                                }
                            }
                        }
                    }
                }
                if (changedProps.ContainsKey("Pocket"))
                {
                    if (targetPlayer.CustomProperties.TryGetValue("Pocket", out object pocketObj))
                    {
                        string pocketObjStr = (string)pocketObj;
                        Pocket pocket = JsonUtility.FromJson<Pocket>(pocketObjStr);

                        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                        {
                            if (PhotonNetwork.PlayerList[i] == targetPlayer)
                            {
                                polyPlayers[i].Pocket = pocket;
                                if (targetPlayer == PhotonNetwork.LocalPlayer)
                                {
                                    localPlayer.Pocket = pocket;
                                }
                            }
                        }
                    }
                }
                if (changedProps.ContainsKey("Incidents"))
                {
                    if (targetPlayer.CustomProperties.TryGetValue("Incidents", out object incidentsObj))
                    {
                        string incidentStr = (string)incidentsObj;
                        Debug.Log(incidentStr);
                        for (int i = 0; i < polyPlayers.Count; i ++)
                        if (targetPlayer == PhotonNetwork.PlayerList[i])
                        {
                            List<Incident> incidents = new List<Incident>(JsonUtility.FromJson<Wrapper<Incident>>(incidentStr).items);
                            this.polyPlayers[i].Incidents = incidents;
                            if (targetPlayer == PhotonNetwork.LocalPlayer)
                            {
                                int j;
                                for (j = 0; j < incidents.Count; j++)
                                {
                                    if (incidents[j].State == IncidentState.UNTOUCHED)
                                        break;
                                }
                                if (incidents.Count == j)
                                {
                                    endTurnBtn.interactable = true;
                                }
                                else
                                {
                                    endTurnBtn.interactable = false;
                                }
                                this.localPlayer.Incidents = incidents;
                            }
                        }
                    }
                }
                if (changedProps.ContainsKey("Talents"))
                {
                    if (targetPlayer.CustomProperties.TryGetValue("Talents", out object talentsObj))
                    {
                        string talentsStr = (string)talentsObj;
                        Debug.Log(talentsStr);
                        for (int i = 0; i < polyPlayers.Count; i++)
                            if (targetPlayer == PhotonNetwork.PlayerList[i])
                            {
                                List<Talent> talents = new List<Talent>(JsonUtility.FromJson<Wrapper<Talent>>(talentsStr).items);
                                this.polyPlayers[i].Talents = talents;
                                if (targetPlayer == PhotonNetwork.LocalPlayer)
                                {
                                    this.localPlayer.Talents = talents;
                                }
                            }
                    }
                }
                if (changedProps.ContainsKey("GoodFood"))
                {
                    if (targetPlayer.CustomProperties.TryGetValue("GoodFood", out object goodFoodObj))
                    {
                        int goodFood = (int)goodFoodObj;
                        for (int i = 0; i < polyPlayers.Count; i++)
                            if (targetPlayer == PhotonNetwork.PlayerList[i])
                            {
                                this.polyPlayers[i]._goodFoodNumber = goodFood;
                                if (targetPlayer == PhotonNetwork.LocalPlayer)
                                {
                                    this.localPlayer._goodFoodNumber = goodFood;
                                }
                            }
                    }
                }
                
                if (changedProps.ContainsKey("BadFood"))
                {
                    if (targetPlayer.CustomProperties.TryGetValue("BadFood", out object badFoodObj))
                    {
                        int badFood = (int)badFoodObj;
                        for (int i = 0; i < polyPlayers.Count; i++)
                            if (targetPlayer == PhotonNetwork.PlayerList[i])
                            {
                                this.polyPlayers[i]._badFoodNumber = badFood;
                                if (targetPlayer == PhotonNetwork.LocalPlayer)
                                {
                                    this.localPlayer._badFoodNumber = badFood;
                                }
                            }
                    }
                }

                if (changedProps.ContainsKey("FoodHealthStatus"))
                {
                    if (targetPlayer.CustomProperties.TryGetValue("FoodHealthStatus", out object foodHealthStatusObj))
                    {
                        int foodHealthStatus = (int)foodHealthStatusObj;
                        for (int i = 0; i < polyPlayers.Count; i++)
                            if (targetPlayer == PhotonNetwork.PlayerList[i])
                            {
                                this.polyPlayers[i]._foodHealthStatus = foodHealthStatus;
                                if (targetPlayer == PhotonNetwork.LocalPlayer)
                                {
                                    this.localPlayer._foodHealthStatus = foodHealthStatus;
                                }
                            }
                    }
                }
                if (changedProps.ContainsKey("CharactersLoaded"))
                {
                    int i = 0;
                    for (i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                    {
                        Player player = PhotonNetwork.PlayerList[i];
                        if (player.CustomProperties.TryGetValue("CharactersLoaded", out object charactersLoadedObj))
                        {
                            int charactersLoaded = (int)charactersLoadedObj;
                            if (charactersLoaded == 1)
                                continue;
                        }
                        break;
                    }

                    if (i == PhotonNetwork.PlayerList.Length)
                    {

                        if (PhotonNetwork.IsMasterClient)
                        {
                            onAllPlayerCharactersLoaded();
                        }
                    }
                }
                if (changedProps.ContainsKey("EndTurn"))
                {
                    int i = 0;
                    for (i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                    {
                        Player player = PhotonNetwork.PlayerList[i];
                        if (player.CustomProperties.TryGetValue("EndTurn", out object endTurnObj))
                        {
                            int endTurn = (int)endTurnObj;
                            if (endTurn == 1)
                            {
                                polyPlayers[i].OnWaitingForTurnCompletion.Invoke();
                            }
                        }
                    }
                    for (i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                    {
                        Player player = PhotonNetwork.PlayerList[i];
                        if (player.CustomProperties.TryGetValue("EndTurn", out object endTurnObj))
                        {
                            int endTurn = (int)endTurnObj;
                            if (endTurn == 1)
                                continue;
                        }
                        break;
                    }

                    if (i == PhotonNetwork.PlayerList.Length)
                    {
                        for (i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                        {
                            polyPlayers[i].OnTurnCompleted.Invoke();
                        }
                        if (PhotonNetwork.IsMasterClient)
                        {
                            ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable
                            {
                                { "flowstatus", "END_MONTH" },
                                { "month", _months + 1 },
                            };
                            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
                        }
                    }
                    
                    for (i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                    {
                        Player player = PhotonNetwork.PlayerList[i];
                        if (player.CustomProperties.TryGetValue("EndTurn", out object endTurnObj))
                        {
                            int endTurn = (int)endTurnObj;
                            if (endTurn == 0)
                                continue;
                        }
                        break;
                    }

                    if (i == PhotonNetwork.PlayerList.Length)
                    {
                        
                        if (PhotonNetwork.IsMasterClient)
                        {
                            ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable
                            {
                                { "flowstatus", "BEGIN_MONTH" }
                            };
                            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
                        }
                    }
                }
            }
        }

        void onAllPlayerCharactersLoaded()
        {
            loadedLevelData.Persons.Shuffle();
            loadedLevelData.MayorPersons.Shuffle();
            loadedLevelData.Homes.Shuffle();
            loadedLevelData.MayorHomes.Shuffle();
            loadedLevelData.Unemployed.Shuffle();
            loadedLevelData.Jobs.Shuffle();
            loadedLevelData.MayorJobs.Shuffle();
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                bool isMayor = false;
                Player player = PhotonNetwork.PlayerList[i];

                if (player.CustomProperties.TryGetValue("IsMayor", out object isMayorObj))
                {
                    isMayor = (bool)isMayorObj;

                    if (isMayor)
                    {
                        if (PhotonNetwork.IsMasterClient)
                        {
                            Pocket pocket = new Pocket();
                            pocket.SetBalance(Currency.FIAT, Mathf.FloorToInt(MayorBaseStartingMoney * PhotonNetwork.PlayerList.Length * MayorStartingMoneyFactor));
                            Person person = loadedLevelData.MayorPersons[0];
                            List<Talent> talents = new List<Talent>();
                            talents.Add(loadedLevelData.MayorTalents[person.TalentId]);
                            ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable
                            {
                                { "Person", JsonUtility.ToJson(person) },
                                { "Home", JsonUtility.ToJson(loadedLevelData.MayorHomes.SelectRandom(1)[0]) },
                                { "Job", JsonUtility.ToJson(loadedLevelData.MayorJobs.SelectRandom(1)[0]) },
                                { "Talents", JsonUtility.ToJson(new Wrapper<Talent> { items = talents.ToArray() }) },
                                { "Pocket", JsonUtility.ToJson(pocket) }
                            };
                            player.SetCustomProperties(playerProperties);
                        }
                    }
                }
                if (!isMayor)
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        Pocket pocket = new Pocket();
                        pocket.SetBalance(Currency.FIAT, RegularStartingMoney);
                        Person person = loadedLevelData.Persons.SelectRandom(1)[0];
                        List<Talent> talents = new List<Talent>();
                        talents.Add(loadedLevelData.Talents[person.TalentId]);
                        ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable
                        {
                            { "Person", JsonUtility.ToJson(loadedLevelData.Persons.SelectRandom(1)[0]) },
                            { "Home", JsonUtility.ToJson(loadedLevelData.Homes.SelectRandom(1)[0]) },
                            { "Job", JsonUtility.ToJson(loadedLevelData.Jobs.SelectRandom(1)[0]) },
                            { "Talents", JsonUtility.ToJson(new Wrapper<Talent> { items = talents.ToArray() }) },
                            { "Pocket", JsonUtility.ToJson(pocket) }
                        };
                        player.SetCustomProperties(playerProperties);
                    }
                }
            }
        }

        void SpawnPlayer()
        {
            characters = new List<GameObject> ();
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                // check Mayor
                Player player = PhotonNetwork.PlayerList[i];
                PolyPlayer polyPlayer = new PolyPlayer();
                polyPlayer.Mayor = false;
                GameObject character = null;
                bool isMayor = false;

                if (player.CustomProperties.TryGetValue("IsMayor", out object isMayorObj))
                {
                    isMayor = (bool)isMayorObj;
                    
                    if (isMayor)
                    {
                        character = Instantiate(cityCharacterPrefab, SpawnPoints[i].position, Quaternion.identity);
                    }
                }
                if (!isMayor)
                {
                    character = Instantiate(characterPrefab, SpawnPoints[i].position, Quaternion.identity);
                }
                polyPlayer.Mayor = isMayor;
                character.transform.Rotate(0, 180 - 20 * (3 - i), 0);
                polyPlayer.LoadedCharacter = character.GetComponent<Character>();
                character.transform.Find("Canvas").Find("Symbols").gameObject.GetComponent<PlayerDisplaySymbols>().model = polyPlayer;
                polyPlayer.LoadedCharacter.model = polyPlayer;
                characters.Add(character);
                polyPlayers.Add(polyPlayer);
                if (player == PhotonNetwork.LocalPlayer)
                {
                    localPlayer = polyPlayer;
                }
            }

            ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable
            {
                { "CharactersLoaded", 1 },
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);

            PlayerGetIncidents playerGetIncidents = GetComponent<PlayerGetIncidents>();
            foreach (Incident incident in loadedLevelData.Incidents)
            {
                if (incident.Type == "Luck")
                {
                    playerGetIncidents.luckRoulette.addPocket(incident, incident.PickPoolSize);
                }
                else if (incident.Type == "Disaster")
                {
                    playerGetIncidents.disasterRoulette.addPocket(incident, incident.PickPoolSize);
                }
                else if (incident.Type == "Talent")
                {
                    playerGetIncidents.talentRoulette.addPocket(incident, incident.PickPoolSize);
                }
                else if (incident.Type == "City")
                {
                    playerGetIncidents.cityIncidents.Add(incident);
                }
            }
        }

        public void onBeginMonth()
        {
            endMonthOverview.onClose();
            cityOverview.onClose();
            Debug.Log("On Begin Month");
            List<Incident> allIncidents = loadedLevelData.Incidents;
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                Player player = PhotonNetwork.PlayerList[i];
                if (player.CustomProperties.TryGetValue("IsMayor", out object isMayorObj))
                {
                    bool isMayor = (bool)isMayorObj;

                    if (isMayor)
                    {
                        IEnumerable<Incident> cityIncidents = allIncidents.FindAll(e => e.Type == "RecurrentCity").Select(e => e.Clone());
                        List<Incident> incidents = new();
                        foreach (Incident incident in cityIncidents)
                        {
                            if (incident.ContainsTags(infrastructureTags))
                            {
                                Building linkedBuilding = Buildings.FirstOrDefault(e => e.IsLinkedWith(incident));
                                if (linkedBuilding != null)
                                {
                                    Incident taxIncident = allIncidents.Find(e => e.EquivalentTags(taxTags));
                                    if (taxIncident != null)
                                    {
                                        float debtMultiplier = Mathf.Abs(linkedBuilding.State - 2f);
                                        int taxes = 0;
                                        taxIncident.ApplicationCost.TryGetExpenses(Currency.FIAT, out taxes);

                                        //Set the maintenance cost 
                                        taxes = 50;

                                        int playerCount = PhotonNetwork.PlayerList.Length - 1;
                                        //print(debtMultiplier + " * " + playerCount + " * " + taxes + " * " + 0.2);
                                        int infrastructureCost = Mathf.FloorToInt(debtMultiplier * playerCount * taxes /* * Level.instance.InfrastructureCostFactor*/);
                                        incident.ApplicationCost.SetExpenses(Currency.FIAT, infrastructureCost);
                                        incidents.Add(incident);
                                    }
                                    else
                                    {
                                        Debug.Log("Cannot find the tax incident, which makes it impossible to calculate infrastructure costs");
                                    }
                                }
                            }
                            else
                            {
                                incidents.Add(incident);
                            }
                        }
                        AddIncidentToPlayer(player, incidents);
                        continue;
                    }
                }

                PolyPlayer polyPlayer = polyPlayers[i];
                IEnumerable<Incident> regularIncidents = allIncidents.FindAll(e => e.Type == "Recurrent").Select(e => e.Clone());
                List<Incident> incidentsForPlayer = new();
                foreach (Incident incident in regularIncidents)
                {
                    if (incident.EquivalentTags(rentTags))
                    {
                        incident.ApplicationCost.SetExpenses(Currency.FIAT, polyPlayer.Home.Rent);
                    }
                    else if (incident.EquivalentTags(salaryTags))
                    {
                        incident.ApplicationBenefit.SetIncome(Currency.FIAT, polyPlayer.Job.Salary);
                    }
                    //else if (incident.EquivalentTags(taxTags))
                    //{
                    //    Offer tmp = ScriptableObject.CreateInstance<Offer>();
                    //    JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(this.taxOffer), tmp);
                    //    tmp.guid = Guid.NewGuid();
                    //    tmp.buyingCost = new Cost(incident.ApplicationBenefit);
                    //    tmp.buyingBenefit = new Benefit(incident.ApplicationCost);

                    //    //Is it a flat tax?
                    //    if (Options_Controller.flatTax)
                    //    {
                    //        //Set the value to the base tax amount
                    //        tmp.buyingBenefit.Income[0].value = Options_Controller.baseTaxAmount;
                    //        int modValue = (int)(player.Job.Salary * (Options_Controller.baseTaxRate / 100f));
                    //        print("Added value due to flat tax: " + modValue + " at a " + Options_Controller.baseTaxRate);
                    //        tmp.buyingBenefit.Income[0].value += modValue;
                    //        print("The new tax value is : " + tmp.buyingBenefit.Income[0].value);
                    //    }
                    //    //Is it a progressive tax?
                    //    else if (Options_Controller.progTax)
                    //    {
                    //        int modValue = 0;
                    //        if (player.Job.Salary > 0)
                    //        {
                    //            tmp.buyingBenefit.Income[0].value = Options_Controller.baseTaxAmount;
                    //            modValue = (int)((Remap(player.Job.Salary, 0, 1300, Options_Controller.baseTaxRate, Options_Controller.progressiveTaxUpper) / 100) * player.Job.Salary);
                    //            print("Remapped: " + (Remap(player.Job.Salary, 0, 1300, Options_Controller.baseTaxRate, Options_Controller.progressiveTaxUpper)));
                    //            print("Added value due to progressive tax: " + modValue);
                    //            tmp.buyingBenefit.Income[0].value += modValue;
                    //            print("The new tax value is : " + tmp.buyingBenefit.Income[0].value);
                    //        }
                    //        else
                    //        {
                    //            tmp.buyingBenefit.Income[0].value = Options_Controller.baseTaxAmount;
                    //            print("The new tax value is : " + tmp.buyingBenefit.Income[0].value);
                    //        }
                    //    }

                    //    /*
                    //    //Add the tax modifications
                    //    for(int i = 0; i < tmp.buyingBenefit.Income.Count; i++)
                    //    {


                    //    }
                    //    */

                    //    incident.AddSerializedOffer = JsonUtility.ToJson(tmp);
                    //}
                    //player.ServerAddIncident(incident);
                    incidentsForPlayer.Add(incident);
                }
                AddIncidentToPlayer(player, incidentsForPlayer);
            }
        }

        public void onEndMonth()
        {
            endMonthOverview.onOpen();
            cityOverview.onOpen();
            overViewEndBtnText.text = "Ok";
        }

        public void onOverviewEndBtn()
        {
            overViewEndBtnText.text = "Waiting...";
            ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable
            {
                { "EndTurn", 0 }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
        }

        public void onEndTurn()
        {
            ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable
            {
                { "EndTurn", 1 }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
        }

        private void AddIncidentToPlayer(Player player, List<Incident> new_incidents)
        {
            Debug.Log("AddIncidentToPlayer");
            // Get Incidents of player
            if (player.CustomProperties.TryGetValue("Incidents", out object incidentsObj))
            {
                //string incidentStr = (string)incidentsObj;
                //List<Incident> incidents = new List<Incident>(JsonUtility.FromJson<Wrapper<Incident>>(incidentStr).items);
                //for (int i = 0; i < new_incidents.Count; i ++)
                //{
                //    incidents.Add(new_incidents[i]);
                //}
                ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable
                {
                    { "Incidents", JsonUtility.ToJson(new Wrapper<Incident> { items = new_incidents.ToArray() }) }
                };
                player.SetCustomProperties(playerProperties);
            }
            else
            {
                List<Incident> incidents = new List<Incident> ();
                for (int i = 0; i < new_incidents.Count; i++)
                {
                    incidents.Add(new_incidents[i]);
                }
                Debug.Log(JsonUtility.ToJson(incidents));
                ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable
                {
                    { "Incidents", JsonUtility.ToJson(new Wrapper<Incident> { items = incidents.ToArray() }) }
                };
                player.SetCustomProperties(playerProperties);
            }
        }

        private IEnumerator loadText(string path, System.Action<string> callback)
        {
            string url = System.IO.Path.Combine(Application.streamingAssetsPath, path);

            if (url.Contains("://"))
            {
                WWW www = new WWW(url);
                yield return www;
                if (string.IsNullOrEmpty(www.error))
                {
                    callback(www.text);
                }
                else
                {
                    Debug.LogError(www.error);
                }
            }
            else
            {
                callback(System.IO.File.ReadAllText(url));
            }
        }

        [Serializable]
        public class Wrapper<T>
        {
            public T[] items;
        }

        public void applyIncident(Incident incident, bool remove)
        {
            if (incident.State == IncidentState.UNTOUCHED)
            {
                List<Talent> currentTalents = new List<Talent>(localPlayer.Talents);
                List<Incident> currentIncidents = new List<Incident>(localPlayer.Incidents);
                Pocket currentPocket = new Pocket(localPlayer.Pocket);

                // Determine, which incidents the specified incident will remove.
                HashSet<int> toResolve = new HashSet<int>();
                toResolve.UnionWith(incident.ApplicationBenefit.getRemovableIncidents(currentIncidents));

                // Mark those incidents as resolved.
                foreach (int i in toResolve.OrderByDescending(q => q))
                {
                    currentIncidents[i].State = IncidentState.RESOLVED;

                    if (currentIncidents[i].EquivalentTags(foodTags))
                    {
                        localPlayer._goodFoodNumber++;
                    }
                }

                // Apply both benefit and cast of the specified incident.
                incident.ApplicationBenefit.applyBenefit(currentTalents, currentIncidents, currentPocket);
                incident.ApplicationCost.applyCost(currentPocket);

                // If the incident defines an offer to be added to the player's own marketplace, add it.
                //if (!String.IsNullOrEmpty(incident.AddSerializedOffer))
                //{
                //    this.ServerCreateOffer(this.OwnMarketplace.guid.ToString(), incident.AddSerializedOffer);
                //}

                // Try to find the incident in the player's list of incidents.
                int idx = currentIncidents.FindIndex(e => e.Equals(incident));
                if (idx >= 0)
                {
                    // Mark the incident as applied.
                    currentIncidents[idx].State = IncidentState.APPLIED;

                    if (currentIncidents[idx].EquivalentTags(foodTags))
                    {
                        //this.ServerAddBadFood();
                        localPlayer._badFoodNumber--;
                    }

                    // Remove the incident if it should be.
                    if (remove)
                    {
                        currentIncidents.RemoveAt(idx);
                    }
                }
                else
                {
                    RootLogger.Warning(this, "Rpc: The incident {0} was not found on the player {1}.", incident, this.name);
                }

                ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable
                {
                    { "Talents", JsonUtility.ToJson(new Wrapper<Talent> { items = currentTalents.ToArray() }) },
                    { "Incidents", JsonUtility.ToJson(new Wrapper<Incident> { items = currentIncidents.ToArray() }) },
                    { "Pocket", JsonUtility.ToJson(currentPocket) },
                    { "GoodFood", localPlayer._goodFoodNumber },
                    { "BadFood", localPlayer._badFoodNumber },
                    { "FoodHealthStatus", localPlayer._goodFoodNumber - localPlayer._badFoodNumber },
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
            }
            else
            {
                RootLogger.Warning(this, "Rpc: The incident {0} was already applied or resolved.", incident);
            }
        }
    }
}

