using UnityEditor;
using UnityEngine;
using System;
using System.Collections;

public class CreateAssetBundles : MonoBehaviour
{
	[MenuItem("Assets/DownloadBrains")]
	static void DownloadBrains()
	{
		ObjSpawner objSpawner = GameObject.FindObjectOfType<ObjSpawner>();

		string[] subjects = new string[] { "fsaverage_joel" }; //RhinoRequestor.EditorSubjectListRequest();
		foreach (string subject in subjects)
		{
			Debug.Log ("Spawning objs of: " + subject);
			objSpawner.EditorSpawn (subject);
		}
	}

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

	[MenuItem("Assets/CreatePrefabFromSelected")]
	static void CreatePrefabMenu ()
	{
		string meshDirectory = "Assets/savedMesh";
		if(!System.IO.Directory.Exists(meshDirectory))
		{
			System.IO.Directory.CreateDirectory(meshDirectory);
		}

		GameObject[] gos = Selection.gameObjects;

		foreach (GameObject go in gos)
		{
			string outputFolder = "Assets/savedMesh/" + go.name + "/";
			if(!System.IO.Directory.Exists(outputFolder))
			{
				System.IO.Directory.CreateDirectory(outputFolder);
			}

			foreach (MeshFilter meshFilter in go.GetComponentsInChildren<MeshFilter>())
			{
				Mesh mesh = meshFilter.mesh;
				string meshPath = outputFolder + meshFilter.gameObject.name + "_M" + ".asset";
				AssetDatabase.CreateAsset (mesh, meshPath);
				AssetImporter assetImporter = AssetImporter.GetAtPath (meshPath);
				assetImporter.assetBundleName = go.name;
				Mesh loadedMesh = AssetDatabase.LoadAssetAtPath <Mesh>(meshPath);
				meshFilter.gameObject.GetComponent<MeshCollider> ().sharedMesh = loadedMesh;
			}

			var prefab = PrefabUtility.CreateEmptyPrefab (outputFolder + go.name + ".prefab");
			PrefabUtility.ReplacePrefab (go, prefab);

			AssetDatabase.Refresh ();

			string path = AssetDatabase.GetAssetPath (prefab);
			AssetImporter prefabImporter = AssetImporter.GetAtPath (path);
			prefabImporter.assetBundleName = go.name;
		}
	}
}