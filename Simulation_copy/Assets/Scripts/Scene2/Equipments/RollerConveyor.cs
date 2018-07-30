using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public enum ConveyorType//输送机类型
{
    RollerConveyor = 0,
    BeltConveyor = 1
}
public enum ConveyorDirection//输送机排列方向
{
    XAxisPlus = 0,//X正方向展开
    XAxisMinus = 1,//Xfu方向展开
    ZAxisPlus = 2,//Z正方向展开
    ZAxisMinus = 3//Z负方向展开
}
public enum RollerConveyorType {
    Intact = 0,Origion=1, Terminus=2, Transition=3
}

public struct RollerConveyor_Parameter
{
    public float RCLength;//滚筒输送机长度
    public float RCWidth;//滚筒输送机宽度
    public float RCHigh;//滚筒输送机高度
    public float RollerRadius;//滚筒半径
    //public float BoardHigh;//边板部分高度
    //public float BoardWidth;//单边板宽度
}
public struct Conveyor_Parameter
{
    public float Length;//输送线长度
    public int Num;//所需输送机个数
    public ConveyorType Type;//输送机类型
    public ConveyorDirection Direction;////输送机展开排列方向
    public RollerConveyor_Parameter RCP;//输送机参数；长宽高，滚筒半径
}

public class RollerConveyor : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        float RCLength = 1.8f, RCWidth = 0.9f, RCHigh = 0.65f, RollerR = 0.045f;
        RollerConveyor_Parameter RCP;
        RCP.RCLength = RCLength;
        RCP.RCWidth = RCWidth;
        RCP.RCHigh = RCHigh;
        RCP.RollerRadius = RollerR;
        //SRCP.BoardWidth = RollerR;
        //RCP.BoardHigh = 2 * RollerR + RollerR / 4;
    }

    // Update is called once per frame
    void Update()
    {

    }
    //创建滚筒输送机滚筒部分
    #region Create_RollerPart
    public static void Create_RollerPart(RollerConveyor_Parameter RCP, GameObject RollerPart)
    {
        //创建滚筒输送部分
        float RCLength = RCP.RCLength;//滚筒输送机长度
        float RCWidth = RCP.RCWidth;//滚筒输送机宽度
        float RollerR = RCP.RollerRadius;//滚筒输送机半径
        float RollerLength = RCWidth - 0.002f;
        float BoardHigh = 2 * RCP.RollerRadius;//边板的高度设为滚筒输送机半径的2.25倍
        float interval = 4 * RollerR / 3;//初始化滚筒间距离
        float rn = RCLength / (2 * RollerR + interval);//浮点数不能做求余运算
        int RollerNum = (int)Math.Ceiling(rn);
        interval = RCLength / RollerNum - 2 * RollerR;//根据滚筒输送机长度调整后的滚筒间距

        GameObject Roller = Resources.Load("Scene/RollerConveyor/RollerX") as GameObject;
        for (int i = 0; i < RollerNum; i++)
        {
            GameObject clone = Instantiate(Roller); clone.name = "Roller" + (i + 1).ToString();
            clone.transform.localScale = new Vector3(RollerLength, 2 * RollerR, 2 * RollerR);
            clone.transform.parent = RollerPart.transform;
            clone.transform.localPosition = new Vector3(0, -BoardHigh / 2, interval / 2 + RollerR + i * (2 * RollerR + interval));
        }
    }
    #endregion
    //创建滚筒输送机边板部分
    #region Create_SideBoard
    public static void Create_SideBoard(RollerConveyor_Parameter RCP, GameObject SideBoard)
    {
        //创建滚筒输送机边板部分
        float RCLength = RCP.RCLength;//滚筒输送机长度
        float RollerR = RCP.RollerRadius;//滚筒输送机半径
        float RCWidth = RCP.RCWidth;//滚筒输送机宽度
        float BoardHigh = 2.25f * RCP.RollerRadius;//边板部分的高度为滚筒半径的2.25倍
        float BoardWidth = RCP.RollerRadius;//单边板的宽度设为滚筒半径
        Vector3 SideBoard1_size = new Vector3(BoardWidth, BoardHigh, RCLength);//边板1的尺寸
        GameObject SB1 = Resources.Load("Scene/RollerConveyor/Sideboard1") as GameObject;//加载边板1的组件
        GameObject SideBoard1 = Instantiate(SB1); SideBoard1.transform.localScale = SideBoard1_size;

        Vector3 SideBoard2_size1 = new Vector3(BoardWidth, BoardWidth / 10, RCLength);//边板2组件1的尺寸
        Vector3 SideBoard2_size2 = new Vector3(BoardWidth / 10, BoardHigh, RCLength);//边板2组件1的尺寸
        Vector3 SideBoard2_size3 = new Vector3(BoardWidth, BoardWidth / 10, RCLength);//边板3组件1的尺寸
        GameObject SideBoard2 = new GameObject();
        GameObject SideBoard2_1 = Instantiate(SB1); SideBoard2_1.transform.localScale = SideBoard2_size1;
        GameObject SideBoard2_2 = Instantiate(SB1); SideBoard2_2.transform.localScale = SideBoard2_size2;
        GameObject SideBoard2_3 = Instantiate(SB1); SideBoard2_3.transform.localScale = SideBoard2_size3;
        SideBoard2_1.transform.parent = SideBoard2.transform;
        SideBoard2_1.transform.localPosition = new Vector3(-SideBoard2_size1.x / 2, -SideBoard2_size1.y / 2, SideBoard2_size1.z / 2);
        SideBoard2_2.transform.parent = SideBoard2.transform;
        SideBoard2_2.transform.localPosition = new Vector3(-SideBoard2_size2.x / 2, -SideBoard2_size2.y / 2, SideBoard2_size2.z / 2);
        SideBoard2_3.transform.parent = SideBoard2.transform;
        SideBoard2_3.transform.localPosition = new Vector3(-SideBoard2_size3.x / 2, -SideBoard2_size2.y + SideBoard2_size3.y / 2, SideBoard2_size3.z / 2);

        SideBoard1.transform.parent = SideBoard.transform; SideBoard2.transform.parent = SideBoard.transform;
        SideBoard1.transform.localPosition = new Vector3(-RCWidth / 2 + SideBoard1_size.x / 2, -SideBoard1_size.y / 2, SideBoard1_size.z / 2);
        SideBoard2.transform.localPosition = new Vector3(RCWidth / 2, 0, 0);
        //GameObject.Find("ScriptsContainer").GetComponent<MyClass>().Create2(SideBoard);
    }
    #endregion 
    //创建滚筒输送机支撑部分
    #region Create_SupportFoot
    public static void Create_SupportFoot(RollerConveyor_Parameter RCP, GameObject SupportFoot)
    {
        //创建滚筒输送机的支撑架
        float RCWith = RCP.RCWidth;//输送机的宽度
        float SupportHigh = RCP.RCHigh - 2.25f * RCP.RollerRadius;//支撑架的高度=输送机高度-边板高度
        float SupportWidth = RCP.RollerRadius;//支撑架的宽度=边板宽度
        Vector3 UprightSupport_size = new Vector3(SupportWidth, SupportHigh, 2 * SupportWidth);//支撑架立柱的尺寸
        Vector3 HorizontalSupport_size = new Vector3(RCWith, SupportWidth, 2 * SupportWidth);//支撑架水平支柱尺寸

        GameObject SF = Resources.Load("Scene/RollerConveyor/SupportFoot") as GameObject;//加载构件
        GameObject obj1 = Instantiate(SF); obj1.transform.localScale = UprightSupport_size;//创建立柱
        GameObject obj2 = Instantiate(obj1);
        GameObject obj3 = Instantiate(SF); obj3.transform.localScale = HorizontalSupport_size;//创建水平支柱
        GameObject obj4 = Instantiate(obj3);
        obj1.transform.parent = SupportFoot.transform;
        obj1.transform.localPosition = new Vector3(RCWith / 2 - SupportWidth / 2, SupportHigh / 2, SupportWidth);
        obj2.transform.parent = SupportFoot.transform;
        obj2.transform.localPosition = new Vector3(-RCWith / 2 + SupportWidth / 2, SupportHigh / 2, SupportWidth);
        obj3.transform.parent = SupportFoot.transform;
        obj3.transform.localPosition = new Vector3(0, SupportHigh / 5 + SupportWidth / 2, SupportWidth);
        obj4.transform.parent = SupportFoot.transform;
        obj4.transform.localPosition = new Vector3(0, 4 * SupportHigh / 5 - SupportWidth / 2, SupportWidth);
        //GameObject.Find("ScriptsContainer").GetComponent<MyClass>().Create2(SupportFoot);
    }
    #endregion
    //创建滚筒输送机
    #region Create_RollerConveyor
    public static void Create_RollerConveyor(RollerConveyor_Parameter RCP, GameObject OBJ, RollerConveyorType type)
    {
        //type指的是滚筒输送机的类型
        //"Intact"完整型；"Origion"起点型；"Terminus"终点型；"Transition"过渡型
        float BoardWidth = RCP.RollerRadius;
        GameObject RollerPart = new GameObject(); RollerPart.name = "RollerPart";//滚筒部分
        Create_RollerPart(RCP, RollerPart);
        GameObject SideBoard = new GameObject(); SideBoard.name = "SideBoard";//边板部分
        Create_SideBoard(RCP, SideBoard);
        GameObject SupportFoot = new GameObject(); SupportFoot.name = "SupportFoot";//支撑架
        Create_SupportFoot(RCP, SupportFoot);
        GameObject SupportFoot2 = Instantiate(SupportFoot); SupportFoot2.name = SupportFoot.name + 2.ToString();

        //创建不同类型的滚筒输送机
        switch (type)
        {
            case RollerConveyorType.Intact:
                RollerPart.transform.parent = OBJ.transform; RollerPart.transform.localPosition = new Vector3(0, RCP.RCHigh, 0);
                SideBoard.transform.parent = OBJ.transform; SideBoard.transform.localPosition = new Vector3(0, RCP.RCHigh, 0);
                SupportFoot.transform.parent = OBJ.transform; SupportFoot.transform.localPosition = new Vector3(0, 0, 0);
                SupportFoot2.transform.parent = OBJ.transform; SupportFoot2.transform.localPosition = new Vector3(0, 0, RCP.RCLength - 2 * BoardWidth);
                OBJ.name = "IntactRollerConveyor";
                break;
            case RollerConveyorType.Origion:
                RollerPart.transform.parent = OBJ.transform; RollerPart.transform.localPosition = new Vector3(0, RCP.RCHigh, 0);
                SideBoard.transform.parent = OBJ.transform; SideBoard.transform.localPosition = new Vector3(0, RCP.RCHigh, 0);
                SupportFoot.transform.parent = OBJ.transform; SupportFoot.transform.localPosition = new Vector3(0, 0, 0);
                //SupportFoot2.transform.parent = OBJ.transform; SupportFoot2.transform.localPosition = new Vector3(0, 0, RCP.RCLength - BoardWidth);
                DestroyImmediate(SupportFoot2);
                OBJ.name = "OrigionRollerConveyor";
                break;
            case RollerConveyorType.Terminus:
                RollerPart.transform.parent = OBJ.transform; RollerPart.transform.localPosition = new Vector3(0, RCP.RCHigh, 0);
                SideBoard.transform.parent = OBJ.transform; SideBoard.transform.localPosition = new Vector3(0, RCP.RCHigh, 0);
                SupportFoot.transform.parent = OBJ.transform; SupportFoot.transform.localPosition = new Vector3(0, 0, -BoardWidth);
                //SupportFoot2.transform.parent = OBJ.transform; SupportFoot2.transform.localPosition = new Vector3(0, 0, RCP.RCLength - BoardWidth);
                DestroyImmediate(SupportFoot2);
                OBJ.name = "TerminusRollerConveyor";
                break;
            case RollerConveyorType.Transition:
                RollerPart.transform.parent = OBJ.transform; RollerPart.transform.localPosition = new Vector3(0, RCP.RCHigh, 0);
                SideBoard.transform.parent = OBJ.transform; SideBoard.transform.localPosition = new Vector3(0, RCP.RCHigh, 0);
                SupportFoot.transform.parent = OBJ.transform; SupportFoot.transform.localPosition = new Vector3(0, 0, -BoardWidth);
                SupportFoot2.transform.parent = OBJ.transform; SupportFoot2.transform.localPosition = new Vector3(0, 0, RCP.RCLength - 2 * BoardWidth);
                OBJ.name = "TransitionRollerConveyor";
                break;
        }
    }
    #endregion
    //创建皮带式输送机
    #region Create_BeltConveyor
    public static void Create_BeltConveyor(RollerConveyor_Parameter RCP, GameObject OBJ)
    {
        float BoardWidth = RCP.RollerRadius;
        GameObject BeltPart = new GameObject(); BeltPart.name = "BeltPart";//滚筒部分
        Create_BeltPart(RCP, BeltPart);
        GameObject SideBoard = new GameObject(); SideBoard.name = "SideBoard";//边板部分
        Create_SideBoard(RCP, SideBoard);
        GameObject SupportFoot = new GameObject(); SupportFoot.name = "SupportFoot";//支撑架
        Create_SupportFoot(RCP, SupportFoot);
        GameObject SupportFoot2 = Instantiate(SupportFoot); SupportFoot2.name = SupportFoot.name + 2.ToString();
        //构造皮带式输送机
        BeltPart.transform.parent = OBJ.transform; BeltPart.transform.localPosition = new Vector3(0, RCP.RCHigh, 0);
        SideBoard.transform.parent = OBJ.transform; SideBoard.transform.localPosition = new Vector3(0, RCP.RCHigh, 0);
        SupportFoot.transform.parent = OBJ.transform; SupportFoot.transform.localPosition = new Vector3(0, 0, 0);
        SupportFoot2.transform.parent = OBJ.transform; SupportFoot2.transform.localPosition = new Vector3(0, 0, RCP.RCLength - 2 * BoardWidth);
    }
    #endregion
    //创建皮带式输送机的皮带部分
    #region Create_BeltPart
    public static void Create_BeltPart(RollerConveyor_Parameter RCP, GameObject BeltPart)
    {
        //创建滚筒输送部分
        float RCLength = RCP.RCLength;//滚筒输送机长度
        float RCWidth = RCP.RCWidth;//滚筒输送机宽度
        float RollerR = RCP.RollerRadius;//滚筒输送机半径
        float RollerLength = RCWidth - 0.002f;
        float BoardHigh = 2 * RCP.RollerRadius;//边板的高度设为滚筒输送机半径的2.25倍
        float interval = 4 * RollerR / 3;//初始化滚筒间距离
        float rn = RCLength / (2 * RollerR + interval);//浮点数不能做求余运算
        int RollerNum = (int)Math.Ceiling(rn);
        float thickness = RollerR/10;//皮带厚度
        interval = RCLength / RollerNum - 2 * RollerR;//根据滚筒输送机长度调整后的滚筒间距

        GameObject Roller = Resources.Load("Scene/RollerConveyor/RollerX") as GameObject;
        for (int i = 0; i < RollerNum; i++)
        {
            GameObject clone = Instantiate(Roller); clone.name = "Roller" + (i + 1).ToString();
            clone.transform.localScale = new Vector3(RollerLength, 2 * RollerR, 2 * RollerR);
            clone.transform.parent = BeltPart.transform;
            clone.transform.localPosition = new Vector3(0, -BoardHigh / 2, interval / 2 + RollerR + i * (2 * RollerR + interval));
        }
        //增加皮带部分
        GameObject Belt = Resources.Load("Scene/RollerConveyor/Belt") as GameObject;
        GameObject Belt1 = Instantiate(Belt); float BeltLength = RCLength - interval - RollerR * 2;
        Belt1.transform.localScale = new Vector3(RollerLength - 2 * RollerR, thickness, BeltLength);
        GameObject Belt2 = Instantiate(Belt1);
        Belt1.transform.parent = BeltPart.transform; Belt2.transform.parent = BeltPart.transform;
        Belt1.transform.localPosition = new Vector3(0, -BoardHigh / 2 + RollerR - thickness / 2, interval / 2 + RollerR + BeltLength / 2);
        Belt2.transform.localPosition = new Vector3(0, -BoardHigh / 2 - RollerR + thickness / 2, RCLength / 2);
    }
    #endregion
    //创建输送线
    #region Create_Conveyors
    public static void Create_Conveyors(Conveyor_Parameter CP, GameObject Conveyors)
    {
        RollerConveyor_Parameter RCP = CP.RCP; RCP.RCLength = CP.Length / CP.Num;
        switch (CP.Type)
        {
            case ConveyorType.RollerConveyor:
                if (CP.Num == 1)
                {
                    GameObject OBJ1 = new GameObject(); Create_RollerConveyor(RCP, OBJ1, RollerConveyorType.Intact);//完整型
                    OBJ1.transform.parent = Conveyors.transform; OBJ1.transform.localPosition = new Vector3(0, 0, 0);
                }
                if (CP.Num == 2)
                {
                    GameObject OBJ1 = new GameObject(); Create_RollerConveyor(RCP, OBJ1, RollerConveyorType.Origion);//起点型
                    OBJ1.transform.parent = Conveyors.transform; OBJ1.transform.localPosition = new Vector3(0, 0, 0);
                    GameObject OBJ2 = new GameObject(); Create_RollerConveyor(RCP, OBJ2, RollerConveyorType.Transition);//终点型
                    OBJ2.transform.parent = Conveyors.transform; OBJ2.transform.localPosition = new Vector3(0, 0, CP.Length/CP.Num);
                }
                if (CP.Num > 2)
                {
                    GameObject OBJ1 = new GameObject(); Create_RollerConveyor(RCP, OBJ1, RollerConveyorType.Origion);//起点型
                    OBJ1.transform.parent = Conveyors.transform; OBJ1.transform.localPosition = new Vector3(0, 0, 0);
                    GameObject OBJ2 = new GameObject(); Create_RollerConveyor(RCP, OBJ2, RollerConveyorType.Transition);//终点型
                    OBJ2.transform.parent = Conveyors.transform; OBJ2.transform.localPosition = new Vector3(0, 0, CP.Length - RCP.RCLength);
                    GameObject OBJ3 = new GameObject(); Create_RollerConveyor(RCP, OBJ3, RollerConveyorType.Terminus);
                    for (int i = 0; i < CP.Num - 2; i++)
                    {
                        GameObject clone = Instantiate(OBJ3); clone.name = OBJ3.name + (i + 1).ToString();
                        clone.transform.parent = Conveyors.transform; clone.transform.localPosition = new Vector3(0, 0, (i + 1) * RCP.RCLength);
                    }
                    DestroyImmediate(OBJ3);
                }
                else
                {
                    Debug.Log("The Number of Conveyors is invalid!");
                }
                break;
            case ConveyorType.BeltConveyor:
                GameObject obj1 = new GameObject(); Create_BeltConveyor(RCP, obj1);
                for (int i = 0; i < CP.Num; i++)
                {
                    GameObject clone = Instantiate(obj1); clone.name = obj1.name + (i + 1).ToString();
                    clone.transform.parent = Conveyors.transform.parent;
                    clone.transform.localPosition = new Vector3(0, 0, i * RCP.RCLength);
                }
                break;
        }
        
    }
    #endregion
    //创建货物停放平台
    #region Create_PlatForm
    public static void Create_PlatForm(RollerConveyor_Parameter RCP, Vector3 CargoSize, GameObject PlatForm)
    {
        float PlatFormHigh = RCP.RCHigh - RCP.RollerRadius / 8;//平台的高度
        float PlatFormWidth_X = RCP.RCWidth;//X向宽度
        float PlatFormWidth_Z = CargoSize.z + CargoSize.z / 10;
        //GameObject obj1 = Resources.Load("Scene/RollerConveyor/Belt") as GameObject;

    }
    #endregion
}
