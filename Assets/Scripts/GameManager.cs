using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour {

    NetworkManager networkManager = new NetworkManager();
    MapManager mapManager = new MapManager();
    PlayerManager playerManager;

    private void Start()
    {
        networkManager = new NetworkManager("localhost", 10000);
        StartCoroutine(ProcessMessage());
    }

    public void SetConnection()
    {
        //start Network Manager
        networkManager.SetConnection();
        //start Map Manager
        //get map size and obstacles
        networkManager.PutMessage("choose_map_random");
        /*string recvMessage;
        while (true)
        {
            recvMessage = networkManager.GetMessage();
            if (!recvMessage.Equals("none"))
            {
                string[] splitRecvMessage = recvMessage.Split(' ');
                if (splitRecvMessage[0].Equals("choose_map_random"))
                {
                    mapManager = new MapManager(splitRecvMessage[1]);
                }
                break;
            }
        }*/
        //start Player Manager
    }

    public void CloseConnection()
    {
        networkManager.CloseConnection();
    }

    public void Action()
    {
        mapManager.StartNewRound();
        mapManager.FindAvailablePathForward(4, 4, 2, 8);
        mapManager.ColorizeAvailablePath();

        mapManager.FindMovePathForward(5, 5, 3, 8);
        mapManager.ColorizeMovePath();
        mapManager.Show();
    }
	
    IEnumerator ProcessMessage()
    {
        string recvMessage;
        while (true)
        {
            recvMessage = networkManager.GetMessage();
            if (!recvMessage.Equals("none"))
            {
                string[] splitRecvMessage = recvMessage.Split(' ');
                if (splitRecvMessage[0].Equals("choose_map_random"))
                {
                    mapManager = new MapManager(splitRecvMessage[1]);
                }
                break;
            }
            yield return null;
        }
    }
}
