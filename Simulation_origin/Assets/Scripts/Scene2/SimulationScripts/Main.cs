using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using UnityEngine.UI;

public class Main : MonoBehaviour
{

    public float Speed;
    private bool[] finish1;
    private bool[] finish2;
    public bool AccomplishValue;
    private string CargoName;
    private GameObject[] Cargo;
    private GameObject[] Piler;
    private int PilerNums;//Piler的总数目
    private int PilerNum;
    private float Times1;//没用到
    private bool[] KeyFrame;
    private void Awake()
    {
        //摄像机参数设置
        GameObject.Find("Main Camera").AddComponent<InitCamera>();
        //参数计算
        Speed = 1.5f;

        //全局变量初始化操作
        #region 
        GlobalVariable.KPD = this.GetComponent<ShowKeyPositionData>().KeyPositionsData;
        int CargosNum = GlobalVariable.KPD.HighBaysNum * GlobalVariable.KPD.StorePositions.StoreColumnPositions.Length * GlobalVariable.KPD.StorePositions.StoreFloorPositions.Length * 2;
        GlobalVariable.TempQueue = new Queue<GameObject>();
        int HighBayNum = GlobalVariable.KPD.HighBaysNum; int ColumnNum = GlobalVariable.KPD.StorePositions.StoreColumnPositions.Length;
        int FloorNum = GlobalVariable.KPD.StorePositions.StoreFloorPositions.Length; int PlaceNum = GlobalVariable.KPD.StorePositions.StorePlacePosition.Length;
        GlobalVariable.BinState = new StorageBinState[HighBayNum, FloorNum, ColumnNum, PlaceNum];
        GlobalVariable.StoredCargosNameList = new List<string>();
        GlobalVariable.TempQueue = new Queue<GameObject>();
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
        //单向9个
        GlobalVariable.UnidirectionalConveyorStates = new State[PilerNums + 5, 2];
        GlobalVariable.BidirectionalConveyorStates = new State[PilerNums, 3, 2];
        GlobalVariable.ConveyorDirections = new Direction[PilerNums];
        //顶升增多1个
        GlobalVariable.LiftTransferStates = new State[PilerNums + 2];
        GlobalVariable.ConveyorQueue = new Queue<GameObject>[PilerNums];
        GlobalVariable.PilerQueue = new Queue<GameObject>[PilerNums];
        GlobalVariable.Wait = new WaitState[PilerNums];
        GlobalVariable.EnterQueue = new Queue<GameObject>[PilerNums];
        GlobalVariable.ExitQueue = new Queue<GameObject>[PilerNums];
        GlobalVariable.PilerBodyPartPositions = new Vector3[PilerNums];
        #endregion
        //局部变量初始化操作
        #region
        Cargo = new GameObject[PilerNums];
        Piler = new GameObject[PilerNums];
        finish1 = new bool[PilerNums];
        finish2 = new bool[PilerNums];
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
            
            GlobalVariable.ConveyorDirections[i] = Direction.Enter;
            GlobalVariable.LiftTransferStates[i] = State.Off;
            GlobalVariable.BidirectionalConveyorStates[i, 0, 0] = State.Off; GlobalVariable.BidirectionalConveyorStates[i, 0, 1] = State.Off;
            GlobalVariable.BidirectionalConveyorStates[i, 1, 0] = State.Off; GlobalVariable.BidirectionalConveyorStates[i, 1, 1] = State.Off;
            GlobalVariable.BidirectionalConveyorStates[i, 2, 0] = State.Off; GlobalVariable.BidirectionalConveyorStates[i, 2, 1] = State.Off;
            GlobalVariable.PilerBodyPartPositions[i] = GameObject.Find("WarehouseScene/PilerGroup").transform.Find("Piler" + (i + 1).ToString()).transform.Find("BodyPart").transform.localPosition;
        }
        for (int i=0;i<PilerNums + 5; i++)
        {
            GlobalVariable.UnidirectionalConveyorStates[i, 0] = State.Off; GlobalVariable.UnidirectionalConveyorStates[i, 1] = State.Off;
        }
        GlobalVariable.LiftTransferStates[PilerNums] = State.Off;
        GlobalVariable.LiftTransferStates[PilerNums + 1] = State.Off;
        #endregion

        //事件系统
        GameObject EventSystem = Instantiate((GameObject)Resources.Load("Scene/Simulation/EventSystem"));
        EventSystem.name = "EventSystem";
        //输送线关键位置
        int UniCSum, BiCSum, LiftSum;
        UniCSum = PilerNums + 5;
        LiftSum = PilerNums + 2;
        GlobalVariable.UnidirectionalPositions = new Vector3[UniCSum, 2];
        GlobalVariable.BidirectionalPositions = new Vector3[PilerNums, 3, 2];
        GlobalVariable.LiftTransferPositions = new Vector3[LiftSum, 2];
        //GlobalVariable.LiftTransferPositions关键点坐标
        #region
        //先计算第一个liftTransfer的坐标
        Vector3 liftPos0 = GlobalVariable.KPD.EnterPosition;
        liftPos0.z = liftPos0.z - GlobalVariable.KPD.ConveyorLengths[0] * 2 - GlobalVariable.KPD.ConveyorWidth / 2;
        liftPos0.y = GlobalVariable.KPD.HighValues[1];//顶升高度
        GlobalVariable.LiftTransferPositions[0, 0] = liftPos0;

        GlobalVariable.LiftTransferPositions[0, 1] = liftPos0;
        GlobalVariable.LiftTransferPositions[0, 1].y = GlobalVariable.KPD.HighValues[0];//高度不一样
        

        float liftTempX = liftPos0.x;
        //其他liftTransfer的坐标(从第二个开始)
        for (int i = 1; i < LiftSum; i++)
        {
            GlobalVariable.LiftTransferPositions[i, 0] = liftPos0;
            if (i != PilerNums + 1)
            {
                GlobalVariable.LiftTransferPositions[i, 0].x = liftPos0.x - GlobalVariable.KPD.EnterRCP_Length
                    - (i - 1) * GlobalVariable.KPD.ConveyorLengths[1] - i * GlobalVariable.KPD.ConveyorWidth;
            } else//最后一个不一样
            {
                GlobalVariable.LiftTransferPositions[i, 0].x = liftPos0.x - GlobalVariable.KPD.EnterRCP_Length
                    - (i - 2) * GlobalVariable.KPD.ConveyorLengths[1] - i * GlobalVariable.KPD.ConveyorWidth
                    - GlobalVariable.KPD.ExitRCP_Length;
            }
            GlobalVariable.LiftTransferPositions[i, 1] = GlobalVariable.LiftTransferPositions[i, 0];
            GlobalVariable.LiftTransferPositions[i, 1].y = GlobalVariable.KPD.HighValues[0];//高度不一样
        }
        #endregion
        //GlobalVariable.UnidirectionalPositions关键点的坐标
        #region
        //GlobalVariable.UnidirectionalPositions[0] = GlobalVariable.KPD.EnterPosition;
        //入口2个皮带输送机
        for (int i = 0; i < 2; i++)
        {
            GlobalVariable.UnidirectionalPositions[i, 0] = GlobalVariable.KPD.EnterPosition;
            GlobalVariable.UnidirectionalPositions[i, 0].z = GlobalVariable.KPD.EnterPosition.z - GlobalVariable.KPD.CargoSize.z / 2
                - i * GlobalVariable.KPD.ConveyorLengths[0];
            GlobalVariable.UnidirectionalPositions[i, 1] = GlobalVariable.KPD.EnterPosition;
            GlobalVariable.UnidirectionalPositions[i, 1].z = GlobalVariable.UnidirectionalPositions[i,0].z
                - (GlobalVariable.KPD.ConveyorLengths[0] - GlobalVariable.KPD.CargoSize.z);
        }
        //中间的滚筒输送机
        Vector3 RCPos0 = liftPos0;//第一个滚筒的位置
        RCPos0.x = RCPos0.x - GlobalVariable.KPD.ConveyorWidth / 2 - GlobalVariable.KPD.CargoSize.x / 2;
        GlobalVariable.UnidirectionalPositions[2, 0] = RCPos0;
        GlobalVariable.UnidirectionalPositions[2, 1] = RCPos0;
        GlobalVariable.UnidirectionalPositions[2, 1].x = GlobalVariable.UnidirectionalPositions[2, 0].x
                - (GlobalVariable.KPD.EnterRCP_Length - GlobalVariable.KPD.CargoSize.x);
        //其他滚筒
        for (int i = 3; i < PilerNums + 3; i++)
        {
            GlobalVariable.UnidirectionalPositions[i, 0] = RCPos0;
            GlobalVariable.UnidirectionalPositions[i, 0].x = RCPos0.x - GlobalVariable.KPD.EnterRCP_Length
                - (i - 3) * GlobalVariable.KPD.ConveyorLengths[1] - (i - 2) * GlobalVariable.KPD.ConveyorWidth;
            GlobalVariable.UnidirectionalPositions[i, 1] = GlobalVariable.UnidirectionalPositions[i, 0];
            if (i != PilerNums + 2)
            {
                GlobalVariable.UnidirectionalPositions[i, 1].x = GlobalVariable.UnidirectionalPositions[i, 0].x
                    - (GlobalVariable.KPD.ConveyorLengths[1] - GlobalVariable.KPD.CargoSize.x);
            } else {//最后一个滚筒不一样
                GlobalVariable.UnidirectionalPositions[i, 1].x = GlobalVariable.UnidirectionalPositions[i, 0].x
                    - (GlobalVariable.KPD.ExitRCP_Length - GlobalVariable.KPD.CargoSize.x);
            }
        }
        //出库的皮带输送机
        for (int i = PilerNums + 3; i < UniCSum; i++)
        {
            //以最后一个顶升的位置作为参考
            GlobalVariable.UnidirectionalPositions[i, 0] = GlobalVariable.LiftTransferPositions[LiftSum - 1, 1];
            GlobalVariable.UnidirectionalPositions[i, 0].z = GlobalVariable.UnidirectionalPositions[i, 0].z +
                GlobalVariable.KPD.ConveyorWidth / 2 + GlobalVariable.KPD.CargoSize.z / 2 
                + GlobalVariable.KPD.ConveyorLengths[0] * (i-(PilerNums+3));
            GlobalVariable.UnidirectionalPositions[i, 1] = GlobalVariable.UnidirectionalPositions[i, 0];
            GlobalVariable.UnidirectionalPositions[i, 1].z = GlobalVariable.UnidirectionalPositions[i, 1].z
                + GlobalVariable.KPD.ConveyorLengths[0] - GlobalVariable.KPD.CargoSize.z;
        }
        #endregion
        //GlobalVariable.BidirectionalPositions关键点坐标
        #region
        for (int i = 0; i < PilerNums; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                GlobalVariable.BidirectionalPositions[i, j, 0] = GlobalVariable.LiftTransferPositions[i + 1, 1];
                GlobalVariable.BidirectionalPositions[i, j, 0].z = GlobalVariable.LiftTransferPositions[i + 1, 1].z
                    - GlobalVariable.KPD.ConveyorWidth / 2 - GlobalVariable.KPD.CargoSize.z / 2 - j * GlobalVariable.KPD.ConveyorLengths[0] - 0.2F;
                GlobalVariable.BidirectionalPositions[i, j, 1] = GlobalVariable.BidirectionalPositions[i, j, 0];
                GlobalVariable.BidirectionalPositions[i, j, 1].z = GlobalVariable.BidirectionalPositions[i, j, 0].z
                    - (GlobalVariable.KPD.ConveyorLengths[0] - GlobalVariable.KPD.CargoSize.z - 0.4F);
            }
        }

        #endregion
        //测试：
        #region 
        //GameObject tempCargo = Instantiate((GameObject)Resources.Load("Scene/Simulation/Cargo"));
        //tempCargo.transform.parent = GameObject.Find("WarehouseScene").transform;
        //tempCargo.transform.localPosition = new Vector3(0, 0, 0);
        ////单向输送机
        //for (int i = 0; i < UniCSum; i++)
        //{
        //    GameObject obj = Instantiate(tempCargo);
        //    obj.transform.localPosition = GlobalVariable.UnidirectionalPositions[i, 0];
        //    obj.transform.parent = GameObject.Find("WarehouseScene").transform;
        //    obj.name = "cargo_" + i + "_0";
        //    GameObject obj1 = Instantiate(tempCargo);
        //    obj1.transform.localPosition = GlobalVariable.UnidirectionalPositions[i, 1];
        //    obj1.transform.parent = GameObject.Find("WarehouseScene").transform;
        //    obj1.name = "cargo_" + i + "_1";
        //}
        ////liftTransfer
        //for (int i = 0; i < PilerNums + 2; i++)
        //{
        //    GameObject obj = Instantiate(tempCargo);
        //    obj.transform.localPosition = GlobalVariable.LiftTransferPositions[i, 0];
        //    obj.transform.parent = GameObject.Find("WarehouseScene").transform;
        //    GameObject obj1 = Instantiate(tempCargo);
        //    obj1.transform.localPosition = GlobalVariable.LiftTransferPositions[i, 1];
        //    obj1.transform.parent = GameObject.Find("WarehouseScene").transform;
        //}
        //BiConveyor
        //for (int i = 0; i < PilerNums; i++)
        //{
        //    for (int j = 0; j < 3; j++)
        //    {
        //        GameObject obj = Instantiate(tempCargo);
        //        obj.transform.localPosition = GlobalVariable.BidirectionalPositions[i, j, 0];
        //        obj.transform.parent = GameObject.Find("WarehouseScene").transform;
        //        GameObject obj1 = Instantiate(tempCargo);
        //        obj1.transform.localPosition = GlobalVariable.BidirectionalPositions[i, j, 1];
        //        obj1.transform.parent = GameObject.Find("WarehouseScene").transform;
        //    }
        //}
        #endregion
    }
    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Times1 = Time.time;//没有用到
        if (GlobalVariable.TempQueue.Count > 0)//得到堆垛机编号
        {
            PilerNum = (GlobalVariable.TempQueue.Peek().GetComponent<ShowCargoInfo>().Cargomessage.PositionInfo.HighBayNum + 1) / 2;
        }
        for (int i = 0; i < PilerNums; i++)
        {
            //入库货物情况
            if (PilerNum == i + 1 && finish1[i] == true && GlobalVariable.TempQueue.Count > 0)
            {
                Cargo[i] = GlobalVariable.TempQueue.Peek();
                //是要入库的货物
                if (Cargo[i].GetComponent<OperatingState>().state == CargoState.WaitIn)
                {
                    KeyFrame[i] = true;
                    Cargo[i].AddComponent<CargoEnter>().enabled = false;
                    Cargo[i].GetComponent<CargoEnter>().Speed = Speed;
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
                //遍历所有入库货物
                foreach (GameObject cargo in GlobalVariable.EnterQueue[i])
                {
                    //执行CargoEnter动画
                    cargo.GetComponent<CargoEnter>().enabled = true;
                    //在货物已经到达单向输送机0时 从tempQueue出队
                    if (GlobalVariable.UnidirectionalConveyorStates[0, 0] == State.On && finish1[i] == false)
                    {
                        //出队
                        GlobalVariable.TempQueue.Dequeue();
                        finish1[i] = true;//本堆垛机不能用了
                    }
                }
            }
            //出库货物出库动画
            if (GlobalVariable.ExitQueue[i].Count > 0)
            {
                //遍历出库队列
                foreach (GameObject cargo in GlobalVariable.ExitQueue[i])
                {
                    //执行出库动画
                    cargo.GetComponent<CargoExit>().enabled = true;
                }
            }
        }
        //堆垛机动画
        foreach (GameObject piler in Piler)//遍历所有的堆垛机
        {
            int Num = piler.GetComponent<PilerProperty>().PilerNum - 1;//堆垛机编号
            //添加堆垛机动画脚本   
            //当前堆垛机空闲 且 有货物在排队（不论是入库还是出库）
            if (GlobalVariable.PilersState[Num] == State.Off && GlobalVariable.ConveyorQueue[Num].Count > 0)
            {
                //拿出一个货物
                GameObject cargo0 = GlobalVariable.ConveyorQueue[Num].Peek();
                //添加堆垛机入库动画脚本
                //货物正在入库 且 堆垛机 
                if (cargo0.GetComponent<OperatingState>().state == CargoState.Enter && GlobalVariable.Wait[Num] == WaitState.WaitEnter)
                {
                    //给堆垛机添加入库动画
                    piler.AddComponent<PilerOfEnter>().enabled = false;
                    piler.GetComponent<PilerOfEnter>().Speed = Speed;
                    piler.GetComponent<PilerOfEnter>().Cargo = cargo0;
                    piler.GetComponent<PilerOfEnter>().PilerNum = Num;
                    GlobalVariable.PilersState[Num] = State.On;//堆垛机打开
                    piler.GetComponent<PilerProperty>().PilerState = State.On;
                    piler.GetComponent<PilerProperty>().WorkState = Direction.Enter;
                }
                //添加堆垛机出库动画脚本
                if (cargo0.GetComponent<OperatingState>().state == CargoState.WaitOut && GlobalVariable.Wait[Num] == WaitState.None)
                {
                    piler.AddComponent<PilerOfExit>().enabled = false;
                    piler.GetComponent<PilerOfExit>().Speed = Speed;
                    piler.GetComponent<PilerOfExit>().Cargo = GlobalVariable.ConveyorQueue[Num].Peek();
                    piler.GetComponent<PilerOfExit>().PilerNum = Num;
                    GlobalVariable.PilersState[Num] = State.On;
                    piler.GetComponent<PilerProperty>().PilerState = State.On;
                    piler.GetComponent<PilerProperty>().WorkState = Direction.Exit;
                    GlobalVariable.BidirectionalConveyorStates[Num, 2, 1] = State.On;
                }
            }
            //执行动画脚本
            if (GlobalVariable.PilersState[Num] == State.On)
            {
                //入库脚本
                if (piler.GetComponent<PilerProperty>().WorkState == Direction.Enter)
                {
                    piler.GetComponent<PilerOfEnter>().enabled = true;
                }
                //出库脚本
                if (piler.GetComponent<PilerProperty>().WorkState == Direction.Exit)
                {
                    piler.GetComponent<PilerOfExit>().enabled = true;
                }
            }
        }
    }
}
