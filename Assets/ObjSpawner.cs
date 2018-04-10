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

	public GameObject popupPrefab;
	public Material brainMaterial;
	public UnityEngine.UI.Text loadingText;

	private static string RHINO_ADDRESS = "http://rhino2.psych.upenn.edu:8083"; //"http://localhost:8000";
	private static string FILE_REQUEST_ENDPOINT = "/api/v1/brain/vizdata/";
	private static string OBJ_LIST_ENDPOINT = "/api/v1/brain/list_brain_objs/";

	void Start()
	{
		string currentUrl = Application.absoluteURL;
		if (currentUrl.Length == 0)
		{
			Debug.LogWarning ("Using default RHINO_ADDRESS");
			return;
		}
		string baseUrl = currentUrl.Substring (0, currentUrl.Length - 6);
		RHINO_ADDRESS = baseUrl;
		Debug.Log ("Using rhino address: " + RHINO_ADDRESS);
	}

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

	public override IEnumerator Spawn(string subjectName)
	{
		hcp = new GameObject ("hcp");
		hcp.transform.parent = gameObject.transform;
		dk = new GameObject ("dk");
		dk.transform.parent = gameObject.transform;
		dkLeftParent = new GameObject ("dk left");
		dkLeftParent.transform.parent = dk.transform;
		dkRightParent = new GameObject ("dk right");
		dkRightParent.transform.parent = dk.transform;
		hcpLeftParent = new GameObject ("hcp left");
		hcpLeftParent.transform.parent = hcp.transform;
		hcpRightParent = new GameObject ("hcp right");
		hcpRightParent.transform.parent = hcp.transform;

		//BUILD NAME TO OBJ DICT
		CoroutineWithData objFilePathListRequest = new CoroutineWithData (this, ObjFilePathListRequest (subjectName));
		yield return objFilePathListRequest.coroutine;
		string[] filenames = (string[])objFilePathListRequest.result;
		foreach (string filename in filenames)
		{
			if (System.IO.Path.GetExtension (filename).Equals (".obj"))
			{
				loadingText.text = "Downloading: " + filename;
				yield return StartCoroutine(FileRequestToObj (subjectName, filename));
			}
			else
			{
				loadingText.text = "Error: I got a non-obj file from list objs get: " + filename;
				throw new UnityException ("I got a non-obj file from list objs get: " + filename);
			}
		}
			
		Debug.Log ("Load finished");

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
		importantChild.GetComponent<MouseOverOutline> ().popupInfoPrefab = popupPrefab;
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

	private static IEnumerator ObjFilePathListRequest(string subjectName)
	{
		string url_parameters = "?subject=" + subjectName;

		var request = UnityEngine.Networking.UnityWebRequest.Get(RHINO_ADDRESS + OBJ_LIST_ENDPOINT + url_parameters);

		request.SendWebRequest ();
		Debug.Log ("Sending request to: " + request.url);
		while (!request.downloadHandler.isDone)
			yield return null;
	
		yield return request.downloadHandler.text.Split(',');	
	}

	public static IEnumerator FileRequest(string subjectName, string fileName)
	{
		string url_parameters = "?subject=" + subjectName + "&static_file=" + fileName;

		var request = UnityEngine.Networking.UnityWebRequest.Get(RHINO_ADDRESS + FILE_REQUEST_ENDPOINT + url_parameters);

		request.SendWebRequest ();
		Debug.Log ("Sending request to: " + request.url);
		while (!request.downloadHandler.isDone)
			yield return null;

		yield return request.downloadHandler.data;
	}

	private IEnumerator FileRequestToObj(string subjectName, string fileName)
	{
		CoroutineWithData objRequest = new CoroutineWithData (this, FileRequest (subjectName, fileName));
		yield return objRequest.coroutine;
		byte[] objData = (byte[])objRequest.result;
		//Debug.Log (fileName + ": " + objData.Length.ToString());
		BuildBrainPiece(objData, fileName);
	}
}