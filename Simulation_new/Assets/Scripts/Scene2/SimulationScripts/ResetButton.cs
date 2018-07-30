using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetButton : MonoBehaviour {

    public Place place1;
    // Use this for initialization
    public void Click()
    {
        GameObject.Find("HighBayNum").transform.Find("InputField").GetComponent<InputField>().text = null;
        GameObject.Find("FloorNum").transform.Find("InputField").GetComponent<InputField>().text = null;
        GameObject.Find("ColumnNum").transform.Find("InputField").GetComponent<InputField>().text = null;
        GameObject.Find("PlaceNum").transform.Find("InputField").GetComponent<InputField>().text = null;
        GameObject.Find("CargoNum").transform.Find("InputField").GetComponent<InputField>().text = null;
        GameObject.Find("EnterTime").transform.Find("InputField").GetComponent<InputField>().text = null;
        string CargoDescription = GameObject.Find("CargoDescription").transform.Find("InputField").GetComponent<InputField>().text;
    }
}
