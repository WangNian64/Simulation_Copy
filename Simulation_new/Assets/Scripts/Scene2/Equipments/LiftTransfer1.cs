using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LiftTransferParameter
{
    public float High;//顶升移载机高度
    public float Width;//顶升移载机宽度
    public float GearDiameter;//顶升移载机齿轮直径
    public float RollerRadius;//顶升移载机滚筒半径
    public Vector3 GearSize;//顶升移载机齿轮实际尺寸
    public Vector3 UnitChainSize;//顶升移载机单节链条尺寸
    public float TempLength;//链条实际长度（竖直投影长度）
}

public class LiftTransfer1 : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    #region CreateChain //设计链条
    public static void CreateChain(GameObject Chain,ref LiftTransferParameter LTP)
    {
        //GearDiameter为齿轮直径
        //ChainLength为链条长度
        //float High = 0.1f;
        //链条水平长度
        //float ChainLength = 1.2f;
        //链条轴承直径
        //float GearDiameter = 0.1f;//链条轴承直径
        //采用八边形方法设计弧形区域，根据设计思想计算链条关节的尺寸
        float GearDiameter = LTP.GearDiameter;
        float ChainLength = LTP.Width - 2 * 0.05f;
        float length0;//链条关节有效计算长度（链条关节长度的2/3）
        length0 = GearDiameter * Mathf.Tan(22.5f * (Mathf.PI / 180f));
        //计算链条关节的尺寸
        float UnitLength = length0 * 3 / 2;//链条关节实际长度
        GameObject obj0 = Resources.Load<GameObject>("Scene/ChainEquipment/UnitChain");//加载链条关节小构件模型
        Vector3 obj0_size = obj0.GetComponent<MeshFilter>().sharedMesh.bounds.size;//构件模型的尺寸
        float Scale0 = UnitLength / obj0_size.z;//根据需要的实际尺寸计算Scale值
        GameObject obj = Instantiate(obj0);//实例化模型
        obj.transform.localScale = new Vector3(Scale0, Scale0, Scale0);//实例化模型的尺寸符合要求
        Vector3 UnitChainSize = new Vector3(Scale0 * obj0_size.x, Scale0 * obj0_size.y, UnitLength);
        LTP.UnitChainSize = UnitChainSize;//单节链条的实际尺寸
        //计算链条水平长度及链条关节数目
        float Length0 = ChainLength - GearDiameter - length0 - UnitChainSize.y;//链条水平设计部分长度
        int Num = (int)(Length0 / length0); //Debug.Log(Scale0);
        float Length1 = Num * length0;//链条水平设计部分的有效长度
        LTP.TempLength = Length1 + GearDiameter + length0 + UnitChainSize.y;
        //Debug.Log(LTP.TempLength);
        //拐弯处弧形链条设计
        GameObject OBJ1 = new GameObject();//弧形链条设计（八边形模拟设计）
        OBJ1.name = "huxing";
        GameObject obj01 = Instantiate(obj); obj01.transform.parent = OBJ1.transform;
        obj01.transform.localPosition = new Vector3(0, GearDiameter / 2, 0);
        GameObject obj02 = Instantiate(obj01); obj02.transform.parent = OBJ1.transform;
        obj02.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(1, 0, 0), -45f);
        GameObject obj03 = Instantiate(obj01); obj03.transform.parent = OBJ1.transform;
        obj03.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(1, 0, 0), -90f);
        GameObject obj04 = Instantiate(obj01); obj04.transform.parent = OBJ1.transform;
        obj04.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(1, 0, 0), -135f);
        GameObject obj05 = Instantiate(obj01); obj05.transform.parent = OBJ1.transform;
        obj05.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(1, 0, 0), -180f);
       
        //设计链条的水平部分
        GameObject OBJ2 = new GameObject();//链条水平部分（一条）
        OBJ2.name = "shuiping";
        Vector3 position0 = new Vector3(0, 0, -Length1 / 2);
        for (int i = 0; i < Num; i++)
        {
            GameObject clone = Instantiate(obj); clone.transform.parent = OBJ2.transform; clone.name = OBJ2.name + "0" + (i+1).ToString();
            clone.transform.localPosition = new Vector3(0, 0, position0.z + (length0 / 2 + i * length0));
        }
        //GameObject obj_ = OBJ2.transform.Find("shuiping01");
        //组件单条链条
        //GameObject OBJ = new GameObject();
        OBJ1.transform.parent = Chain.transform; OBJ1.transform.localPosition = new Vector3(0, 0, -Length1 / 2 - length0 / 2);
        OBJ2.transform.parent = Chain.transform; OBJ2.transform.localPosition = new Vector3(0, GearDiameter / 2, 0);
        GameObject OBJ3 = Instantiate(OBJ1); OBJ3.transform.Rotate(180f, 0, 0);
        OBJ3.transform.parent = Chain.transform; OBJ3.transform.localPosition = new Vector3(0, 0, Length1 / 2 + length0 / 2);
        GameObject OBJ4 = Instantiate(OBJ2); OBJ4.transform.Rotate(180f, 0, 0);
        OBJ4.transform.parent = Chain.transform; OBJ4.transform.localPosition = new Vector3(0, -GearDiameter / 2, 0);

        DestroyImmediate(obj);
        //ChainLength = Length1 + length0 + GearDiameter;
        
        
    }
    #endregion

    #region CreateGearChain //设计带齿轮链条
    public static void CreateGearChain(GameObject GearChain, ref LiftTransferParameter LTP)
    {
        float GearDiameter = LTP.GearDiameter;//齿轮半径
        Vector3 UnitChainSize = LTP.UnitChainSize;//单节链条实际尺寸
        GameObject Chain = new GameObject(); Chain.name = "Chain";
        CreateChain(Chain, ref LTP);
        float TempLength = LTP.TempLength;//链条实际长度（投影长度）
        //添加齿轮信息
        GameObject obj = Resources.Load<GameObject>("Scene/ChainEquipment/Gear");
        Vector3 obj_size = obj.GetComponent<MeshFilter>().sharedMesh.bounds.size;//齿轮模型尺寸
        float Scale1 = GearDiameter / obj_size.y;
        GameObject Gear = Instantiate(obj); Gear.name = obj.name;
        Gear.transform.localScale = new Vector3(Scale1, Scale1, Scale1);
        LTP.GearSize = new Vector3(Scale1 * obj_size.x, GearDiameter, GearDiameter);//齿轮实际尺寸
        GameObject Gear2 = Instantiate(Gear); Gear2.name = Gear.name + 2.ToString();//第二个齿轮
        //链条添加齿轮设计
        Chain.transform.parent = GearChain.transform;
        Chain.transform.localPosition = new Vector3(0, 0, 0);
        Gear.transform.parent = GearChain.transform;
        Gear.transform.localPosition = new Vector3(0, 0, -TempLength / 2 + GearDiameter / 2 + LTP.UnitChainSize.y / 2);//第一个齿轮位置设计
        Gear2.transform.parent = GearChain.transform;
        Gear2.transform.localPosition = new Vector3(0, 0, TempLength / 2 - GearDiameter / 2 - LTP.UnitChainSize.y / 2);//第二个齿轮位置设计
        
    }
    #endregion

    #region CreateLiftChain //设计顶升链条
    public static void CreateLiftChain(GameObject LiftChain, ref LiftTransferParameter LTP)
    {
        //创建齿轮链条
        GameObject GearChain = new GameObject(); GearChain.name = "GearChain";//创建齿轮链条
        CreateGearChain(GearChain, ref LTP);
        //设计顶升固定
        float BoardThickness = 0.005f;//采用的边板厚度
        float BoardHigh = 2 * LTP.GearDiameter;//采用的边板高度
        float BoardWidth = LTP.GearDiameter / 2;//采用的边板的宽度
        GameObject obj1 = Resources.Load<GameObject>("Scene/ChainEquipment/Board");//加载边板构件
        GameObject Board = Instantiate(obj1);
        Board.transform.localScale = new Vector3(BoardThickness, BoardHigh, BoardWidth);//修改边板尺寸为实际尺寸
        GameObject Board2 = Instantiate(Board);
        GameObject Board3 = Instantiate(Board);
        GameObject Board4 = Instantiate(Board);
        GameObject Board0 = Instantiate(obj1);
        Vector3 Board0Size = new Vector3(LTP.GearSize.x, LTP.GearDiameter / 2, LTP.Width);//底板设计
        Board0.transform.localScale = Board0Size;

        GameObject LiftBoards = new GameObject(); LiftBoards.name = "LiftBoards";
        Board0.transform.parent = LiftBoards.transform;
        Board0.transform.localPosition = new Vector3(0, Board0Size.y / 2, 0);
        float TempValue1 = Board0Size.x / 2 + BoardThickness / 2;
        float TempValue2 = LTP.TempLength / 2 - LTP.UnitChainSize.y / 2 - LTP.GearDiameter / 2;
        Board.transform.parent = LiftBoards.transform;
        Board.transform.localPosition = new Vector3(TempValue1, BoardHigh / 2, TempValue2);
        Board2.transform.parent = LiftBoards.transform;
        Board2.transform.localPosition = new Vector3(TempValue1, BoardHigh / 2, -TempValue2);
        Board3.transform.parent = LiftBoards.transform;
        Board3.transform.localPosition = new Vector3(-TempValue1, BoardHigh / 2, -TempValue2);
        Board4.transform.parent = LiftBoards.transform;
        Board4.transform.localPosition = new Vector3(-TempValue1, BoardHigh / 2, TempValue2);
        //组件顶升链条
        float TempValue3 = LTP.UnitChainSize.y / 2 + LTP.GearDiameter / 2;
        GearChain.transform.parent = LiftChain.transform;
        GearChain.transform.localPosition = new Vector3(0, -TempValue3, 0);
        LiftBoards.transform.parent = LiftChain.transform;
        LiftBoards.transform.localPosition = new Vector3(0, -TempValue3 - BoardHigh + BoardWidth / 2,0);

    }
    #endregion

    #region CreateFrame //设计框架
    public static void CreateFrame(GameObject Frame, ref LiftTransferParameter LTP)
    {
        float High = LTP.High; float Width = LTP.Width;
        float BoardWidth = LTP.RollerRadius;
        float BoardHigh = LTP.High * 2 / 3;
        float BoardHigh2 = BoardHigh - 2 * LTP.RollerRadius;
        float FootHigh = LTP.High / 3;
        GameObject obj = Resources.Load<GameObject>("Scene/ChainEquipment/Board");
        //创建框架1
        GameObject Board = Instantiate(obj); Board.transform.localScale = new Vector3(BoardWidth, BoardHigh2, Width);
        GameObject Board2 = Instantiate(Board); Board2.transform.Rotate(0, 180, 0);

        GameObject Board3 = Instantiate(obj); Board3.transform.localScale = new Vector3(Width, BoardHigh, BoardWidth);
        GameObject Board4 = Instantiate(Board3); Board4.transform.Rotate(0, 180, 0);
        GameObject Frame1 = new GameObject(); Frame1.name = "Frame1";
        Board.transform.parent = Frame1.transform; Board.transform.localPosition = new Vector3(Width / 2 - BoardWidth / 2, BoardHigh2/2, 0);
        Board2.transform.parent = Frame1.transform; Board2.transform.localPosition = new Vector3(-Width / 2 + BoardWidth / 2, BoardHigh2 / 2, 0);
        Board3.transform.parent = Frame1.transform; Board3.transform.localPosition = new Vector3(0, BoardHigh / 2, Width / 2 - BoardWidth / 2);
        Board4.transform.parent = Frame1.transform; Board4.transform.localPosition = new Vector3(0, BoardHigh/2, -Width / 2 + BoardWidth / 2);
        //创建框架2（脚架）
        GameObject Foot = Instantiate(Board); Foot.transform.localScale = new Vector3(BoardWidth, FootHigh, BoardWidth);
        GameObject Foot2 = Instantiate(Foot); GameObject Foot3 = Instantiate(Foot); GameObject Foot4 = Instantiate(Foot);
        GameObject Frame2 = new GameObject(); Frame2.name = "Foots"; float TempValue = Width / 2 - BoardWidth / 2;
        Foot.transform.parent = Frame2.transform; Foot.transform.localPosition = new Vector3(TempValue, 0, TempValue);
        Foot2.transform.parent = Frame2.transform; Foot2.transform.localPosition = new Vector3(TempValue, 0, -TempValue);
        Foot3.transform.parent = Frame2.transform; Foot3.transform.localPosition = new Vector3(-TempValue, 0, TempValue);
        Foot4.transform.parent = Frame2.transform; Foot4.transform.localPosition = new Vector3(-TempValue, 0, -TempValue);
        //创建框架
        Frame1.transform.parent = Frame.transform; Frame1.transform.localPosition = new Vector3(0, FootHigh, 0);
        Frame2.transform.parent = Frame.transform; Frame2.transform.localPosition = new Vector3(0, FootHigh/2, 0);
    }
    #endregion

    #region CreateTransPort
    public static void CreateTransPort(GameObject RollerPart, GameObject LiftPart, ref LiftTransferParameter LTP)
    {
        float GearDiameter = LTP.GearDiameter;
        float Width = LTP.Width;
        float High = LTP.High;
        float RollerRadius = LTP.RollerRadius;
        //创建滚筒
        GameObject obj0 = Resources.Load<GameObject>("Scene/ChainEquipment/Roller");
        GameObject Roller = Instantiate(obj0); Roller.name = obj0.name;
        Roller.transform.localScale = new Vector3(RollerRadius, RollerRadius, Width);
        //创建滚筒输送部分
        float interval = 4 * RollerRadius / 3;//初始化滚筒间距离
        float TempWidth = Width;
        float rn = TempWidth / (2 * RollerRadius + interval);//浮点数不能做求余运算
        int RollerNum = (int)Mathf.Ceil(rn);
        interval = TempWidth / RollerNum - 2 * RollerRadius;//根据滚筒输送机长度调整后的滚筒间距
        //float tempPositionX = -Width / 2 + 0.05f;
        for (int i = 0; i < RollerNum; i++)
        {
            GameObject clone = Instantiate(Roller); clone.name = "Roller" + (i + 1).ToString();
            clone.transform.parent = RollerPart.transform;
            clone.transform.localPosition = new Vector3(-Width / 2 + interval / 2 + RollerRadius + i * (2 * RollerRadius + interval), -RollerRadius / 2, 0);
        }

        //创建顶升链条
        GameObject LiftChain = new GameObject(); LiftChain.name = "LiftChain";
        CreateLiftChain(LiftChain, ref LTP);
        //创建链条部分
        GameObject ChainPart = new GameObject(); ChainPart.name = "ChainPart";
        for (int i = 0; i < RollerNum - 1; i++)
        {
            GameObject clone = Instantiate(LiftChain); clone.name = LiftChain.name + (i + 1).ToString();
            clone.transform.parent = ChainPart.transform;
            clone.transform.localPosition = new Vector3(-Width / 2 + interval + 2 * RollerRadius + i * (2 * RollerRadius + interval), 0, 0);
        }
        
        //创建顶升部分
        GameObject obj = Resources.Load<GameObject>("Scene/ChainEquipment/Board");
        GameObject Board = Instantiate(obj);
        Vector3 BoardSize = new Vector3((RollerNum - 2) * (2 * RollerRadius + interval), GearDiameter / 2, GearDiameter / 2);
        Board.transform.localScale = BoardSize;
        GameObject Board2 = Instantiate(Board);
        ChainPart.transform.parent = LiftPart.transform;
        ChainPart.transform.localPosition = new Vector3(0, 0, 0);
        float TempValue2 = LTP.UnitChainSize.y / 2 + GearDiameter / 2 + (2 - 1 / 4) * GearDiameter;
        float TempValue3 = LTP.TempLength / 2 - LTP.UnitChainSize.y/2 - GearDiameter/2;
        Board.transform.parent = LiftPart.transform;
        Board.transform.localPosition = new Vector3(0, -TempValue2+BoardSize.y, TempValue3);
        Board2.transform.parent = LiftPart.transform;
        Board2.transform.localPosition = new Vector3(0, -TempValue2+BoardSize.y, -TempValue3);
        DestroyImmediate(Roller); DestroyImmediate(LiftChain);
        ///////////////////修改部分
    }
    #endregion

    #region CreateLiftTransfer
    public static void CreateLiftTransfer(GameObject LiftChain, ref LiftTransferParameter LTP)
    {
        //创建框架
        GameObject Frame = new GameObject(); Frame.name = "Frame";
        CreateFrame(Frame, ref LTP);
        //创建滚筒部分和顶升部分
        GameObject RollerPart = new GameObject(); RollerPart.name = "RollerPart";
        GameObject LiftPart = new GameObject(); LiftPart.name = "LiftPart";
        CreateTransPort(RollerPart, LiftPart, ref LTP);
        //创建顶升移载机
        Frame.transform.parent = LiftChain.transform; Frame.transform.localPosition = new Vector3(0, 0, 0);
        RollerPart.transform.parent = LiftChain.transform; RollerPart.transform.localPosition = new Vector3(0, LTP.High, 0);
        LiftPart.transform.parent = LiftChain.transform; LiftPart.transform.localPosition = new Vector3(0, LTP.High, 0);

        
    }
    #endregion
}
