using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PilerOfEnter : MonoBehaviour {

    private GameObject OBJ1;//大场景
    private GameObject OBJ2;//Cargos
    private GameObject PilerBodyPart;//水平移动部分
    private GameObject PilerUpPart;//上下移动部分
    private GameObject PilerForks;//货叉组合
    private GameObject PilerFork1;//Forks下的Fork1
    public GameObject Cargo;//要叉的货物

    public bool Accomplish;
    public float Speed;
    
    private CargoMessage CM; //货物信息

    private Vector3[] BodyPartPositions;//堆垛机主体部分关键坐标，有3个
    private Vector3[] UpPartPositions;//堆垛机上下移动部分关键坐标，有7个
    private Vector3[] ForksPositions;//堆垛机叉组移动关键坐标，有3个
    private Vector3[] Fork1Positions;//堆垛机叉1移动关键坐标，有3个
    private Vector3 TargetPosition;//货物最终位置
    private Vector3 ForkSize;//货叉尺寸
    public bool[] finish;//判断条件
    private int PlaceNum;//A还是B,A=0,B=1
    public int PilerNum;//对应Piler编号
    private float high;//货物座空隙高度
    private Vector3 RotationAngle;
    // Use this for initialization
    void Start () {
        OBJ1 = GameObject.Find("WarehouseScene");//场景
        OBJ2 = GameObject.Find("WarehouseScene/Cargos");//货物组（已经入库的）
        CM = Cargo.GetComponent<ShowCargoInfo>().Cargomessage;
        RotationAngle = new Vector3(0, 0, 0) - OBJ1.transform.eulerAngles;
        //设计移动速度
        Speed = 0.8f;
        GlobalVariable.PilersState[PilerNum] = State.On;
        int Tempi1 = CM.PositionInfo.HighBayNum; int Tempi2 = (Tempi1 + 1) / 2;
        high = 0.12f;//货物底座的高度
        if (CM.PositionInfo.place == 0)
        {
            PlaceNum = 0;
        }
        else
        {
            PlaceNum = 1;
        }
        //阶段划分
        finish = new bool[7];
        finish[0] = false; finish[1] = false; finish[2] = false; finish[3] = false; finish[4] = false; finish[5] = false; finish[6] = false;
        // 堆垛机动画设计
        #region 
        //获取相关对象
        Vector3 CargoPosition = Cargo.transform.position;//货物坐标（绝对坐标）
        string PilerName = this.name;
        Vector3 PilerPosition = this.transform.position;//堆垛机的坐标（绝对坐标）
        string HighBayName = OBJ1.name + "/HighBayGroup/HighBayGroup/HighBay" + Tempi1.ToString();
        GameObject HighBay = GameObject.Find(HighBayName);
        Vector3 HighBayPosition = HighBay.transform.position;//高架库坐标
        string BodyPartName = PilerName + "/BodyPart";
        PilerBodyPart = GameObject.Find(BodyPartName);//堆垛机主体
        Vector3 PilerBodyPartPosition = PilerBodyPart.transform.localPosition;//堆垛机主体,局部坐标(相对于堆垛机)
        string UpPartName = BodyPartName + "/UpPart";
        PilerUpPart = GameObject.Find(UpPartName);//堆垛机上下移动部分
        Vector3 PilerUpPartPosition = PilerUpPart.transform.localPosition;//堆垛机上下移动部分,局部坐标(相对于堆垛机)
        string ForksName = UpPartName + "/Forks"; PilerForks = GameObject.Find(ForksName);

        Vector3 ForksSize = new Vector3(0, 0, 0);
        MyClass.MeshSize(PilerForks, ref ForksSize);//计算货叉组的尺寸 
        Vector3 ForksPosition = PilerForks.transform.localPosition;

        string Fork1Name = ForksName + "/Fork1"; PilerFork1 = GameObject.Find(Fork1Name);
        Vector3 Fork1Position = PilerFork1.transform.localPosition;

        //堆垛机主体移动关键位置坐标，3个
        #region
        //通过MainShelf（终点货架）的位置得到货物最终位置TargetPosition
        string MainShelfName = OBJ1.name + "/HighBayGroup/HighBayGroup/HighBay" + CM.PositionInfo.HighBayNum.ToString() + "/MainShelf";
        MainShelfName = MainShelfName + "_" + CM.PositionInfo.FloorNum.ToString() + "_" + CM.PositionInfo.ColumnNum.ToString();
        GameObject MainShelf = GameObject.Find(MainShelfName);
        Vector3 MainShelfPosition = MainShelf.transform.position;
        TargetPosition = MainShelfPosition;
        TargetPosition.z = TargetPosition.z + GlobalVariable.KPD.StorePositions.StorePlacePosition[PlaceNum];
        //BodyPart关键坐标，3个
        BodyPartPositions = new Vector3[5];//

        float TempValue1 = (Quaternion.Euler(RotationAngle) * (CargoPosition - PilerForks.transform.position)).z;
        BodyPartPositions[0] = PilerBodyPart.transform.localPosition;//Body初始位置
        BodyPartPositions[1] = BodyPartPositions[0];
        BodyPartPositions[1].z = BodyPartPositions[0].z + TempValue1;//货物等着被叉的位置
        BodyPartPositions[2] = BodyPartPositions[1];
        float TempValue1_1 = (Quaternion.Euler(RotationAngle) * (MainShelfPosition- CargoPosition)).z + GlobalVariable.KPD.StorePositions.StorePlacePosition[PlaceNum];
        //货物最终位置
        BodyPartPositions[2].z = BodyPartPositions[1].z + TempValue1_1;
        #endregion
        //上下移动部分关键位置坐标，7个
        #region
        UpPartPositions = new Vector3[7];
        //0->1需要抬升的高度
        float TempValue2 = (Quaternion.Euler(RotationAngle) * (CargoPosition - PilerForks.transform.position)).y + ForksSize.y / 2;
        UpPartPositions[0] = PilerUpPart.transform.localPosition;//堆垛机UpPart目标位置0，原位置(相对位置）
        UpPartPositions[1] = UpPartPositions[0];
        UpPartPositions[1].y = UpPartPositions[0].y + TempValue2;//堆垛机UpPart目标位置1，因为是相对位置，只需要修改y
        UpPartPositions[2] = UpPartPositions[1];
        UpPartPositions[2].y = UpPartPositions[1].y + high - ForksSize.y;//堆垛机UpPart目标位置2，抬升到货叉接触货物
        UpPartPositions[3] = UpPartPositions[2];
        UpPartPositions[3].y = UpPartPositions[2].y + 0.2f;//堆垛机UpPart目标位置3，继续抬升，让王货物离开平台（这里设置为0.2f)
        UpPartPositions[4] = UpPartPositions[3];
        float TempValue2_1 = (Quaternion.Euler(RotationAngle) * (MainShelfPosition - CargoPosition)).y + high - ForksSize.y + 0.2f;
        UpPartPositions[4].y = UpPartPositions[1].y + TempValue2_1;//堆垛机UpPart目标位置4，高度为目标货架的高度，且货叉还是抬升两次的高度
        UpPartPositions[5] = UpPartPositions[4];
        UpPartPositions[5].y = UpPartPositions[1].y + TempValue2_1 - 0.2f;//堆垛机UpPart目标位置5，货叉下降一点，0.2f
        UpPartPositions[6] = UpPartPositions[5];
        UpPartPositions[6].y = UpPartPositions[5].y - (high - ForksSize.y);//堆垛机UpPart目标位置6，货叉继续下降，货叉与平台齐平
        #endregion
        //货叉关键坐标，3个
        #region
        float TempValue3 = 2 * ForksSize.x / 3;//叉组伸出的距离
        ForksPositions = new Vector3[3];
        ForksPositions[0] = PilerForks.transform.localPosition;//叉组位置0，原位置
        ForksPositions[1] = ForksPositions[0];
        ForksPositions[1].x = ForksPositions[1].x + TempValue3;//叉组位置1，货叉组整体伸出2/3

        //剩下的用Fork1伸出
        Fork1Positions = new Vector3[3];
        //Fork1还要伸出
        float TempValue4 = (Quaternion.Euler(RotationAngle) * (CargoPosition - PilerPosition)).x - TempValue3;
        Fork1Positions[0] = PilerFork1.transform.localPosition;//叉1位置0，Fork1原位置
        Fork1Positions[1] = Fork1Positions[0];
        Fork1Positions[1].x = Fork1Positions[1].x + TempValue4;//叉1位置1，向前伸出
        //放下货物货叉需要移动的总距离
        float TempValue4_1 = Mathf.Abs((Quaternion.Euler(RotationAngle) * (HighBayPosition - PilerPosition)).x);
        //奇偶数货架的货叉放下时方向不一样
        switch (Tempi1 % 2)
        {
            //叉组伸2/3
            //Fork1伸剩余的
            case 1:
                ForksPositions[2] = ForksPositions[0]; ForksPositions[2].x = ForksPositions[0].x + TempValue3;//叉组位置2
                Fork1Positions[2] = Fork1Positions[0]; Fork1Positions[2].x = Fork1Positions[0].x + TempValue4_1 - TempValue3;//叉1位置2
                break;
            case 0:
                ForksPositions[2] = ForksPositions[0]; ForksPositions[2].x = ForksPositions[0].x - TempValue3;
                Fork1Positions[2] = Fork1Positions[0]; Fork1Positions[2].x = Fork1Positions[0].x - TempValue4_1 + TempValue3;
                break;
        }
        #endregion
        #endregion
    }

    // Update is called once per frame
    void FixedUpdate () {
        //堆垛机运动过程
        if (finish[0] == false)
        {
            //BodyPart和UpPart一起运动
            //BodyPart到达货物等待位置，UpPart在y值上和平台齐平
            PilerBodyPart.transform.localPosition = Vector3.MoveTowards(PilerBodyPart.transform.localPosition, BodyPartPositions[1], Speed * Time.deltaTime);
            PilerUpPart.transform.localPosition = Vector3.MoveTowards(PilerUpPart.transform.localPosition, UpPartPositions[1], Speed * Time.deltaTime);
            if (PilerBodyPart.transform.localPosition == BodyPartPositions[1])
            {
                if (PilerUpPart.transform.localPosition == UpPartPositions[1])
                {
                    //叉组伸2/3
                    PilerForks.transform.localPosition = Vector3.MoveTowards(PilerForks.transform.localPosition, ForksPositions[1], Speed * Time.deltaTime);
                    if (PilerForks.transform.localPosition == ForksPositions[1])
                    {
                        //Fork1再伸出剩余的
                        PilerFork1.transform.localPosition = Vector3.MoveTowards(PilerFork1.transform.localPosition, Fork1Positions[1], Speed * Time.deltaTime);
                        if (PilerFork1.transform.localPosition == Fork1Positions[1])
                        {
                            finish[0] = true;
                        }
                    }
                }
            }
        }

        if (finish[0] == true && finish[1] == false)
        {
            //货叉抬起到接触货物
            PilerUpPart.transform.localPosition = Vector3.MoveTowards(PilerUpPart.transform.localPosition, UpPartPositions[2], Speed * Time.deltaTime);
            if (PilerUpPart.transform.localPosition == UpPartPositions[2])
            {
                //货物跟随货叉1运动
                Cargo.transform.parent = PilerFork1.transform;
                finish[1] = true;
            }
        }

        if (finish[1]==true && finish[2]==false)
        {
            //货叉继续抬高，让货物离开平台
            PilerUpPart.transform.localPosition = Vector3.MoveTowards(PilerUpPart.transform.localPosition, UpPartPositions[3], Speed * Time.deltaTime);
            if (PilerUpPart.transform.localPosition == UpPartPositions[3])
            {
                //Fork1收回
                PilerFork1.transform.localPosition = Vector3.MoveTowards(PilerFork1.transform.localPosition, Fork1Positions[0], Speed * Time.deltaTime);
                if (PilerFork1.transform.localPosition == Fork1Positions[0])
                {
                    //叉组收回
                    PilerForks.transform.localPosition = Vector3.MoveTowards(PilerForks.transform.localPosition, ForksPositions[0], Speed * Time.deltaTime);
                    if (PilerForks.transform.localPosition == ForksPositions[0])
                    {
                        //2结束，接下来准备移动货物到最终位置
                        finish[2] = true;
                        //Piler入库动画结束
                        GlobalVariable.Wait[PilerNum] = WaitState.None;
                        GlobalVariable.BidirectionalConveyorStates[PilerNum, 2, 1] = State.Off;
                        GlobalVariable.ConveyorQueue[PilerNum].Dequeue();
                        GlobalVariable.EnterQueue[PilerNum].Dequeue();
                        DestroyImmediate(Cargo.GetComponent<CargoEnter>());
                    }
                }
            }
        }
        
        if (finish[2] == true && finish[3] == false)
        {
            //BodyPart到达最终位置
            PilerBodyPart.transform.localPosition = Vector3.MoveTowards(PilerBodyPart.transform.localPosition, BodyPartPositions[2], Speed * Time.deltaTime);
            //UpPart到达放下前的最终高度
            PilerUpPart.transform.localPosition = Vector3.MoveTowards(PilerUpPart.transform.localPosition, UpPartPositions[4], Speed * Time.deltaTime);
            if (PilerBodyPart.transform.localPosition == BodyPartPositions[2] && PilerUpPart.transform.localPosition == UpPartPositions[4])
            {
                //叉组再次伸出2/3，准备放下货物
                PilerForks.transform.localPosition = Vector3.MoveTowards(PilerForks.transform.localPosition, ForksPositions[2], Speed * Time.deltaTime);
                if (PilerForks.transform.localPosition == ForksPositions[2])
                {
                    //之后Fork1继续伸出
                    PilerFork1.transform.localPosition = Vector3.MoveTowards(PilerFork1.transform.localPosition, Fork1Positions[2], Speed * Time.deltaTime);
                    if (PilerFork1.transform.localPosition == Fork1Positions[2])
                    {
                        finish[3] = true;
                    }
                }
            }
        }

        if (finish[3] == true && finish[4] == false)
        {
            //UpPart下降到 货物接触平台
            PilerUpPart.transform.localPosition = Vector3.MoveTowards(PilerUpPart.transform.localPosition, UpPartPositions[5], Speed * Time.deltaTime);
            if (PilerUpPart.transform.localPosition == UpPartPositions[5])
            {
                //货物放入cargos中，状态改为“已入库”
                Cargo.transform.parent = OBJ2.transform;
                Cargo.GetComponent<OperatingState>().state = CargoState.Stored;
                //修改该货物进程面板中的状态信息
                GameObject.Find("ProcessInterface/MainBody/Scroll View/Viewport/Content").transform.Find(Cargo.name).transform.Find("State").GetComponent<Text>().text = "货物状态：" + "完成入库";
                Functions.ChangeColor(Cargo.name, StorageBinState.Stored);//修改状态面板中该货物对应仓位的颜色表示
                finish[4] = true;
            }
        }

        if (finish[4] == true && finish[5] == false)
        {
            //UpPart继续下降到与平台齐平
            PilerUpPart.transform.localPosition = Vector3.MoveTowards(PilerUpPart.transform.localPosition, UpPartPositions[6], Speed * Time.deltaTime);
            if (PilerUpPart.transform.localPosition == UpPartPositions[6])
            {
                //收回Fork1
                PilerFork1.transform.localPosition = Vector3.MoveTowards(PilerFork1.transform.localPosition, Fork1Positions[0], Speed * Time.deltaTime);
                if (PilerFork1.transform.localPosition == Fork1Positions[0])
                {
                    //收回叉组
                    PilerForks.transform.localPosition = Vector3.MoveTowards(PilerForks.transform.localPosition, ForksPositions[0], Speed * Time.deltaTime);
                    if (PilerForks.transform.localPosition == ForksPositions[0])
                    {
                        finish[5] = true;
                        //原地结束
                        Accomplish = true;
                        finish[6] = true;
                        //PilerOfEnter结束
                        GlobalVariable.PilersState[PilerNum] = State.Off;
                        DestroyImmediate(GameObject.Find("ProcessInterface/MainBody/Scroll View/Viewport/Content").transform.Find(Cargo.name).gameObject);
                        this.GetComponent<PilerProperty>().PilerState = State.Off;
                        DestroyImmediate(this.GetComponent<PilerOfEnter>());
                    }
                }
            }
        }
        
        //if (finish[5] == true && finish[6] == false)
        //{
        //    PilerBodyPart.transform.localPosition = Vector3.MoveTowards(PilerBodyPart.transform.localPosition, BodyPartPositions[1], Speed * Time.deltaTime);
        //    PilerUpPart.transform.localPosition = Vector3.MoveTowards(PilerUpPart.transform.localPosition, UpPartPositions[1], Speed * Time.deltaTime);
        //    if (PilerBodyPart.transform.localPosition == BodyPartPositions[1] && PilerUpPart.transform.localPosition == UpPartPositions[1])
        //    {
        //        Accomplish = true;
        //        finish[6] = true;
        //        //GlobalVariable.CargosState[PilerNum, 5] = true;
        //        GlobalVariable.PilersState[PilerNum] = State.Off;
        //        DestroyImmediate(GameObject.Find("ProcessInterface/MainBody/Scroll View/Viewport/Content").transform.Find(Cargo.name).gameObject);
        //        this.GetComponent<PilerProperty>().PilerState = State.Off;
        //        DestroyImmediate(this.GetComponent<PilerOfEnter>());
        //    }
        //}
	}
}
