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
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun.Demo.PunBasics;


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
        public Material[] lizardBodyMaterials;
        public Material[] lizardCrystalMaterials;

        public Sprite spriteMovement;
        public static MainGameManager Instance = null;
        public MarketplaceSet _marketplaceDb = null;
        public Marketplace ComplementaryIntroMarket;
        public Sprite spriteQBuilding;
        public Sprite spriteQMay;
        public GameObject highlightCamera;
        public int highlightPriority = 2000;
        public GameObject buildingIcon;
        public UiResource Resource;

        public Offer taxOffer = null;

        public GameObject playerMarketplaceObj = null;
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
        public List<PolyPlayer> polyPlayers = new List<PolyPlayer>();
        public PolyPlayer _localPlayer = null;

        public Panel endMonthOverview = null;
        public Panel cityOverview = null;

        public bool _polyMoneyIntroduced = false;

        private float cacheAngleLower = 0;
        private float cacheAngleUpper = 0;

        public bool PolymoneyIntroduced
        {
            get
            {
                return this._polyMoneyIntroduced;
            }
            set
            {
                if (this._polyMoneyIntroduced != value)
                {
                    this._polyMoneyIntroduced = value;
                    this.onLevelStateChanged.Invoke();
                }
            }
        }
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
            Vector2 alertBigSize = new Vector2(800, 600);
            Vector2 alertBigSize1 = new Vector2(1200, 900);
            while (characters.Count < PhotonNetwork.PlayerList.Length)
                yield return null;

            KoboldTools.Alert.info("tutoIntroQuest", new KoboldTools.Alert.AlertParams { useLocalization = true, title = "tutoMStoryIslandTitle", closeText = "btnLetPlay" });
            while (KoboldTools.Alert.open)
                yield return null;
            KoboldTools.Alert.info("tutoMStoryIsland", new KoboldTools.Alert.AlertParams { useLocalization = true, title = "tutoMStoryIslandTitle", closeText = "btnOk", size = alertBigSize1 });
            while (KoboldTools.Alert.open)
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
                    KoboldTools.Alert.info("tutoMWelcome", new KoboldTools.Alert.AlertParams { useLocalization = true, title = "tutoMWelcomeTitle", closeText = "btnOk", size = alertBigSize });
                }
                else
                {
                    KoboldTools.Alert.info("tutoPIntro1", new KoboldTools.Alert.AlertParams { useLocalization = true, title = "tutoPWelcomeTitle", closeText = "btnOk", size = alertBigSize });
                }
            }
            else
            {
                KoboldTools.Alert.info("tutoPIntro1", new KoboldTools.Alert.AlertParams { useLocalization = true, title = "tutoPWelcomeTitle", closeText = "btnOk", size = alertBigSize });
            }
            while (KoboldTools.Alert.open)
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
                KoboldTools.Alert.tutorial("tutoMoveMajor", new KoboldTools.Alert.AlertParams { useLocalization = true, closeText = "tutoCloseAlertButton", sprite = spriteMovement });
                while (KoboldTools.Alert.open)
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
                KoboldTools.Alert.tutorial("tutoMoveEndMajor", new KoboldTools.Alert.AlertParams { useLocalization = true, closeText = "tutoCloseAlertButton" });
                while (KoboldTools.Alert.open)
                    yield return null;
            }
            else
            {

                walkArea.SetActive(true);
                KoboldTools.Alert.tutorial("tutoMoveCitizen", new KoboldTools.Alert.AlertParams { useLocalization = true, closeText = "tutoCloseAlertButton", sprite = spriteMovement });
                while (KoboldTools.Alert.open)
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
                KoboldTools.Alert.tutorial("tutoMoveEndCitizen", new KoboldTools.Alert.AlertParams { useLocalization = true, closeText = "tutoCloseAlertButton" });
                while (KoboldTools.Alert.open)
                    yield return null;
                spinWheelCamera.Priority = 1000;
                PlayerGetIncidents playerGetIncidents = GetComponent<PlayerGetIncidents>();
                cacheAngleLower = playerGetIncidents.minTargetAngle;
                cacheAngleUpper = playerGetIncidents.maxTargetAngle;
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
                                { "months", _months + 1 },
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

                if (changedProps.ContainsKey("OwnMarketplace"))
                {
                    if (targetPlayer.CustomProperties.TryGetValue("OwnMarketplace", out object ownMpObj))
                    {
                        string id = (string)ownMpObj;
                        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                        {
                            if (PhotonNetwork.PlayerList[i] == targetPlayer)
                            {
                                polyPlayers[i]._ownMarketplace = new Guid(id);
                                if (targetPlayer == PhotonNetwork.LocalPlayer)
                                {
                                    localPlayer._ownMarketplace = new Guid(id);
                                }
                            }
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
                PolyPlayer polyPlayer = polyPlayers[i];

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
                            string ownMarketplaceId = CreateMarketplace(polyPlayer, person.Title, Localisation.instance.getLocalisedFormat("marketplaceCitizenSubtitle", person.Title));
                            ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable
                            {
                                { "Person", JsonUtility.ToJson(person) },
                                { "Home", JsonUtility.ToJson(loadedLevelData.MayorHomes.SelectRandom(1)[0]) },
                                { "Job", JsonUtility.ToJson(loadedLevelData.MayorJobs.SelectRandom(1)[0]) },
                                { "Talents", JsonUtility.ToJson(new Wrapper<Talent> { items = talents.ToArray() }) },
                                { "Pocket", JsonUtility.ToJson(pocket) },
                                { "OwnMarketplace", ownMarketplaceId }
                            };
                            player.SetCustomProperties(playerProperties);
                            this._marketplaceDb.syncProvider.SetMarketSeller(ComplementaryIntroMarket.guid.ToString(), polyPlayer.uniqueId);
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
                        string ownMarketplaceId = CreateMarketplace(polyPlayer, person.Title, Localisation.instance.getLocalisedFormat("marketplaceCitizenSubtitle", person.Title));
                        ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable
                        {
                            { "Person", JsonUtility.ToJson(person) },
                            { "Home", JsonUtility.ToJson(loadedLevelData.Homes.SelectRandom(1)[0]) },
                            { "Job", JsonUtility.ToJson(loadedLevelData.Jobs.SelectRandom(1)[0]) },
                            { "Talents", JsonUtility.ToJson(new Wrapper<Talent> { items = talents.ToArray() }) },
                            { "Pocket", JsonUtility.ToJson(pocket) },
                            { "OwnMarketplace", ownMarketplaceId }
                        };
                        player.SetCustomProperties(playerProperties);
                    }
                }
            }
        }

        private string CreateMarketplace(PolyPlayer player, string title, string description)
        {
            Debug.Log("create market place");
            // setup marketplace and use syncprovider of the marketplace set to rpc creation over the network
            Marketplace newMarketplace = ScriptableObject.CreateInstance<Marketplace>();
            newMarketplace.init(title, description);

            string marketplaceGuid = Guid.NewGuid().ToString();
            this._marketplaceDb.syncProvider.AddMarketplace(JsonUtility.ToJson(newMarketplace), marketplaceGuid);
            this._marketplaceDb.syncProvider.SetMarketSeller(marketplaceGuid, player.uniqueId);
            //player.ServerSetOwnMarketplace(marketplaceGuid);
            Destroy(newMarketplace);
            return marketplaceGuid;
        }

        void SpawnPlayer()
        {
            characters = new List<GameObject> ();
            int playerId = 0;
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                // check Mayor
                Player player = PhotonNetwork.PlayerList[i];
                PolyPlayer polyPlayer = new PolyPlayer();
                if (player.CustomProperties.TryGetValue("uniqueId", out object uniqueId))
                {
                    polyPlayer.uniqueId = (string) uniqueId;
                }
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
                    playerId++;

                    SkinnedMeshRenderer renderer = character.GetComponentInChildren<SkinnedMeshRenderer>();

                    int matIdx = Array.FindIndex(renderer.materials, e => e.name.Contains("lizard_crystals"));
                    renderer.materials[matIdx].CopyPropertiesFromMaterial(this.lizardCrystalMaterials[playerId]);
                    matIdx = Array.FindIndex(renderer.materials, e => e.name.Contains("lizard_body"));
                    renderer.materials[matIdx].CopyPropertiesFromMaterial(this.lizardBodyMaterials[playerId]);
                }
                polyPlayer.Mayor = isMayor;
                character.transform.Rotate(0, 180 - 20 * (3 - i), 0);
                polyPlayer.LoadedCharacter = character.GetComponent<Character>();
                polyPlayer._marketplaceDb = _marketplaceDb;
                character.transform.Find("Canvas").Find("Symbols").gameObject.GetComponent<PlayerDisplaySymbols>().model = polyPlayer;
                polyPlayer.LoadedCharacter.model = polyPlayer;
                characters.Add(character);
                polyPlayer.player = player;
                polyPlayer.Resource = Resource;
                polyPlayer.registerOfferApplyEvent();
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

        public static float Remap(float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
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
                    else if (incident.EquivalentTags(taxTags))
                    {
                        Offer tmp = ScriptableObject.CreateInstance<Offer>();
                        JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(this.taxOffer), tmp);
                        tmp.guid = Guid.NewGuid();
                        tmp.buyingCost = new Cost(incident.ApplicationBenefit);
                        tmp.buyingBenefit = new Benefit(incident.ApplicationCost);

                        //Is it a flat tax?
                        if (Options_Controller.flatTax)
                        {
                            //Set the value to the base tax amount
                            tmp.buyingBenefit.Income[0].value = Options_Controller.baseTaxAmount;
                            int modValue = (int)(polyPlayer.Job.Salary * (Options_Controller.baseTaxRate / 100f));
                            print("Added value due to flat tax: " + modValue + " at a " + Options_Controller.baseTaxRate);
                            tmp.buyingBenefit.Income[0].value += modValue;
                            print("The new tax value is : " + tmp.buyingBenefit.Income[0].value);
                        }
                        //Is it a progressive tax?
                        else if (Options_Controller.progTax)
                        {
                            int modValue = 0;
                            if (polyPlayer.Job.Salary > 0)
                            {
                                tmp.buyingBenefit.Income[0].value = Options_Controller.baseTaxAmount;
                                modValue = (int)((Remap(polyPlayer.Job.Salary, 0, 1300, Options_Controller.baseTaxRate, Options_Controller.progressiveTaxUpper) / 100) * polyPlayer.Job.Salary);
                                print("Remapped: " + (Remap(polyPlayer.Job.Salary, 0, 1300, Options_Controller.baseTaxRate, Options_Controller.progressiveTaxUpper)));
                                print("Added value due to progressive tax: " + modValue);
                                tmp.buyingBenefit.Income[0].value += modValue;
                                print("The new tax value is : " + tmp.buyingBenefit.Income[0].value);
                            }
                            else
                            {
                                tmp.buyingBenefit.Income[0].value = Options_Controller.baseTaxAmount;
                                print("The new tax value is : " + tmp.buyingBenefit.Income[0].value);
                            }
                        }

                        /*
                        //Add the tax modifications
                        for(int i = 0; i < tmp.buyingBenefit.Income.Count; i++)
                        {


                        }
                        */

                        incident.AddSerializedOffer = JsonUtility.ToJson(tmp);
                    }
                    //player.ServerAddIncident(incident);
                    incidentsForPlayer.Add(incident);
                }
                AddIncidentToPlayer(player, incidentsForPlayer);
            }
            if (months == 2)
            {
                VC<PolyPlayer>.addModelToAllControllers(this.localPlayer, playerMarketplaceObj);
                StartCoroutine(marketRoutine());
            }
            else if (months > 2)
            {
                PlayerGetIncidents playerGetIncidents = GetComponent<PlayerGetIncidents>();
                if (localPlayer.Mayor)
                {
                    if (playerGetIncidents.disaterChance > 0)
                    {
                        int chance = UnityEngine.Random.Range(0, 100);
                        if (chance < playerGetIncidents.disaterChance)
                        {
                            playerGetIncidents.addCityEvent();
                        }
                    }
                }
                else
                {
                    playerGetIncidents.startWheelSpinning();
                }
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

        public void AddIncidentToPlayer(Player player, List<Incident> new_incidents)
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

        private IEnumerator marketRoutine()
        {
            PlayerGetIncidents playerGetIncidents = GetComponent<PlayerGetIncidents>();
            if (localPlayer.Mayor)
            {
                if (playerGetIncidents.disaterChance > 0)
                {
                    int chance = UnityEngine.Random.Range(0, 100);
                    if (chance < playerGetIncidents.disaterChance)
                    {
                        playerGetIncidents.addCityEvent();
                    }
                }
                KoboldTools.Alert.info("tutoMQStory", new KoboldTools.Alert.AlertParams { title = "tutoMQStoryTitle", useLocalization = true, hideCloseButton = false, closeText = "btnOk" });
            }
            else
            {
                playerGetIncidents.minTargetAngle = 1215f;
                playerGetIncidents.maxTargetAngle = 1215f;
                playerGetIncidents.startWheelSpinning();
                playerGetIncidents.minTargetAngle = cacheAngleLower;
                playerGetIncidents.maxTargetAngle = cacheAngleUpper;

                KoboldTools.Alert.tutorial("tutoMQStory", new KoboldTools.Alert.AlertParams { title = "tutoMQStoryTitle", useLocalization = true, hideCloseButton = false, closeText = "btnOk" });
                while (KoboldTools.Alert.open)
                    yield return null;

                localPlayer.WatchedMarket = this.ComplementaryIntroMarket;
                while (localPlayer.WatchedMarket != null)
                {
                    //player did not close his special building yet
                    if (localPlayer.OwnedMarketplaces.Count() > 0)
                    {
                        //player has bought a marketplace, close the market
                        localPlayer.WatchedMarket = null;
                    }
                    yield return null;
                }

                if (localPlayer.OwnedMarketplaces.Count > 0)
                {
                    // tell players to click on their building
                    Debug.Log(localPlayer.OwnedMarketplaces[0]);
                    Building building = Buildings.FirstOrDefault(b => localPlayer.OwnedMarketplaces[0] == b.Marketplace);
                    Debug.Log(building);
                    CinemachineVirtualCamera cam = this.highlightCamera.GetComponentInChildren<CinemachineVirtualCamera>();
                    if (building != null)
                    {
                        this.highlightCamera.transform.position = building.transform.position;
                        if (cam != null)
                        {
                            cam.Priority = highlightPriority;
                        }
                    }

                    yield return new WaitForSeconds(0.5f);

                    if (building != null && building.Marketplace.seller.Equals(localPlayer))
                    {
                        KoboldTools.Alert.tutorial("tutoQSearchBuilding", new KoboldTools.Alert.AlertParams { useLocalization = true, closeText = "btnOk", sprite = spriteQBuilding });
                        while (Alert.open)
                            yield return null;
                    }

                    while (
                        building != null
                        && building.Marketplace.seller.Equals(localPlayer)
                        && (localPlayer.WatchedMarket == null
                            || (localPlayer.WatchedMarket == localPlayer.OwnMarketplace
                                || !localPlayer.WatchedMarket.seller.Equals(localPlayer))))
                    {
                        //player did not open his special building yet
                        yield return null;
                    }

                    if (localPlayer.WatchedMarket != null)
                    {
                        KoboldTools.Alert.tutorial("tutoQInvest", new KoboldTools.Alert.AlertParams { useLocalization = true, title = "tutoQInvestHeader", closeText = "tutoCloseAlertButton" });

                        while (localPlayer.WatchedMarket != null)
                        {
                            yield return null;
                        }

                        if (localPlayer.OwnedMarketplaces.All(m => m.offers.Count == 0))
                        {
                            KoboldTools.Alert.tutorial("tutoQNoOffer", new KoboldTools.Alert.AlertParams { useLocalization = true, title = "tutoQNoOfferHeader", closeText = "tutoCloseAlertButton" });
                            while (KoboldTools.Alert.open)
                            {
                                yield return null;
                            }
                        }
                    }

                    yield return new WaitForSeconds(0.5f);
                    if (cam != null)
                    {
                        cam.Priority = 0;
                    }

                }
                //KoboldTools.Alert.info("turnWaitOthers", new KoboldTools.Alert.AlertParams { useLocalization = true, hideCloseButton = true });

                //buildingIcon.SetActive(true);

            }
        }
    
        public PolyPlayer getPlayerById(string guid)
        {
            for (int i = 0; i < polyPlayers.Count; i++)
            {
                if (polyPlayers[i].uniqueId == guid)
                    return polyPlayers[i];
            }
            return null;
        }

        public void applyOffer(PolyPlayer buyer, PolyPlayer seller, Offer offer)
        {
            photonView.RPC("RpcApplyOffer", RpcTarget.AllBuffered, buyer.uniqueId, seller.uniqueId, JsonUtility.ToJson(offer));
        }

        [PunRPC]
        public void RpcApplyOffer(string buyerId, string sellerId, string offerData)
        {
            PolyPlayer buyer = getPlayerById(buyerId);
            PolyPlayer seller = getPlayerById(sellerId);
            Offer offer = new Offer();
            JsonUtility.FromJsonOverwrite(offerData, offer);
            //Guid oGuid = offer.guid;
            //bool found = false;
            //foreach (Marketplace market in this._marketplaceDb.marketplaces)
            //{
            //    foreach (Offer offer1 in market.offers)
            //    {
            //        if (offer1.guid == oGuid)
            //        {
            //            offer = offer1;
            //            found = true;
            //            break;
            //        }
            //    }
            //    if (found)
            //    {
            //        break;
            //    }
            //}
            if (buyerId == null || sellerId == null)
            {
                RootLogger.Exception(this, "Buyer or seller is null (buyer: {0}, seller: {1})", buyer, seller);
            }
            if (object.ReferenceEquals(buyer, seller))
            {
                RootLogger.Exception(this, "Players should not buy their own offers (buyer: {0}, seller: {1})", buyer, seller);
            }

            RootLogger.Info(this, "Server: Applying the offer {0}, buyer: {1}, seller: {2}", offer, buyer, seller);

            // Determine, whether the offer that's being applied trades in complementary currency (anything not FIAT).
            ExitGames.Client.Photon.Hashtable buyerProperties = new ExitGames.Client.Photon.Hashtable { };
            ExitGames.Client.Photon.Hashtable sellerProperties = new ExitGames.Client.Photon.Hashtable { };
            int revenue = buyer.CalculateRevenue(offer, Currency.Q);

            // Apply the player points based on revenue.
            if (revenue > 0)
            {
                //buyer.ServerSetPoints(buyer.Points + revenue);
                buyerProperties.Add("Points", buyer.Points + revenue);
                // Increase every building's luminance by the amount determined in the threshold list.
                Building building = Buildings.FirstOrDefault(e => e.DisplaysLuminance && e.Marketplace != null && e.Marketplace.offers.Contains(offer));
                if (building != null)
                {
                    building.Luminance += LuminancePerPoint * revenue;
                }
            }

            // Apply the buyer portion
            offer.buyingCost.applyCost(buyer.Pocket);
            offer.buyingBenefit.applyBenefit(buyer.Talents, buyer.Incidents, buyer.Pocket);

            if (seller != null)
            {
                // Apply the seller points based on revenue.
                if (revenue > 0)
                {
                    //seller.ServerSetPoints(seller.Points + revenue);
                    sellerProperties.Add("Points", seller.Points + revenue);
                }

                // Apply the seller portion.
                offer.sellingCost.applyCost(seller.Pocket);
                offer.sellingBenefit.applyBenefit(seller.Talents, seller.Incidents, seller.Pocket);

                sellerProperties.Add("Talents", JsonUtility.ToJson(new Wrapper<Talent> { items = seller.Talents.ToArray() }));
                sellerProperties.Add("Pocket", JsonUtility.ToJson(seller.Pocket));
                AddIncidentToPlayer(seller.player, seller.Incidents);
            }
            else
            {
                RootLogger.Warning(this, "Server: The seller is not known");
            }

            buyerProperties.Add("Talents", JsonUtility.ToJson(new Wrapper<Talent> { items = buyer.Talents.ToArray() }));
            buyerProperties.Add("Pocket", JsonUtility.ToJson(buyer.Pocket));
            AddIncidentToPlayer(buyer.player, buyer.Incidents);

            // Invoke the offer applied event and remove the offer unless it is persistent.
            offer.offerApplied.Invoke(offer, buyer);
            if (seller.uniqueId.Equals(localPlayer.uniqueId))
            {
                seller.OnOfferApplied.Invoke(offer, buyer);
            }

            // Remove the offer if it exists somewhere on the market.
            if (!offer.persistent)
            {
                foreach (Marketplace market in this._marketplaceDb.marketplaces)
                {
                    foreach (Offer tmpOffer in market.offers)
                    {
                        if (tmpOffer.guid.Equals(offer.guid))
                        {
                            this._marketplaceDb.syncProvider.RemoveOffer(market.guid.ToString(), offer.guid.ToString());
                            return;
                        }
                    }
                }
            }
        }

        public void createOffer(string playerId, string marketId, Offer offer)
        {
            photonView.RPC("RPCCreateOffer", RpcTarget.AllBuffered, playerId, marketId, JsonUtility.ToJson(offer));
        }

        public void ClientUpdateIncident(PolyPlayer player, Incident incident)
        {
            photonView.RPC("RPCClientUpdateIncident", RpcTarget.AllBuffered, player.uniqueId, JsonUtility.ToJson(incident));
        }

        [PunRPC]
        public void RPCClientUpdateIncident(string playerId, string jsonData)
        {
            PolyPlayer player = getPlayerById(playerId);
            Incident incident = JsonUtility.FromJson<Incident>(jsonData);
            if (incident != null)
            {
                int idx = player.Incidents.FindIndex(e => e.Equals(incident));
                if (idx >= 0)
                {
                    if (!player.Incidents[idx].Identical(incident))
                    {
                        RootLogger.Debug(this, "Rpc: Updating an incident: {0} (json: {1})", incident, jsonData);
                        player.Incidents[idx] = incident;
                        player.PlayerStateChanged.Invoke();
                    }
                }
                else
                {
                    RootLogger.Exception(this, "The incident {0} was not found on the player {1}.", incident, this.name);
                }
            }
            else
            {
                RootLogger.Exception(this, "Unable to deserialize the following data into an Incident: '{0}'", jsonData);
            }
        }

        [PunRPC]

        public void RPCCreateOffer(string playerId, string marketId, string offerData)
        {
            PolyPlayer player = getPlayerById(playerId);
            Guid offerGuid = Guid.NewGuid();
            Offer deserializedOffer = ScriptableObject.CreateInstance<Offer>();
            JsonUtility.FromJsonOverwrite(offerData, deserializedOffer);

            deserializedOffer.creationCost.applyCost(player.Pocket);
            deserializedOffer.creationBenefit.applyBenefit(player.Talents, player.Incidents, player.Pocket);

            if (PhotonNetwork.IsMasterClient)
            {
                this._marketplaceDb.syncProvider.AddOffer(marketId, offerData, offerGuid.ToString());
            }
            ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable {
                { "Pocket", JsonUtility.ToJson(player.Pocket) },
            };
            Destroy(deserializedOffer);
        }

        public void removeOffer(PolyPlayer player, Offer offer)
        {
            Guid oGuid = offer.guid;
            foreach (Marketplace market in this._marketplaceDb.marketplaces)
            {
                foreach (Offer offer1 in market.offers)
                {
                    if (offer1.guid == oGuid)
                    {
                        this._marketplaceDb.syncProvider.RemoveOffer(market.guid.ToString(), oGuid.ToString());
                        return;
                    }
                }
            }
        }
    }
}

