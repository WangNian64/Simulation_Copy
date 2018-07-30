using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//对应出库按钮
public class CargoExitButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CargoExit()
    {
        //Item1在货物信息面板里
        string CargoName = this.transform.parent.transform.Find("Item1").transform.Find("Value").GetComponent<Text>().text;
        GameObject Cargo = GameObject.Find(CargoName);
        
        CargoMessage CM = Cargo.GetComponent<ShowCargoInfo>().Cargomessage;
        int HighBayNum = CM.PositionInfo.HighBayNum; int FloorNum = CM.PositionInfo.FloorNum;
        int ColumnNum = CM.PositionInfo.ColumnNum; Place PlaceNum = CM.PositionInfo.place;
        StorageBinState state = StorageBinState.InStore;
        switch (PlaceNum)
        {
            case Place.A:
                state = GlobalVariable.BinState[HighBayNum - 1, FloorNum - 1, ColumnNum - 1, 0];// = GlobalVariable.StorageBinState.Stay2Exit;
                //NumofPlace = 0;
                break;
            case Place.B:
                state = GlobalVariable.BinState[HighBayNum - 1, FloorNum - 1, ColumnNum - 1, 1];// = GlobalVariable.StorageBinState.Stay2Exit;
                //NumofPlace = 1;
                break;
        }

        //已经入库
        if (state == StorageBinState.Stored)
        {
            string BinName = "StorageStateInterface/MainBody/Scroll View/Viewport/Content/ShelfPanel" + HighBayNum.ToString();
            BinName = BinName + "/Scroll View/Viewport/Content/Panel/BinsPanel/FloorItem" + FloorNum.ToString();
            BinName = BinName + "/" + PlaceNum + "Panel/Bin_" + CargoName;
            //GlobalVariable.ExitCargosList.Add(Cargo);//出库列表增加该货物
            //GlobalVariable.TempQueue.Enqueue(Cargo);//临时队列增加该货物
            GlobalVariable.ConveyorQueue[(HighBayNum + 1) / 2 - 1].Enqueue(Cargo);//出库货物加入队列
            //GlobalVariable.ExitQueue[(HighBayNum + 1) / 2 - 1].Enqueue(Cargo);
            GlobalVariable.ConveyorDirections[(HighBayNum + 1) / 2 - 1] = Direction.Exit;//输送线方向改为Exit（出库优先）
            GameObject.Find(BinName).GetComponent<Image>().color = GlobalVariable.BinColor[4];
            Cargo.GetComponent<OperatingState>().state = CargoState.WaitOut;

            //进程列表添加一个出货的货物
            GameObject Item = Instantiate((GameObject)Resources.Load("Scene/Simulation/Item"));
            Item.name = Cargo.name;
            Item.transform.Find("Name").GetComponent<Text>().text = Item.name;
            Item.transform.Find("State").GetComponent<Text>().text = "货物状态：" + "等待出库";
            Item.transform.parent = GameObject.Find("ProcessInterface/MainBody/Scroll View/Viewport/Content").transform;
            GlobalVariable.ConveyorDirections[HighBayNum] = Direction.Exit;
            Debug.Log("该货物即将出库！");
        }
        else if (state == StorageBinState.Stay2Exit)
        {
            Debug.Log("该货物已经准备出库！");
        }
        else if (state == StorageBinState.OutStore)
        {
            Debug.Log("该货物正在出库！");
        }
        else if (state == StorageBinState.NotStored)
        {
            Debug.Log("该货物已经出库！");
        }
    }
}
