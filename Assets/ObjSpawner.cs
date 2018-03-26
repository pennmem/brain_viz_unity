﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityExtension;

public class ObjSpawner : MonoBehaviour
{
	public GameObject dkLeftParent;
	public GameObject dkRightParent;
	public GameObject hcpLeftParent;
	public GameObject hcpRightParent;
	public string pathToFolderWithObjs;

	public MonoBehaviour[] enableWhenFinished;
	public GameObject[] disableWhenFinished;

	void Awake ()
	{
		if (!this.enabled)
			return;

		string[] filesInFolder = System.IO.Directory.GetFiles(pathToFolderWithObjs);
		List<GameObject> loadedObjs = new List<GameObject> ();

		foreach (string filePath in filesInFolder)
		{
			if (System.IO.Path.GetExtension (filePath).Equals (".obj"))
			{
				string fileName = System.IO.Path.GetFileName (filePath);
				if (fileName.Contains("pial"))
					continue;

				// Load the .obj using TriLib
				GameObject targetObject;
				using (TriLib.AssetLoader assetLoader = new TriLib.AssetLoader())
				{
					byte[] fileData = System.IO.File.ReadAllBytes(filePath);
					targetObject = assetLoader.LoadFromMemory(fileData, ".obj");
				}

				targetObject.name = fileName;
				loadedObjs.Add (targetObject);

				//set up the components
				GameObject importantChild = targetObject.GetComponentInChildren<MeshRenderer>().gameObject;
				//GameObject importantChild = targetObject.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
				importantChild.AddComponent<MeshCollider>();
				importantChild.AddComponent<MouseOverOutline> ().outline = importantChild.AddComponent<cakeslice.Outline> ();
				importantChild.AddComponent<BrainPiece> ();
				importantChild.name = targetObject.name;

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
			}
		}
		Debug.Log ("Load finished");

		foreach (MonoBehaviour monoBehavior in enableWhenFinished)
			monoBehavior.enabled = true;
		foreach (GameObject disableMe in disableWhenFinished)
			disableMe.SetActive (false);
	}
}
