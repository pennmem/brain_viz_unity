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
	public string pathToFolderWithObjs;

	public MonoBehaviour[] enableWhenFinished;
	public GameObject[] disableWhenFinished;

	public GameObject popupPrefab;
	public GameObject popupCanvas;

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
		string[] filePaths = System.IO.Directory.GetFiles (pathToFolderWithObjs);
		foreach (string filePath in filePaths)
		{
			if (System.IO.Path.GetExtension (filePath).Equals (".obj"))
			{
				nameToObjDict.Add (System.IO.Path.GetFileName (filePath), System.IO.File.ReadAllBytes (filePath));
			}
		}
		return nameToObjDict;
	}

	private Dictionary<string, byte[]> GetNameToObjDict()
	{

		Dictionary<string, byte[]> nameToObjDict = new Dictionary<string, byte[]> ();
		string[] filePaths = System.IO.Directory.GetFiles (pathToFolderWithObjs);
		foreach (string filePath in filePaths)
		{
			if (System.IO.Path.GetExtension (filePath).Equals (".obj"))
			{
				nameToObjDict.Add (System.IO.Path.GetFileName (filePath), System.IO.File.ReadAllBytes (filePath));
			}
		}
		return nameToObjDict;
	}

	private static string[] ObjFilePathListRequest()
	{
		string url_parameters = "?subject=337";

		var request = new UnityEngine.Networking.UnityWebRequest("http://rhino2.psych.upenn.edu:8080/api/v1/brain/objlist/" + url_parameters, "GET");
		request.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
		Debug.Log (request.url);

		UnityEngine.Networking.UnityWebRequestAsyncOperation asyncOperation = request.SendWebRequest ();
		while (!asyncOperation.isDone)
			;
		Debug.Log (request.downloadHandler.text);
		Debug.Log (request.uploadedBytes);

	}

	public static byte[] FileRequest()
	{
		// naive parameters that are actually parameters
		//		WWWForm requestForm = new WWWForm ();
		//		requestForm.AddField ("subject", "337");
		//		requestForm.AddField ("static_file", "VOX_coords_mother_dykstra.txt");
		//		Dictionary<string, string> postParameters = new Dictionary<string, string>();
		//		postParameters.Add ("subject", "337");
		//		postParameters.Add ("static_file", "VOX_coords_mother_dykstra.txt");
		//		string json_body = "{ \"subject\":\"337\", \"static_file\":\"VOX_coords_mother_dykstra.txt\" }";
		//      string body = "subject=337&static_file=VOX_coords_mother_dykstra.txt";

		string url_parameters = "?subject=337&static_file=VOX_coords_mother_dykstra.txt";

		//var request = UnityEngine.Networking.UnityWebRequest.Put ("http://rhino2.psych.upenn.edu:8080/api/v1/brain/data/", body);
		var request = new UnityEngine.Networking.UnityWebRequest("http://rhino2.psych.upenn.edu:8080/api/v1/brain/vizdata/" + url_parameters, "GET");
		//request.uploadHandler = new UnityEngine.Networking.UploadHandlerRaw (System.Text.Encoding.UTF8.GetBytes (body));
		request.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
		//request.SetRequestHeader ("Content-Type", "application/x-www-form-urlencoded");

		//request.uploadHandler.data = System.Text.Encoding.UTF8.GetBytes (body);
		Debug.Log (request.url);
		//Debug.Log (System.Text.Encoding.UTF8.GetString(request.uploadHandler.data));

		UnityEngine.Networking.UnityWebRequestAsyncOperation asyncOperation = request.SendWebRequest ();
		while (!asyncOperation.isDone)
			;
		Debug.Log (request.downloadHandler.text);
		Debug.Log (request.uploadedBytes);

	}

//  old temporary load from disk version
//	private Dictionary<string, byte[]> GetNameToObjDict()
//	{
//		Dictionary<string, byte[]> nameToObjDict = new Dictionary<string, byte[]> ();
//		string[] filePaths = System.IO.Directory.GetFiles (pathToFolderWithObjs);
//		foreach (string filePath in filePaths)
//		{
//			if (System.IO.Path.GetExtension (filePath).Equals (".obj"))
//			{
//				nameToObjDict.Add (System.IO.Path.GetFileName (filePath), System.IO.File.ReadAllBytes (filePath));
//			}
//		}
//		return nameToObjDict;
//	}
}