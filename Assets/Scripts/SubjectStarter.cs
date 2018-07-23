using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubjectStarter : MonoBehaviour
{
	public UnityEngine.UI.InputField subjectNameInput;
	public Spawner[] spawners; //these will be spawned when regular subject are loaded
	public Spawner[] averageBrainSpawners; //these will be spawned when average brain subject is loaded
	public GameObject loadingMessage;
	public GameObject inputUI;
	public Colorizer colorizer;
	public MonoBehaviour[] disableWhileLoading;
	public UnityEngine.UI.Button goButton;

    /// <summary>
    /// called from the go button
    /// </summary>
	public void Go()
	{
		StartCoroutine (DoLoad());
	}

    /// <summary>
    /// called from the use average brain button.  it simply sets the subject name to the average brain name and then clicks go
    /// </summary>
	public void GoAverageBrain()
	{
		subjectNameInput.text = "fsaverage_joel";
		goButton.onClick.Invoke ();
	}

    /// <summary>
    /// this loads a subject.  it will do some things differently if it's loading fsaverage_joel (the average brain)
    /// </summary>
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

		List<Spawner> all_spawners = new List<Spawner> ();
		all_spawners.AddRange (spawners);
		all_spawners.AddRange (averageBrainSpawners);
		foreach (Spawner spawner in all_spawners)
		{
			loadingtext.text = "Despawning: " + spawner.gameObject.name;
			spawner.Despawn ();
		}

		inputUI.SetActive (false);
		yield return null;

		if (average_brain)
		{
            //spawn all spawners for the average brain
			foreach (Spawner spawner in averageBrainSpawners)
			{
				spawner.gameObject.SetActive (true);
				loadingtext.text = "Spawning: " + spawner.gameObject.name;
				yield return StartCoroutine (spawner.Spawn (subjectName, average_brain: true));
				Debug.Log (spawner.gameObject.name + " loaded: " + spawner.gameObject.transform.childCount);
			}
		}
		else
		{
            //spawn all spawners for regular subjects
			foreach (Spawner spawner in spawners)
			{
				spawner.gameObject.SetActive (true);
				loadingtext.text = "Spawning: " + spawner.gameObject.name;
				yield return StartCoroutine (spawner.Spawn (subjectName));
				Debug.Log (spawner.gameObject.name + " loaded: " + spawner.gameObject.transform.childCount);
			}
		}

		loadingtext.text = "<b>Mouse over something to display info</b>";
		colorizer.Initialize ();

		foreach (MonoBehaviour monoBehavior in disableWhileLoading)
			monoBehavior.enabled = true;
	}
}