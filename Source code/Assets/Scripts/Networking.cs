using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Networking : MonoBehaviour {

    private const string typeName = "SMP_GAME_PROJECT";
    private const string gameName = "Dr. Jekyll or Mr. Hyde";
    private HostData[] hostList;
    public GameObject playerPrefab;
    bool[,] grid;
    int height, width;

    private void SpawnPlayer() {
        Vector3 position = generateRandomPosition();
        Network.Instantiate(playerPrefab, position, Quaternion.identity, 0);
    }

    public Vector3 generateRandomPosition()
    {
        int x = (int)Random.Range(-13.0f, 13.0f);
        int y = (int)Random.Range(-9.0f, 9.0f);
        int xGrid = x + 13;
        int yGrid = y + 9;

        if (grid[xGrid, yGrid] == true)
        {
            return generateRandomPosition();
        }
        else
        {
            grid[xGrid, yGrid] = true;
            return new Vector3(x, y, 0);
        }
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
        grid = SceneGeneration.getGrid();
        width = SceneGeneration.width;
        height = SceneGeneration.height;
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
