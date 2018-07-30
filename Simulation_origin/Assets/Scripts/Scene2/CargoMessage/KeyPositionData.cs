using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PositionsList
{
    public float[,] HighBayPositions;//各高架库坐标
    public float[,] PilerPositons;//各堆垛机坐标
    public float[,] ConveyorPositons;//各输送机流水线坐标
    public float[,,,] StorageBinPositions;//高架库仓位坐标
}
[System.Serializable]
public struct StorePositions
{
    public float[] StoreFloorPositions;//层坐标
    public float[] StoreColumnPositions;//列坐标
    public float[] StorePlacePosition;//位坐标（相对于列坐标调整）
}

[System.Serializable]
public class SceneInformation
{
    public int HighBaysNum;//高架库数目
    public int PilersNum;//堆垛机数目
    public int ConveyorsNum;//输送带数目
    public float[,] Positions;//关键点位置
    //public float[,] HighBaysPosition;
    //public PositionsList PositionList;
    //public FloatArray2D HighBaysPosition;
}

[System.Serializable]
public class KeyPositionsData
{
    public int HighBaysNum;//高架库数目
    public Vector3 EnterPosition;//入口坐标
    //public Vector3[] LiftTransPositions;//
    public float[] HighValues;//输送线高度值（2个）
    //public float[] ConveyorLengths;//输送机长度（2个）
    public float[] ConveyorLinesValues;//入库输送线X值
    public float[] ConveyorLengths;//入库输送线长度
    public float ConveyorWidth;
    public float[] PilerLinesValues;//堆垛机线路X值
    public Vector3[] HighBaysPositions;//高架库的坐标值
    public Vector3 CargoSize;//货物尺寸
    public StorePositions StorePositions;

    public float EnterRCP_Length;//入口滚筒输送机长度
    public float ExitRCP_Length;//出口滚筒输送机长度
}

