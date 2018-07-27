using UnityEditor;
using UnityEngine;

public class LoadScene : EditorWindow
{
    private static LoadScene window;
    string Name = "WarehouseScene";

    [MenuItem("CustomObject/LoadScene", priority = 0)]
    static void Warehouse_Tool()
    {
        Rect window_position1 = new Rect(500, 1000, 300, 400);
        //获取当前窗口实例
        window = EditorWindow.GetWindowWithRect<LoadScene>(window_position1);
        //显示窗口
        window.Show();
    }

    private void OnGUI()
    {
        Name = EditorGUILayout.TextField("模型名称：", Name);
        
        if (GUILayout.Button("载入", GUILayout.Height(20)))
        {
            string path = "Scene/Simulation/";
            LoadOn(path, Name);
        }
    }

    public void LoadOn(string path,string Name) {
        string path0 = path + Name;
        GameObject obj = (GameObject)Resources.Load(path0);//载入模型
        GameObject OBJ = Instantiate(obj);
        OBJ.name = obj.name;
        //导入MainInterface
        string path1 = path + "MainInterface";
        GameObject MainInterface = Instantiate((GameObject)Resources.Load(path1));
        MainInterface.name = "MainInterface";
        //导入ProcessInterface
        string path2 = path + "ProcessInterface";
        GameObject ProcessInterface = Instantiate((GameObject)Resources.Load(path2));
        ProcessInterface.name = "ProcessInterface";
        //导入StorageStateInterface
        string path3 = path + "StorageStateInterface";
        GameObject StorageStateInterface = Instantiate((GameObject)Resources.Load(path3));
        StorageStateInterface.name = "StorageStateInterface";
        OBJ.AddComponent<Main>();

        //PositionsList POL = (PositionsList)OBJ.GetComponent<SceneData>().Positionslist;
        //Vector3 position = new Vector3(POL.HighBayPositions[3, 0], POL.HighBayPositions[3, 1], POL.HighBayPositions[3, 2]);
        //int Num1 = OBJ.GetComponent<ShowSceneInfo>().SceneInformation.HighBaysNum;
        //Debug.Log(position);
        //Debug.Log(Num1);
    }

}
