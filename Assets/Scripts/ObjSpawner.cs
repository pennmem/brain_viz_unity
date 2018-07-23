using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityExtension;

/// <summary>
/// the spawner for pieces of the brain
/// </summary>
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

    /// <summary>
    /// Spawns the brain for one subject- this may be "fsaverage_joel", the average brain.  This will try to use assetBundles and call ObjSpawn if it can't.
    /// </summary>
    /// <returns>The spawn.</returns>
    /// <param name="subject">Subject.</param>
    /// <param name="average_brain">If set to <c>true</c> average brain.</param>
	public override IEnumerator Spawn(string subject, bool average_brain = false)
	{
		CoroutineWithData subjectListRequest = new CoroutineWithData (this, RhinoRequestor.AssetBundleRequest(subject));
		yield return subjectListRequest.coroutine;
		Debug.Log (subjectListRequest.result);
		if (subjectListRequest.result == null)
		{
			loadingText.text = "AssetBundle not found. Performing alternative (slower) load. Please wait.";
			ObjSpawn (subject, average_brain);
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

		//due to weirdness, flip everything
		//gameObject.transform.localScale = new Vector3 (-gameObject.transform.localScale.x, gameObject.transform.localScale.y, gameObject.transform.localScale.z);

		hcp.SetActive (false);
	}

	public override void Despawn()
	{
		base.Despawn ();

		if (currentBundle != null)
			currentBundle.Unload (true);
	}

    /// <summary>
    /// this performs the load from Objs instead of an .assetBundle
    /// it is used in case no .assetBundle exists and by CreateAssetBundles to create the asset bundles in the first place.
    /// </summary>
    /// <param name="subjectName">Subject name.</param>
    /// <param name="average_brain">If set to <c>true</c> average brain.</param>
	public void ObjSpawn(string subjectName, bool average_brain = false)
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

        //CoroutineWithData objListRequest = new CoroutineWithData (this, RhinoRequestor.ObjFilePathListRequest (subjectName));
        //yield return objListRequest.coroutine;
        string[] filenames = RhinoRequestor.EditorObjFilePathListRequest(subjectName);//(string[])objListRequest.result;
		//Debug.Log (filenames.Length);
		foreach (string filename in filenames)
		{
			if (System.IO.Path.GetExtension (filename).Equals (".obj"))
			{
				//if (loadingText != null)
				//	loadingText.text = "Downloading: " + filename;
                EditorFileRequestToObj (subjectName, filename);
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

	private IEnumerator FileRequestToObj(string subjectName, string fileName)
	{
		CoroutineWithData objRequest = new CoroutineWithData (this, RhinoRequestor.FileRequest (subjectName, fileName));
		yield return objRequest.coroutine;
		byte[] objData = (byte[])objRequest.result;
		BuildBrainPiece(objData, fileName);
	}

    private void EditorFileRequestToObj(string subjectName, string fileName)
    {
        byte[] objData = RhinoRequestor.EditorFileRequest(subjectName, fileName);
        BuildBrainPiece(objData, fileName);
    }
}