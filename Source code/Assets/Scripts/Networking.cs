using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Networking : MonoBehaviour {

    public NetworkView nView;
    private const string typeName = "SMP_GAME_PROJECT";
    private const string gameName = "Dr. Jekyll or Mr. Hyde";
    private string id = "Server id: " + System.Guid.NewGuid().ToString();
    private string _gameName = "";
    private HostData[] hostList;
    private bool gameStarted = false;
    public GameObject playerPrefab;

    bool[,] grid;
    int height;
    int width;

    private void SpawnPlayer()
    {
        Vector3 position = generateRandomPosition();
        Network.Instantiate(playerPrefab, position, Quaternion.identity, 0);
    }


    private Vector3 generateRandomPosition()
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
    }

    void OnConnectedToServer() {
        //Debug.Log("Server Joined");
        //grid = SceneGeneration.getGrid();
        //width = SceneGeneration.width;
        //height = SceneGeneration.height;
        //SpawnPlayer();
    }

    private void StartServer() {
        Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
        MasterServer.RegisterHost(typeName, gameName);
        //MasterServer.RegisterHost(typeName, id);
    }


    void OnServerInitialized()
    {
        //Debug.Log("Server Initialized");
        //grid = SceneGeneration.getGrid();
        //width = SceneGeneration.width;
        //height = SceneGeneration.height;
        //SpawnPlayer();
    }

    private void StopServer()
    {
        Network.Disconnect();
    }

    private void ResetGame() {
        Application.LoadLevel(0);
    }

    void OnGUI()
    {
        //if (!Network.isClient && !Network.isServer) {
        //    if (GUI.Button(new Rect(10, 60, 100, 50), "Start Server"))
        //        StartServer();

        //    if (GUI.Button(new Rect(10, 110, 100, 50), "Refresh Hosts"))
        //        RefreshHostList();

        //    if (hostList != null) {
        //        for (int i = 0; i < hostList.Length; i++) {
        //            if (GUI.Button(new Rect(10, 160 + (50 * i), 100, 50), hostList[i].gameName))
        //                JoinServer(hostList[i]);
        //        }
        //    }
        //}
        //// Add reset scene button
        //if (GUI.Button(new Rect(10, 10, 100, 50), "Reset Game"))
        //    ResetGame();

        int h = Screen.height;
        int w = Screen.width;
        int dim = 100;

        if (!gameStarted && !Network.isServer && Application.loadedLevel == 1)
        {
            string text = "Waiting for host to start the game...";
            var s = new GUIStyle();
            s.normal.textColor = Color.yellow;
            GUI.Box(new Rect(0, 0, w, h), text);
        }

        if ((Network.isServer || Network.isClient) && Application.loadedLevel == 0)
        {
            int nr = Network.connections.Length + 1;
            string text = "Nr. of players connected: " + nr.ToString();
            var s = new GUIStyle();
            s.normal.textColor = Color.yellow;
            GUI.Label(new Rect(w / 2 - 55, h / 2 + 70, 180, 30), text);
        }

        if (Network.isClient && Application.loadedLevel == 0)
        {
            string text = "Server: " + _gameName;
            //string text = _gameName;
            var s = new GUIStyle();
            s.normal.textColor = Color.yellow;
            GUI.Label(new Rect(w / 2 - 60, h / 2 + 110, 300, 40), text);
            //GUI.Label(new Rect(w / 2 - 100, h / 2 + 110, 300, 40), text);
        }
        if (Network.isClient && Application.loadedLevel == 0)
        { 
            string text = "Waiting for host to load the game...";
            GUI.Label(new Rect(w / 2 - 80, h / 2 - 80, 300, 30), text);
        }


        if (Network.isServer && Application.loadedLevel == 1 && !gameStarted)
        {
            if (GUI.Button(new Rect(w / 2 - dim / 2, h / 2 - dim / 2, 2 * dim, dim), "Start Game"))
            {
                gameStarted = true;
                nView.RPC("spawnPlayers", RPCMode.All);
            }
        }
        if (Network.isServer && Application.loadedLevel == 1)
        {
            if (GUI.Button(new Rect(10, 10, 100, 50), "Reset Game"))
                nView.RPC("resetGame", RPCMode.All);
        }

        if (!Network.isServer && !Network.isClient && hostList != null)
        {
            if (hostList != null && hostList.Length > 0)
                GUI.Label(new Rect(10, 10, 350, 30), new GUIContent("Online server:"));

            for (int i = 0; i < hostList.Length; i++)
            {
                if (GUI.Button(new Rect(10, 40 + (40 * i), 350, 30), hostList[i].gameName)) {
                    _gameName = hostList[i].gameName;
                    JoinServer(hostList[i]);
                }
            }
        }

        if (!Network.isClient && !Network.isServer && Application.loadedLevel == 0)
        {
            if(hostList == null || (hostList != null && hostList.Length == 0))
                if (GUI.Button(new Rect(w / 2 - 50, h / 2 + 70, 150, 30), "Start Server"))
                    StartServer();
            
        }

        if (Network.isServer && Application.loadedLevel == 0)
        {
            if (GUI.Button(new Rect(w / 2 - 50, h / 2 + 110, 150, 30), "Stop Server"))
            {
                nView.RPC("stopServer", RPCMode.All);
            }
        }

        if (Network.isServer && Application.loadedLevel == 0)
        {
            if (GUI.Button(new Rect(w / 2 - 50, h / 2 + 150, 150, 30), "Load Map"))
            {
                nView.RPC("loadGameScene", RPCMode.All);
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        //MasterServer.ipAddress = "127.0.0.1";
        nView = GetComponent<NetworkView>();
    }

    // Update is called once per frame
    void Update()
    {
        RefreshHostList();
    }

    [RPC]
    public void loadGameScene()
    {
        Application.LoadLevel(1);
    }

    [RPC]
    public void resetGame()
    {
        ResetGame();
    }

    [RPC]
    public void stopServer()
    {
        StopServer();
    }

    [RPC]
    public void spawnPlayers()
    {
        gameStarted = true;
        grid = SceneGeneration.getGrid();
        width = SceneGeneration.width;
        height = SceneGeneration.height;
        SpawnPlayer();
    }
}
