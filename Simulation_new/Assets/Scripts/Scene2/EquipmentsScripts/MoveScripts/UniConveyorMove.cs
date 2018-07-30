using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UniConveyorMove : MonoBehaviour {
    public float speed;
    public Vector3 direction;
	// Use this for initialization
	void Start () {
        UniConveyorState ucs = this.gameObject.GetComponent<ShowEquipState>().equipmentState as UniConveyorState;
        speed = ucs.deliverSpeed;
        direction = ucs.deliverDirection;
	}
	
	// Update is called once per frame
	void Update () {
        UniConveyorState ucs = this.gameObject.GetComponent<ShowEquipState>().equipmentState as UniConveyorState;
        speed = ucs.deliverSpeed;
        direction = ucs.deliverDirection;
        //让该设备上所有的货物都运动
        List<GameObject> cargoList = new List<GameObject>();
        FindExtension.FindChildWithTag(this.gameObject, "Cargo", ref cargoList);
        Debug.Log(cargoList.Count);
        if (ucs.workState == State.On)
        {
            foreach (GameObject cargo in cargoList)
            {
                cargo.transform.localPosition += direction * speed * Time.deltaTime;
            }
        }
    }
}
