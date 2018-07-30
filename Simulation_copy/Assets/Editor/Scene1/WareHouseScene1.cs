using UnityEditor;
using UnityEngine;
using System;
using System.IO;



public struct ParametersList1
{
    public float Distance;//场景坐标点到坐标原点的Z距离
    public HighStoreShelf_Parameter HP;
    public MultiHighBay_Parameter MHP;
    public RollerConveyor_Parameter RCP;
    public PilerParameter PP;
    public Vector3 CargoSize;//货物尺寸
    public Vector3 PlatFormSize;//平台尺寸
}
public class WareHouseScene1 : EditorWindow
{
    private static WareHouseScene1 window;

    string path1 = @"Assets/Resources/Scene/高架库参数表";
    int Num = 8;
    //int Order = 0;
    float TunnelWidth = 1.6f;//堆垛机间巷道距离
    float HookupDistance = 0.4f;//堆垛机之间的连接距离
    string Name = "WarehouseScene";
    string path2 = "Assets/Resources/Scene/Simulation/";

    float enterRollerLength = 0;
    float exitRollerLength = 0;
    [MenuItem("CustomObject/WareHouseScene1", priority = 0)]
    static void WareHouseScene_Tool()
    {
        Rect window_position1 = new Rect(500, 1000, 300, 400);
        //获取当前窗口实例
        window = EditorWindow.GetWindowWithRect<WareHouseScene1>(window_position1);
        //显示窗口
        window.Show();
    }

    private void OnGUI()
    {
        //获取用户输入
        path1 = EditorGUILayout.TextField("高架库设计参数文件：", path1);
        Num = EditorGUILayout.IntField("高架库数目：", Num);
        TunnelWidth = EditorGUILayout.FloatField("高架库间巷道距离：", TunnelWidth);
        HookupDistance = EditorGUILayout.FloatField("高架库间连接距离：", HookupDistance);
        Name = EditorGUILayout.TextField("场景名称：", Name);
        path2 = EditorGUILayout.TextField("场景输出路径：", path2);
        enterRollerLength = EditorGUILayout.FloatField("入口滚筒输送机长度：", enterRollerLength);
        exitRollerLength = EditorGUILayout.FloatField("出口滚筒输送机长度：", exitRollerLength);

        HighStoreShelf_Parameter HP; string Path1 = path1 + ".txt";
        Subassembly.HighBay_ReadTxt(ref Path1, out HP);
        MultiHighBay_Parameter MHP;
        MHP.Num = Num; MHP.TunnelWidth = TunnelWidth; MHP.HookupDistance = HookupDistance;
        ParametersList1 PL;
        PL.HP = HP; PL.MHP = MHP; PL.Distance = 0;//坐标点到场景起点的Z距离
        CreateSize(ref HP, ref MHP, out PL);
        PositionsList POL;

        if (GUILayout.Button("生成场景", GUILayout.Height(20)))
        {
            Create_PositionsList(ref PL, out POL);
            string Path2 = path2 + Name;
            Create_Scene(PL, POL, Path2);
            //保存场景
            //Object tempPrefab = EditorUtility.CreateEmptyPrefab(path2 + Name + ".prefab");
            //根据场景输送关键数据
        }

        //if (GUILayout.Button("生成货箱", GUILayout.Height(20)))
        //{
        //    GameObject Cargo = new GameObject(); Cargo.name = "Cargo";
        //    CreateCargo(PL.CargoSize, Cargo);

        //    //Debug.Log(0);
        //}

    }
    //创建仓库场景
    #region Create_Scene
    public void Create_Scene(ParametersList1 PL, PositionsList POL, string Path)
    {
        HighStoreShelf_Parameter HP = PL.HP;
        MultiHighBay_Parameter MHP = PL.MHP;
        RollerConveyor_Parameter RCP = PL.RCP;
        float TempValue1 = RCP.RCLength;
        float TempValue2 = RCP.RCWidth;
        float TempValue3 = TempValue2 - HP.Size.x;
        GameObject WarehouseScene = new GameObject(); WarehouseScene.name = Name;//创建场景
        //存放货物
        GameObject Cargos = new GameObject();
        Cargos.name = "Cargos"; Cargos.transform.parent = WarehouseScene.transform;
        Cargos.transform.localPosition = new Vector3(0, 0, 0);
        //添加地面
        GameObject obj = (GameObject)Resources.Load("Scene/Dimian");
        GameObject dimian = Instantiate(obj);
        float Width = HP.Size.z + 8 * RCP.RCLength;
        dimian.transform.localScale = new Vector3(Width, 0.1f, Width);
        dimian.transform.parent = WarehouseScene.transform;
        dimian.transform.localPosition = new Vector3(-(Width / 2 - RCP.RCLength * 4), -0.05f, -(Width / 2 - 2 * RCP.RCLength));
        //添加入口滚筒输送机
        GameObject EnterRollerConveyor = new GameObject(); 
        RollerConveyorType type = RollerConveyorType.Intact;
        RollerConveyor_Parameter RCP2 = new RollerConveyor_Parameter();
        RCP2 = PL.RCP; RCP2.RCHigh = RCP2.RCHigh - 0.1f;
        RCP2.RCLength = enterRollerLength;//长度自定义
        //RCP2.RCLength = PL.HP.Size.x * 2 + PL.MHP.HookupDistance + PL.MHP.TunnelWidth - RCP2.RCWidth;
        RollerConveyor.Create_RollerConveyor(RCP2, EnterRollerConveyor, type);
        EnterRollerConveyor.name = "EnterRollerConveyor";
        EnterRollerConveyor.transform.parent = WarehouseScene.transform;
        EnterRollerConveyor.transform.Rotate(0, -90, 0);
        EnterRollerConveyor.transform.localPosition = new Vector3((RCP2.RCLength + TempValue3), 0, -TempValue2 / 2);
        //添加入口顶升移栽机
        LiftTransferParameter LTP = new LiftTransferParameter();
        LTP.High = PL.RCP.RCHigh - 0.1f;
        LTP.Width = PL.RCP.RCWidth;
        LTP.RollerRadius = PL.RCP.RollerRadius;
        LTP.GearDiameter = PL.RCP.RollerRadius * 2;
        GameObject enterLiftTransfer = new GameObject();
        LiftTransfer1.CreateLiftTransfer(enterLiftTransfer, ref LTP);
        enterLiftTransfer.name = "EnterLiftTransfer";
        enterLiftTransfer.transform.parent = WarehouseScene.transform;
        float tempEnterX = RCP2.RCLength - (PL.HP.Size.x - LTP.Width) + LTP.Width / 2;//入口顶升的X坐标
        enterLiftTransfer.transform.localPosition = new Vector3(tempEnterX, 0, -TempValue2 / 2);
        //添加入口皮带输送机
        GameObject EnterBeltConveyors = new GameObject(); EnterBeltConveyors.name = "EnterBeltConveyors";
        Create_EnterBeltConveyors(PL, EnterBeltConveyors);
        EnterBeltConveyors.transform.parent = WarehouseScene.transform;
        EnterBeltConveyors.transform.localPosition = new Vector3(tempEnterX, 0, 0);
        //添加顶升移载机设备
        GameObject LiftTransferGroup = new GameObject(); LiftTransferGroup.name = "LiftTransferGroup";
        Create_LiftTransferGroup(PL, LiftTransferGroup);
        LiftTransferGroup.transform.parent = WarehouseScene.transform;
        LiftTransferGroup.transform.localPosition = new Vector3(0, 0, -TempValue2 / 2);
        //添加滚筒输送机（垂直于高架库方向）
        GameObject RollerConveyorGroup = new GameObject(); RollerConveyorGroup.name = "RollerConveyorGroup";
        Create_RollerConveyorGroup(PL, RollerConveyorGroup);
        RollerConveyorGroup.transform.parent = WarehouseScene.transform;
        RollerConveyorGroup.transform.localPosition = new Vector3(0, 0, -TempValue2 / 2);
        //添加皮带输送机（平行于高架库方向）
        GameObject BeltConveyorGroup = new GameObject(); BeltConveyorGroup.name = "BeltConveyorGroup";
        Create_BeltConveyorGroup(PL, BeltConveyorGroup);
        BeltConveyorGroup.transform.parent = WarehouseScene.transform;
        BeltConveyorGroup.transform.localPosition = new Vector3(0, 0, -TempValue2);
        //为场景添加高架库设备
        GameObject HighBays1 = new GameObject(); HighBays1.name = "HighBayGroup";
        HighBay2.Create_HighBays(HP, MHP, HighBays1);
        HighBays1.transform.parent = WarehouseScene.transform;
        HighBays1.transform.localPosition = new Vector3(0, 0, -(3 * TempValue1 + TempValue2));
        //为场景添加堆垛机设备
        GameObject PilerGroup = new GameObject(); PilerGroup.name = "PilerGroup";
        Create_Pilers(PL, PilerGroup);
        PilerGroup.transform.parent = WarehouseScene.transform;
        PilerGroup.transform.localPosition = new Vector3(0, 0, -(TempValue1 + TempValue2));
        //添加出口滚筒输送机
        GameObject ExitRollerConveyor = new GameObject();
        RollerConveyor_Parameter exitRCP = PL.RCP;
        exitRCP.RCLength = exitRollerLength; exitRCP.RCHigh = PL.RCP.RCHigh - 0.1f;
        RollerConveyor.Create_RollerConveyor(exitRCP, ExitRollerConveyor, RollerConveyorType.Intact);
        ExitRollerConveyor.name = "ExitRollerConveyor";
        ExitRollerConveyor.transform.parent = WarehouseScene.transform;
        int j = (PL.MHP.Num + 1) / 2 - 1;
        float tempExitX = -PL.HP.Size.x - j * (PL.HP.Size.x * 2 + PL.MHP.HookupDistance + PL.MHP.TunnelWidth);
        ExitRollerConveyor.transform.Rotate(0, -90, 0);
        ExitRollerConveyor.transform.localPosition = new Vector3(tempExitX, 0, -TempValue2 / 2);
        //添加出口顶升移栽机
        GameObject ExitLiftTransfer = new GameObject();
        ExitLiftTransfer.name = "ExitLiftTransfer";
        LiftTransfer1.CreateLiftTransfer(ExitLiftTransfer, ref LTP);
        ExitLiftTransfer.transform.parent = WarehouseScene.transform;
        tempExitX = tempExitX - exitRollerLength - LTP.Width / 2;
        ExitLiftTransfer.transform.localPosition = new Vector3(tempExitX, 0, -TempValue2 / 2);
        //添加出口皮带输送线
        GameObject ExitBeltConveyors = new GameObject();
        ExitBeltConveyors.name = "ExitBeltConveyors";
        Create_ExitBeltConveyors(PL, ExitBeltConveyors);
        ExitBeltConveyors.transform.parent = WarehouseScene.transform;
        ExitBeltConveyors.transform.localPosition = new Vector3(tempExitX, 0, 0);
        //给场景添加Message
        WarehouseScene.AddComponent<ShowKeyPositionData>();
        ShowKeyPositionData ShowKeyData = WarehouseScene.GetComponent<ShowKeyPositionData>();
        KeyPositionsData KPD = new KeyPositionsData();
        //float[] HighValues = { 1.2f, 1.1f };
        //SI.HighValues = HighValues;
        Create_SceneInfo(ref PL, out KPD);
        ShowKeyData.KeyPositionsData = KPD;
        //Debug.Log(KPD.ConveyorLengths);
        //WarehouseScene.AddComponent<SceneData>();
        //SceneData SD = WarehouseScene.GetComponent<SceneData>();
        //Information sd = new Information(); sd.Name = "Name"; sd.Num = 2;
        ////WarehouseScene.GetComponent<SceneData>().
        //SD.IN = sd;
        GameObject Cargo = new GameObject(); Cargo.name = "Cargo";
        Cargo1.Create_Cargo(KPD.CargoSize,Cargo);
        //SD.IN.Num = 2; SD.IN.Name = "A";
        //输出场景
        MyClass.CreatePrefab(WarehouseScene, Path);
        MyClass.CreatePrefab(Cargo, path2+Cargo.name);

        //根据参数预创建一个存储状态面板
        //GameObject StorageStateInterface = new GameObject();
        //StorageStateInterface.name = "StorageStateInterface";
        BinsPanel.Ajustment(MHP.Num, HP.FloorsNum, HP.ColumnsNum);
        BinsPanel.Ajustment2(MHP.Num, HP.FloorsNum, HP.ColumnsNum);
        //GameObject.Find("ScriptsContainer").GetComponent<MyClass>().CreatePrefab(StorageStateInterface, path2 +"Simulation/" +StorageStateInterface.name);
    }
    #endregion
    //创建多线路堆垛机
    #region Create_Pilers
    public void Create_Pilers(ParametersList1 PL, GameObject OBJ)
    {
        float Depth = PL.HP.Size.x;
        float TunnelWidth = PL.MHP.TunnelWidth;
        float HookupDistance = PL.MHP.HookupDistance;
        GameObject OBJ1 = new GameObject(); OBJ1.name = "Piler";
        Piler1.Create(PL.PP, OBJ1);
        //修改叉之间距离
        #region
        GameObject Fork1 = OBJ1.transform.Find("BodyPart").transform.Find("UpPart").transform.Find("Forks").transform.Find("Fork1").gameObject;
        GameObject Fork2 = OBJ1.transform.Find("BodyPart").transform.Find("UpPart").transform.Find("Forks").transform.Find("Fork2").gameObject;
        GameObject PartFork1 = OBJ1.transform.Find("BodyPart").transform.Find("UpPart").transform.Find("Part").transform.Find("叉").gameObject;
        GameObject PartFork2 = OBJ1.transform.Find("BodyPart").transform.Find("UpPart").transform.Find("Part").transform.Find("叉_1").gameObject;
        DestroyImmediate(OBJ1.transform.Find("BodyPart").transform.Find("UpPart").transform.Find("Part").transform.Find("螺丝").gameObject);//删除螺丝
        float distance = Fork1.transform.Find("Fork1_1").transform.localPosition.y - Fork1.transform.Find("Fork1_2").transform.localPosition.y;
        Vector3 ForksSize = new Vector3();
        MyClass.MeshSize(Fork1, ref ForksSize);
        float tempvalue = (PL.CargoSize.z - ForksSize.z - 0.25f) / 2; Debug.Log(tempvalue);
        Vector3 position1 = Fork1.transform.Find("Fork1_1").transform.localPosition; position1.y = position1.y + tempvalue; Fork1.transform.Find("Fork1_1").transform.localPosition = position1;
        Vector3 position2 = Fork1.transform.Find("Fork1_2").transform.localPosition; position2.y = position2.y - tempvalue; Fork1.transform.Find("Fork1_2").transform.localPosition = position2;
        Vector3 position3 = Fork2.transform.Find("Fork2_1").transform.localPosition; position3.y = position3.y + tempvalue; Fork2.transform.Find("Fork2_1").transform.localPosition = position3;
        Vector3 position4 = Fork2.transform.Find("Fork2_2").transform.localPosition; position4.y = position4.y - tempvalue; Fork2.transform.Find("Fork2_2").transform.localPosition = position4;
        Vector3 position5 = PartFork1.transform.localPosition; position5.y = position5.y - tempvalue; PartFork1.transform.localPosition = position5;
        Vector3 position6 = PartFork2.transform.localPosition; position6.y = position6.y + tempvalue; PartFork2.transform.localPosition = position6;
        #endregion
        int NumofPilers = (PL.MHP.Num + 1) / 2;//所需堆垛机个数
        for (int i = 0; i < NumofPilers; i++)
        {
            GameObject clone = Instantiate(OBJ1); clone.name = OBJ1.name + (i + 1).ToString();
            clone.transform.parent = OBJ.transform;
            float TempValue = -(TunnelWidth / 2 + i * (TunnelWidth + HookupDistance) + (2 * i + 1) * Depth);
            clone.transform.localPosition = new Vector3(TempValue, 0, 0);
        }
        DestroyImmediate(OBJ1);
    }
    #endregion
    //多线路输送机(平行于高架库方向)
    #region Create_BeltConveyorGroup
    public void Create_BeltConveyorGroup(ParametersList1 PL, GameObject ConveyorGroup)
    {
        HighStoreShelf_Parameter HP = PL.HP;
        MultiHighBay_Parameter MHP = PL.MHP;
        //先创建一条输送线,再clone
        GameObject BeltConveyors = new GameObject();BeltConveyors.name = "BeltConveyors";
        GameObject BeltConveyor = new GameObject();BeltConveyor.name = "BeltConveyor";
        RollerConveyor.Create_BeltConveyor(PL.RCP, BeltConveyor);
        BeltConveyor.transform.Rotate(0, 180, 0);
        for (int i = 0;i < (PL.MHP.Num + 1) / 2; i++)
        {
            GameObject clone1 = Instantiate(BeltConveyors); clone1.name = BeltConveyors.name + (i + 1).ToString();
            for (int j = 0; j < 3; j++)
            {
                GameObject clone = Instantiate(BeltConveyor);
                clone.name = BeltConveyor.name + (i + 1) + "_" + (j + 1);
                clone.transform.parent = clone1.transform;
                float tempZ = PL.RCP.RCLength * j;
                clone.transform.localPosition = new Vector3(0, 0, -tempZ);
            }
            clone1.transform.parent = ConveyorGroup.transform;
            float TempValue1 = PL.RCP.RCWidth / 2 - ((2 * i + 1) * HP.Size.x + i * MHP.TunnelWidth + i * MHP.HookupDistance);
            clone1.transform.localPosition = new Vector3(TempValue1, 0, 0);
        }
        DestroyImmediate(BeltConveyor);
        DestroyImmediate(BeltConveyors);
    }
    #endregion
    //单向输送机（垂直于高架库方向）
    #region Create_RollerConveyorGroup
    public void Create_RollerConveyorGroup(ParametersList1 PL, GameObject ConveyorGroup)
    {
        RollerConveyor_Parameter RCP = new RollerConveyor_Parameter();
        RCP.RCHigh = PL.RCP.RCHigh - 0.1f;
        RCP.RCWidth = PL.RCP.RCWidth;
        RCP.RCLength = PL.MHP.HookupDistance + PL.MHP.TunnelWidth + 2 * PL.HP.Size.x - RCP.RCWidth;
        RCP.RollerRadius = PL.RCP.RollerRadius;

        GameObject Conveyor = new GameObject(); 
        RollerConveyorType type = RollerConveyorType.Intact;
        RollerConveyor.Create_RollerConveyor(RCP, Conveyor, type);
        Conveyor.name = "RollerConveyor";
        for (int i = 0; i < (PL.MHP.Num + 1) / 2 - 1; i++)
        {
            GameObject clone = Instantiate(Conveyor); clone.name = Conveyor.name + (i + 1).ToString();
            clone.transform.parent = ConveyorGroup.transform;
            float TempValue = -PL.HP.Size.x - i * (PL.HP.Size.x* 2 + PL.MHP.HookupDistance + PL.MHP.TunnelWidth);
            clone.transform.localPosition = new Vector3(TempValue, 0, 0);
            clone.transform.Rotate(0, -90, 0);
        }
        DestroyImmediate(Conveyor);
    }
    #endregion
    //创建顶升移栽组合
    #region Create_LiftTransferGroup
    public void Create_LiftTransferGroup(ParametersList1 PL, GameObject LiftTransferGroup)
    {
        LiftTransferParameter LTP = new LiftTransferParameter();
        LTP.High = PL.RCP.RCHigh - 0.1f;
        LTP.Width = PL.RCP.RCWidth;
        LTP.RollerRadius = PL.RCP.RollerRadius;
        LTP.GearDiameter = PL.RCP.RollerRadius * 2;

        GameObject LiftTransfer = new GameObject(); LiftTransfer.name = "LiftTransfer";
        LiftTransfer1.CreateLiftTransfer(LiftTransfer, ref LTP);
        for (int i = 0; i < (PL.MHP.Num + 1) / 2; i++)
        {
            GameObject clone = Instantiate(LiftTransfer); clone.name = LiftTransfer.name + (i + 1).ToString();
            clone.transform.parent = LiftTransferGroup.transform;
            float TempValue = PL.RCP.RCWidth / 2 - ((2 * i + 1) * PL.HP.Size.x + i * PL.MHP.TunnelWidth + i * PL.MHP.HookupDistance);
            clone.transform.localPosition = new Vector3(TempValue, 0, 0);
        }
        DestroyImmediate(LiftTransfer);
    }
    #endregion
    //创建入库的皮带输送机
    #region Create_EnterBeltConveyors
    public void Create_EnterBeltConveyors(ParametersList1 PL, GameObject BeltConveyors)
    {
        GameObject BeltConveyor = new GameObject();
        BeltConveyor.name = "EnterBeltConveyor";
        RollerConveyor.Create_BeltConveyor(PL.RCP, BeltConveyor);
        for (int i = 0; i < 2; i++)
        {
            GameObject clone = Instantiate(BeltConveyor);
            clone.name = BeltConveyor.name + (i + 1);
            clone.transform.parent = BeltConveyors.transform;
            float tempZ = PL.RCP.RCLength * i;
            clone.transform.localPosition = new Vector3(0, 0, tempZ);
        }
        DestroyImmediate(BeltConveyor);
    }
    #endregion
    //创建出库的皮带输送机
    #region Create_ExitBeltConveyors
    public void Create_ExitBeltConveyors(ParametersList1 PL, GameObject BeltConveyors)
    {
        GameObject BeltConveyor = new GameObject();
        BeltConveyor.name = "ExitBeltConveyor";
        RollerConveyor.Create_BeltConveyor(PL.RCP, BeltConveyor);
        for (int i = 0; i < 2; i++)
        {
            GameObject clone = Instantiate(BeltConveyor);
            clone.name = BeltConveyor.name + (i + 1);
            clone.transform.parent = BeltConveyors.transform;
            float tempZ = PL.RCP.RCLength * i;
            clone.transform.localPosition = new Vector3(0, 0, tempZ);
        }
        DestroyImmediate(BeltConveyor);
    }
    #endregion


//创建货物
#region CreateCargo
public void CreateCargo(Vector3 CargoSize, GameObject Cargo)
    {
        Cargo1.Create_Cargo(CargoSize, Cargo);
        Debug.Log(CargoSize.x);
        Debug.Log(CargoSize.y);
        Debug.Log(CargoSize.z);
    }
    #endregion
    //计算产生参数
    #region CreateSize
    public void CreateSize(ref HighStoreShelf_Parameter HP, ref MultiHighBay_Parameter MHP, out ParametersList1 PAL)
    {
        //Size1是货物的尺寸；RCP是输送机尺寸
        //ParametersList PL = new ParametersList();
        float Distance0 = HP.ColumnWidth * 2;//坐标点到场景起点的Z距离

        float tempsize = HP.VerticalStanchionWidth;


        Vector3 CargoSize;
        //CargoSize.x = HP.Size.x;
        //CargoSize.y = HP.Size.x;
        //CargoSize.z = HP.Size.x;
        CargoSize = new Vector3(0.4f, 0.4f, 0.4f);


        RollerConveyor_Parameter RCP;
        RCP.RCLength = HP.ColumnWidth;//输送机的长度设为高架库单列宽度
        RCP.RollerRadius = 0.05f;//滚筒半径
        RCP.RCWidth = CargoSize.x + 4 * RCP.RollerRadius;//货物的X尺寸+2*滚筒半径+tempsize
        RCP.RCHigh = 1.1f;//输送机高度设为1.2m

        PilerParameter PP;
        PP.PilerHigh = HP.Size.y;
        PP.PilerLength = HP.Size.z+2*HP.ColumnWidth;

        Vector3 PlatFormSize;
        PlatFormSize.x = CargoSize.x + 2 * RCP.RollerRadius + tempsize; ;//平台X尺寸与输送机宽度一致
        PlatFormSize.y = RCP.RCHigh - RCP.RollerRadius / 8;//平台高度与输送线高度一致，低于输送机高度为滚筒半径的1/8
        PlatFormSize.z = CargoSize.z + CargoSize.z / 10;

        ParametersList1 PL;
        PL.Distance = Distance0; PL.HP = HP; PL.MHP = MHP; PL.RCP = RCP; PL.PP = PP;
        PL.CargoSize = CargoSize; PL.PlatFormSize = PlatFormSize;
        PAL = PL;
        //Debug.Log(PL.HP.HighBaySize[1]);
    }
    #endregion
    //输出多线路运动数据
    #region Create_PositionsList
    public void Create_PositionsList(ref ParametersList1 PL, out PositionsList PositonsList)
    {
        HighStoreShelf_Parameter HP = PL.HP;
        MultiHighBay_Parameter MHP = PL.MHP;
        RollerConveyor_Parameter RCP = PL.RCP;
        float ConveryorsLength = 2 * HP.ColumnWidth;
        //高架库位置信息
        //HighBayPositions[i,0]记录第i个高架库坐标的X值，[i,1]记录第i个高架库坐标的Y值，[i,2]记录第i个高架库坐标的Z值;
        float[,] HighBayPositions = new float[MHP.Num, 3];
        for (int i = 0; i < MHP.Num; i++)
        {
            HighBayPositions[i, 0] = -(HP.Size.x / 2 + i * HP.Size.x + ((i + 1) / 2) * MHP.TunnelWidth + (i / 2) * MHP.HookupDistance);
            HighBayPositions[i, 1] = 0; HighBayPositions[i, 2] = -PL.Distance - ConveryorsLength + HP.ColumnWidth;
        }
        //输送线的数据
        //ConveyorPositions[i,0]记录第i个高架库坐标的X值，[i,1]记录第i个高架库坐标的Y值，[i,2]记录第i个高架库坐标的Z值;
        //[i,4]记录输送线的长度，
        float[,] ConveyorPositions = new float[MHP.Num, 4];
        for (int i = 0; i < MHP.Num; i++)
        {
            ConveyorPositions[i, 1] = 0; ConveyorPositions[i, 2] = -PL.Distance;
            ConveyorPositions[i, 3] = 2 * HP.ColumnWidth;
            switch (i % 2)
            {
                case 0:
                    ConveyorPositions[i, 0] = -((i + 1) * HP.Size.x + (i / 2) * MHP.TunnelWidth + (i / 2) * MHP.HookupDistance);
                    ConveyorPositions[i, 0] = ConveyorPositions[i, 0] + RCP.RCWidth / 2;
                    break;
                case 1:
                    ConveyorPositions[i, 0] = -(i * HP.Size.x + ((i + 1) / 2) * MHP.TunnelWidth + (i / 2) * MHP.HookupDistance);
                    ConveyorPositions[i, 0] = ConveyorPositions[i, 0] - RCP.RCWidth / 2;
                    break;
            }

        }
        //堆垛机的坐标数据
        //PilerPositions[i,0]记录第i个高架库坐标的X值，[i,1]记录第i个高架库坐标的Y值，[i,2]记录第i个高架库坐标的Z值;
        float[,] PilerPositions = new float[(MHP.Num + 1) / 2, 3];
        for (int i = 0; i < (MHP.Num + 1) / 2; i++)
        {
            PilerPositions[i, 0] = -(MHP.TunnelWidth / 2 + i * (MHP.TunnelWidth + MHP.HookupDistance) + (2 * i + 1) * HP.Size.x);
            PilerPositions[i, 1] = 0; PilerPositions[i, 2] = -PL.Distance - ConveryorsLength + 2 * HP.ColumnWidth;
        }
        //高架库仓位数据
        float[,,,] StorageBinPositions = new float[HP.FloorsNum, HP.ColumnsNum, 2, 3];
        float temphigh = HP.FloorsHigh[0];
        for (int i = 0; i < HP.FloorsNum; i++)
        {

            for (int j = 0; j < HP.ColumnsNum; j++)
            {
                StorageBinPositions[i, j, 0, 0] = 0; StorageBinPositions[i, j, 0, 1] = temphigh;
                StorageBinPositions[i, j, 0, 2] = -(HP.ColumnWidth * (j + 1) + HP.ColumnWidth / 4);
                StorageBinPositions[i, j, 1, 0] = 0; StorageBinPositions[i, j, 1, 1] = temphigh;
                StorageBinPositions[i, j, 1, 2] = -(HP.ColumnWidth * (j + 1) + 3 * HP.ColumnWidth / 4);
            }
            temphigh = temphigh + HP.FloorsHigh[i + 1];
        }
        //
        PositionsList POL;
        POL.HighBayPositions = HighBayPositions; POL.PilerPositons = PilerPositions; POL.ConveyorPositons = ConveyorPositions;
        POL.StorageBinPositions = StorageBinPositions;
        PositonsList = POL;
    }
    #endregion
    //场景关键点信息
    #region Create_SceneInfo
    public void Create_SceneInfo(ref ParametersList1 PL, out KeyPositionsData KPD)
    {
        RollerConveyor_Parameter RCP = PL.RCP;
        HighStoreShelf_Parameter HP = PL.HP;
        MultiHighBay_Parameter MHP = PL.MHP;
        KeyPositionsData KP = new KeyPositionsData();
        KP.HighBaysNum = MHP.Num;//高架库数目
        
        //Vector3[] LiftTransferPositions = new Vector3[]
        KP.HighValues = new float[] { RCP.RCHigh, RCP.RCHigh - 0.1f };//两种输送线高度
        KP.ConveyorLengths = new float[2] { RCP.RCLength, HP.Size.x * 2 + MHP.HookupDistance + MHP.TunnelWidth - RCP.RCWidth };
        KP.EnterPosition = new Vector3((KP.ConveyorLengths[1] + RCP.RCWidth - HP.Size.x), RCP.RCHigh - 0.1f, -RCP.RCWidth / 2);//入口处坐标
        float[] ConveyorLinesValues = new float[(MHP.Num + 1) / 2];
        float[] PilerLinesValues = new float[(MHP.Num + 1) / 2];
        
        for (int i = 0; i < (MHP.Num + 1) / 2; i++)
        {
            ConveyorLinesValues[i] = -(-RCP.RCWidth / 2 + (2 * i + 1) * HP.Size.x + i * MHP.TunnelWidth + i * MHP.HookupDistance);
            PilerLinesValues[i] = -(MHP.TunnelWidth / 2 + i * (MHP.TunnelWidth + MHP.HookupDistance) + (2 * i + 1) * HP.Size.x);
        }
        KP.ConveyorLinesValues = ConveyorLinesValues;//每条入库输送线的X值
        //KP.ConveyorLinesLength = 3 * RCP.RCLength;//入库输送线的长度
        KP.ConveyorWidth = RCP.RCWidth;//输送线宽度
        KP.PilerLinesValues = PilerLinesValues;//每条堆垛机线路的X值
        Vector3[] HighBaysPositions = new Vector3[MHP.Num];
        for (int i = 0; i < MHP.Num; i++)
        {
            float TempValue = HP.Size.x / 2 + i * HP.Size.x + ((i + 1) / 2) * MHP.TunnelWidth + (i / 2) * MHP.HookupDistance;
            HighBaysPositions[i] = new Vector3(-TempValue, 0, -(RCP.RCWidth + 2 * RCP.RCLength));
        }
        KP.HighBaysPositions = HighBaysPositions;//高架库的坐标
        KP.CargoSize = PL.CargoSize;//货物尺寸

        StorePositions StorePositions = new StorePositions();
        float[] StoreFloorPositions = new float[HP.FloorsNum];
        float[] StoreColumnPositions = new float[HP.ColumnsNum];
        float TempValue1 = 0;
        for (int i = 0; i < HP.FloorsNum; i++)
        {
            TempValue1 += HP.FloorsHigh[i];
            StoreFloorPositions[i] = TempValue1;
        }
        for (int j = 0; j < HP.ColumnsNum; j++)
        {
            StoreColumnPositions[j] = -(HP.ColumnWidth / 2 + (j + 1) * HP.ColumnWidth);
        }
        float[] StorePlacePosition = { HP.ColumnWidth / 4, -HP.ColumnWidth / 4 };
        StorePositions.StoreFloorPositions = StoreFloorPositions;
        StorePositions.StoreColumnPositions = StoreColumnPositions;
        StorePositions.StorePlacePosition = StorePlacePosition;
        KP.StorePositions = StorePositions;//高架库仓位坐标信息
        //货物入口坐标（相对于设备）
        KP.CargoEnterPosition = new Vector3(0, KP.HighValues[0], KP.ConveyorLengths[0]);
        KPD = KP;
    }
    #endregion
}

