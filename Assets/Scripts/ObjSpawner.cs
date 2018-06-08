﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityExtension;

public class ObjSpawner : Spawner
{
	private GameObject dk;
	private GameObject hcp;
	private GameObject dkLeftParent;
	private GameObject dkRightParent;
	private GameObject hcpLeftParent;
	private GameObject hcpRightParent;

	public Material brainMaterial;
	public UnityEngine.UI.Text loadingText;

	private AssetBundle currentBundle;

	public void SetDKActive(bool active)
	{
		dk.SetActive(active);
	}

	public void SetHCPActive(bool active)
	{
		hcp.SetActive(active);
	}

	public void SetLeftActive(bool active)
	{
		if (dk.activeSelf)
			dkLeftParent.SetActive (active);
		else
			hcpLeftParent.SetActive(active);
	}

	public void SetRightActive(bool active)
	{
		if (dk.activeSelf)
			dkRightParent.SetActive (active);
		else
			hcpRightParent.SetActive(active);
	}

	public override IEnumerator Spawn(string subject, bool average_brain = false)
	{
		CoroutineWithData subjectListRequest = new CoroutineWithData (this, RhinoRequestor.AssetBundleRequest(subject));
		yield return subjectListRequest.coroutine;
		if (subjectListRequest.result == null)
		{
			loadingText.text = "AssetBundle not found. Performign alternative (slower) load. Please wait.";
			yield return null;
			EditorSpawn (subject, average_brain);
			yield break;
		}
		AssetBundle bundle = (AssetBundle)subjectListRequest.result;
		GameObject brainPrefab = bundle.LoadAsset<GameObject>(subject);
		GameObject brain = Instantiate (brainPrefab);
		brain.transform.parent = gameObject.transform;
		dk = brain.transform.GetChild (1).gameObject;
		hcp = brain.transform.GetChild (0).gameObject;
		dkLeftParent = dk.transform.GetChild (0).gameObject;
		dkRightParent = dk.transform.GetChild (1).gameObject;
		hcpLeftParent = hcp.transform.GetChild (0).gameObject;
		hcpRightParent = hcp.transform.GetChild (1).gameObject;

		currentBundle = bundle;

		hcp.SetActive (false);
	}

	public override void Despawn()
	{
		base.Despawn ();

		if (currentBundle != null)
			currentBundle.Unload (true);
	}

	public void EditorSpawn(string subjectName, bool average_brain = false)
	{
		float startTime = Time.time;

		GameObject subject = new GameObject (subjectName);
		subject.transform.parent = gameObject.transform;
		hcp = new GameObject ("hcp");
		hcp.transform.parent = subject.transform;
		dk = new GameObject ("dk");
		dk.transform.parent = subject.transform;
		dkLeftParent = new GameObject ("dk left");
		dkLeftParent.transform.parent = dk.transform;
		dkRightParent = new GameObject ("dk right");
		dkRightParent.transform.parent = dk.transform;
		hcpLeftParent = new GameObject ("hcp left");
		hcpLeftParent.transform.parent = hcp.transform;
		hcpRightParent = new GameObject ("hcp right");
		hcpRightParent.transform.parent = hcp.transform;

		string[] filenames = (string[])RhinoRequestor.EditorObjFilePathListRequest (subjectName);
		Debug.Log (filenames.Length);
		foreach (string filename in filenames)
		{
			if (System.IO.Path.GetExtension (filename).Equals (".obj"))
			{
				if (loadingText != null)
					loadingText.text = "Downloading: " + filename;
				FileRequestToObj (subjectName, filename);
			}
			else
			{
				loadingText.text = "Error: I got a non-obj file from list objs get: " + filename;
				throw new UnityException ("I got a non-obj file from list objs get: " + filename);
			}
		}
			
		Debug.Log ("Load finished: " + (Time.time - startTime).ToString());

		//due to weirdness, flip everything
		gameObject.transform.localScale = new Vector3 (-gameObject.transform.localScale.x, gameObject.transform.localScale.y, gameObject.transform.localScale.z);

		hcp.SetActive (false);
	}

	private void BuildBrainPiece(byte[] objData, string objName)
	{
		GameObject targetObject = ObjIOLoad (objData);

		targetObject.name = objName;

		//set up the components
		GameObject importantChild = targetObject.GetComponentInChildren<MeshRenderer>().gameObject;
		//GameObject importantChild = targetObject.transform.GetChild(0).GetChild(0).GetChild(0).gameObject; //is this faster?  signs point to no
		importantChild.AddComponent<MeshCollider>();
		importantChild.AddComponent<MouseOverOutline> ().outline = importantChild.AddComponent<cakeslice.Outline> ();
		cakeslice.Outline clickOutline = importantChild.AddComponent<cakeslice.Outline> ();
		clickOutline.color = 1;
		clickOutline.enabled = false;
		importantChild.GetComponent<MouseOverOutline> ().clickOutline = clickOutline;
		importantChild.AddComponent<BrainPiece> ();
		importantChild.name = targetObject.name;

		if (objName.Contains ("hcp") && objName.Contains ("lh"))
			targetObject.transform.parent = hcpLeftParent.transform;
		else if (objName.Contains ("hcp") && objName.Contains ("rh"))
			targetObject.transform.parent = hcpRightParent.transform;
		else if (objName.Contains ("lh"))
			targetObject.transform.parent = dkLeftParent.transform;
		else if (objName.Contains ("rh"))
			targetObject.transform.parent = dkRightParent.transform;
		else
			throw new UnityException ("An obj with an unrecorgnized naming exists.");
	}

	private GameObject ObjIOLoad(byte[] fileData)
	{
		/////////// Load the .obj using OBJIO
		GameObject targetObject = new GameObject();
		System.IO.Stream lStream = new System.IO.MemoryStream(fileData);
		OBJData lOBJData = OBJLoader.LoadOBJ(lStream);
		MeshFilter filter = targetObject.AddComponent<MeshFilter>();
		filter.mesh.LoadOBJ(lOBJData);
		lStream.Close();
		lStream = null;
		lOBJData = null;
		targetObject.AddComponent<MeshRenderer> ().material = brainMaterial;
		filter.mesh.RecalculateNormals ();
		return targetObject;
		///////////
	}

	private GameObject TriLibLoad(byte[] fileData)
	{
		// Load the .obj using TriLib
		GameObject targetObject;
		using (TriLib.AssetLoader assetLoader = new TriLib.AssetLoader())
		{
			targetObject = assetLoader.LoadFromMemory(fileData, ".obj");
		}
		return targetObject;
	}

	private void FileRequestToObj(string subjectName, string fileName)
	{
		byte[] objData = (byte[])RhinoRequestor.EditorFileRequest (subjectName, fileName);
		//Debug.Log (fileName + ": " + objData.Length.ToString());
		BuildBrainPiece(objData, fileName);
	}
}