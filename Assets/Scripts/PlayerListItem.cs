using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Herman;

public class PlayerListItem : MonoBehaviour
{
    public TMP_Text NickNameInput;
    public Toggle RunAsMayorToggle;
    public Toggle ReadyToggle;

    public LobbyManager lobbyManager;
    private string uniqueId; 

    void Start()
    {
        // Check if this is the local player's UI
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("uniqueId", out object localUniqueIdObj) && (string)localUniqueIdObj == this.uniqueId)
        {
            //if (NickNameInput != null)
            //{
            //    NickNameInput.onValueChanged.AddListener(OnInputChange);
            //}
            if (RunAsMayorToggle != null)
            {
                RunAsMayorToggle.onValueChanged.AddListener(OnMayorToggleChange);
            }
            if (ReadyToggle != null)
            {
                ReadyToggle.onValueChanged.AddListener(OnReadyToggleChange);
            }
        }
    }
    // This method will be called to setup each room entry
    public void Initialize(string NickName, bool MayorEnable, bool ReadyEnable, bool isLocalPlayer, string uniqueId)
    {
        NickNameInput.text = NickName;
        RunAsMayorToggle.isOn = MayorEnable;
        ReadyToggle.isOn = ReadyEnable;
        this.uniqueId = uniqueId; // Set the uniqueId

        // Enable or disable based on whether it's the local player
        //NickNameInput.interactable = isLocalPlayer;
        RunAsMayorToggle.interactable = isLocalPlayer;
        ReadyToggle.interactable = isLocalPlayer;
    }
    public void OnReadyStateChanged()
    {
        if (lobbyManager != null)
        {
            lobbyManager.CheckAllPlayersReady();
        }
    }
    public void UpdateMayorStatus(bool isMayor)
    {
        // Update the Mayor Toggle UI element or other relevant UI
        RunAsMayorToggle.isOn = isMayor;
    }
    public void UpdateReadyState(bool isReady)
    {
        ReadyToggle.isOn = isReady;
    }
    public void OnInputChange(string newValue)
    {
        // Update the custom property
        var properties = new ExitGames.Client.Photon.Hashtable
        {
            { "PlayerInput", newValue }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
    }
    public void UpdateInputField(string newValue)
    {
        NickNameInput.text = newValue;
    }
    
    public void OnMayorToggleChange(bool isToggled)
    {
        Debug.Log("Mayor Toggle Changed: " + isToggled);

        // Update the custom property for Mayor status
        var properties = new ExitGames.Client.Photon.Hashtable
    {
        { "IsMayor", isToggled }
    };
        PhotonNetwork.LocalPlayer.SetCustomProperties(properties);

        // Any additional logic based on mayor status
        // Example: Notify other parts of the game, update UI, etc.
    }
    public void OnReadyToggleChange(bool isToggled)
    {
        Debug.Log("Ready Toggle Changed: " + isToggled);

        // Update the custom property for Ready status
        var properties = new ExitGames.Client.Photon.Hashtable
    {
        { "IsReady", isToggled }
    };
        PhotonNetwork.LocalPlayer.SetCustomProperties(properties);

        // Any additional logic based on ready status
        // Example: Check if all players are ready, update UI, etc.
        if (lobbyManager != null)
        {
            lobbyManager.CheckAllPlayersReady();
        }
    }

}
