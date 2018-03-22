using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityExtension;

public class ObjSpawner : MonoBehaviour
{
	public GameObject dkLeftParent;
	public GameObject dkRightParent;
	public GameObject hcpLeftParent;
	public GameObject hcpRightParent;
	public GameObject baseObject;
	public string pathToFolderWithObjs;
	public Material brainMaterial;

	public MonoBehaviour[] enableWhenFinished;

	void Awake ()
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

				if (fileName.Contains ("hcp") && fileName.Contains ("lh"))
					targetObject.transform.parent = hcpLeftParent.transform;
				else if (fileName.Contains ("hcp") && fileName.Contains ("rh"))
					targetObject.transform.parent = hcpRightParent.transform;
				else if (fileName.Contains ("lh"))
					targetObject.transform.parent = dkLeftParent.transform;
				else if (fileName.Contains ("rh"))
					targetObject.transform.parent = dkRightParent.transform;
				else
					throw new UnityException ("An obj with an unrecorgnized naming exists.");

				//yield return null;
			}
		}
		Debug.Log ("Load finished");
		foreach (MonoBehaviour monoBehavior in enableWhenFinished)
			monoBehavior.enabled = true;
	}

	void Start()
	{
		gameObject.transform.localScale = new Vector3 (-1, 1, 1);
	}


}
