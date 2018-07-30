using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoEnter : MonoBehaviour
{
    //变量
    #region
    //private KeyPositionsData KPD; //场景关键数据
    //public GameObject OBJ1;
    public float Speed; //移动速度
    public int PilerNum;//当前货物对应的Piler下标
    private CargoMessage CM; //货物信息
    private Vector3[] KeyPositions;//货物运动关键点坐标

    private GameObject LiftTransfer1;//需要将货物放到滚筒的顶升移载机
    private GameObject LiftPart1;//该顶升移载机的顶升部分

    private GameObject LiftTransfer2;//需要抬升的顶升移栽机（根据货物去的HighBayNum进行计算）
    private GameObject LiftPart2;//顶升移栽机的抬升部分
    public bool AllRight;//全部完成
    private Vector3 RotationAngle;
    private int KeyStep;//单向输送机的关键点下标
    private int KeysNum;//双向输送机的关键点下标
    private bool[] StepState;
    private bool[,] UnidirectionState;//单向输送线可行性状态
    private bool[,] LiftTransferState;//顶升移载机可行性状态
    private bool[,] BidirectionState;//双向输送线可行性状态
    private bool[,] UnidirectionFinish;//是否到达单向输送机关键点
    private bool[,] LiftTransferFinish;//是否到达顶升移载机关键点
    private bool[,] BidirectionFinish;//是否到达双向输送线关键点
    private bool KeyFrame1;//阶段1
    private bool KeyFrame2;//阶段2
    private bool KeyFrame3;//阶段3
    #endregion
    //初始化
    void Start()
    {
        //获取主要参数
        CM = this.GetComponent<ShowCargoInfo>().Cargomessage;
        RotationAngle = GameObject.Find("WarehouseScene").transform.eulerAngles;
        //第一个顶升移载机
        string LiftTransfer1Name = "WarehouseScene/EnterLiftTransfer";
        LiftTransfer1 = GameObject.Find(LiftTransfer1Name);
        string LiftPart1Name = LiftTransfer1Name + "/LiftPart";
        LiftPart1 = GameObject.Find(LiftPart1Name);
        //Tempi2是第二个顶升移栽机的下标
        int Tempi1 = CM.PositionInfo.HighBayNum; int Tempi2 = (Tempi1 + 1) / 2;
        //第二个顶升移栽对象
        string LiftTransfer2Name = "WarehouseScene/LiftTransferGroup/LiftTransfer" + Tempi2.ToString();
        LiftTransfer2 = GameObject.Find(LiftTransfer2Name);
        string LiftPart2Name = LiftTransfer2Name + "/LiftPart";
        LiftPart2 = GameObject.Find(LiftPart2Name);
        //货箱关键点状态
        int PilerNums = (GlobalVariable.KPD.HighBaysNum + 1) / 2;
        PilerNum = Tempi2 - 1;
        int EnterUniCSum = PilerNums + 2;
        UnidirectionState = new bool[EnterUniCSum, 2];
        UnidirectionFinish = new bool[EnterUniCSum, 2];
        int EnterLiftSum = PilerNums + 1;
        LiftTransferState = new bool[EnterLiftSum, 2];
        LiftTransferFinish = new bool[EnterLiftSum, 3];
        //入库对应的双向输送机
        BidirectionState = new bool[3, 2];
        BidirectionFinish = new bool[3, 2];
        for (int i = 0; i < EnterUniCSum; i++)
        {
            UnidirectionState[i, 0] = false; UnidirectionState[i, 1] = false;
            UnidirectionFinish[i, 0] = false; UnidirectionFinish[i, 1] = false;
        }
        for (int i = 0; i < EnterLiftSum; i++)
        {
            LiftTransferState[i, 0] = false; LiftTransferFinish[i, 1] = false;
            LiftTransferFinish[i, 0] = false; LiftTransferFinish[i, 1] = false; LiftTransferFinish[i, 2] = false;
        }
        for (int i = 0; i < 3; i++)
        {
            BidirectionState[i, 0] = false; BidirectionState[i, 1] = false;
            BidirectionFinish[i, 0] = false; BidirectionFinish[i, 1] = false;
        }
        AllRight = false;
        KeyStep = 0;
        KeysNum = 0;
        KeyFrame1 = false;
        KeyFrame2 = false;
        KeyFrame3 = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //货物到达UniConveyor0位置
        #region
        if (UnidirectionFinish[0, 0] == false)
        {
            //访问未知情况
            //当0,0关闭时更新状态为可用
            if (GlobalVariable.UnidirectionalConveyorStates[0, 0] == State.Off && UnidirectionState[0, 0] == false)
            {
                GlobalVariable.UnidirectionalConveyorStates[0, 0] = State.On;
                UnidirectionState[0, 0] = true;
            }
            //移动到第一个关键点
            if (UnidirectionState[0, 0] == true)
            {
                this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, GlobalVariable.UnidirectionalPositions[0, 0], Speed * Time.deltaTime);
                if (this.transform.localPosition == GlobalVariable.UnidirectionalPositions[0, 0])
                {
                    UnidirectionFinish[0, 0] = true;
                    KeyFrame1 = true;
                }
            }
        }
        #endregion
        //货物从UniConveyor0->第一个顶升->下一个UniConveyor
        #region
        if (KeyFrame1 == true && KeyFrame2 == false)
        {
            //UniConveyor运输货物
            UniConveyor1_0(KeyStep);
            //从UniConveyor0->1
            UniConveyor0_1(KeyStep);
        }
        #endregion
        //阶段2过程
        #region
        if (KeyFrame2 == true && KeyFrame3 == false)
        {
            //顶升将货物->滚筒
            LiftTransferPutDown();
            //从顶升移栽机->下一个单向输送机，下标以顶升为准
            LiftTransfer2UniConveyor(KeyStep);
            //经过该单向输送机
            UniConveyor0_1(KeyStep);
            //从单向输送机->下一个顶升移栽机
            UniConveyor2LiftTransfer(KeyStep);
        }
        #endregion
        //阶段3过程
        #region
        if (KeyFrame3 == true && AllRight == false)
        {
            //顶升移栽机->双向输送机0
            LiftTransferEnter();
            //双向输送机1->0
            BiConveyor1_0(KeysNum);
            //双向输送机0->1
            BiConveyor0_1(KeysNum);
        }
        #endregion
    }
    //UniConveyor上0-1位置移动
    public void UniConveyor0_1(int i)
    {
        //已经到0没有到1
        if (UnidirectionFinish[i, 0] == true && UnidirectionFinish[i, 1] == false)
        {
            //访问1位置可行性
            //若1关闭且不可行，改为开启并可行，0关闭（不再占用0）
            if (GlobalVariable.UnidirectionalConveyorStates[i, 1] == State.Off && UnidirectionState[i, 1] == false)
            {
                GlobalVariable.UnidirectionalConveyorStates[i, 1] = State.On;
                UnidirectionState[i, 1] = true;
                GlobalVariable.UnidirectionalConveyorStates[i, 0] = State.Off;
            }
            //0-1位置移动
            //1可行且没有结束
            if (UnidirectionState[i, 1] == true && UnidirectionFinish[i, 1] == false)
            {
                //移动货物到1
                this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, GlobalVariable.UnidirectionalPositions[i, 1], Speed * Time.deltaTime);
                if (this.transform.localPosition == GlobalVariable.UnidirectionalPositions[i, 1])
                {
                    //1结束
                    UnidirectionFinish[i, 1] = true;
                    if (i == 1)
                    {
                        KeyFrame2 = true;
                    }
                }
            }
        }  
    }
    //UniConveyor上1-0位置移动
    public void UniConveyor1_0(int i)
    {
        if (UnidirectionFinish[i, 1] == true && UnidirectionFinish[i + 1, 0] == false)
        {
            //访问下一个UniConveyor0位置可行情况
            if (GlobalVariable.UnidirectionalConveyorStates[i + 1, 0] == State.Off && UnidirectionState[i + 1, 0] == false)
            {
                GlobalVariable.UnidirectionalConveyorStates[i + 1, 0] = State.On;
                UnidirectionState[i + 1, 0] = true;
                GlobalVariable.UnidirectionalConveyorStates[i, 1] = State.Off;
            }
            //过渡移动
            if (UnidirectionState[i + 1, 0] == true && UnidirectionFinish[i + 1, 0] == false)
            {
                this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, GlobalVariable.UnidirectionalPositions[i + 1, 0], Speed * Time.deltaTime);
                if (this.transform.localPosition == GlobalVariable.UnidirectionalPositions[i + 1, 0])
                {
                    UnidirectionFinish[i + 1, 0] = true;
                    KeyStep = KeyStep + 1;
                }
            }
        }
    }
    //第一个顶升将货物放到滚筒
    public void LiftTransferPutDown()
    {
        //移栽机从2->1 1->0
        if (UnidirectionFinish[1, 1] == true && LiftTransferFinish[0, 0] == false)
        {
            //访问输送机可行情况
            if (GlobalVariable.LiftTransferStates[0] == State.Off && LiftTransferState[0, 0] == false)
            {
                GlobalVariable.LiftTransferStates[0] = State.On;
                LiftTransferState[0, 0] = true;
                //添加顶升移栽机动画（暂不执行）
                LiftTransfer1.AddComponent<LiftTransferMove>().enabled = false;
                LiftTransfer1.GetComponent<LiftTransferMove>().Speed = Speed / 30;
                LiftTransfer1.GetComponent<LiftTransferMove>().pattern = LiftTransferMove.Pattern.up;
            }
            //顶升移载机抬起
            //0可用且2未完成
            if (LiftTransferState[0, 0] == true && LiftTransferFinish[0, 2] == false)
            {
                LiftTransfer1.GetComponent<LiftTransferMove>().enabled = true;
                if (LiftTransfer1.GetComponent<LiftTransferMove>().Finish1 == true)//抬升结束
                {
                    LiftTransferFinish[0, 2] = true;
                    GlobalVariable.UnidirectionalConveyorStates[1, 1] = State.Off;
                }
            }
            //货物过渡到顶升移载机上（此时1才完成）2->1
            if (LiftTransferFinish[0, 2] == true && LiftTransferFinish[0, 1] == false)
            {
                this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, GlobalVariable.LiftTransferPositions[0, 1], Speed * Time.deltaTime);
                if (this.transform.localPosition == GlobalVariable.LiftTransferPositions[0, 1])
                {
                    LiftTransferFinish[0, 1] = true;
                    this.transform.parent = LiftPart1.transform;
                    //准备下落
                    LiftTransfer1.GetComponent<LiftTransferMove>().pattern = LiftTransferMove.Pattern.down;
                }
            }
            //顶升移载机降下 1->0
            if (LiftTransferFinish[0, 1] == true && LiftTransferFinish[0, 0] == false)
            {
                //下降动画
                LiftTransfer1.GetComponent<LiftTransferMove>().enabled = true;
                if (LiftTransfer1.GetComponent<LiftTransferMove>().Finish2 == true)
                {
                    LiftTransferFinish[0, 0] = true;
                    this.transform.parent = GameObject.Find("WarehouseScene").transform;
                    DestroyImmediate(LiftTransfer1.GetComponent<LiftTransferMove>());
                    KeyStep++;
                }
            }
        }
    }
    //UniConveyor过渡到顶升移载机
    public void UniConveyor2LiftTransfer(int i)
    {
        if (UnidirectionFinish[i, 1] == true && LiftTransferFinish[i - 1, 0] == false)
        {
            //访问顶升移载机
            //还没有到最后一个顶升移栽机
            if (i != PilerNum + 2)
            {
                //访问顶升移载机
                if (GlobalVariable.LiftTransferStates[i - 1] == State.Off && LiftTransferState[i - 1, 0] == false)
                {
                    GlobalVariable.LiftTransferStates[i - 1] = State.On;
                    LiftTransferState[i - 1, 0] = true;
                    GlobalVariable.UnidirectionalConveyorStates[i, 1] = State.Off;
                }
                //过渡
                //移栽机0可行且未结束
                if (LiftTransferState[i - 1, 0] == true && LiftTransferFinish[i - 1, 0] == false)
                {
                    //到移栽机位置
                    this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, GlobalVariable.LiftTransferPositions[i - 1, 0], Speed * Time.deltaTime);
                    if (this.transform.localPosition == GlobalVariable.LiftTransferPositions[i - 1, 0])
                    {
                        //结束
                        LiftTransferFinish[i - 1, 0] = true;

                        //关键点下标+1
                        KeyStep = KeyStep + 1;///////////////
                    }
                }
            }
            //已经到达最后一个移栽机（需要抬升）
            if (i == PilerNum + 2)
            {
                //访问顶升移载机
                //如果是入库
                if (GlobalVariable.ConveyorDirections[PilerNum] == Direction.Enter)
                {
                    //顶升移栽机关闭且不可用
                    if (GlobalVariable.LiftTransferStates[i - 1] == State.Off && LiftTransferState[i - 1, 0] == false)
                    {
                        //开启
                        GlobalVariable.LiftTransferStates[i - 1] = State.On;
                        //0可行
                        LiftTransferState[i - 1, 0] = true;
                        //输送机的1要关闭
                        GlobalVariable.UnidirectionalConveyorStates[i, 1] = State.Off;
                    }
                }
                //过渡
                //0可行且未完成
                if (LiftTransferState[i - 1, 0] == true && LiftTransferFinish[i - 1, 0] == false)
                {
                    //从单向输送机到达顶升移栽机
                    this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, GlobalVariable.LiftTransferPositions[i - 1, 0], Speed * Time.deltaTime);
                    if (this.transform.localPosition == GlobalVariable.LiftTransferPositions[i - 1, 0])
                    {
                        //已经到移栽机的0位置                  
                        LiftTransferFinish[i - 1, 0] = true;
                        //关键点下标+1
                        KeyStep = KeyStep + 1;
                        KeyFrame3 = true;
                        //接下来的动画交给LiftPart
                        //货物成为LiftPart的子组件
                        //给顶升移栽机添加抬升脚本
                        //模式为Up，暂时还不执行
                        this.transform.parent = LiftPart2.transform;
                        LiftTransfer2.AddComponent<LiftTransferMove>().enabled = false;
                        LiftTransfer2.GetComponent<LiftTransferMove>().Speed = Speed / 30;
                        LiftTransfer2.GetComponent<LiftTransferMove>().pattern = LiftTransferMove.Pattern.up;
                    }
                }
            }
        }
    }
    //顶升移载机过渡到UniConveyor（下标要变）
    public void LiftTransfer2UniConveyor(int i)
    {
        //到达顶升移载机，还没有到达下一个输送机的0点
        if (LiftTransferFinish[i - 2, 0] == true && UnidirectionFinish[i, 0] == false)
        {
            //访问UniConveyor
            if (GlobalVariable.UnidirectionalConveyorStates[i, 0] == State.Off && UnidirectionState[i, 0] == false)
            {
                GlobalVariable.UnidirectionalConveyorStates[i, 0] = State.On;
                UnidirectionState[i, 0] = true;
            }
            //过渡（下一个关键点可用且未完成）
            //因为没有到达最后一个顶升移栽机，故不需要抬升，直接过去就行
            if (UnidirectionState[i, 0] == true && UnidirectionFinish[i, 0] == false)
            {
                this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, GlobalVariable.UnidirectionalPositions[i, 0], Speed * Time.deltaTime);
                //已经过了顶升移栽机，更新数据
                if (this.transform.localPosition == GlobalVariable.UnidirectionalPositions[i, 0])
                {
                    //未完成->已完成
                    UnidirectionFinish[i, 0] = true;
                    //工作状态：关闭
                    GlobalVariable.LiftTransferStates[i - 2] = State.Off;
                }
            }
        }
    }
    //顶升移载机工作
    public void LiftTransferEnter()
    {
        //顶升移栽机从0->1 1->2 就是抬升并下落的过程
        if (LiftTransferFinish[PilerNum + 1, 0] == true && LiftTransferFinish[PilerNum + 1, 2] == false)
        {
            //顶升抬起货物（0->1）
            if (LiftTransferFinish[PilerNum + 1, 1] == false)
            {
                //启动LiftPart的抬升动画
                LiftTransfer2.GetComponent<LiftTransferMove>().enabled = true;
                //抬升过程结束
                if (LiftTransfer2.GetComponent<LiftTransferMove>().Finish1 == true)
                {
                    LiftTransferFinish[PilerNum + 1, 1] = true;
                    this.transform.parent = GameObject.Find("WarehouseScene").transform;
                }
            }
            //货物离开顶升移栽机（货物从顶升1->双向输送机0）
            if (LiftTransferFinish[PilerNum + 1, 1] == true && BidirectionFinish[0, 0] == false)
            {
                //访问BiConveyor
                if (GlobalVariable.BidirectionalConveyorStates[PilerNum, 0, 0] == State.Off && BidirectionState[0, 0] == false)
                {
                    GlobalVariable.BidirectionalConveyorStates[PilerNum, 0, 0] = State.On;
                    BidirectionState[0, 0] = true;
                }
                //货物过渡（到biConveyor的0）
                if (BidirectionState[0, 0] == true && BidirectionFinish[0, 0] == false)
                {
                    this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, GlobalVariable.BidirectionalPositions[PilerNum, 0, 0], Speed * Time.deltaTime);
                    //到0
                    if (this.transform.localPosition == GlobalVariable.BidirectionalPositions[PilerNum, 0, 0])
                    {
                        //0结束
                        BidirectionFinish[0, 0] = true;
                        //移栽机变为down
                        LiftTransfer2.GetComponent<LiftTransferMove>().pattern = LiftTransferMove.Pattern.down;
                        KeysNum = 0;
                    }
                }
            }
            //顶升降下(从顶升1到2）
            if (BidirectionFinish[0, 0] == true)//货物已经到双向输送机的0
            {
                LiftTransfer2.GetComponent<LiftTransferMove>().enabled = true;//启动下降脚本
                if (LiftTransfer2.GetComponent<LiftTransferMove>().Finish2 == true)//下降完成
                {
                    LiftTransferFinish[PilerNum + 1, 2] = true;
                    GlobalVariable.LiftTransferStates[PilerNum + 1] = State.Off;
                    //移除
                    DestroyImmediate(LiftTransfer2.GetComponent<LiftTransferMove>());
                }
            }
        }
    }
    //BiConveyor0-1位置移动
    public void BiConveyor0_1(int j)
    {
        if (BidirectionFinish[j, 0] == true && BidirectionFinish[j, 1] == false)
        {
            //访问1位置
            if (GlobalVariable.BidirectionalConveyorStates[PilerNum, j, 1] == State.Off && BidirectionState[j, 1] == false)
            {
                GlobalVariable.BidirectionalConveyorStates[PilerNum, j, 1] = State.On;
                BidirectionState[j, 1] = true;
                GlobalVariable.BidirectionalConveyorStates[PilerNum, j, 0] = State.Off;
            }
            //0-1位置移动
            if (BidirectionState[j, 1] == true && BidirectionFinish[j, 1] == false)
            {
                this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, GlobalVariable.BidirectionalPositions[PilerNum, j, 1], Speed * Time.deltaTime);
                if (this.transform.localPosition == GlobalVariable.BidirectionalPositions[PilerNum, j, 1])
                {
                    BidirectionFinish[j, 1] = true;
                    if (j == 2)//到最后一个双向输送机
                    {
                        AllRight = true;//CargoEnter结束
                        //等待被入库
                        GlobalVariable.Wait[PilerNum] = WaitState.WaitEnter;
                    }
                }
            }
        }
    }
    //BiConveyor之间过渡
    public void BiConveyor1_0(int j)
    {
        if (BidirectionFinish[j, 1] == true && BidirectionFinish[j + 1, 0] == false)
        {
            //访问0位置
            if (GlobalVariable.BidirectionalConveyorStates[PilerNum, j + 1, 0] == State.Off && BidirectionState[j + 1, 0] == false)
            {
                GlobalVariable.BidirectionalConveyorStates[PilerNum, j + 1, 0] = State.On;
                BidirectionState[j + 1, 0] = true;
                GlobalVariable.BidirectionalConveyorStates[PilerNum, j, 1] = State.Off;
            }
            //过渡
            if (BidirectionState[j + 1, 0] == true && BidirectionFinish[j + 1, 0] == false)
            {
                this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, GlobalVariable.BidirectionalPositions[PilerNum, j + 1, 0], Speed * Time.deltaTime);
                if (this.transform.localPosition == GlobalVariable.BidirectionalPositions[PilerNum, j + 1, 0])
                {
                    BidirectionFinish[j + 1, 0] = true;
                    KeysNum = KeysNum + 1;//下标更新
                }
            }
        }
    }
}
