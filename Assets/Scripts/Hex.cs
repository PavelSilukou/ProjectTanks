using UnityEngine;
using System.Collections;

public class Hex : MonoBehaviour {

    public Hex[] neighbors = new Hex[6];
    //public Hex[] neighborsL2 = new Hex[6];
    
    public int X;
    public int Y;

    public bool isMove = true;
    public bool isShoot = true;

    public Color defaultColor = Color.white;
    public Color availablePathColor = Color.blue;
    public Color movePathColor = Color.red;

    public void SetAvailablePathColor ()
    {
        gameObject.GetComponent<Renderer>().material.color = this.availablePathColor;
    }

    public void SetDefaultColor ()
    {
        gameObject.GetComponent<Renderer>().material.color = this.defaultColor;
    }

    public void SetMovePathColor ()
    {
        gameObject.GetComponent<Renderer>().material.color = this.movePathColor;
    }
}
