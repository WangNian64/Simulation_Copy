using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CargoInfo
{
    public string Name;//货物名称
    public Vector3 Size;
    public string Number1;//货物编号
    public int Num;//货物件数
    public PositionInfo PositionInfo;//所要存储所在仓位信息
    public string InputTime;//入库时间
    public string Description;//其他描述
}

public class Cargo1 : MonoBehaviour
{
    //货物
    // Use this for initialization
    void Start()
    {
        GameObject OBJ = new GameObject(); OBJ.name = "Cargo";
        Vector3 ContainerSize = new Vector3(1.25f, 0.8f, 1.05f);
        //Create_Cargo(ContainerSize,CI, OBJ);
    }

    // 货物架板
    #region Create_Tray
    public static void Create_Tray(Vector2 Size, GameObject Tray)
    {
        GameObject obj = (GameObject)Resources.Load("Scene/Cargo/Cube1");//创建底座
        GameObject obj1 = Instantiate(obj); Vector3 size1 = new Vector3(Size.x, 0.02f, 0.1f); obj1.transform.localScale = size1;//X向条板
        GameObject obj2 = Instantiate(obj); Vector3 size2 = new Vector3(0.14f, 0.02f, Size.y); obj2.transform.localScale = size2;//Z向条板
        GameObject obj3 = Instantiate(obj); Vector3 size3 = new Vector3(Size.x, 0.02f, Size.y); obj3.transform.localScale = size3;//全板
        GameObject obj4 = Instantiate(obj); Vector3 size4 = new Vector3(0.14f, 0.09f, 0.1f); obj4.transform.localScale = size4;//支撑块

        GameObject OBJ1 = new GameObject(); OBJ1.name = "X";//X向条板+3个支撑块
        GameObject obj4_1 = Instantiate(obj4); GameObject obj4_2 = Instantiate(obj4);
        obj1.transform.parent = OBJ1.transform; obj1.transform.localPosition = new Vector3(0, size1.y / 2, 0);
        obj4.transform.parent = OBJ1.transform; obj4.transform.localPosition = new Vector3(0, size1.y + size4.y / 2, 0);
        obj4_1.transform.parent = OBJ1.transform; obj4_1.transform.localPosition = new Vector3(size1.x / 2 - size4.x / 2, size1.y + size4.y / 2, 0);
        obj4_2.transform.parent = OBJ1.transform; obj4_2.transform.localPosition = new Vector3(-size1.x / 2 + size4.x / 2, size1.y + size4.y / 2, 0);

        GameObject OBJ2 = new GameObject(); OBJ2.name = "Z";//3个Z向条板+全板
        GameObject obj2_1 = Instantiate(obj2); GameObject obj2_2 = Instantiate(obj2);
        obj3.transform.parent = OBJ2.transform; obj3.transform.localPosition = new Vector3(0, -size3.y / 2, 0);
        obj2.transform.parent = OBJ2.transform; obj2.transform.localPosition = new Vector3(0, -size3.y - size2.y / 2, 0);
        obj2_1.transform.parent = OBJ2.transform; obj2_1.transform.localPosition = new Vector3(size3.x / 2 - size2.x / 2, -size3.y - size2.y / 2, 0);
        obj2_2.transform.parent = OBJ2.transform; obj2_2.transform.localPosition = new Vector3(-size3.x / 2 + size2.x / 2, -size3.y - size2.y / 2, 0);

        GameObject OBJ11 = Instantiate(OBJ1); GameObject OBJ12 = Instantiate(OBJ1);
        OBJ1.transform.parent = Tray.transform; OBJ1.transform.localPosition = new Vector3(0, 0, 0);
        OBJ11.transform.parent = Tray.transform; OBJ11.transform.localPosition = new Vector3(0, 0, size2.z / 2 - size4.z / 2);
        OBJ12.transform.parent = Tray.transform; OBJ12.transform.localPosition = new Vector3(0, 0, -size2.z / 2 + size4.z / 2);
        OBJ2.transform.parent = Tray.transform; OBJ2.transform.localPosition = new Vector3(0, 0.15f, 0);

        //GameObject.Find("ScriptsContainer").GetComponent<MyClass>().Create2(Tray);
    }
    #endregion

    #region Create_Tray1
    public static void Create_Tray1(Vector2 Size, GameObject Tray)
    {
        GameObject obj = (GameObject)Resources.Load("Scene/Cargo/Cube1");//创建底座
        GameObject OBJ = Instantiate(obj); Vector3 size = new Vector3(0.1f, 0.12f, 0.1f); OBJ.transform.localScale = size;
        GameObject OBJJ = Instantiate(obj); Vector3 size1 = new Vector3(Size.x, 0.03f, Size.y); OBJJ.transform.localScale = size1;
        OBJJ.transform.parent = Tray.transform; OBJJ.transform.localPosition = new Vector3(0, 0.135f, 0);
        float TempValue1 = (Size.x / 2 - 0.05f); float TempValue2 = (Size.y / 2 - 0.05f);
        GameObject OBJ0 = Instantiate(OBJ); OBJ0.name = "OBJ0"; OBJ0.transform.parent = Tray.transform; OBJ0.transform.localPosition = new Vector3(0, 0.06f, 0);
        GameObject OBJ01 = Instantiate(OBJ); OBJ01.name = "OBJ01"; OBJ01.transform.parent = Tray.transform; OBJ01.transform.localPosition = new Vector3(0, 0.06f, TempValue2);
        GameObject OBJ02 = Instantiate(OBJ); OBJ02.name = "OBJ02"; OBJ02.transform.parent = Tray.transform; OBJ02.transform.localPosition = new Vector3(0, 0.06f, -TempValue2);
        GameObject OBJ1 = Instantiate(OBJ); OBJ1.name = "OBJ1"; OBJ1.transform.parent = Tray.transform; OBJ1.transform.localPosition = new Vector3(TempValue1, 0.06f, 0);
        GameObject OBJ11 = Instantiate(OBJ); OBJ11.name = "OBJ11"; OBJ11.transform.parent = Tray.transform; OBJ11.transform.localPosition = new Vector3(TempValue1, 0.06f, TempValue2);
        GameObject OBJ12 = Instantiate(OBJ); OBJ12.name = "OBJ12"; OBJ12.transform.parent = Tray.transform; OBJ12.transform.localPosition = new Vector3(TempValue1, 0.06f, -TempValue2);
        GameObject OBJ2 = Instantiate(OBJ); OBJ2.name = "OBJ2"; OBJ2.transform.parent = Tray.transform; OBJ2.transform.localPosition = new Vector3(-TempValue1, 0.06f, 0);
        GameObject OBJ21 = Instantiate(OBJ); OBJ21.name = "OBJ21"; OBJ21.transform.parent = Tray.transform; OBJ21.transform.localPosition = new Vector3(-TempValue1, 0.06f, TempValue2);
        GameObject OBJ22 = Instantiate(OBJ); OBJ22.name = "OBJ22"; OBJ22.transform.parent = Tray.transform; OBJ22.transform.localPosition = new Vector3(-TempValue1, 0.06f, -TempValue2);
        DestroyImmediate(OBJ);
        //GameObject.Find("ScriptsContainer").GetComponent<MyClass>().Create2(Tray);
    }
    #endregion

    // 货物箱
    #region
    public static void Create_Container(Vector3 Size, GameObject OBJ)
    {
        GameObject obj2 = (GameObject)Resources.Load("Scene/Cargo/Cube2");//创建货物
        //货物箱
        GameObject obj2_1 = Instantiate(obj2);
        obj2_1.transform.localScale = Size;
        obj2_1.transform.parent = OBJ.transform; obj2_1.transform.localPosition = new Vector3(0, Size.y / 2, 0);
        //GameObject.Find("ScriptsContainer").GetComponent<MyClass>().Create2(OBJ);
    }
    #endregion
    // 货物
    #region Create_Cargo
    public static void Create_Cargo(Vector3 Size, GameObject Cargo)
    {
        //Vector3 Size = CI.Size;
        Vector2 size1 = new Vector2(Size.x, Size.z);
        Vector3 size2 = new Vector3(Size.x, Size.y - 0.15f, Size.z);
        GameObject Tray = new GameObject(); Tray.name = "Tray";
        Create_Tray1(size1, Tray);
        GameObject Container = new GameObject(); Container.name = "Container";
        Create_Container(size2, Container);
        Tray.transform.parent = Cargo.transform; Tray.transform.localPosition = new Vector3(0, 0, 0);
        Container.transform.parent = Cargo.transform; Container.transform.localPosition = new Vector3(0, 0.15f, 0);
        MyClass.Create2(Cargo);
    }
    public static void Create_Cargo(CargoInfo CI, GameObject Cargo)
    {
        Vector3 Size = CI.Size;
        Vector2 size1 = new Vector2(Size.x, Size.z);
        Vector3 size2 = new Vector3(Size.x, Size.y - 0.15f, Size.z);
        GameObject Tray = new GameObject(); Tray.name = "Tray";
        Create_Tray1(size1, Tray);
        GameObject Container = new GameObject(); Container.name = "Container";
        Create_Container(size2, Container);
        Tray.transform.parent = Cargo.transform; Tray.transform.localPosition = new Vector3(0, 0, 0);
        Container.transform.parent = Cargo.transform; Container.transform.localPosition = new Vector3(0, 0.15f, 0);
        MyClass.Create2(Cargo);
        //给货物添加Message
        Cargo.AddComponent<ShowCargoInfo>();
        ShowCargoInfo ShowCargoInfo = Cargo.GetComponent<ShowCargoInfo>();
        CargoMessage CargoMessage;
        CreateCargoInfo(ref CI, out CargoMessage);
        ShowCargoInfo.Cargomessage = CargoMessage;

    }
    #endregion

    #region Create_CargoInfo
    public static void CreateCargoInfo(ref CargoInfo CI, out CargoMessage CargoMessage)
    {
        CargoMessage CM = new CargoMessage();
        CM.Name = CI.Name;
        CM.Size = CI.Size;
        CM.Number1 = CI.Number1;
        CM.Num = CI.Num;
        CM.PositionInfo = CI.PositionInfo;
        CM.InputTime = CI.InputTime;
        CM.Description = CI.Description;
        CargoMessage = CM;
    }
    #endregion
}
