using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using UnityEngine.UI;

public class Main : MonoBehaviour
{

    public float Speed;
    private bool[] finish1;
    public bool AccomplishValue;
    private string CargoName;
    private GameObject[] Cargo;
    private GameObject[] Piler;
    private int PilerNums;//Piler的总数目
    private int PilerNum;
    private bool[] KeyFrame;
    
    //初始化所有设备的状态信息
    private void initializeEquipState()
    {
        //赋值
        //双向输送线
        for (int i = 0; i < PilerNums; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                
                BiConveyorState biConveyorState = new BiConveyorState();
                biConveyorState.kind = GlobalVariable.biConveyorName;
                biConveyorState.index = (i + 1) + "_" + (j + 1);
                biConveyorState.workState = State.Off;
                biConveyorState.facilityState = FacilityState.Normal;
                biConveyorState.deliverDirection = new Vector3(0, 0, 1);
                biConveyorState.deliverSpeed = Speed;
                biConveyorState.isExcusive = Exclusive.No;
                GameObject.Find(biConveyorState.kind + biConveyorState.index).GetComponent<ShowEquipState>().equipmentState = biConveyorState;
            }
        }
        //单向输送线
        for (int i = 0; i < PilerNums + 4; i++)
        {
            UniConveyorState uniConveyorState = new UniConveyorState();
            uniConveyorState.kind = GlobalVariable.uniConveyorName;
            uniConveyorState.index = (i + 1) + "";
            uniConveyorState.workState = State.Off;
            if (i == 0)//初始化第一个设备ON
            {
                uniConveyorState.workState = State.On;
            }
            uniConveyorState.facilityState = FacilityState.Normal;

            //退出的三个输送线
            if (i<PilerNums + 1)
            {
                uniConveyorState.deliverDirection = new Vector3(0, 0, 1);
            } else
            {
                uniConveyorState.deliverDirection = new Vector3(0, 0, 1);
            }
            uniConveyorState.deliverSpeed = Speed;
            uniConveyorState.isExcusive = Exclusive.No;
            GameObject.Find(uniConveyorState.kind + uniConveyorState.index).GetComponent<ShowEquipState>().equipmentState = uniConveyorState;
        }
        //顶升移载机
        for (int i = 0; i < PilerNums + 1; i++)
        {
            LiftTransferState liftTransferState = new LiftTransferState();
            liftTransferState.kind = GlobalVariable.liftTransferName;
            liftTransferState.index = (i + 1) + "";
            liftTransferState.workState = State.Off;
            liftTransferState.facilityState = FacilityState.Normal;
            liftTransferState.deliverDirection = new Vector3(-1, 0, 0);
            liftTransferState.deliverSpeed = Speed;
            liftTransferState.isExcusive = Exclusive.No;
            //顶升部分
            LiftPartState liftPartState = new LiftPartState();
            liftPartState.kind = GlobalVariable.liftPartName;
            liftPartState.index = (i + 1) + "";
            liftPartState.workState = State.Off;
            liftPartState.facilityState = FacilityState.Normal;
            liftPartState.deliverDirection = new Vector3(0, 1, 0);//默认抬升
            liftPartState.deliverSpeed = Speed / 30;//顶升的抬升速度是1/30
            liftPartState.isExcusive = Exclusive.Yes;
            liftPartState.liftPattern = LiftPattern.up;
            GameObject.Find(liftTransferState.kind + liftTransferState.index).GetComponent<ShowEquipState>().equipmentState = liftTransferState;
            GameObject.Find(liftPartState.kind + liftPartState.index)
                .GetComponent<ShowEquipState>().equipmentState = liftPartState;
        }
    }
    private void Awake()
    {
        //摄像机参数设置
        GameObject.Find("Main Camera").AddComponent<InitCamera>();
        //速度
        Speed = 1.5f;

        //全局变量初始化操作
        #region 
        GlobalVariable.KPD = this.GetComponent<ShowKeyPositionData>().KeyPositionsData;
        int CargosNum = GlobalVariable.KPD.HighBaysNum * GlobalVariable.KPD.StorePositions.StoreColumnPositions.Length * GlobalVariable.KPD.StorePositions.StoreFloorPositions.Length * 2;
        GlobalVariable.TempQueue = new Queue<GameObject>();
        int HighBayNum = GlobalVariable.KPD.HighBaysNum; int ColumnNum = GlobalVariable.KPD.StorePositions.StoreColumnPositions.Length;
        int FloorNum = GlobalVariable.KPD.StorePositions.StoreFloorPositions.Length; int PlaceNum = GlobalVariable.KPD.StorePositions.StorePlacePosition.Length;
        GlobalVariable.BinState = new StorageBinState[HighBayNum, FloorNum, ColumnNum, PlaceNum];
        //GlobalVariable.EnterCargosList = new List<GameObject>();
        GlobalVariable.StoredCargosNameList = new List<string>();
        //GlobalVariable.ExitCargosList = new List<GameObject>();

        GlobalVariable.BinColor = new Color[6];
        GlobalVariable.BinColor[0] = new Color(192f / 255f, 192f / 255f, 192f / 255f, 255f / 255f);
        GlobalVariable.BinColor[1] = new Color(255f / 255f, 255f / 255f, 0f / 255f, 255f / 255f);
        GlobalVariable.BinColor[2] = new Color(255f / 255f, 128f / 255f, 0f / 255f, 255f / 255f);
        GlobalVariable.BinColor[3] = new Color(255f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
        GlobalVariable.BinColor[4] = new Color(128f / 255f, 42f / 255f, 0f / 42f, 255f / 255f);
        GlobalVariable.BinColor[5] = new Color(0f / 255f, 255f / 255f, 0f / 255f, 255f / 255f);
        //GlobalVariable.CargosState = new bool[4, 8];

        //GlobalVariable.CargoStateList = new List<GlobalVariable.CargoState>();
        PilerNums = (GameObject.Find("WarehouseScene").GetComponent<ShowKeyPositionData>().KeyPositionsData.HighBaysNum + 1) / 2;
        GlobalVariable.PilersState = new State[PilerNums];
        GlobalVariable.UnidirectionalConveyorStates = new State[PilerNums + 4, 2];
        GlobalVariable.BidirectionalConveyorStates = new State[PilerNums, 3, 2];
        GlobalVariable.ConveyorDirections = new Direction[PilerNums];
        GlobalVariable.LiftTransferStates = new State[PilerNums + 1];
        GlobalVariable.ConveyorQueue = new Queue<GameObject>[PilerNums];
        GlobalVariable.PilerQueue = new Queue<GameObject>[PilerNums];
        GlobalVariable.Wait = new WaitState[PilerNums];
        GlobalVariable.EnterQueue = new Queue<GameObject>[PilerNums];
        GlobalVariable.ExitQueue = new Queue<GameObject>[PilerNums];
        GlobalVariable.PilerBodyPartPositions = new Vector3[PilerNums];


        //新增
        initializeEquipState();

        #endregion
        //局部变量初始化操作
        #region
        Cargo = new GameObject[PilerNums];
        Piler = new GameObject[PilerNums];
        finish1 = new bool[PilerNums];
        KeyFrame = new bool[PilerNums];
        for (int i = 0; i < PilerNums; i++)
        {
            KeyFrame[i] = false;
            finish1[i] = true;
            string PilerName = "WarehouseScene/PilerGroup/Piler" + (i + 1).ToString();
            Piler[i] = GameObject.Find(PilerName);
            Piler[i].AddComponent<PilerProperty>().PilerState = State.Off;
            Piler[i].GetComponent<PilerProperty>().PilerNum = i + 1;
            Piler[i].GetComponent<PilerProperty>().WorkState = Direction.Enter;
            GlobalVariable.ConveyorQueue[i] = new Queue<GameObject>();
            GlobalVariable.PilerQueue[i] = new Queue<GameObject>();
            GlobalVariable.Wait[i] = WaitState.None;
            GlobalVariable.EnterQueue[i] = new Queue<GameObject>();
            GlobalVariable.ExitQueue[i] = new Queue<GameObject>();
            GlobalVariable.PilersState[i] = State.Off;
            GlobalVariable.UnidirectionalConveyorStates[i, 0] = State.Off; GlobalVariable.UnidirectionalConveyorStates[i, 1] = State.Off;
            GlobalVariable.ConveyorDirections[i] = Direction.Enter;
            GlobalVariable.LiftTransferStates[i] = State.Off;
            GlobalVariable.BidirectionalConveyorStates[i, 0, 0] = State.Off; GlobalVariable.BidirectionalConveyorStates[i, 0, 1] = State.Off;
            GlobalVariable.BidirectionalConveyorStates[i, 1, 0] = State.Off; GlobalVariable.BidirectionalConveyorStates[i, 1, 1] = State.Off;
            GlobalVariable.BidirectionalConveyorStates[i, 2, 0] = State.Off; GlobalVariable.BidirectionalConveyorStates[i, 2, 1] = State.Off;
            GlobalVariable.PilerBodyPartPositions[i] = GameObject.Find("WarehouseScene/PilerGroup").transform.Find("Piler" + (i + 1).ToString()).transform.Find("BodyPart").transform.localPosition;
        }
        GlobalVariable.UnidirectionalConveyorStates[PilerNums, 0] = State.Off; GlobalVariable.UnidirectionalConveyorStates[PilerNums, 1] = State.Off;
        GlobalVariable.UnidirectionalConveyorStates[PilerNums + 1, 0] = State.Off; GlobalVariable.UnidirectionalConveyorStates[PilerNums + 1, 1] = State.Off;
        GlobalVariable.UnidirectionalConveyorStates[PilerNums + 2, 0] = State.Off; GlobalVariable.UnidirectionalConveyorStates[PilerNums + 2, 1] = State.Off;
        GlobalVariable.UnidirectionalConveyorStates[PilerNums + 3, 0] = State.Off; GlobalVariable.UnidirectionalConveyorStates[PilerNums + 3, 1] = State.Off;
        GlobalVariable.LiftTransferStates[PilerNums] = State.Off;
        #endregion

        //事件系统
        GameObject EventSystem = Instantiate((GameObject)Resources.Load("Scene/Simulation/EventSystem"));
        EventSystem.name = "EventSystem";
        //输送线关键位置
        GlobalVariable.UnidirectionalPositions = new Vector3[PilerNums + 4, 2];
        GlobalVariable.BidirectionalPositions = new Vector3[PilerNums, 3, 2];
        GlobalVariable.LiftTransferPositions = new Vector3[PilerNums + 1, 2];
        //GlobalVariable.LiftTransferPositions关键点坐标
        #region
        for (int i = 0; i <= PilerNums; i++)
        {
            GlobalVariable.LiftTransferPositions[i, 0] = GlobalVariable.KPD.EnterPosition;
            GlobalVariable.LiftTransferPositions[i, 0].x = GlobalVariable.KPD.EnterPosition.x - (i + 1) * GlobalVariable.KPD.ConveyorLengths[1]
                - i * GlobalVariable.KPD.ConveyorWidth - GlobalVariable.KPD.ConveyorWidth / 2;
            GlobalVariable.LiftTransferPositions[i, 1] = GlobalVariable.LiftTransferPositions[i, 0];
            GlobalVariable.LiftTransferPositions[i, 1].y = GlobalVariable.KPD.HighValues[0];
        }
        #endregion
        //GlobalVariable.UnidirectionalPositions关键点的坐标
        #region
        //GlobalVariable.UnidirectionalPositions[0] = GlobalVariable.KPD.EnterPosition;
        for (int i = 0; i <= PilerNums; i++)
        {
            GlobalVariable.UnidirectionalPositions[i, 0] = GlobalVariable.KPD.EnterPosition;
            GlobalVariable.UnidirectionalPositions[i, 0].x = GlobalVariable.KPD.EnterPosition.x - GlobalVariable.KPD.CargoSize.x / 2
                - i * (GlobalVariable.KPD.ConveyorLengths[1] + GlobalVariable.KPD.ConveyorWidth);
            GlobalVariable.UnidirectionalPositions[i, 1] = GlobalVariable.UnidirectionalPositions[i, 0];
            GlobalVariable.UnidirectionalPositions[i, 1].x = GlobalVariable.UnidirectionalPositions[i, 0].x
                - (GlobalVariable.KPD.ConveyorLengths[1] - GlobalVariable.KPD.CargoSize.x);
        }
        GlobalVariable.UnidirectionalPositions[PilerNums + 1, 0] = GlobalVariable.LiftTransferPositions[PilerNums, 1];
        GlobalVariable.UnidirectionalPositions[PilerNums + 1, 0].z = GlobalVariable.UnidirectionalPositions[PilerNums, 1].z
            + (GlobalVariable.KPD.ConveyorWidth + GlobalVariable.KPD.CargoSize.z) / 2 + 0.2f;
        GlobalVariable.UnidirectionalPositions[PilerNums + 1, 1] = GlobalVariable.UnidirectionalPositions[PilerNums + 1, 0];
        GlobalVariable.UnidirectionalPositions[PilerNums + 1, 1].z = GlobalVariable.UnidirectionalPositions[PilerNums + 1, 0].z
            + (GlobalVariable.KPD.ConveyorLengths[0] - GlobalVariable.KPD.CargoSize.z - 0.4f);
        GlobalVariable.UnidirectionalPositions[PilerNums + 2, 0] = GlobalVariable.UnidirectionalPositions[PilerNums + 1, 0];
        GlobalVariable.UnidirectionalPositions[PilerNums + 2, 0].z = GlobalVariable.UnidirectionalPositions[PilerNums + 1, 0].z + GlobalVariable.KPD.ConveyorLengths[0];
        GlobalVariable.UnidirectionalPositions[PilerNums + 2, 1] = GlobalVariable.UnidirectionalPositions[PilerNums + 1, 1];
        GlobalVariable.UnidirectionalPositions[PilerNums + 2, 1].z = GlobalVariable.UnidirectionalPositions[PilerNums + 1, 1].z + GlobalVariable.KPD.ConveyorLengths[0];
        GlobalVariable.UnidirectionalPositions[PilerNums + 3, 0] = GlobalVariable.UnidirectionalPositions[PilerNums + 2, 0];
        GlobalVariable.UnidirectionalPositions[PilerNums + 3, 0].z = GlobalVariable.UnidirectionalPositions[PilerNums + 2, 0].z + GlobalVariable.KPD.ConveyorLengths[0];
        GlobalVariable.UnidirectionalPositions[PilerNums + 3, 1] = GlobalVariable.UnidirectionalPositions[PilerNums + 2, 1];
        GlobalVariable.UnidirectionalPositions[PilerNums + 3, 1].z = GlobalVariable.UnidirectionalPositions[PilerNums + 2, 1].z + GlobalVariable.KPD.ConveyorLengths[0];
        #endregion
        //GlobalVariable.BidirectionalPositions关键点坐标
        #region
        for (int i = 0; i < PilerNums; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                GlobalVariable.BidirectionalPositions[i, j, 0] = GlobalVariable.LiftTransferPositions[i, 1];
                GlobalVariable.BidirectionalPositions[i, j, 0].z = GlobalVariable.LiftTransferPositions[i, 1].z
                    - GlobalVariable.KPD.ConveyorWidth / 2 - GlobalVariable.KPD.CargoSize.z / 2 - j * GlobalVariable.KPD.ConveyorLengths[0] - 0.2F;
                GlobalVariable.BidirectionalPositions[i, j, 1] = GlobalVariable.BidirectionalPositions[i, j, 0];
                GlobalVariable.BidirectionalPositions[i, j, 1].z = GlobalVariable.BidirectionalPositions[i, j, 0].z
                    - (GlobalVariable.KPD.ConveyorLengths[0] - GlobalVariable.KPD.CargoSize.z - 0.4F);
            }
        }

        #endregion
    }
    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GlobalVariable.TempQueue.Count > 0)//得到堆垛机编号
        {
            PilerNum = (GlobalVariable.TempQueue.Peek().GetComponent<ShowCargoInfo>().Cargomessage.PositionInfo.HighBayNum + 1) / 2;
        }
        for (int i = 0; i < PilerNums; i++)
        {
            //入库货物情况
            if (PilerNum == i + 1 && finish1[i] == true && GlobalVariable.TempQueue.Count > 0)
            {
                Debug.Log("有货物进入Tempueue");
                Cargo[i] = GlobalVariable.TempQueue.Peek();
                //是要入库的货物
                if (Cargo[i].GetComponent<OperatingState>().state == CargoState.WaitIn)
                {
                    KeyFrame[i] = true;
                    Cargo[i].AddComponent<CargoEnterController>().enabled = false;
                    Cargo[i].GetComponent<OperatingState>().state = CargoState.Enter;//改为正在入库
                    GameObject.Find("ProcessInterface/MainBody/Scroll View/Viewport/Content").transform.Find(Cargo[i].name).transform.Find("State").GetComponent<Text>().text = "货物状态：" + "正在入库";//修改进程面板中该货物状态信息
                    Functions.ChangeColor(Cargo[i].name, StorageBinState.InStore);//颜色修改为“正在入库”
                    finish1[i] = false;
                    //进入排队队列 和 入队队列
                    GlobalVariable.ConveyorQueue[i].Enqueue(Cargo[i]);
                    GlobalVariable.EnterQueue[i].Enqueue(Cargo[i]);
                }
            }
            //入库货物入库动画
            if (GlobalVariable.EnterQueue[i].Count > 0)
            {
                Debug.Log("有货物进入入库队列");
                //遍历所有入库货物
                foreach (GameObject cargo in GlobalVariable.EnterQueue[i])
                {
                    //执行CargoEnter动画
                    cargo.GetComponent<CargoEnterController>().enabled = true;//问题就在这

                    //在货物已经到达单向输送机0时 从tempQueue出队
                    if (GlobalVariable.UnidirectionalConveyorStates[0, 0] == State.On && finish1[i] == false)
                    {
                        //出队
                        GlobalVariable.TempQueue.Dequeue();
                        finish1[i] = true;//本堆垛机不能用了
                    }
                }
            }
        }


        //if (GlobalVariable.TempQueue.Count > 0)//得到堆垛机编号
        //{
        //    PilerNum = (GlobalVariable.TempQueue.Peek().GetComponent<ShowCargoInfo>().Cargomessage.PositionInfo.HighBayNum + 1) / 2;
        //}
        //for (int i = 0; i < PilerNums; i++)
        //{
        //    //入库货物情况
        //    if (PilerNum == i + 1 && finish1[i] == true && GlobalVariable.TempQueue.Count > 0)
        //    {
        //        Cargo[i] = GlobalVariable.TempQueue.Peek();
        //        //是要入库的货物
        //        if (Cargo[i].GetComponent<OperatingState>().state == CargoState.WaitIn)
        //        {
        //            KeyFrame[i] = true;
        //            Cargo[i].AddComponent<CargoEnter>().enabled = false;
        //            Cargo[i].GetComponent<CargoEnter>().Speed = Speed;
        //            Cargo[i].GetComponent<OperatingState>().state = CargoState.Enter;//改为正在入库
        //            GameObject.Find("ProcessInterface/MainBody/Scroll View/Viewport/Content").transform.Find(Cargo[i].name).transform.Find("State").GetComponent<Text>().text = "货物状态：" + "正在入库";//修改进程面板中该货物状态信息
        //            Functions.ChangeColor(Cargo[i].name, StorageBinState.InStore);//颜色修改为“正在入库”
        //            finish1[i] = false;
        //            //进入排队队列 和 入队队列
        //            GlobalVariable.ConveyorQueue[i].Enqueue(Cargo[i]);
        //            GlobalVariable.EnterQueue[i].Enqueue(Cargo[i]);
        //        }
        //    }
        //    //入库货物入库动画
        //    if (GlobalVariable.EnterQueue[i].Count > 0)
        //    {
        //        //遍历所有入库货物
        //        foreach (GameObject cargo in GlobalVariable.EnterQueue[i])
        //        {
        //            //执行CargoEnter动画
        //            cargo.GetComponent<CargoEnter>().enabled = true;
        //            //在货物已经到达单向输送机0时 从tempQueue出队
        //            if (GlobalVariable.UnidirectionalConveyorStates[0, 0] == State.On && finish1[i] == false)
        //            {
        //                //出队
        //                GlobalVariable.TempQueue.Dequeue();
        //                finish1[i] = true;//本堆垛机不能用了
        //            }
        //        }
        //    }
        //    //出库货物出库动画
        //    if (GlobalVariable.ExitQueue[i].Count > 0)
        //    {
        //        //遍历出库队列
        //        foreach (GameObject cargo in GlobalVariable.ExitQueue[i])
        //        {
        //            //执行出库动画
        //            cargo.GetComponent<CargoExit>().enabled = true;
        //        }
        //    }
        //}
        ////堆垛机动画
        //foreach (GameObject piler in Piler)//遍历所有的堆垛机
        //{
        //    int Num = piler.GetComponent<PilerProperty>().PilerNum - 1;//堆垛机编号
        //    //添加堆垛机动画脚本   
        //    //当前堆垛机空闲 且 有货物在排队（不论是入库还是出库）
        //    if (GlobalVariable.PilersState[Num] == State.Off && GlobalVariable.ConveyorQueue[Num].Count > 0)
        //    {
        //        //拿出一个货物
        //        GameObject cargo0 = GlobalVariable.ConveyorQueue[Num].Peek();
        //        //添加堆垛机入库动画脚本
        //        //货物正在入库 且 堆垛机 
        //        if (cargo0.GetComponent<OperatingState>().state == CargoState.Enter && GlobalVariable.Wait[Num] == WaitState.WaitEnter)
        //        {
        //            //给堆垛机添加入库动画
        //            piler.AddComponent<PilerOfEnter>().enabled = false;
        //            piler.GetComponent<PilerOfEnter>().Speed = Speed;
        //            piler.GetComponent<PilerOfEnter>().Cargo = cargo0;
        //            piler.GetComponent<PilerOfEnter>().PilerNum = Num;
        //            GlobalVariable.PilersState[Num] = State.On;//堆垛机打开
        //            piler.GetComponent<PilerProperty>().PilerState = State.On;
        //            piler.GetComponent<PilerProperty>().WorkState = Direction.Enter;
        //        }
        //        //添加堆垛机出库动画脚本
        //        if (cargo0.GetComponent<OperatingState>().state == CargoState.WaitOut && GlobalVariable.Wait[Num] == WaitState.None)
        //        {
        //            piler.AddComponent<PilerOfExit>().enabled = false;
        //            piler.GetComponent<PilerOfExit>().Speed = Speed;
        //            piler.GetComponent<PilerOfExit>().Cargo = GlobalVariable.ConveyorQueue[Num].Peek();
        //            piler.GetComponent<PilerOfExit>().PilerNum = Num;
        //            GlobalVariable.PilersState[Num] = State.On;
        //            piler.GetComponent<PilerProperty>().PilerState = State.On;
        //            piler.GetComponent<PilerProperty>().WorkState = Direction.Exit;
        //            GlobalVariable.BidirectionalConveyorStates[Num, 2, 1] = State.On;
        //        }
        //    }
        //    //执行动画脚本
        //    if (GlobalVariable.PilersState[Num] == State.On)
        //    {
        //        //入库脚本
        //        if (piler.GetComponent<PilerProperty>().WorkState == Direction.Enter)
        //        {
        //            piler.GetComponent<PilerOfEnter>().enabled = true;
        //        }
        //        //出库脚本
        //        if (piler.GetComponent<PilerProperty>().WorkState == Direction.Exit)
        //        {
        //            piler.GetComponent<PilerOfExit>().enabled = true;
        //        }
        //    }
        //}
    }
}
