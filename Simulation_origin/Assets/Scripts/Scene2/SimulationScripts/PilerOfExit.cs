using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//堆垛机出库动画，基本和入库动画相反
public class PilerOfExit: MonoBehaviour {

    private GameObject OBJ1;
    private GameObject OBJ2;
    private GameObject PilerBodyPart;
    private GameObject PilerUpPart;
    private GameObject PilerForks;
    private GameObject PilerFork1;
    public GameObject Cargo;
    //public string CargoName;//运输货物对象名称

    public bool Accomplish;
    public float Speed;

    private CargoMessage CM; //货物信息

    private Vector3[] BodyPartPositions;//堆垛机主体部分关键坐标
    private Vector3[] UpPartPositions;//堆垛机上下移动部分关键坐标
    private Vector3[] ForksPositions;//堆垛机叉组移动关键坐标
    private Vector3[] Fork1Positions;//堆垛机叉1移动关键坐标
    private Vector3 TargetPosition;//取货位置
    private Vector3 TargetPosition1;//放货位置
    private Vector3 ForkSize;//货叉尺寸
    public bool[] finish;//判断条件
    private int PlaceNum;
    public int PilerNum;
    private float high;//货物座空隙高度
    private Vector3 RotationAngle;
    // Use this for initialization
    void Start () {
        OBJ1 = GameObject.Find("WarehouseScene");
        OBJ2 = GameObject.Find("WarehouseScene/Cargos");
        CM = Cargo.GetComponent<ShowCargoInfo>().Cargomessage;
        RotationAngle = new Vector3(0, 0, 0) - OBJ1.transform.eulerAngles;
        //设计移动速度
        Speed = 0.8f;
        //Piler开始工作
        GlobalVariable.PilersState[PilerNum] = State.On;
        int Tempi1 = CM.PositionInfo.HighBayNum; int Tempi2 = (Tempi1 + 1) / 2;
        high = 0.12f;
        if (CM.PositionInfo.place == 0)
        {
            PlaceNum = 0;
        }
        else
        {
            PlaceNum = 1;
        }
        finish = new bool[7];
        finish[0] = false; finish[1] = false; finish[2] = false; finish[3] = false; finish[4] = false; finish[5] = false; finish[6] = false;
        #region  堆垛机动画设计
        //获取相关对象
        Vector3 CargoPosition = Cargo.transform.position;
        string PilerName = this.name;
        Vector3 PilerPosition = this.transform.position;//堆垛机的坐标,绝对坐标
        string HighBayName = OBJ1.name + "/HighBayGroup/HighBayGroup/HighBay" + Tempi1.ToString();
        GameObject HighBay = GameObject.Find(HighBayName);
        Vector3 HighBayPosition = HighBay.transform.position;//高架库坐标,绝对坐标
        string BodyPartName = PilerName + "/BodyPart";
        PilerBodyPart = GameObject.Find(BodyPartName);//堆垛机主体
        Vector3 PilerBodyPartPosition = PilerBodyPart.transform.localPosition;//堆垛机主体局部坐标
        string UpPartName = BodyPartName + "/UpPart";
        PilerUpPart = GameObject.Find(UpPartName);//堆垛机上下移动部分
        Vector3 PilerUpPartPosition = PilerUpPart.transform.localPosition;//堆垛机上下移动部分局部坐标
        string ForksName = UpPartName + "/Forks"; PilerForks = GameObject.Find(ForksName);

        Vector3 ForksSize = new Vector3(0, 0, 0);
        MyClass.MeshSize(PilerForks, ref ForksSize);
        Vector3 ForksPosition = PilerForks.transform.localPosition;
        string Fork1Name = ForksName + "/Fork1"; PilerFork1 = GameObject.Find(Fork1Name);
        Vector3 Fork1Position = PilerFork1.transform.localPosition;

        //堆垛机主体移动关键位置坐标
        #region
        string MainShelfName = OBJ1.name + "/HighBayGroup/HighBayGroup/HighBay" + CM.PositionInfo.HighBayNum.ToString() + "/MainShelf";
        MainShelfName = MainShelfName + "_" + CM.PositionInfo.FloorNum.ToString() + "_" + CM.PositionInfo.ColumnNum.ToString();
        GameObject MainShelf = GameObject.Find(MainShelfName);
        Vector3 MainShelfPosition = MainShelf.transform.position;
        TargetPosition = Quaternion.Euler(RotationAngle) * MainShelfPosition;
        TargetPosition.z = TargetPosition.z + GlobalVariable.KPD.StorePositions.StorePlacePosition[PlaceNum];
        TargetPosition1 = Quaternion.Euler(RotationAngle) * (GameObject.Find("WarehouseScene").transform.localPosition + GlobalVariable.BidirectionalPositions[PilerNum, 2, 1]);
        BodyPartPositions = new Vector3[3];//堆垛机BodyPart目标位置0
        float TempValue1 = (Quaternion.Euler(RotationAngle)* (CargoPosition - PilerForks.transform.position)).z;
        float tempvalue1 = (GlobalVariable.BidirectionalPositions[PilerNum,2,1]-Cargo.transform.localPosition).z;
        BodyPartPositions[0] = PilerBodyPart.transform.localPosition;//Body初始位置
        BodyPartPositions[1] = BodyPartPositions[0];
        BodyPartPositions[1].z = BodyPartPositions[0].z + TempValue1;//Body关键位置1；到达货物z位置
        BodyPartPositions[2] = BodyPartPositions[1];
        BodyPartPositions[2].z = BodyPartPositions[1].z + tempvalue1;//Body关键位置2，放货位置（双向输送线1位置）
        #endregion
        //上下移动部分关键位置坐标
        #region
        UpPartPositions = new Vector3[7];
        float TempValue2 = (Quaternion.Euler(RotationAngle) * (CargoPosition - PilerForks.transform.position)).y + ForksSize.y / 2;
        UpPartPositions[0] = PilerUpPart.transform.localPosition;//堆垛机UpPart目标位置0,原位
        UpPartPositions[1] = UpPartPositions[0]; UpPartPositions[1].y = UpPartPositions[0].y + TempValue2;//堆垛机UpPart目标位置1，到达货物高度（没有抬升）
        UpPartPositions[2] = UpPartPositions[1];
        UpPartPositions[2].y = UpPartPositions[1].y + high - ForksSize.y;//堆垛机UpPart目标位置2；抬升到与货物接触
        UpPartPositions[3] = UpPartPositions[2];
        UpPartPositions[3].y = UpPartPositions[2].y + 0.2f;//堆垛机UpPart目标位置3；接触货物后抬高0.2m
        UpPartPositions[4] = UpPartPositions[3];
        float TempValue2_1 = GlobalVariable.KPD.HighValues[0]+OBJ1.transform.localPosition.y - (Quaternion.Euler(RotationAngle) * CargoPosition).y + high - ForksSize.y + 0.2f;
        UpPartPositions[4].y = UpPartPositions[1].y + TempValue2_1;//堆垛机UpPart目标位置4,下降到平台高度（叉组还需要下降）
        UpPartPositions[5] = UpPartPositions[4];
        UpPartPositions[5].y = UpPartPositions[1].y + TempValue2_1 - 0.2f;//堆垛机UpPart目标位置5；降下高度使货物与输送机接触
        UpPartPositions[6] = UpPartPositions[5];
        UpPartPositions[6].y = UpPartPositions[5].y - (high - ForksSize.y);//堆垛机UpPart目标位置6；降低高度使货叉与货物分离
        #endregion
        //货叉关键坐标
        #region
        float TempValue3 = 2 * ForksSize.x / 3;
        ForksPositions = new Vector3[3];
        ForksPositions[0] = PilerForks.transform.localPosition;//叉组位置0;原位
        ForksPositions[2] = ForksPositions[0];
        ForksPositions[2].x = ForksPositions[1].x + TempValue3;//叉组位置2;叉组向平台伸出2/3（放货）
        
        Fork1Positions = new Vector3[3];
        string name = "WarehouseScene/BeltConveyorGroup/BeltConveyors" + (PilerNum+1).ToString();
        Vector3 BeltConveyorPosition = GameObject.Find(name).transform.position;
        //伸出距离-叉组的2/3剩余的，由Fork1完成
        float TempValue4 = (Quaternion.Euler(RotationAngle) * (BeltConveyorPosition -  PilerPosition)).x - TempValue3;
        Fork1Positions[0] = PilerFork1.transform.localPosition;//叉1位置0；叉组伸出2/3长度
        Fork1Positions[2] = Fork1Positions[0];
        Fork1Positions[2].x = Fork1Positions[0].x + TempValue4;//叉1位置1；叉1伸出到货物位置
        //TempValue4_1是叉组伸出的总距离
        float TempValue4_1 = Mathf.Abs((Quaternion.Euler(RotationAngle) * (HighBayPosition - PilerPosition)).x);
        //顺序和入库不同，1位置奇偶不同
        //1是取货
        switch (Tempi1 % 2)
        {
            case 1:
                ForksPositions[1] = ForksPositions[0]; ForksPositions[1].x = ForksPositions[0].x + TempValue3;//叉组位置1
                Fork1Positions[1] = Fork1Positions[0]; Fork1Positions[1].x = Fork1Positions[0].x + (TempValue4_1 - TempValue3);//叉1位置1
                break;
            case 0:
                ForksPositions[1] = ForksPositions[0]; ForksPositions[1].x = ForksPositions[0].x - TempValue3;
                Fork1Positions[1] = Fork1Positions[0]; Fork1Positions[1].x = Fork1Positions[0].x - (TempValue4_1 - TempValue3);
                break;
        }
        #endregion
        #endregion
    }

    // Update is called once per frame
    void FixedUpdate () {

        if (finish[0] == false)
        {
            //BodyPart到达货物z位置
            PilerBodyPart.transform.localPosition = Vector3.MoveTowards(PilerBodyPart.transform.localPosition, BodyPartPositions[1], Speed * Time.deltaTime);
            //UpPart到达货物y高度（未抬升）
            PilerUpPart.transform.localPosition = Vector3.MoveTowards(PilerUpPart.transform.localPosition, UpPartPositions[1], Speed * Time.deltaTime);
            if (PilerBodyPart.transform.localPosition == BodyPartPositions[1] && PilerUpPart.transform.localPosition == UpPartPositions[1])
            {
                //叉组前伸2/3
                PilerForks.transform.localPosition = Vector3.MoveTowards(PilerForks.transform.localPosition, ForksPositions[1], Speed * Time.deltaTime);
                if (PilerForks.transform.localPosition == ForksPositions[1])
                {
                    //Fork1伸出剩余部分，准备抬升
                    PilerFork1.transform.localPosition = Vector3.MoveTowards(PilerFork1.transform.localPosition, Fork1Positions[1], Speed * Time.deltaTime);
                    if (PilerFork1.transform.localPosition == Fork1Positions[1])
                    {
                        finish[0] = true;
                    }
                }
            }
        }

        if (finish[0] == true && finish[1] == false)
        {
            //UpPart抬升到叉组接触货物
            PilerUpPart.transform.localPosition = Vector3.MoveTowards(PilerUpPart.transform.localPosition, UpPartPositions[2], Speed * Time.deltaTime);
            if (PilerUpPart.transform.localPosition == UpPartPositions[2])
            {
                //货物变为Fork1子组件
                Cargo.transform.parent = PilerFork1.transform;
                finish[1] = true;
                GameObject.Find("ProcessInterface/MainBody/Scroll View/Viewport/Content").transform.Find(Cargo.name).transform.Find("State").GetComponent<Text>().text = "货物状态：" + "正在出库";//修改进程中该货物状态信息
                Functions.ChangeColor(Cargo.name, StorageBinState.OutStore);//修改该货物对应仓位的状态表示颜色
                //开始出库
                Cargo.GetComponent<OperatingState>().state = CargoState.Exit;
            }
        }

        if (finish[1] == true && finish[2] == false)
        {
            //UpPart继续抬升，将货物带离平台
            PilerUpPart.transform.localPosition = Vector3.MoveTowards(PilerUpPart.transform.localPosition, UpPartPositions[3], Speed * Time.deltaTime);
            if (PilerUpPart.transform.localPosition == UpPartPositions[3])
            {
                //Fork1收回
                PilerFork1.transform.localPosition = Vector3.MoveTowards(PilerFork1.transform.localPosition, Fork1Positions[0], Speed * Time.deltaTime);
                if (PilerFork1.transform.localPosition == Fork1Positions[0])
                {
                    //叉组收回，此时货物回到UpPart平台上
                    PilerForks.transform.localPosition = Vector3.MoveTowards(PilerForks.transform.localPosition, ForksPositions[0], Speed * Time.deltaTime);
                    if (PilerForks.transform.localPosition == ForksPositions[0])
                    {
                        finish[2] = true;
                    }
                }
            }
        }

        if (finish[2] == true && finish[3] == false)
        {
            //BodyPart到达双向输送机末端
            PilerBodyPart.transform.localPosition = Vector3.MoveTowards(PilerBodyPart.transform.localPosition, BodyPartPositions[2], Speed * Time.deltaTime);
            //UpPart下降到双向输送机高度
            //UpPart还是抬升一部分的
            PilerUpPart.transform.localPosition = Vector3.MoveTowards(PilerUpPart.transform.localPosition, UpPartPositions[4], Speed * Time.deltaTime);
            if (PilerBodyPart.transform.localPosition == BodyPartPositions[2] && PilerUpPart.transform.localPosition == UpPartPositions[4])
            {
                //叉组伸出2/3，准备放货
                PilerForks.transform.localPosition = Vector3.MoveTowards(PilerForks.transform.localPosition, ForksPositions[2], Speed * Time.deltaTime);
                if (PilerForks.transform.localPosition == ForksPositions[2])
                {
                    //Fork1伸出剩余部分
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
            //UpPart下降，叉组和货物底部接触
            PilerUpPart.transform.localPosition = Vector3.MoveTowards(PilerUpPart.transform.localPosition, UpPartPositions[5], Speed * Time.deltaTime);
            if (PilerUpPart.transform.localPosition == UpPartPositions[5])
            {
                //货物变为场景子物体
                Cargo.transform.parent = OBJ1.transform;
                finish[4] = true;
            }
        }

        if (finish[4] == true && finish[5]==false)
        {
            //UpPart下落到与平台一致
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
                        //全部结束
                        finish[5] = true;
                        Accomplish = true;
                        GlobalVariable.PilersState[PilerNum] = State.Off;
                        GlobalVariable.BidirectionalConveyorStates[PilerNum, 2, 1] = State.On;
                        GlobalVariable.Wait[PilerNum] = WaitState.WaitExit;//似乎没有用
                        GlobalVariable.ConveyorQueue[PilerNum].Peek().AddComponent<CargoExit>().enabled = false;
                        GlobalVariable.ConveyorQueue[PilerNum].Peek().GetComponent<CargoExit>().Speed = Speed;
                        //Piler的出库动画完成
                        //从排队队列中取出一个放入出库队列,
                        GlobalVariable.ExitQueue[PilerNum].Enqueue(GlobalVariable.ConveyorQueue[PilerNum].Peek());
                        //排队队列减少一个（出库优先）
                        GlobalVariable.ConveyorQueue[PilerNum].Dequeue();
                        DestroyImmediate(this.GetComponent<PilerOfExit>());
                    }
                }
            }
        }

        
	}
}
