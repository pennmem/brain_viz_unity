using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubjectSelector : MonoBehaviour
{
	public UnityEngine.UI.InputField subjectNameInput;
	public Spawner[] spawners;
	public GameObject loadingMessage;
	public GameObject inputUI;

	public void Go()
	{
		StartCoroutine (DoLoad ());
	}

	private IEnumerator DoLoad()
	{
		loadingMessage.SetActive (true);
		loadingMessage.GetComponentInChildren<UnityEngine.UI.Text> ().text = "<b>Loading . . .</b>";

		string subjectName = subjectNameInput.text;
		foreach (Spawner spawner in spawners)
			spawner.Despawn ();

		inputUI.SetActive (false);
		yield return null;

		foreach (Spawner spawner in spawners)
		{
			yield return StartCoroutine(spawner.Spawn (subjectName));
			Debug.Log (spawner.gameObject.name + " loaded: " +spawner.gameObject.transform.childCount);
		}

		loadingMessage.GetComponentInChildren<UnityEngine.UI.Text> ().text = "<b>Mouse over something to display info</b>";
	}
}