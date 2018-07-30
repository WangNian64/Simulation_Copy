using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
public class MyClass : MonoBehaviour
{
    void Start()
    {
       
    }

    public static void CreatePrefab(GameObject go, string Path)
    {
        //先创建一个空的预制物体
        //预制物体保存在工程中路径，可以修改("Assets/" + name + ".prefab");
        Object tempPrefab = PrefabUtility.CreateEmptyPrefab(Path + ".prefab");
        //然后拿我们场景中的物体   替换空的预制物体
        tempPrefab = PrefabUtility.ReplacePrefab(go, tempPrefab);
        //返回创建后的预制物体
        //return tempPrefab;
    }
    public static void Create2(GameObject obj)
    {
        var meshFilters = obj.GetComponentsInChildren<MeshFilter>();
        var combines = new CombineInstance[meshFilters.Length];
        var materialList = new List<Material>();
        //string newMeshPath = path;
        for (int i = 0; i < meshFilters.Length; i++)
        {
            combines[i].mesh = meshFilters[i].sharedMesh;
            combines[i].transform = Matrix4x4.TRS(meshFilters[i].transform.position - obj.transform.position,
                meshFilters[i].transform.rotation, meshFilters[i].transform.lossyScale);
            var materials = meshFilters[i].GetComponent<MeshRenderer>().sharedMaterials;
            foreach (var material in materials)
            {
                materialList.Add(material);
            }
        }
        var newMesh = new Mesh();
        newMesh.CombineMeshes(combines, false);

#if !UNITY_5_5_OR_NEWER
            //Mesh.Optimize was removed in version 5.5.2p4.
            newMesh.Optimize();
#endif
        //GameObject obj = obj;
        obj.AddComponent<MeshFilter>().sharedMesh = newMesh;
        obj.AddComponent<MeshCollider>().sharedMesh = newMesh;
        obj.AddComponent<MeshRenderer>().sharedMaterials = materialList.ToArray();
        string newMeshPath = "Assets\\Resources\\SaveMesh\\"+obj.name;
        newMeshPath = newMeshPath + obj.name;
        AssetDatabase.CreateAsset(newMesh, newMeshPath);
        AssetDatabase.Refresh();
        //Selection.activeObject = newMesh;
        for (int i = obj.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(obj.transform.GetChild(i).gameObject);
        }

    }
    //计算尺寸
    public static void MeshSize(GameObject obj, ref Vector3 Size)
    {
        var meshFilters = obj.GetComponentsInChildren<MeshFilter>();
        var combines = new CombineInstance[meshFilters.Length];
        for (int i = 0; i < meshFilters.Length; i++)
        {
            combines[i].mesh = meshFilters[i].sharedMesh;
            combines[i].transform = Matrix4x4.TRS(meshFilters[i].transform.position - obj.transform.position,
                meshFilters[i].transform.rotation, meshFilters[i].transform.lossyScale);
        }
        var newMesh = new Mesh();
        newMesh.CombineMeshes(combines, false);
        Size = newMesh.bounds.size;
    }
    static void ExportWholeSelectionToSingle()
    {
        Transform[] selection = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);

        if (selection.Length == 0)
        {
            EditorUtility.DisplayDialog("No source object selected!", "Please select one or more target objects", "");
            return;
        }

        int exportedObjects = 0;

        ArrayList mfList = new ArrayList();

        for (int i = 0; i < selection.Length; i++)
        {
            Component[] meshfilter = selection[i].GetComponentsInChildren(typeof(MeshFilter));

            for (int m = 0; m < meshfilter.Length; m++)
            {
                exportedObjects++;
                mfList.Add(meshfilter[m]);
            }
        }

        if (exportedObjects > 0)
        {
            MeshFilter[] mf = new MeshFilter[mfList.Count];

            for (int i = 0; i < mfList.Count; i++)
            {
                mf[i] = (MeshFilter)mfList[i];
            }

            string filename = EditorApplication.currentScene + "_" + exportedObjects;

            int stripIndex = filename.LastIndexOf('/');//FIXME: Should be Path.PathSeparator

            if (stripIndex >= 0)
                filename = filename.Substring(stripIndex + 1).Trim();

            //GameObject.Find("Main Camera").GetComponent<ObjExporter>(). MeshesToFile(mf, targetFolder, filename);


            EditorUtility.DisplayDialog("Objects exported", "Exported " + exportedObjects + " objects to " + filename, "");
        }
        else
            EditorUtility.DisplayDialog("Objects not exported", "Make sure at least some of your selected objects have mesh filters!", "");
    }
    public static bool isCrossPos(GameObject go, Vector3 pos)
    {
        return true;
    }
}