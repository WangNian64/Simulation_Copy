using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//有bug
public class CargoEnterController : MonoBehaviour {
    public GameObject cargo;//所属货物
    public CargoMessage cargoMessage;//货物信息
	// Use this for initialization
	void Start () {
        cargo = this.gameObject;
        cargoMessage = cargo.GetComponent<ShowCargoInfo>().Cargomessage;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        cargoMessage = cargo.GetComponent<ShowCargoInfo>().Cargomessage;
        Debug.Log("进入cargoEnterController");
        Debug.Log("货物设备队列的数目：" + cargoMessage.EquipmentsQueue.Count);

        //货物信息
        //从货物的设备队列中取出一个设备
        GameObject equipment = cargoMessage.EquipmentsQueue.Peek();
        //货物经过当前设备，准备进入下一个设备
        if (EquipExtension.isCrossEquip(cargo, equipment))
        {
            //出队，换新设备
            cargoMessage.EquipmentsQueue.Dequeue();
            GameObject nextEquip = cargoMessage.EquipmentsQueue.Peek();
            if (nextEquip != null)
            {
                cargo.transform.parent = nextEquip.transform;
            }
            else
            {
                Debug.Log("货物" + cargo.name + "入库已经完成！");
            }
        }

        //设备队列不空
        //while (cargoMessage.EquipmentsQueue.Count > 0)
        //{
        //    //货物信息
        //    //从货物的设备队列中取出一个设备
        //    GameObject equipment = cargoMessage.EquipmentsQueue.Peek();
        //    //货物经过当前设备，准备进入下一个设备
        //    if (EquipExtension.isCrossEquip(cargo, equipment))
        //    {
        //        //出队，换新设备
        //        cargoMessage.EquipmentsQueue.Dequeue();
        //        GameObject nextEquip = cargoMessage.EquipmentsQueue.Peek();
        //        if (nextEquip != null)
        //        {
        //            cargo.transform.parent = nextEquip.transform;
        //        }
        //        else
        //        {
        //            Debug.Log("货物" + cargo.name + "入库已经完成！");
        //        }
        //    }
        //}
    }
}
