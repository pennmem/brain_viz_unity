using UnityEditor;
using UnityEngine;
using System;
using System.Collections;

public class CreateAssetBundles : MonoBehaviour
{
    //Step 1: Download Brains.  Do this in a scene with an objSpawner, and note that downloading all brains will take a while.  The editor is single threaded and will freeze while downloading- you can check progress by looking at requests on the server.
    //Step 2: Create prefabs of the brains you want to make asset bundles for.  Do this by selecting the downloaded brains in the scene first.
    //Step 3: Build prefabs into asset bundles.  This will automatically build all assets bundles.  Prefabs will already have been added to the correct asset bundle.  This is fast-ish.

    //step 1
    [MenuItem("Assets/DownloadBrains")]
    static void DownloadBrains()
    {
        //Use this is a scene with an objSpawner
        ObjSpawner objSpawner = FindObjectOfType<ObjSpawner>();

        //THE BRAINS YOU WANT TO DOWNLOAD
        string[] subjects = RhinoRequestor.EditorSubjectListRequest(); //for all brains, can also be picked individually by making this some other array of strings.

		foreach (string subject in subjects)
		{
			Debug.Log ("Spawning objs of: " + subject);
			objSpawner.ObjSpawn (subject);
		}
	}

    //step 2
    [MenuItem("Assets/CreatePrefabFromSelected")]
    static void CreatePrefabMenu()
    {
        string meshDirectory = "Assets/savedMesh";
        if (!System.IO.Directory.Exists(meshDirectory))
        {
            System.IO.Directory.CreateDirectory(meshDirectory);
        }

        GameObject[] gos = Selection.gameObjects;

        foreach (GameObject go in gos)
        {
            string outputFolder = "Assets/savedMesh/" + go.name + "/";
            if (!System.IO.Directory.Exists(outputFolder))
            {
                System.IO.Directory.CreateDirectory(outputFolder);
            }

            foreach (MeshFilter meshFilter in go.GetComponentsInChildren<MeshFilter>())
            {
                Mesh mesh = meshFilter.mesh;
                string meshPath = outputFolder + meshFilter.gameObject.name + "_M" + ".asset";
                AssetDatabase.CreateAsset(mesh, meshPath);
                AssetImporter assetImporter = AssetImporter.GetAtPath(meshPath);
                assetImporter.assetBundleName = go.name;
                Mesh loadedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
                meshFilter.gameObject.GetComponent<MeshCollider>().sharedMesh = loadedMesh;
            }

            var prefab = PrefabUtility.CreateEmptyPrefab(outputFolder + go.name + ".prefab");
            PrefabUtility.ReplacePrefab(go, prefab);

            AssetDatabase.Refresh();

            string path = AssetDatabase.GetAssetPath(prefab);
            AssetImporter prefabImporter = AssetImporter.GetAtPath(path);
            prefabImporter.assetBundleName = go.name;
        }
    }

    //step 3
	[MenuItem("Assets/BuildAssetBundles")]
	static void BuildAssetBundles()
	{
		string assetBundleDirectory = "Assets/AssetBundles";
		if(!System.IO.Directory.Exists(assetBundleDirectory))
		{
			System.IO.Directory.CreateDirectory(assetBundleDirectory);
		}

		BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.WebGL);
	}

}