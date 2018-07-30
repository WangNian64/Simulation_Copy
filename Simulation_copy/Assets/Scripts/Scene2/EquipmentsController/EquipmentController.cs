using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

//控制设备运动,根据不同状态控制设备的on/off
public class EquipmentController : MonoBehaviour {
    public GameObject equipment;//当前设备
    public Vector3 equipSize;//设备尺寸


	// Use this for initialization
	void Start () {
        equipment = this.gameObject;
        MyClass.MeshSize(equipment, ref equipSize);
    }
	// Update is called once per frame
    //控制设备的开启和关闭
	void FixedUpdate () {
        EquipmentState es = equipment.GetComponent<ShowEquipState>().equipmentState;

        //让该设备上所有的货物都运动
        List<GameObject> cargoList = new List<GameObject>();
        FindExtension.FindGameObjectsWithTagRecursive(equipment, "Cargo", ref cargoList);

        //设备坏了,停止工作
        if(es.facilityState == FacilityState.Error){
            es.workState = State.Off;
        }

        //货物过渡
        foreach (GameObject cargo in cargoList)
        {
            CargoMessage cm = cargo.GetComponent<ShowCargoInfo>().Cargomessage;
            if (EquipExtension.isCrossTrans(cargo, equipment))//到达过渡位置
            {
                GameObject nextEquip = cm.EquipmentsQueue.ElementAt(2);
                EquipmentState nextEquipState = new EquipmentState();//有问题

                List<GameObject> nextCargoList = new List<GameObject>();
                FindExtension.FindGameObjectsWithTagRecursive(equipment, "Cargo", ref nextCargoList);
                //如果是最后一个传送带设备，关闭当前设备
                if (nextEquip == null)
                {
                    es.workState = State.Off;
                } else {
                    nextEquipState = nextEquip.GetComponent<ShowEquipState>().equipmentState;
                }


                //货物下一个设备是最后一个顶升,这个顶升对它来说是独占设备
                if (EquipExtension.isNextLastLiftTransfer(cargo))
                {
                    if (nextCargoList.Count > 0)//顶升上有货物
                    {
                        es.workState = State.Off;
                    } else {
                        nextEquipState.isExcusive = Exclusive.Yes;
                    }
                }
                //下一个设备开启
                if (nextEquipState.workState == State.On)
                {
                    if (nextEquipState.isExcusive == Exclusive.Yes)//如果是独占设备，本设备关闭
                    {
                        es.workState = State.Off;
                    }
                }
                //下一个设备关闭
                if (nextEquipState.workState == State.Off)
                {
                    if (nextCargoList.Count == 0)//下一个设备上没有货物，开启下一个设备
                    {
                        nextEquipState.workState = State.On;
                    } else {//下一个设备有货物，关闭本设备
                        es.workState = State.Off;
                    }
                }
            }
        }
        //过渡思路
        #region
        //if (货物到达设备关键点2)//准备过渡
        //{
        //    拿出下一个设备
        //    if (下一个设备不存在)
        //    {
        //        设备=off
        //    }
        //    if(下一个设备On)
        //    {
        //        if (设备只能被单独占用（比如顶升）){
        //            设备=off
        //        } else {
        //            货物继续运动
        //        }
        //    }
        //    if (下一个设备off && 下一个设备的货物队列 == 0)
        //    {
        //        if (下一个设备 == Normal)
        //        {
        //            下一个设备=on;
        //        }
        //    }
        //    if(下一个设备off && 下一个设备货物队列 > 0)
        //    {
        //        本设备=off;
        //    }
        //}
        #endregion
    }
}
