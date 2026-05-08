using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviourPunCallbacks
{
    [Header("Room Settings")]
    [SerializeField] private string roomName = "TinyKitchenRoom";
    [SerializeField] private byte maxPlayers = 4;

    [Header("Scene")]
    [SerializeField] private string gameScene = "Kitchen";

    private bool isConnecting;
    public Transform[] spawns;
    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        Connect();
        int id = Random.Range(0, spawns.Length);

        PhotonNetwork.Instantiate(
            "Player",
            spawns[id].position,
            Quaternion.identity
        );
    }

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
            return;

        isConnecting = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");

        if (isConnecting)
        {
            PhotonNetwork.JoinOrCreateRoom(
                roomName,
                new RoomOptions { MaxPlayers = maxPlayers },
                TypedLobby.Default
            );
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room: " + PhotonNetwork.CurrentRoom.Name);

        // El host carga la escena y todos entran
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(gameScene);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player Joined: " + newPlayer.NickName);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("Player Left: " + otherPlayer.NickName);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected: " + cause);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Join Room Failed: " + message);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Create Room Failed: " + message);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Inicio");
    }
}