using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Place { A=0,B=1}
[System.Serializable]
public struct PositionInfo
{
    public int HighBayNum;//高架库编号
    public int ColumnNum;//货物存储所在列数
    public int FloorNum;//货物存放所在层数
    public Place place;//货物所在仓位的A位或B位
}

[System.Serializable]
public class CargoMessage {
    public string Name;//货物名称
    public Vector3 Size;//货物尺寸
    public string Number1;//货物编号
    public int Num;//货物件数
    public PositionInfo PositionInfo;//所要存储所在仓位信息
    public string InputTime;//入库时间
    public string Description;//其他描述

    public Queue<GameObject> EquipmentsQueue;//出入库经过的设备队列

}

//public struct PositionsList
//{
//    public float[,] HighBayPositions;//各高架库坐标
//    public float[,] PilerPositons;//各堆垛机坐标
//    public float[,] ConveyorPositons;//各输送机流水线坐标
//    public float[,,,] StorageBinPositions;//高架库仓位坐标
//}
[System.Serializable]
public class HighBaySceneData
{
    public int HighBaysNum;//高架库数目
    public int PilersNum;//堆垛机数目
    public int ConveyorsNum;//输送带数目
    //public PositionsList PositionList;
}

