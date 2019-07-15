using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour 
{

    GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void SetConnectionButton()
    {
        gameManager.SetConnection();
    }

    public void CloseConnectionButton()
    {
        gameManager.CloseConnection();
    }

    public void ActionButton()
    {
        gameManager.Action();
    }

    public void Block()
    {
        Debug.Log("UI is blocked");
    }

    public void Unblock()
    {
        Debug.Log("UI is unblocked");
    }

}
