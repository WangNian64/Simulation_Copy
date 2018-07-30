using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainInterfaceButton : MonoBehaviour {

    public bool ProcessState;
    GameObject ProcessInterface;
    public bool StorageState;
    GameObject StorageStateInterface;
    private Vector3 ProcessPosition1;
    private Vector3 ProcessPosition2;
    private Vector3 StoragePosition1;
    private Vector3 StoragePosition2;
    private GameObject CreateInterface1;//防止重复

    private void Awake()
    {
        CreateInterface1 = GameObject.Find("CreateInterface");
    }
    private void Start()
    {
        ProcessState = true;
        ProcessInterface = GameObject.Find("ProcessInterface");
        ProcessPosition1 = ProcessInterface.transform.Find("MainBody").GetComponent<RectTransform>().position;
        ProcessPosition2 = ProcessPosition1; ProcessPosition2.x = 100f;

        //Instantiate(ProcessInterface);
        //ProcessInterface.SetActive(false);
        StorageState = true;
        StorageStateInterface = GameObject.Find("StorageStateInterface");
        StoragePosition1 = StorageStateInterface.transform.Find("MainBody").GetComponent<RectTransform>().position;
        StoragePosition2 = StoragePosition1; StoragePosition2.x = 5f;
        //StorageStateInterface.SetActive(false);
    }


    public void CreateClick()
    {
        CreateInterface1 = GameObject.Find("CreateInterface");
        if (CreateInterface1 != null)
        {
            Debug.Log("重复！");
            return;
        }
        GameObject CreateInterface = Instantiate((GameObject)Resources.Load("Scene/Simulation/CreateInterface"));
        CreateInterface.name = "CreateInterface";
    }

    public void ListClick()
    {
        switch (ProcessState)
        {
            case false:
                //ProcessInterface.SetActive(true);
                ProcessInterface.transform.Find("MainBody").GetComponent<RectTransform>().position = ProcessPosition1;
                ProcessState = true;
                break;
            case true:
                //ProcessInterface.SetActive(false);
                ProcessInterface.transform.Find("MainBody").GetComponent<RectTransform>().position = ProcessPosition2;
                ProcessState = false;
                break;
        }
    }

    public void StorageClick()
    {
        switch (StorageState)
        {
            case false:
                //StorageStateInterface.SetActive(true);
                StorageStateInterface.transform.Find("MainBody").GetComponent<RectTransform>().position = StoragePosition1;
                StorageState = true;
                break;
            case true:
                //StorageStateInterface.SetActive(false);
                StorageStateInterface.transform.Find("MainBody").GetComponent<RectTransform>().position = StoragePosition2;
                StorageState = false;
                break;
        }
    }
    

    
}
