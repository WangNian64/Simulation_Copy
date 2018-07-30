using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LiftPattern//顶升部分的运动方向
{
    up=0, down=1, off=2
}
//顶升部分
[System.Serializable]
public class LiftPartState : EquipmentState
{
    public LiftPattern liftPattern;//顶升抬升模式
}