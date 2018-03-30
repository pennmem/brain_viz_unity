using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjSpawner : Spawner
{
	private GameObject dk;
	private GameObject hcp;
	private GameObject dkLeftParent;
	private GameObject dkRightParent;
	private GameObject hcpLeftParent;
	private GameObject hcpRightParent;

	public MonoBehaviour[] enableWhenFinished;
	public GameObject[] disableWhenFinished;

	public GameObject popupPrefab;
	public GameObject popupCanvas;

	private static string RHINO_ADDRESS = "http://localhost:8000"; //"http://rhino2.psych.upenn.edu:8080"
	private static string FILE_REQUEST_ENDPOINT = "/api/v1/brain/vizdata/";
	private static string OBJ_LIST_ENDPOINT = "/api/v1/brain/list_brain_objs/";

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
		Dictionary<string, byte[]> nameToObjDict = new Dictionary<string, byte[]> ();
		CoroutineWithData objFilePathListRequest = new CoroutineWithData (this, ObjFilePathListRequest (subjectName));
		yield return objFilePathListRequest.coroutine;
		string[] filenames = (string[])objFilePathListRequest.result;
		foreach (string filename in filenames)
		{
			if (System.IO.Path.GetExtension (filename).Equals (".obj"))
			{
				CoroutineWithData objRequest = new CoroutineWithData (this, FileRequest (subjectName, filename));
				yield return objRequest.coroutine;
				nameToObjDict.Add (filename, (byte[])objRequest.result);
			}
			else
			{
				throw new UnityException ("I got a non-obj file from list objs get: " + filename);
			}
		}

		List<GameObject> loadedObjs = new List<GameObject> ();

		foreach (string objName in nameToObjDict.Keys)
		{
			if (objName.Contains("pial"))
				continue;

			// Load the .obj using TriLib
			GameObject targetObject;
			using (TriLib.AssetLoader assetLoader = new TriLib.AssetLoader())
			{
				byte[] fileData = nameToObjDict[objName];
				targetObject = assetLoader.LoadFromMemory(fileData, ".obj");
			}

			targetObject.name = objName;
			loadedObjs.Add (targetObject);

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
		Debug.Log ("Load finished");

		hcp.SetActive (false);
		foreach (MonoBehaviour monoBehavior in enableWhenFinished)
			monoBehavior.enabled = true;
		foreach (GameObject disableMe in disableWhenFinished)
			disableMe.SetActive (false);
	}

	private static IEnumerator ObjFilePathListRequest(string subjectName)
	{
		string url_parameters = "?subject=" + subjectName;

		var request = UnityEngine.Networking.UnityWebRequest.Get(RHINO_ADDRESS + OBJ_LIST_ENDPOINT + url_parameters);

		request.SendWebRequest ();
		Debug.Log ("Sending request to: " + request.url);
		while (!request.isDone)
			yield return null;

		yield return request.downloadHandler.text.Split(',');

	}

	public static IEnumerator FileRequest(string subjectName, string fileName)
	{
		string url_parameters = "?subject=" + subjectName + "&static_file=" + fileName;

		var request = UnityEngine.Networking.UnityWebRequest.Get(RHINO_ADDRESS + FILE_REQUEST_ENDPOINT + url_parameters);

		request.SendWebRequest ();
		Debug.Log ("Sending request to: " + request.url);
		while (!request.isDone)
			yield return null;

		yield return request.downloadHandler.data;
	}
}