using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Functions : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //修改货物对应的面板的颜色（不同状态）
    public static void ChangeColor(string CargoName, StorageBinState state)
    {
        CargoMessage CM = GameObject.Find(CargoName).GetComponent<ShowCargoInfo>().Cargomessage;
        int HighBayNum = CM.PositionInfo.HighBayNum; int FloorNum = CM.PositionInfo.FloorNum;
        int ColumnNum = CM.PositionInfo.ColumnNum; Place PlaceNum = CM.PositionInfo.place;

        string BinName = "StorageStateInterface/MainBody/Scroll View/Viewport/Content/ShelfPanel" + HighBayNum.ToString();
        BinName = BinName + "/Scroll View/Viewport/Content/Panel/BinsPanel/FloorItem" + FloorNum.ToString();
        BinName = BinName + "/" + PlaceNum.ToString() + "Panel/Bin_" + CargoName;
        switch (PlaceNum)
        {
            case Place.A:
                GlobalVariable.BinState[HighBayNum - 1, FloorNum - 1, ColumnNum - 1, 0] = state;
                break;
            case Place.B:
                GlobalVariable.BinState[HighBayNum - 1, FloorNum - 1, ColumnNum - 1, 1] = state;
                break;
        }
        switch (state)
        {
            case StorageBinState.NotStored:
                GameObject.Find(BinName).GetComponent<Image>().color = GlobalVariable.BinColor[0];
                break;
            case StorageBinState.Reserved:
                GameObject.Find(BinName).GetComponent<Image>().color = GlobalVariable.BinColor[1];
                break;
            case StorageBinState.InStore:
                GameObject.Find(BinName).GetComponent<Image>().color = GlobalVariable.BinColor[2];
                break;
            case StorageBinState.Stored:
                GameObject.Find(BinName).GetComponent<Image>().color = GlobalVariable.BinColor[3];
                break;
            case StorageBinState.Stay2Exit:
                GameObject.Find(BinName).GetComponent<Image>().color = GlobalVariable.BinColor[4];
                break;
            case StorageBinState.OutStore:
                GameObject.Find(BinName).GetComponent<Image>().color = GlobalVariable.BinColor[5];
                break;
        }
    }

    //接口
    //static void Connector(GameObject Cargo)
    //{
    //    string name0 = Cargo.name;
    //    string HighBayNum;
    //    string FloorNum;
    //    string ColumnNum;
    //    string PlaceNum;
    //    int HighBayNum2 = int.Parse(HighBayNum);
    //    int FloorNum2 = int.Parse(FloorNum);
    //    int ColumnNum = int.Parse(ColumnNum);
    //    string CargoName = "Cargo" + "_" + HighBayNum + "_" + FloorNum + "_" + ColumnNum + "_" + PlaceNum;
    //    GlobalVariable.EnterCargosList.Add(Cargo);
    //    GlobalVariable.EnterCargosNameList.Add(CargoName);
    //    GlobalVariable.TempQueue.Enqueue(Cargo);
    //    //GlobalVariable.CargoState state = GlobalVariable.CargoState.Wait;
    //    //GlobalVariable.CargoStateList.Add(GlobalVariable.CargoState.Wait);
    //    string BinName = "StorageStateInterface/MainBody/Scroll View/Viewport/Content/ShelfPanel" + HighBayNum;
    //    BinName = BinName + "/Scroll View/Viewport/Content/Panel/BinsPanel/FloorItem" + FloorNum;
    //    BinName = BinName + "/" + PlaceNum + "Panel/Bin_" + CargoName;
    //    GameObject.Find(BinName).GetComponent<Image>().color = GlobalVariable.BinColor[1];
    //    switch (PlaceNum)
    //    {
    //        case "A":
    //            GlobalVariable.BinState[HighBayNum2 - 1, FloorNum2 - 1, ColumnNum2 - 1, 0] = StorageBinState.Reserved;
    //            break;
    //        case "B":
    //            GlobalVariable.BinState[HighBayNum2 - 1, FloorNum2 - 1, ColumnNum2 - 1, 1] = StorageBinState.Reserved;
    //            break;
    //    }

    //    GameObject Item = Instantiate((GameObject)Resources.Load("Scene/Simulation/Item"));
    //    Item.name = Cargo.name;
    //    Item.transform.Find("Name").GetComponent<Text>().text = Item.name;
    //    Item.transform.Find("State").GetComponent<Text>().text = "货物状态：" + "等待入库";
    //    Item.transform.parent = GameObject.Find("ProcessInterface/MainBody/Scroll View/Viewport/Content").transform;
    //}
}
