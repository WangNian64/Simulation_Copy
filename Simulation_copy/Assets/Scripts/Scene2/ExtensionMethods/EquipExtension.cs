using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static partial class EquipExtension
{
    //返回货物下一个设备的种类
    public static string nextEquipKind(GameObject cargo)
    {
        GameObject nextEquip = cargo.GetComponent<ShowCargoInfo>().Cargomessage.EquipmentsQueue.ElementAt(2);
        if (nextEquip == null)
        {
            return null;
        } else {
            return nextEquip.GetComponent<ShowEquipState>().equipmentState.kind;
        }
    }
    //判断货物的下一个设备是不是最后一个顶升移载机
    public static bool isNextLastLiftTransfer(GameObject cargo)
    {
        int count = 0;
        foreach (GameObject equip in cargo.GetComponent<ShowCargoInfo>().Cargomessage.EquipmentsQueue)
        {
            if (equip.GetComponent<ShowEquipState>().equipmentState.kind.Equals("LiftTransfer"))
            {
                count++;
            }
        }
        return count > 1 ? false : true;
    }
    //判断货物是否到达设备过渡点(
    public static bool isCrossTrans(GameObject cargo, GameObject equipment)
    {
        CargoMessage cm = cargo.GetComponent<ShowCargoInfo>().Cargomessage;
        Vector3 cargoTransPos = new Vector3(0,0,0);
        EquipmentState es = equipment.GetComponent<ShowEquipState>().equipmentState;
        switch (es.kind)
        {
            case "BeltConveyor":
                cargoTransPos.y += GlobalVariable.KPD.HighValues[0];
                cargoTransPos += es.deliverDirection * (GlobalVariable.KPD.ConveyorLengths[0] - cm.Size.x / 2);
                break;
            case "RollerConveyor":
                cargoTransPos.y += GlobalVariable.KPD.HighValues[1];
                cargoTransPos += es.deliverDirection * (GlobalVariable.KPD.ConveyorLengths[1] - cm.Size.x / 2);
                break;
            case "LiftTransfer":
                cargoTransPos.y += GlobalVariable.KPD.HighValues[1];
                //如果下一个设备是顶升部分,在顶升的中心进行过度
                if (EquipExtension.nextEquipKind(cargo).Equals("LiftPart"))
                {

                } else {
                    cargoTransPos += es.deliverDirection * GlobalVariable.KPD.ConveyorWidth;
                }
                break;
            case "LiftPart":
                //抬升是在上升点的中心，放下货物是在下落点的中心 
                return equipment.transform.localPosition.y == GlobalVariable.KPD.HighValues[0];
            default:
                break;
        }
        return cargo.transform.localPosition == cargoTransPos;
    }
    //检测货物是否越过设备
    public static bool isCrossEquip(GameObject cargo, GameObject equipment)
    {
        Vector3 finalCargoPos = new Vector3(0,0,0);
        EquipmentState es = equipment.GetComponent<ShowEquipState>().equipmentState;
        switch (es.kind)
        {
            case "BeltConveyor":
                finalCargoPos.y += GlobalVariable.KPD.HighValues[0];
                finalCargoPos += es.deliverDirection * GlobalVariable.KPD.ConveyorLengths[0];
                break;
            case "RollerConveyor":
                finalCargoPos.y += GlobalVariable.KPD.HighValues[1];
                finalCargoPos += es.deliverDirection * GlobalVariable.KPD.ConveyorLengths[1];
                break;
            case "LiftTransfer":
                finalCargoPos.y += GlobalVariable.KPD.HighValues[1];
                //如果下一个设备是顶升部分吗，在中心就要直接换设备
                if (EquipExtension.nextEquipKind(cargo).Equals("LiftPart"))
                {

                } else {
                    finalCargoPos += es.deliverDirection * GlobalVariable.KPD.ConveyorWidth;
                }
                break;
            case "LiftPart"://这个不一样
                //抬升货物是在顶升边缘，放下货物是在顶升下落的中心 
                finalCargoPos += es.deliverDirection * GlobalVariable.KPD.ConveyorWidth / 2;
                break;
            default:
                break;
        }
        return cargo.transform.localPosition == finalCargoPos;
    }
}
