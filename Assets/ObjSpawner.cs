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
	public string subjectName;

	public MonoBehaviour[] enableWhenFinished;
	public GameObject[] disableWhenFinished;

	public GameObject popupPrefab;
	public GameObject popupCanvas;

	private static string RHINO_ADDRESS = "localhost:8000"; //"http://rhino2.psych.upenn.edu:8080"
	private static string FILE_REQUEST_ENDPOINT = "/api/v1/brain/vizdata/";
	private static string OBJ_LIST_ENDPOINT = "/api/v1/brain/list_brain_objs/";

	void Awake ()
	{
		if (!this.enabled)
			return;

		Dictionary<string, byte[]> nameToObjDict = GetNameToObjDict();
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

		foreach (MonoBehaviour monoBehavior in enableWhenFinished)
			monoBehavior.enabled = true;
		foreach (GameObject disableMe in disableWhenFinished)
			disableMe.SetActive (false);
	}

	private Dictionary<string, byte[]> GetNameToObjDict()
	{
		Dictionary<string, byte[]> nameToObjDict = new Dictionary<string, byte[]> ();
		string[] filenames = ObjFilePathListRequest(subjectName);
		foreach (string filename in filenames)
		{
			if (System.IO.Path.GetExtension (filename).Equals (".obj"))
			{
				nameToObjDict.Add (filename, FileRequest (subjectName, filename));
			}
			else
			{
				throw new UnityException ("I got a non-obj file from list objs get: " + filename);
			}
		}
		return nameToObjDict;
	}

	private static string[] ObjFilePathListRequest(string subjectName)
	{
		string url_parameters = "?subject=" + subjectName;

		var request = UnityEngine.Networking.UnityWebRequest.Get(RHINO_ADDRESS + OBJ_LIST_ENDPOINT + url_parameters);
		Debug.Log (request.url);

		UnityEngine.Networking.UnityWebRequestAsyncOperation asyncOperation = request.SendWebRequest ();
		while (!asyncOperation.isDone)
			;
		return request.downloadHandler.text.Split(',');

	}

	public static byte[] FileRequest(string subjectName, string fileName)
	{
		string url_parameters = "?subject=" + subjectName + "&static_file=" + fileName;

		var request = UnityEngine.Networking.UnityWebRequest.Get(RHINO_ADDRESS + FILE_REQUEST_ENDPOINT + url_parameters);
		Debug.Log (request.url);

		UnityEngine.Networking.UnityWebRequestAsyncOperation asyncOperation = request.SendWebRequest ();
		while (!asyncOperation.isDone)
			;
		
		return request.downloadHandler.data;
	}
}