using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomListItem : MonoBehaviour
{
    public TMP_Text RoomNoText;
    public TMP_Text RoomNameText;
    public TMP_Text RoomCreatorText;
    public TMP_Text PlayersStatusText;
    public Button JoinButton;

    private string roomName;

    // This method will be called to setup each room entry
    public void Initialize(int roomNo, string roomName, int roomCreator, int currentPlayerCount, int maxPlayers)
    {
        this.roomName = roomName;
        RoomNoText.text = roomNo.ToString();
        RoomNameText.text = roomName;
        //RoomCreatorText.text = roomCreator.ToString();
        PlayersStatusText.text = $"{currentPlayerCount}/{maxPlayers}";

        JoinButton.onClick.AddListener(JoinRoom);
    }

    private void JoinRoom()
    {
        Debug.Log("Joining Room: " + roomName);
        PhotonNetwork.JoinRoom(roomName);
    }

    private void OnDestroy()
    {
        // Remove listener to avoid memory leaks
        JoinButton.onClick.RemoveListener(JoinRoom);
    }
}
