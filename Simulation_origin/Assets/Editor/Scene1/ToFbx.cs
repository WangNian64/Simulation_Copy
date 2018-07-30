using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ToFbx :  EditorWindow{

    [MenuItem("CustomObject/ToFbx", priority = 0)]
    static void ToFbxFile()
    {
        GameObject[] meshObjs = new GameObject[1];
        meshObjs[0] = Selection.activeGameObject;
        //用到动态库WRP_FBXExporter
        FBXExporter.ExportFBX("", "WarehouseScene", meshObjs, false);
        Debug.Log("完成");
    }

}
