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
			//GameObject importantChild = targetObject.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
			importantChild.AddComponent<MeshCollider>();
			importantChild.AddComponent<MouseOverOutline> ().outline = importantChild.AddComponent<cakeslice.Outline> ();
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
}
