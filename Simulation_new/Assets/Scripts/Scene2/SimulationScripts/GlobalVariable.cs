using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//有关设备的状态
public enum Exclusive { Yes = 1, No = 0 }//设备是否独占
public enum FacilityKind {LiftTransfer=0,UniConveyor=1,BiConveyor=2,HighBay=3,Piler=4}//设备种类
public enum FacilityState {Normal=0,Error=1}//设备好坏状态


public enum WaitState { None=0,WaitEnter=1,WaitExit=2}//等待状态
public enum State { On=1,Off=0}//设备开关状态
public enum Direction { Enter=1,Exit=0}//输送线方向


public enum CargoState//货物状态
{
    WaitIn = 0, Enter = 1, Stored = 2, WaitOut = 3, Exit = 4
}
public enum StorageBinState//货物面板状态（不同颜色）
{
    NotStored = 0, Reserved = 1, InStore = 2, Stored = 3, Stay2Exit = 4, OutStore = 5
}

public class GlobalVariable
{
    public static KeyPositionsData KPD;
    public static State[] PilersState;//堆垛机状态
    public static List<string> StoredCargosNameList;//入库货物对象名称列表
    public static Queue<GameObject>[] ConveyorQueue;//排队队列
    public static Queue<GameObject>[] EnterQueue;//入库队列
    public static Queue<GameObject>[] ExitQueue;//出库队列
    public static WaitState[] Wait;//到达指示
    public static Queue<GameObject>[] PilerQueue;//堆垛机货物队列
    public static State[,] UnidirectionalConveyorStates;//单向输送机工作状态
    public static State[,,] BidirectionalConveyorStates;//双向输送机工作状态
    public static Direction[] ConveyorDirections;//双向输送机运输方向
    public static State[] LiftTransferStates;//顶升移栽机工作状态
    public static Vector3[,] UnidirectionalPositions;//单向输送线关键点坐标
    public static Vector3[,,] BidirectionalPositions;//双向输送线关键点坐标
    public static Vector3[,] LiftTransferPositions;//堆垛机关键点坐标
    public static Vector3[] PilerBodyPartPositions;//堆垛机bodyPart初始位置
    public static Queue<GameObject> TempQueue;//出入库货物队列
    public static bool FollowState;//相机是否跟随对象
    public static GameObject FollowPlayer;//跟随对象
    public static StorageBinState[,,,] BinState;//所有货位的状态
    public static Color[] BinColor ;//显示面板中Bin的颜色



    //新增
    public const string uniConveyorName = "UniConveyor";
    public const string biConveyorName = "BiConveyor";
    public const string liftTransferName = "LiftTransfer";
    public const string liftPartName = "LiftPart";
    public const string highBayName = "HighBay";
    public const string pilerName = "Piler";

}
