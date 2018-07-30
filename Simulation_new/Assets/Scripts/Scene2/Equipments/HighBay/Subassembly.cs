using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HighStoreShelf_Parameter
{
    public int FloorsNum;//高层货架层数
    public int ColumnsNum;//高层货架列数
    public Vector3 Size;//高层货架的尺寸
    public float VerticalStanchionWidth;//竖直立柱的横截宽度
    public float HorizontalStanchionThick;//水平支柱的厚度
    public float[] FloorsHigh;//每层高度（高度包括水平支柱）
    public float Height2Ground;//首层据地高度
    public float ColumnWidth;//单列宽度（不包括立柱）
}
public class Subassembly : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
    #region HighBay_ReadTxt
    public static void HighBay_ReadTxt(ref string path, out HighStoreShelf_Parameter HP)
    {
        string[] strs1 = System.IO.File.ReadAllLines(path);
        //读取高架库列数
        int i1 = strs1[1].IndexOf("= ") + 1; string str1 = strs1[1].Remove(0, i1);
        int columns_num = int.Parse(str1); //Debug.Log(columns_num);
        //读取高架库列宽
        int i2 = strs1[2].IndexOf("= ") + 1; string str2 = strs1[2].Remove(0, i2);
        float column_width = float.Parse(str2); //Debug.Log(column_width);
        //读取高架库层数
        int i3 = strs1[3].IndexOf("= ") + 1; string str3 = strs1[3].Remove(0, i3);
        int floors_num = int.Parse(str3); //Debug.Log(floors_num);
        //读取高架库首层距地高度
        int i4 = strs1[4].IndexOf("= ") + 1; string str4 = strs1[4].Remove(0, i4);
        float AltitudeAgl = float.Parse(str4); //Debug.Log(AltitudeAgl);
        //读取高架库每层高度
        int i5 = strs1[5].IndexOf("【") + 1; int j5 = strs1[5].IndexOf("】");
        string str5 = strs1[5].Substring(i5, j5 - i5); string[] str5_1 = str5.Split(' ');
        float[] FloorsHigh = new float[floors_num + 1];
        //FloorsHigh[0] = AltitudeAgl;
        float High = 0;
        for (int i = 0; i < floors_num; i++)
        {
            FloorsHigh[i] = float.Parse(str5_1[i]);
            High = High + FloorsHigh[i];
        } //Debug.Log(High);
        //读取高架库仓位深度
        int i6 = strs1[6].IndexOf("= ") + 1; string str6 = strs1[6].Remove(0, i6);
        float HighBay_Depth = float.Parse(str6); //Debug.Log(HighBay_Depth);
        //读取正面水平支柱尺寸
        int i7 = strs1[7].IndexOf("【") + 1; int j7 = strs1[7].IndexOf("】");
        string str7 = strs1[7].Substring(i7, j7 - i7); string[] str7_1 = str7.Split(' ');
        float[] HorizontalStanchion_size = { float.Parse(str7_1[0]), float.Parse(str7_1[1]), float.Parse(str7_1[2]) };//高架库尺寸，【0】、【1】、【2】分别是x,z,y方向尺寸
        //读取侧面支柱倾斜角度
        int i8 = strs1[8].IndexOf("= ") + 1; string str8 = strs1[8].Remove(0, i8);
        float slope_angle = float.Parse(str8); //Debug.Log(slope_angle);
        
        HP.ColumnsNum = columns_num;
        HP.FloorsNum = floors_num;
        HP.FloorsHigh = FloorsHigh;
        HP.Height2Ground = AltitudeAgl;
        HP.HorizontalStanchionThick = HorizontalStanchion_size[2];
        HP.ColumnWidth = HorizontalStanchion_size[0];
        HP.VerticalStanchionWidth = column_width - HP.ColumnWidth;
        float[] size = { (columns_num + 2) * column_width, HighBay_Depth, High };
        HP.Size = new Vector3(HighBay_Depth, High + AltitudeAgl, column_width * columns_num);
    }
    #endregion

    #region Create_MainShelf
    public static void Create_MainShelf(HighStoreShelf_Parameter HSSP, int i, GameObject MainShelf)
    {
        GameObject TopShelf = new GameObject();
        Create_TopShelf(HSSP, TopShelf);
        GameObject EndShelf = new GameObject();
        Create_EndShelf(HSSP, i, EndShelf);
        TopShelf.transform.parent = MainShelf.transform;
        TopShelf.transform.localPosition = new Vector3(0, 0, 0);
        EndShelf.transform.parent = MainShelf.transform;
        EndShelf.transform.localPosition = new Vector3(0, 0, -(HSSP.ColumnWidth + HSSP.VerticalStanchionWidth) / 2);
        //GameObject.Find("ScriptsContainer").GetComponent<MyClass>().Create2(MainShelf);
    }
    #endregion

    #region Create_Foot
    public static void Create_Foot(HighStoreShelf_Parameter HSSP, GameObject Foot)
    {
        GameObject obj = (GameObject)Resources.Load("Scene/HighBay/Cube");
        GameObject obj1 = Instantiate(obj);
        Vector3 obj1_size = new Vector3(HSSP.VerticalStanchionWidth, HSSP.Height2Ground - HSSP.HorizontalStanchionThick, HSSP.VerticalStanchionWidth);
        obj1.transform.localScale = obj1_size;
        GameObject obj2 = Instantiate(obj);
        Vector3 obj2_size = new Vector3(3 * HSSP.VerticalStanchionWidth, 0.01f, 2 * HSSP.VerticalStanchionWidth);
        obj2.transform.localScale = obj2_size;
        GameObject OBJ = new GameObject();
        obj1.transform.parent = OBJ.transform; obj2.transform.parent = OBJ.transform;
        obj1.transform.localPosition = new Vector3(0, obj1_size.y / 2, 0);
        obj2.transform.localPosition = new Vector3(0, obj2_size.y / 2, 0);

        GameObject obj3 = Instantiate(obj);
        obj3.transform.localScale = new Vector3(HSSP.Size.x, HSSP.VerticalStanchionWidth, HSSP.VerticalStanchionWidth);

        GameObject OBJ2 = Instantiate(OBJ);
        OBJ.transform.parent = Foot.transform; OBJ2.transform.parent = Foot.transform; obj3.transform.parent = Foot.transform;
        OBJ.transform.localPosition = new Vector3((HSSP.Size.x - HSSP.VerticalStanchionWidth) / 2, 0, 0);
        OBJ2.transform.localPosition = new Vector3(-(HSSP.Size.x - HSSP.VerticalStanchionWidth) / 2, 0, 0);
        obj3.transform.localPosition = new Vector3(0, HSSP.Height2Ground / 2, 0);
        //GameObject.Find("ScriptsContainer").GetComponent<MyClass>().Create2(Foot);
    }
    #endregion

    #region Create_EndShelf
    public static void Create_EndShelf(HighStoreShelf_Parameter HSSP,int i, GameObject EndShelf)
    {
        //水平支柱
        GameObject obj = (GameObject)Resources.Load("Scene/HighBay/Cube");
        //竖直支柱
        GameObject obj2 = Instantiate(obj); //竖直支柱
        Vector3 obj2_size = new Vector3(HSSP.VerticalStanchionWidth, HSSP.FloorsHigh[i], HSSP.VerticalStanchionWidth);
        obj2.transform.localScale = obj2_size;
        GameObject obj2_2 = Instantiate(obj2);
        float temp_value1 = (HSSP.Size.x - HSSP.VerticalStanchionWidth) / 2;
        //侧面倾斜支柱
        float TempValue1 = HSSP.Size.x - HSSP.VerticalStanchionWidth;
        float TempValue2 = (HSSP.FloorsHigh[i] - HSSP.HorizontalStanchionThick) / 2;
        float TempValue3 = Mathf.Sqrt(TempValue1 * TempValue1 + TempValue2 * TempValue2);//侧面倾斜支柱的长度
        Vector3 SideStanchion_size = new Vector3(HSSP.VerticalStanchionWidth / 2, TempValue3, HSSP.VerticalStanchionWidth / 2);
        float angle = Mathf.Atan(TempValue1 / TempValue2) * 180 / Mathf.PI;
        GameObject obj3_1 = Instantiate(obj); obj3_1.transform.localScale = SideStanchion_size;
        obj3_1.transform.Rotate(0, 0, angle);
        GameObject obj3_2 = Instantiate(obj); obj3_2.transform.localScale = SideStanchion_size;
        obj3_2.transform.Rotate(0, 0, -angle);
        //组合
        obj2.transform.parent = EndShelf.transform; obj2_2.transform.parent = EndShelf.transform;
        obj2.transform.localPosition = new Vector3(temp_value1, HSSP.FloorsHigh[i] / 2 - HSSP.HorizontalStanchionThick, 0);
        obj2_2.transform.localPosition = new Vector3(-temp_value1, HSSP.FloorsHigh[i] / 2 - HSSP.HorizontalStanchionThick, 0);
        obj3_1.transform.parent = EndShelf.transform; obj3_2.transform.parent = EndShelf.transform;
        obj3_1.transform.localPosition = new Vector3(0, (obj2_size.y - HSSP.HorizontalStanchionThick) / 4, 0);
        obj3_2.transform.localPosition = new Vector3(0, 3 * (obj2_size.y - HSSP.HorizontalStanchionThick) / 4, 0);
        
        //GameObject.Find("ScriptsContainer").GetComponent<MyClass>().Create2(EndShelf);
        //GameObject.Find("ScriptsContainer").GetComponent<MyClass>().Create2(EndShelf);
    }
    #endregion

    #region Create_TopShelf
    public static void Create_TopShelf(HighStoreShelf_Parameter HSSP, GameObject TopShelf)
    {
        GameObject obj = (GameObject)Resources.Load("Scene/HighBay/Cube");
        //水平支柱
        GameObject obj1 = Instantiate(obj); //水平支柱
        Vector3 obj1_size = new Vector3(HSSP.VerticalStanchionWidth, HSSP.HorizontalStanchionThick, HSSP.ColumnWidth + 2 * HSSP.VerticalStanchionWidth);
        obj1.transform.localScale = obj1_size;
        GameObject obj1_2 = Instantiate(obj1);
        //水平交叉支柱
        float TempValue1 = HSSP.Size.x-HSSP.VerticalStanchionWidth;
        float TempValue4 = HSSP.ColumnWidth-HSSP.VerticalStanchionWidth;
        float TempValue5 = Mathf.Sqrt(TempValue1 * TempValue1 + TempValue4 * TempValue4);//水平交叉支柱的长度
        float angle1 = Mathf.Atan(TempValue1 / TempValue4) * 180 / Mathf.PI;
        Vector3 CrossStanchion_size = new Vector3(HSSP.VerticalStanchionWidth, HSSP.HorizontalStanchionThick / 2, TempValue5);
        GameObject obj2_1 = Instantiate(obj); obj2_1.transform.localScale = CrossStanchion_size;
        obj2_1.transform.Rotate(0, angle1, 0);
        GameObject obj2_2 = Instantiate(obj); obj2_2.transform.localScale = CrossStanchion_size;
        obj2_2.transform.Rotate(0, -angle1, 0);
        //组合
        obj1.transform.parent = TopShelf.transform; obj1_2.transform.parent = TopShelf.transform;
        float temp_value1 = HSSP.Size.x / 2 - HSSP.VerticalStanchionWidth / 2;
        float temp_value2 = -(HSSP.ColumnWidth + HSSP.VerticalStanchionWidth) / 2;
        obj1.transform.localPosition = new Vector3(temp_value1, -HSSP.HorizontalStanchionThick / 2, 0);
        obj1_2.transform.localPosition = new Vector3(-temp_value1, -HSSP.HorizontalStanchionThick / 2, 0);
        obj2_1.transform.parent = TopShelf.transform; obj2_2.transform.parent = TopShelf.transform;
        obj2_1.transform.localPosition = new Vector3(0, -obj1_size.y + CrossStanchion_size.y / 2, 0);
        obj2_2.transform.localPosition = new Vector3(0, -obj1_size.y + CrossStanchion_size.y / 2, 0);
        //GameObject.Find("ScriptsContainer").GetComponent<MyClass>().Create2(TopShelf);
    }
    #endregion

    #region Create_TopShelf1
    public static void Create_TopShelf1(HighStoreShelf_Parameter HSSP, GameObject TopShelf)
    {
        GameObject obj = (GameObject)Resources.Load("Scene/HighBay/Cube");
        //水平支柱
        GameObject obj1 = Instantiate(obj); //水平支柱
        Vector3 obj1_size = new Vector3(HSSP.VerticalStanchionWidth, HSSP.HorizontalStanchionThick, HSSP.ColumnWidth + HSSP.VerticalStanchionWidth / 2);
        obj1.transform.localScale = obj1_size;
        GameObject obj1_2 = Instantiate(obj1);
        //水平交叉支柱
        float TempValue1 = HSSP.Size.x - HSSP.VerticalStanchionWidth;
        float TempValue4 = HSSP.ColumnWidth - HSSP.VerticalStanchionWidth;
        float TempValue5 = Mathf.Sqrt(TempValue1 * TempValue1 + TempValue4 * TempValue4);//水平交叉支柱的长度
        float angle1 = Mathf.Atan(TempValue1 / TempValue4) * 180 / Mathf.PI;
        Vector3 CrossStanchion_size = new Vector3(HSSP.VerticalStanchionWidth, HSSP.HorizontalStanchionThick / 2, TempValue5);
        GameObject obj2_1 = Instantiate(obj); obj2_1.transform.localScale = CrossStanchion_size;
        obj2_1.transform.Rotate(0, angle1, 0);
        GameObject obj2_2 = Instantiate(obj); obj2_2.transform.localScale = CrossStanchion_size;
        obj2_2.transform.Rotate(0, -angle1, 0);
        //
        GameObject obj3 = Instantiate(obj);
        Vector3 obj3_size = new Vector3(HSSP.Size.x, HSSP.HorizontalStanchionThick, HSSP.VerticalStanchionWidth / 2);
        obj3.transform.localScale = obj3_size;
        //倾斜固定
        GameObject obj4 = Instantiate(obj);
        float tempvalue1 = HSSP.FloorsHigh[HSSP.FloorsNum - 1];
        float tempvalue2 = HSSP.ColumnWidth;
        float tempvalue3 = Mathf.Sqrt(tempvalue1 * tempvalue1 + tempvalue2 * tempvalue2);
        Vector3 obj4_size = new Vector3(HSSP.VerticalStanchionWidth, HSSP.VerticalStanchionWidth, tempvalue3);
        obj4.transform.localScale = obj4_size;
        float angle2 = Mathf.Atan(tempvalue1 / tempvalue2) * 180 / Mathf.PI;
        obj4.transform.Rotate(-angle2, 0, 0);
        GameObject obj5 = Instantiate(obj4);
        //组合
        obj1.transform.parent = TopShelf.transform; obj1_2.transform.parent = TopShelf.transform;
        float temp_value1 = HSSP.Size.x / 2 - HSSP.VerticalStanchionWidth / 2;
        float temp_value2 = -(HSSP.ColumnWidth + HSSP.VerticalStanchionWidth) / 2;
        obj1.transform.localPosition = new Vector3(temp_value1, -HSSP.HorizontalStanchionThick / 2, obj1_size.z / 2);
        obj1_2.transform.localPosition = new Vector3(-temp_value1, -HSSP.HorizontalStanchionThick / 2, obj1_size.z / 2);
        obj2_1.transform.parent = TopShelf.transform; obj2_2.transform.parent = TopShelf.transform;
        obj2_1.transform.localPosition = new Vector3(0, -obj1_size.y + CrossStanchion_size.y / 2, HSSP.ColumnWidth / 2 + HSSP.VerticalStanchionWidth);
        obj2_2.transform.localPosition = new Vector3(0, -obj1_size.y + CrossStanchion_size.y / 2, HSSP.ColumnWidth / 2 + HSSP.VerticalStanchionWidth);
        obj3.transform.parent = TopShelf.transform;
        obj3.transform.localPosition = new Vector3(0, -obj3_size.y / 2, obj1_size.z - obj3_size.z / 2);

        obj4.transform.parent = TopShelf.transform; obj5.transform.parent = TopShelf.transform;
        obj4.transform.localPosition = new Vector3(temp_value1, -tempvalue1 / 2 - obj1_size.y / 2, tempvalue2 / 2);
        obj5.transform.localPosition = new Vector3(-temp_value1, -tempvalue1 / 2 - obj1_size.y / 2, tempvalue2 / 2);
        //GameObject.Find("ScriptsContainer").GetComponent<MyClass>().Create2(TopShelf);
    }
    #endregion

    #region Create_HighStoreShelf
    public static void Create_HighStoreShelf(HighStoreShelf_Parameter HSSP, GameObject HighStoreShelf)
    {
        float TempValue1 = HSSP.ColumnWidth + HSSP.VerticalStanchionWidth;
        float TempValue2 = HSSP.ColumnWidth / 2 + HSSP.VerticalStanchionWidth;
        float TempValue3 = HSSP.Height2Ground;

        for (int i = 0; i < HSSP.FloorsNum; i++)
        {
            GameObject MainShelf = new GameObject(); MainShelf.name = "MainShelf";
            Create_MainShelf(HSSP, i, MainShelf);
            for (int j = 0; j < HSSP.ColumnsNum; j++)
            {
                GameObject clone = Instantiate(MainShelf); clone.name = MainShelf.name + "_" + (i + 1).ToString() + "_" + (j + 1).ToString();
                clone.transform.parent = HighStoreShelf.transform;
                clone.transform.localPosition = new Vector3(0, TempValue3, TempValue2 + j * TempValue1);
            }
            GameObject EndShelf = new GameObject(); EndShelf.name = "EndShelf" + (i + 1).ToString();
            Create_EndShelf(HSSP, i, EndShelf);
            MyClass.Create2(EndShelf);
            EndShelf.transform.parent = HighStoreShelf.transform;
            EndShelf.transform.localPosition = new Vector3(0, TempValue3, HSSP.ColumnsNum * TempValue1 + HSSP.VerticalStanchionWidth / 2);
            TempValue3 = TempValue3 + HSSP.FloorsHigh[i];
            DestroyImmediate(MainShelf);
        }
        GameObject Foot = new GameObject(); Foot.name = "Foot"; Create_Foot(HSSP, Foot);
        GameObject TopShelf = new GameObject(); TopShelf.name = "TopShelf"; Create_TopShelf(HSSP, TopShelf);
        MyClass.Create2(TopShelf);
        for (int i = 0; i < HSSP.ColumnsNum; i++)
        {
            GameObject clone1 = Instantiate(Foot); clone1.name = Foot.name + (i + 1).ToString();
            clone1.transform.parent = HighStoreShelf.transform;
            clone1.transform.localPosition = new Vector3(0, 0, HSSP.VerticalStanchionWidth / 2 + i * TempValue1);

            GameObject clone2 = Instantiate(TopShelf); clone2.name = TopShelf.name + (i + 1).ToString();
            clone2.transform.parent = HighStoreShelf.transform;
            clone2.transform.localPosition = new Vector3(0, TempValue3, TempValue2 + i * TempValue1);
        }
        Foot.name = Foot.name + (HSSP.ColumnsNum + 1).ToString(); Foot.transform.parent = HighStoreShelf.transform;
        Foot.transform.localPosition = new Vector3(0, 0, HSSP.ColumnsNum * TempValue1 + HSSP.VerticalStanchionWidth / 2);
        GameObject TopShelf0 = new GameObject(); TopShelf0.name = TopShelf.name + 0.ToString();
        Create_TopShelf1(HSSP, TopShelf0);
        GameObject TopShelf1 = Instantiate(TopShelf0); TopShelf1.name = TopShelf.name + (HSSP.ColumnsNum + 1).ToString();
        TopShelf1.transform.Rotate(0, 180, 0);
        TopShelf0.transform.parent = HighStoreShelf.transform; TopShelf1.transform.parent = HighStoreShelf.transform;
        TopShelf0.transform.localPosition = new Vector3(0, TempValue3, HSSP.ColumnsNum * TempValue1 + HSSP.VerticalStanchionWidth);
        TopShelf1.transform.localPosition = new Vector3(0, TempValue3, 0);
        DestroyImmediate(TopShelf);
    }
    #endregion
}
