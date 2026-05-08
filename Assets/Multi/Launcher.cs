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

    [Header("Spawn Settings")]
    [Tooltip("Arrastra aquí los Empty GameObjects donde quieres que spawneen los jugadores")]
    [SerializeField] private Transform[] spawnPoints;

    private bool isConnecting;

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        Connect();
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
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions { MaxPlayers = maxPlayers }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room: " + PhotonNetwork.CurrentRoom.Name);

        Vector3 spawnPosition = Vector3.zero;
        Quaternion spawnRotation = Quaternion.identity;

        // === SPAWN EN POSICIÓN PERSONALIZADA ===
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            spawnPosition = spawnPoints[randomIndex].position;
            spawnRotation = spawnPoints[randomIndex].rotation;

            Debug.Log($"Player spawned at spawn point {randomIndex}");
        }
        else
        {
            Debug.LogWarning("No hay spawn points asignados. Spawneando en (0,0,0)");
        }

        PhotonNetwork.Instantiate("Player", spawnPosition, spawnRotation);

        // El Master Client carga la escena
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(gameScene);
        }
    }

    // ====================== OTROS CALLBACKS ======================
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

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Inicio");
    }
}