using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipmentState{
    public string kind;//设备种类
    public string index;//设备编号
    public State workState;//工作状态 on/off
    public FacilityState facilityState;//设备好坏状态 normal/error

    public Vector3 deliverDirection;//输送方向
    public float deliverSpeed;//输送速度
    public Exclusive isExcusive;//是否被独占（针对顶升）
}
