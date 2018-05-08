using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubjectStarter : MonoBehaviour
{
	public UnityEngine.UI.InputField subjectNameInput;
	public Spawner[] spawners;
	public GameObject loadingMessage;
	public GameObject inputUI;
	public Colorizer colorizer;
	public MonoBehaviour[] disableWhileLoading;
	public UnityEngine.UI.Button goButton;

	public void Go()
	{
		StartCoroutine (DoLoad());
	}

	public void GoAverageBrain()
	{
		subjectNameInput.text = "fsaverage_joel";
		goButton.onClick.Invoke ();
	}

	private IEnumerator DoLoad()
	{
		foreach (MonoBehaviour monoBehavior in disableWhileLoading)
			monoBehavior.enabled = false;

		UnityEngine.UI.Text loadingtext = loadingMessage.GetComponentInChildren<UnityEngine.UI.Text> ();

		string subjectName = subjectNameInput.text;
		Debug.Log (subjectName);
		bool average_brain = subjectName.Equals("fsaverage_joel");
		if (!average_brain && (!(subjectName.Length == 6) || !subjectName [0].Equals ('R') || !subjectName [1].Equals ('1') || !char.IsUpper (subjectName [5])))
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
			spawner.gameObject.SetActive (true);
			loadingtext.text = "Spawning: " + spawner.gameObject.name;
			yield return StartCoroutine(spawner.Spawn (subjectName));
			Debug.Log (spawner.gameObject.name + " loaded: " +spawner.gameObject.transform.childCount);
			//only do the first spawner on the list for the average brain
			if (average_brain)
				break;
		}

		loadingtext.text = "<b>Mouse over something to display info</b>";
		colorizer.Initialize ();

		foreach (MonoBehaviour monoBehavior in disableWhileLoading)
			monoBehavior.enabled = true;
	}
}