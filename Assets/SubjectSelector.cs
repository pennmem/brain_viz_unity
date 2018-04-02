﻿using System.Collections;
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
		UnityEngine.UI.Text loadingtext = loadingMessage.GetComponentInChildren<UnityEngine.UI.Text> ();

		string subjectName = subjectNameInput.text;
		if (!subjectName [0].Equals ('R') || !subjectName [1].Equals ('1') || !char.IsUpper (subjectName [5]) || !(subjectName.Length == 6))
		{
			subjectNameInput.text = "INVALID";
			yield break;
		}

		loadingMessage.SetActive (true);
		loadingMessage.GetComponentInChildren<UnityEngine.UI.Text> ().text = "<b>Loading . . .</b>";

		foreach (Spawner spawner in spawners)
		{
			loadingtext.text = "Despawning: " + spawner.gameObject.name;
			spawner.Despawn ();
		}

		inputUI.SetActive (false);
		yield return null;

		foreach (Spawner spawner in spawners)
		{
			loadingtext.text = "Spawning: " + spawner.gameObject.name;
			yield return StartCoroutine(spawner.Spawn (subjectName));
			Debug.Log (spawner.gameObject.name + " loaded: " +spawner.gameObject.transform.childCount);
		}

		loadingtext.text = "<b>Mouse over something to display info</b>";
	}
}