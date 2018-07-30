using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CargoMessageClose : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Click()
    {
        GameObject Cargo = (GameObject)Resources.Load("Scene/Simulation/Cargo");
        Material[] Material = Cargo.GetComponent<Renderer>().sharedMaterials;
        string CargoName = GameObject.Find("CargoMessageInterface").transform.Find("Panel").transform.Find("Item1").transform.Find("Value").GetComponent<Text>().text;
        GameObject.Find(CargoName).GetComponent<Renderer>().sharedMaterials = Material;
        DestroyImmediate(GameObject.Find("CargoMessageInterface"));
        GlobalVariable.FollowState = false;
    }
}
