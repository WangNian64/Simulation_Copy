using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseButton : MonoBehaviour {

    // Use this for initialization
    public void Click()
    {
        GameObject Parent = this.transform.parent.transform.parent.gameObject;
        if (Parent.name == "MessageInterface")
        {
            //关闭货物追踪和货物信息面板
            Undo();
        }
        DestroyImmediate(Parent);
    }

    public void Undo()
    {
        string CargoName = this.transform.parent.transform.Find("Item1").transform.Find("Value").GetComponent<Text>().text;
        GameObject Cargo = (GameObject)Resources.Load("Scene/Simulation/Cargo");
        Material[] Material = Cargo.GetComponent<Renderer>().sharedMaterials;
        GameObject.Find(CargoName).GetComponent<Renderer>().sharedMaterials = Material;
        GlobalVariable.FollowState = false;
    }
}
