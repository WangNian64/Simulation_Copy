using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class CreateButton : MonoBehaviour {

    public Place place1;
    // Use this for initialization
    public void Click()
    {
        string HighBayNum = GameObject.Find("HighBayNum").transform.Find("InputField").GetComponent<InputField>().text;
        string FloorNum = GameObject.Find("FloorNum").transform.Find("InputField").GetComponent<InputField>().text;
        string ColumnNum = GameObject.Find("ColumnNum").transform.Find("InputField").GetComponent<InputField>().text;
        string PlaceNum = GameObject.Find("PlaceNum").transform.Find("InputField").GetComponent<InputField>().text;
        string CargoNum = GameObject.Find("CargoNum").transform.Find("InputField").GetComponent<InputField>().text;
        string EnterTime = GameObject.Find("EnterTime").transform.Find("InputField").GetComponent<InputField>().text;
        string CargoDescription = GameObject.Find("CargoDescription").transform.Find("InputField").GetComponent<InputField>().text;
        bool condition = (HighBayNum != null) && (FloorNum != null) && (ColumnNum != null) && (PlaceNum != null) && (CargoNum != null) && (EnterTime != null) && (CargoDescription != null);
        if (condition)
        {
            int HighBayNum2 = int.Parse(HighBayNum);//库号
            int FloorNum2 = int.Parse(FloorNum);//层号
            int ColumnNum2 = int.Parse(ColumnNum);//列号
            //Place place1;
            switch (PlaceNum)
            {
                case "A":
                    place1 = Place.A;
                    break;
                case "B":
                    place1 = Place.B;
                    break;
                default:
                    GameObject.Find("Notice").GetComponent<Text>().text = "请按要求输入A或B!";
                    break;
            }
            int Num1 = GlobalVariable.KPD.HighBaysNum;
            int Num2 = GlobalVariable.KPD.StorePositions.StoreFloorPositions.Length;
            int Num3 = GlobalVariable.KPD.StorePositions.StoreColumnPositions.Length;
            bool condition1 = (HighBayNum2 > 0 && HighBayNum2 <= Num1) && (FloorNum2 > 0 && FloorNum2 <= Num2) && (ColumnNum2 > 0 && ColumnNum2 < Num3);
            string Sy = "_";
            string CargoName = "Cargo" + Sy + HighBayNum + Sy + FloorNum + Sy + ColumnNum + Sy + PlaceNum;
            if (condition1)
            {
                if (!GlobalVariable.StoredCargosNameList.Contains(CargoName))
                {
                    //生成cargo并添加cargoMessage
                    GameObject Cargo = Instantiate((GameObject)Resources.Load("Scene/Simulation/Cargo"));
                    Cargo.name = CargoName;
                    CargoMessage CI = new CargoMessage();
                    CI.Name = Cargo.name;
                    CI.Size = GlobalVariable.KPD.CargoSize;
                    CI.Number1 = CargoNum;
                    CI.Num = 5;
                    CI.InputTime = EnterTime;
                    CI.Description = CargoDescription;
                    CI.PositionInfo.HighBayNum = HighBayNum2;
                    CI.PositionInfo.ColumnNum = ColumnNum2;
                    CI.PositionInfo.FloorNum = FloorNum2;
                    CI.PositionInfo.place = place1;
                    //添加货物的设备队列
                    CI.EquipmentsQueue = new Queue<GameObject>();

                    //入口要加一些设备
                    for (int i=0;i<(HighBayNum2 + 1) / 2; i++)
                    {
                        CI.EquipmentsQueue.Enqueue(GameObject.Find("UniConveyor" + (i + 1)));
                        CI.EquipmentsQueue.Enqueue(GameObject.Find("LiftTransfer" + (i + 1)));
                    }
                    //顶升部分
                    CI.EquipmentsQueue.Enqueue(GameObject.Find("LiftPart" + (HighBayNum2 + 1) / 2));
                    for (int j = 0; j < 3; j++)
                    {
                        CI.EquipmentsQueue.Enqueue(GameObject.Find("BiConveyor"+ (HighBayNum2 + 1) / 2+ "_" + j));
                    }

                    Cargo.AddComponent<ShowCargoInfo>().Cargomessage = CI;
                    Cargo.AddComponent<OperatingState>().state = CargoState.WaitIn;//开始状态是等待进入
                    //Cargo.transform.parent = GameObject.Find("WarehouseScene").transform;
                    Cargo.transform.parent = CI.EquipmentsQueue.Peek().transform;//货物产生后成为第一个设备的子物体
                    //Cargo.transform.localPosition = GlobalVariable.KPD.EnterPosition;//放到开始位置

                    //货物的初始位置
                    Cargo.transform.localPosition = GlobalVariable.KPD.CargoEnterPosition;


                    GameObject.Find("Notice").GetComponent<Text>().text = "创建成功！";

                    GlobalVariable.TempQueue.Enqueue(Cargo);//放入TempQueue队列

                    //binName是对应货物面板的一个格子
                    string BinName = "StorageStateInterface/MainBody/Scroll View/Viewport/Content/ShelfPanel" + HighBayNum2.ToString();
                    BinName = BinName + "/Scroll View/Viewport/Content/Panel/BinsPanel/FloorItem" + FloorNum2.ToString();
                    BinName = BinName + "/" + PlaceNum + "Panel/Bin_" + CargoName;
                    GameObject.Find(BinName).GetComponent<Image>().color = GlobalVariable.BinColor[1];
                    switch (PlaceNum)
                    {
                        case "A":
                            GlobalVariable.BinState[HighBayNum2 - 1, FloorNum2 - 1, ColumnNum2 - 1, 0] = StorageBinState.Reserved;
                            break;
                        case "B":
                            GlobalVariable.BinState[HighBayNum2 - 1, FloorNum2 - 1, ColumnNum2 - 1, 1] = StorageBinState.Reserved;
                            break;
                    }
                    //更新进程列表（多一项），初始状态是“等待入库”
                    GameObject Item = Instantiate((GameObject)Resources.Load("Scene/Simulation/Item"));
                    Item.name = Cargo.name;
                    Item.transform.Find("Name").GetComponent<Text>().text = Item.name;
                    Item.transform.Find("State").GetComponent<Text>().text = "货物状态：" + "等待入库";
                    Item.transform.parent = GameObject.Find("ProcessInterface/MainBody/Scroll View/Viewport/Content").transform;
                }
                else if(GlobalVariable.StoredCargosNameList.Contains(CargoName))
                {
                    GameObject.Find("Notice").GetComponent<Text>().text = "该货物已存在！";
                }
            }
            
        }
    }
}
