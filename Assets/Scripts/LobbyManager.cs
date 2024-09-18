using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using System;
using KoboldTools;


namespace Herman
{
    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        [Header("Login Panel")]
        public GameObject LoginPanel;

        public TMP_InputField PlayerNameInput;

        [Header("Room List Panel")]
        public GameObject RoomListPanel;
        public GameObject RoomListContent;
        public GameObject RoomListEntryPrefab;


        [Header("Create Room Panel")]
        public GameObject CreateRoomPanel;
        public TMP_InputField RoomNameInputField;
        
        private Dictionary<string, RoomInfo> cachedRoomList;
        private Dictionary<string, GameObject> roomListItems;
        private Dictionary<string, GameObject> playerListItems;

        [Header("Inside Room Panel")]
        public GameObject InsideRoomPanel;

        public Button StartGameButton;
        public GameObject PlayerListEntryPrefab;
        public GameObject PlayerListParent;

        #region UNITY
        public static LobbyManager Instance { get; private set; }
        public void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }

            PhotonNetwork.AutomaticallySyncScene = true;

            cachedRoomList = new Dictionary<string, RoomInfo>();
            roomListItems = new Dictionary<string, GameObject>();

            PlayerNameInput.text = "Player " + UnityEngine.Random.Range(1000, 10000);
        }
        
        #endregion

        #region PUN CALLBACKS
        public override void OnConnectedToMaster()
        {
            this.SetActivePanel(RoomListPanel.name);
            if (!PhotonNetwork.InLobby)
            {
                Debug.Log("joinLobby____Go");
                PhotonNetwork.JoinLobby();
            }
            Debug.Log("Connected_Master");
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log("RoomListUpdated");
            ClearRoomListView();
            UpdateCachedRoomList(roomList);
            UpdateRoomListView();
        }

        public override void OnJoinedLobby()
        {
            // whenever this joins a new lobby, clear any previous room lists
            cachedRoomList.Clear();
            ClearRoomListView();
        }
 
        public override void OnLeftRoom()
        {
            Debug.Log("test: OnLeftRoom");
            SetActivePanel(RoomListPanel.name);

            foreach (GameObject entry in playerListItems.Values)
            {
                Destroy(entry.gameObject);
            }

            playerListItems.Clear();
            playerListItems = null;
        }
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log("test: OnPlayerLeftRoom");

            UpdateRoomListView();

            //if (playerListItems.ContainsKey(otherPlayer.ActorNumber))
            //{
            //    Destroy(playerListItems[otherPlayer.ActorNumber].gameObject);
            //    playerListItems.Remove(otherPlayer.ActorNumber);
            //}
            if (otherPlayer.CustomProperties.TryGetValue("uniqueId", out object uniqueIdObj))
            {
                string uniqueId = (string)uniqueIdObj;
                if (playerListItems.ContainsKey(uniqueId))
                {
                    Destroy(playerListItems[uniqueId].gameObject);
                    playerListItems.Remove(uniqueId);
                }
            }
        }
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("Failed CreateRoom" + message);
            SetActivePanel(LoginPanel.name);
        }
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("Failed JoinRoom" + message);
            SetActivePanel(RoomListPanel.name);
        }
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {

            Debug.Log("test: OnPlayerEnteredRoom" + newPlayer);

            GameObject entry = Instantiate(PlayerListEntryPrefab);
            entry.transform.SetParent(PlayerListParent.transform, false);
            entry.transform.localScale = Vector3.one;
            bool isLocalPlayer = newPlayer == PhotonNetwork.LocalPlayer;

            PlayerListItem item = entry.GetComponent<PlayerListItem>();
            item.lobbyManager = LobbyManager.Instance;

            string uniqueId = (string)newPlayer.CustomProperties["uniqueId"];
            item.Initialize(newPlayer.NickName, false, false, isLocalPlayer, uniqueId); // Pass uniqueId here


            if (isLocalPlayer)
            {
                //TMP_InputField inputField = item.NickNameInput;
                Toggle mayorToggle = item.RunAsMayorToggle;
                Toggle readyToggle = item.ReadyToggle;

                //inputField.onValueChanged.AddListener(item.OnInputChange);
                mayorToggle.onValueChanged.AddListener(item.OnMayorToggleChange);
                readyToggle.onValueChanged.AddListener(item.OnReadyToggleChange);
            }

            playerListItems.Add(uniqueId, entry);

        }
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            Debug.Log("OnPlayerPropertiesUpdate for player: " + targetPlayer.NickName);


            if (targetPlayer.CustomProperties.TryGetValue("uniqueId", out object uniqueIdObj))
            {
                string uniqueId = (string)uniqueIdObj;


                if (playerListItems.TryGetValue(uniqueId, out GameObject playerListItem))
                {
                    PlayerListItem playerItemScript = playerListItem.GetComponent<PlayerListItem>();

                    if (changedProps.ContainsKey("PlayerInput") && targetPlayer == PhotonNetwork.LocalPlayer)
                    {
                        string newInputValue = (string)changedProps["PlayerInput"];
                        playerItemScript.UpdateInputField(newInputValue);
                    }

                    if (changedProps.ContainsKey("IsMayor"))
                    {
                        bool isMayor = (bool)changedProps["IsMayor"];
                        playerItemScript.UpdateMayorStatus(isMayor);
                    }

                    if (changedProps.ContainsKey("IsReady"))
                    {
                        bool isReady = (bool)changedProps["IsReady"];
                        playerItemScript.UpdateReadyState(isReady);
                    }
                }
            }

            // Optionally, re-check if all players are ready
            CheckAllPlayersReady();
        }
        

        public void OnReadyStateChanged(bool isReady)
        {
            ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable
            {
                { "IsReady", isReady }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
        }
        public override void OnCreatedRoom()
        {
            Debug.Log("test: Room is created");

            cachedRoomList.Clear();

            SetActivePanel(InsideRoomPanel.name);
            
        }
        public override void OnJoinedRoom()
        {
            Debug.Log("test: JoinedRoom" + PhotonNetwork.InLobby);


            cachedRoomList.Clear();
            SetActivePanel(InsideRoomPanel.name);
            ClearPlayerListItems();
            playerListItems = new Dictionary<string, GameObject>();

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                GameObject entry = Instantiate(PlayerListEntryPrefab);
                entry.transform.SetParent(PlayerListParent.transform, false);
                entry.transform.localScale = Vector3.one;

                bool isLocalPlayer = p == PhotonNetwork.LocalPlayer;
                PlayerListItem item = entry.GetComponent<PlayerListItem>();

                string uniqueId = (string)p.CustomProperties["uniqueId"];
                item.Initialize(p.NickName, false, false, isLocalPlayer, uniqueId);

                playerListItems.Add(uniqueId, entry);
            }

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }
        #endregion

        #region UI CALLBACKS
        public void OnBackButtonClicked()
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

            SetActivePanel(RoomListPanel.name);
        }
        public void OnLeaveGameButtonClicked()
        {
            PhotonNetwork.LeaveRoom();
        }
        public void OnLoginButtonClicked()
        {
            string playerName = PlayerNameInput.text;

            if (!string.IsNullOrEmpty(playerName))
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;

                var properties = new ExitGames.Client.Photon.Hashtable
                {
                    ["uniqueId"] = GenerateUniqueId()
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(properties);

                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                Debug.LogError("Player Name is invalid.");
            }
        }

        public void OnCreateRoomButtonClicked()
        {
            string roomName = RoomNameInputField.text.ToString();
            
            byte maxPlayers = 6;


            RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers, PlayerTtl = 10000 };

            Debug.Log("OnCreateRoomButtonClicked" + roomName + options);
            PhotonNetwork.CreateRoom(roomName, options, null);
        }
        #endregion
        public void OnStartGameButtonClicked()
        {
            // Pick one mayor


            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            PhotonNetwork.LoadLevel("Main");
        }

        private void SetActivePanel(string activePanel)
        {
            LoginPanel.SetActive(activePanel.Equals(LoginPanel.name));
            CreateRoomPanel.SetActive(activePanel.Equals(CreateRoomPanel.name));
            RoomListPanel.SetActive(activePanel.Equals(RoomListPanel.name));
            InsideRoomPanel.SetActive(activePanel.Equals(InsideRoomPanel.name));

        }
        private void UpdateRoomListView()
        {

            int roomNo = 1; // Start with 1 or any other number you prefer
            foreach (RoomInfo roomInfo in cachedRoomList.Values) // Assume roomList is your list of rooms
            {
                GameObject entryObj = Instantiate(RoomListEntryPrefab, RoomListContent.transform); // parentTransform is where you want to list the rooms
                RoomListItem entry = entryObj.GetComponent<RoomListItem>();
                entry.Initialize(roomNo, roomInfo.Name, roomInfo.masterClientId, roomInfo.PlayerCount, roomInfo.MaxPlayers);
                roomNo++; // Increment the room number for the next entry
                roomListItems.Add(roomInfo.Name, entryObj);
            }
        }
        private void ClearPlayerListItems()
        {
            // Check if the dictionary is not null
            if (playerListItems != null)
            {
                // Iterate and destroy the GameObjects
                foreach (GameObject entry in playerListItems.Values)
                {
                    if (entry != null)
                    {
                        Destroy(entry.gameObject);
                    }
                }

                // Clear and set the dictionary to null
                playerListItems.Clear();
                
            }
        }

        private void ClearRoomListView()
        {
            foreach (GameObject entry in roomListItems.Values)
            {
                Destroy(entry.gameObject);
            }

            roomListItems.Clear();
        }
        private void UpdateCachedRoomList(List<RoomInfo> roomList)
        {
            foreach (RoomInfo info in roomList)
            {
                // Remove room from cached room list if it got closed, became invisible or was marked as removed
                if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
                {
                    if (cachedRoomList.ContainsKey(info.Name))
                    {
                        cachedRoomList.Remove(info.Name);
                    }

                    continue;
                }

                // Update cached room info
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList[info.Name] = info;
                }
                // Add new room info to cache
                else
                {
                    cachedRoomList.Add(info.Name, info);
                }
            }
        }
        private bool CheckPlayersReady()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return false;
            }

          

            return true;

        }
        public void CheckAllPlayersReady()
        {
            foreach (var item in playerListItems)
            {
                PlayerListItem playerItem = item.Value.GetComponent<PlayerListItem>();
                if (!playerItem.ReadyToggle.isOn)
                {
                    StartGameButton.interactable = false;
                    return;
                }

            }
            Debug.Log("StartButton____Active");
            StartGameButton.interactable = true;
        }
        private string GenerateUniqueId()
        {
            return Guid.NewGuid().ToString();
        }

    }
}