using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityExtension;

public class ObjSpawner : MonoBehaviour
{
	public string pathToFolderWithObjs;
	public Material brainMaterial;

	IEnumerator Start ()
	{
		string[] filesInFolder = System.IO.Directory.GetFiles(pathToFolderWithObjs);
		List<GameObject> loadedObjs = new List<GameObject> ();

		foreach (string filePath in filesInFolder)
		{
			if (System.IO.Path.GetExtension (filePath).Equals (".obj"))
			{
				string fileName = System.IO.Path.GetFileName (filePath);
				if (fileName.Contains ("hcp") || fileName.Contains("pial"))
					continue;
				//Debug.Log (fileName);

				GameObject targetObject = new GameObject ();
				targetObject.name = fileName;
				loadedObjs.Add (targetObject);

				//	Load the OBJ in
				System.IO.Stream lStream = new System.IO.FileStream(filePath, System.IO.FileMode.Open);
				OBJData lOBJData = OBJLoader.LoadOBJ(lStream);
				MeshFilter filter = targetObject.AddComponent<MeshFilter>();
				filter.mesh.LoadOBJ(lOBJData);
				lStream.Close();
				lStream = null;
				lOBJData = null;

//				ObjImporter importer = new ObjImporter ();
//				Mesh mesh = importer.ImportFile (filePath);
//				MeshFilter filter = targetObject.AddComponent<MeshFilter> ();
//				filter.mesh = mesh;

				filter.mesh.name = fileName;
				filter.mesh.RecalculateNormals ();
				filter.mesh.RecalculateTangents ();
				filter.mesh.RecalculateBounds ();
				MeshRenderer meshRenderer = targetObject.AddComponent<MeshRenderer>();
				meshRenderer.material = brainMaterial;

				targetObject.transform.parent = gameObject.transform;

				yield return null;
			}
		}
		Debug.Log ("Load finished");
	}


}
