using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Networking : MonoBehaviour {

    private const string typeName = "SMP_GAME_PROJECT";
    private const string gameName = "Dr. Jekyll or Mr. Hyde";
    private HostData[] hostList;
    public GameObject playerPrefab;

    private void SpawnPlayer() {
        Network.Instantiate(playerPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
    }

    private void RefreshHostList() {
        MasterServer.RequestHostList(typeName);
    }

    void OnMasterServerEvent(MasterServerEvent msEvent) {
        if (msEvent == MasterServerEvent.HostListReceived)
            hostList = MasterServer.PollHostList();
    }

    private void JoinServer(HostData hostData) {
        Network.Connect(hostData);
   //     SceneManager.LoadScene("Up");
    }

    void OnConnectedToServer() {
        Debug.Log("Server Joined");
        SpawnPlayer();
    }

    private void StartServer() {
        Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
        MasterServer.RegisterHost(typeName, gameName);
    //    SceneManager.LoadScene("Up");
    }

    void OnServerInitialized() {
        Debug.Log("Server Initialized");
        SpawnPlayer();
    }

    private void ResetGame() {
        Application.LoadLevel(Application.loadedLevel);
        Network.Disconnect();
    }
 
    void OnGUI() {
        if (!Network.isClient && !Network.isServer) {
            if (GUI.Button(new Rect(10, 60, 100, 50), "Start Server"))
                StartServer();

            if (GUI.Button(new Rect(10, 110, 100, 50), "Refresh Hosts"))
                RefreshHostList();

            if (hostList != null) {
                for (int i = 0; i < hostList.Length; i++) {
                    if (GUI.Button(new Rect(10, 160 + (50 * i), 100, 50), hostList[i].gameName))
                        JoinServer(hostList[i]);
                }
            }
        }
        // Add reset scene button
        if (GUI.Button(new Rect(10, 10, 100, 50), "Reset Game"))
            ResetGame();

    }

	// Use this for initialization
	void Start () {
        //MasterServer.ipAddress = "127.0.0.1";
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
