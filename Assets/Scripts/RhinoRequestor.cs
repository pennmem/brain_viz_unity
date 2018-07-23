using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityExtension;

public static class RhinoRequestor
{
	private static string RHINO_ADDRESS = "http://localhost:8000";//"http://rhino2.psych.upenn.edu:8083"; //
	private static string FILE_REQUEST_ENDPOINT = "/api/v1/data/brain/vizdata/";
	private static string OBJ_LIST_ENDPOINT = "/api/v1/data/brain/list_brain_objs/";
	private static string SUBJECT_LIST_ENDPOINT = "/api/v1/data/brain/list_viz_subjects/";
	private static string ELECTRODE_ENDPOINT = "/api/v1/data/brain/electrodes/";

	private static void CheckRhinoAddress()
	{
		string currentUrl = Application.absoluteURL;
		if (currentUrl.Length == 0)
		{
			return;
		}
		string baseUrl = currentUrl.Substring (0, currentUrl.Length - 6);
		RHINO_ADDRESS = baseUrl;
	}

    //this class contains coroutines for "asynchronous" execution of each needed rhino request.
    //it also contains "editor" versions which are not coroutines (since there are no coroutines in the editor).  these are used for the asset bundle generation.

	public static IEnumerator ObjFilePathListRequest(string subjectName)
	{
		CheckRhinoAddress ();

		string url_parameters = "?subject=" + subjectName;

		var request = UnityEngine.Networking.UnityWebRequest.Get(RHINO_ADDRESS + OBJ_LIST_ENDPOINT + url_parameters);

		request.SendWebRequest ();
		Debug.Log ("Sending request to: " + request.url);
		while (!request.downloadHandler.isDone)
			yield return null;
	
		yield return request.downloadHandler.text.Split(',');	
	}

	public static IEnumerator SubjectListRequest()
	{
		CheckRhinoAddress ();

		var request = UnityEngine.Networking.UnityWebRequest.Get(RHINO_ADDRESS + SUBJECT_LIST_ENDPOINT);

		request.SendWebRequest ();
		Debug.Log ("Sending request to: " + request.url);
		while (!request.downloadHandler.isDone)
			yield return null;

		yield return request.downloadHandler.text.Split(',');	
	}

	public static IEnumerator FileRequest(string subjectName, string fileName)
	{
		CheckRhinoAddress ();

		string url_parameters = "?subject=" + subjectName + "&static_file=" + fileName;

		var request = UnityEngine.Networking.UnityWebRequest.Get(RHINO_ADDRESS + FILE_REQUEST_ENDPOINT + url_parameters);

		request.SendWebRequest ();
		//Debug.Log ("Sending request to: " + request.url);
		while (!request.downloadHandler.isDone)
			yield return null;

		yield return request.downloadHandler.data;
	}

	public static IEnumerator ElectrodeRequest(string subjectName)
	{
		CheckRhinoAddress ();

		string url_parameters = "?subject=" + subjectName;

		var request = UnityEngine.Networking.UnityWebRequest.Get(RHINO_ADDRESS + ELECTRODE_ENDPOINT + url_parameters);

		request.SendWebRequest ();
		Debug.Log ("Sending request to: " + request.url);
		while (!request.downloadHandler.isDone)
			yield return null;

		yield return request.downloadHandler.text;
	}

	public static IEnumerator AssetBundleRequest(string subjectName)
	{
		CheckRhinoAddress ();

		string url_parameters = "?subject=" + subjectName + "&static_file=assetBundle";

		var request = UnityEngine.Networking.UnityWebRequestAssetBundle.GetAssetBundle(RHINO_ADDRESS + FILE_REQUEST_ENDPOINT + url_parameters);

		request.SendWebRequest ();
		Debug.Log ("Sent request to: " + request.url);
		while (!request.downloadHandler.isDone)
			yield return null;
		yield return UnityEngine.Networking.DownloadHandlerAssetBundle.GetContent(request);
	}
		
	public static string[] EditorObjFilePathListRequest(string subjectName)
	{
		CheckRhinoAddress ();

		string url_parameters = "?subject=" + subjectName;

		var request = UnityEngine.Networking.UnityWebRequest.Get(RHINO_ADDRESS + OBJ_LIST_ENDPOINT + url_parameters);

		request.SendWebRequest ();
		Debug.Log ("Sending request to: " + request.url);
		while (!request.downloadHandler.isDone)
			;

		return request.downloadHandler.text.Split(',');	
	}

	public static string[] EditorSubjectListRequest()
	{
		CheckRhinoAddress ();

		var request = UnityEngine.Networking.UnityWebRequest.Get(RHINO_ADDRESS + SUBJECT_LIST_ENDPOINT);

		request.SendWebRequest ();
		Debug.Log ("Sending request to: " + request.url);
		while (!request.downloadHandler.isDone)
			;

		return request.downloadHandler.text.Split(',');	
	}

	public static byte[] EditorFileRequest(string subjectName, string fileName)
	{
		CheckRhinoAddress ();

		string url_parameters = "?subject=" + subjectName + "&static_file=" + fileName;

		var request = UnityEngine.Networking.UnityWebRequest.Get(RHINO_ADDRESS + FILE_REQUEST_ENDPOINT + url_parameters);

		request.SendWebRequest ();
		//Debug.Log ("Sending request to: " + request.url);
		while (!request.downloadHandler.isDone)
			;

		return request.downloadHandler.data;
	}
}