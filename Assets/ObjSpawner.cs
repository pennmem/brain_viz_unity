using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityExtension;

public class ObjSpawner : MonoBehaviour
{
	public GameObject dkParent;
	public GameObject hcpParent;
	public GameObject baseObject;
	public string pathToFolderWithObjs;
	public Material brainMaterial;

	public MonoBehaviour[] enableWhenFinished;

	void Start ()
	{
		string[] filesInFolder = System.IO.Directory.GetFiles(pathToFolderWithObjs);
		List<GameObject> loadedObjs = new List<GameObject> ();

		foreach (string filePath in filesInFolder)
		{
			if (System.IO.Path.GetExtension (filePath).Equals (".obj"))
			{
				string fileName = System.IO.Path.GetFileName (filePath);
				if (fileName.Contains("pial"))
					continue;

				GameObject targetObject = Instantiate<GameObject>(baseObject);
				targetObject.name = fileName;
				loadedObjs.Add (targetObject);

				//	Load the .obj using the OBJ-IO asset
				System.IO.Stream lStream = new System.IO.FileStream(filePath, System.IO.FileMode.Open);
				OBJData lOBJData = OBJLoader.LoadOBJ(lStream);
				MeshFilter filter = targetObject.GetComponent<MeshFilter>();
				filter.mesh.LoadOBJ(lOBJData);
				lStream.Close();
				lStream = null;
				lOBJData = null;

				filter.mesh.name = fileName;
				targetObject.GetComponent<MeshCollider> ().sharedMesh = filter.mesh;
				filter.mesh.RecalculateNormals ();
				//filter.mesh.RecalculateTangents ();
				//filter.mesh.RecalculateBounds ();
				MeshRenderer meshRenderer = targetObject.GetComponent<MeshRenderer>();
				meshRenderer.material = brainMaterial;

				if (fileName.Contains ("hcp"))
					targetObject.transform.parent = hcpParent.transform;
				else
					targetObject.transform.parent = dkParent.transform;

				//yield return null;
			}
		}
		Debug.Log ("Load finished");
		foreach (MonoBehaviour monoBehavior in enableWhenFinished)
			monoBehavior.enabled = true;
	}


}
