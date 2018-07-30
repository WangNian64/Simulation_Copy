using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiConveyorMove : MonoBehaviour {
    public float speed;
    public Vector3 direction;
    // Use this for initialization
    void Start () {
        BiConveyorState bcs = this.gameObject.GetComponent<ShowEquipState>().equipmentState as BiConveyorState;
        speed = bcs.deliverSpeed;
        direction = bcs.deliverDirection;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        BiConveyorState bcs = this.gameObject.GetComponent<ShowEquipState>().equipmentState as BiConveyorState;
        speed = bcs.deliverSpeed;
        direction = bcs.deliverDirection;
        //让该设备上所有的货物都运动
        List<GameObject> cargoList = new List<GameObject>();
        FindExtension.FindGameObjectsWithTagRecursive(this.gameObject, "Cargo", ref cargoList);
        if (bcs.workState == State.On)
        {
            foreach (GameObject cargo in cargoList)
            {
                cargo.transform.localPosition += direction * speed * Time.deltaTime;
            }
        }
    }
}
