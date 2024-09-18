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

        private int defaultCamAliveCnt = 0;
        
        public Transform[] SpawnPoints;

        public GameObject characterPrefab;
        public GameObject cityCharacterPrefab;
        public TMP_Text personNameText;

        private List<GameObject> characters;

        public string levelDataJson = "leveldata.json";
        public LevelData loadedLevelData = null;
        public PlayableDirector intoIntroductionTimeline;
        public PlayableDirector intoGameTimeline;
        public CameraFollowPlayer cameraFollowPlayer;

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
            Alert.show(true, "tutoMStoryIslandTitle", "tutoIntroQuest", null, "btnLetPlay");
            while (Alert.open)
                yield return null;
            Alert.show(true, "tutoMStoryIslandTitle", "tutoMStoryIsland", null, "btnOk");
            while (Alert.open)
                yield return null;
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (PhotonNetwork.PlayerList[i] == PhotonNetwork.LocalPlayer)
                {
                    cameraFollowPlayer._player = characters[i];
                    cameraFollowPlayer.characterChanged();
                    break;
                }
            }
            intoIntroductionTimeline.Play();
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("IsMayor", out object isMayorGetObj))
            {
                bool isMayor = (bool)isMayorGetObj;
                if (isMayor)
                {
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
                                Debug.Log(i + " " + characters.Count);
                                characters[i].GetComponent<Character>().steeringTarget = point;
                            }
                        }
                    }
                }
                if (changedProps.ContainsKey("PersonName"))
                {
                    if (targetPlayer.CustomProperties.TryGetValue("PersonName", out object personNameObj))
                    {
                        string personName = (string)personNameObj;

                        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                        {
                            if (PhotonNetwork.PlayerList[i] == targetPlayer)
                            {
                                Debug.Log(i + " " + characters.Count);
                                characters[i].transform.Find("Canvas").Find("Name Tag").GetComponent<Text>().text = Localisation.instance.getLocalisedText(personName);
                                if (targetPlayer == PhotonNetwork.LocalPlayer)
                                {
                                    personNameText.text = Localisation.instance.getLocalisedText(personName);
                                }
                            }
                        }
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
                GameObject character = null;

                Debug.Log(loadedLevelData.MayorPersons.SelectRandom(1));

                if (player.CustomProperties.TryGetValue("IsMayor", out object isMayorObj))
                {
                    bool isMayor = (bool)isMayorObj;
                    
                    if (isMayor)
                    {
                        character = Instantiate(cityCharacterPrefab, SpawnPoints[i].position, Quaternion.identity);
                        ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable
                        {
                            { "PersonName", loadedLevelData.MayorPersons.SelectRandom(1)[0].Title }
                        };
                        player.SetCustomProperties(playerProperties);
                    }
                    else
                    {
                        character = Instantiate(characterPrefab, SpawnPoints[i].position, Quaternion.identity);
                        ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable
                        {
                            { "PersonName", loadedLevelData.Persons.SelectRandom(1)[0].Title }
                        };
                        player.SetCustomProperties(playerProperties);
                    }
                }
                else
                {
                    character = Instantiate(characterPrefab, SpawnPoints[i].position, Quaternion.identity);
                    ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable
                    {
                        { "PersonName", loadedLevelData.Persons.SelectRandom(1)[0].Title }
                    };
                    player.SetCustomProperties(playerProperties);
                }
                character.transform.Rotate(0, 180 - 20 * (3 - i), 0);
                characters.Add(character);
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
    }
}

