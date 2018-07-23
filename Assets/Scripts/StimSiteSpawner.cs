using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The spawner for stim site spheres.
/// </summary>
public class StimSiteSpawner : Spawner
{
	public GameObject stimSiteIndicatorPrefab;

	private List<StimSite> stimSites = new List<StimSite> ();

	private const float MAGNITUDE_CUTOFF = 100f;

    /// <summary>
    /// Uses a rhino request to get the file that contains prior stim site information.
    /// </summary>
    /// <returns>The spawn.</returns>
    /// <param name="subjectName">Subject name.</param>
    /// <param name="average_brain">If set to <c>true</c> average brain.</param>
	public override IEnumerator Spawn(string subjectName, bool average_brain = false)
	{
		CoroutineWithData getStimSiteCSVReader = new CoroutineWithData (this, GetStimSiteCSVReader(subjectName));
		yield return getStimSiteCSVReader.coroutine;
		using(System.IO.StringReader reader = (System.IO.StringReader)getStimSiteCSVReader.result)
		{
			stimSites = new List<StimSite> ();

			reader.ReadLine (); //discard the first line, which contains column names
			string line;
			while ((line = reader.ReadLine()) != null)
			{
				GameObject indicator = Instantiate (stimSiteIndicatorPrefab);
				StimSite stimSite = indicator.GetComponent<StimSite> ();

				string[] values = line.Split(',');

				stimSite.Initialize
				(
					values [0],
					values [1],
					values [2],
					float.Parse (values [3]),
					bool.Parse (values [4]),
					-float.Parse (values [5]), //weird flip
					float.Parse (values [6]),
					float.Parse (values [7])
				);

				if (stimSite.transform.position.sqrMagnitude > MAGNITUDE_CUTOFF * MAGNITUDE_CUTOFF)
				{
					Destroy (stimSite.gameObject);
				}
				else
				{
					stimSites.Add (stimSite);
				}
				indicator.transform.parent = gameObject.transform;
			}
		}
	}
	

	private IEnumerator GetStimSiteCSVReader(string subjectName)
	{
		CoroutineWithData fileRequest = new CoroutineWithData (this, RhinoRequestor.FileRequest (subjectName, subjectName + "_allcords.csv"));
		yield return fileRequest.coroutine;
		string csvText = System.Text.Encoding.Default.GetString((byte[])fileRequest.result);
		yield return new System.IO.StringReader(csvText);
	}

    /// <summary>
    /// Called by the slider to show and hide stim sites.
    /// </summary>
    /// <param name="deltaRecallMinimum">Delta recall minimum.</param>
	public void ShowStimSitesByDeltaRecall(float deltaRecallMinimum)
	{
		foreach (StimSite stimSite in stimSites)
		{
			stimSite.gameObject.SetActive (Mathf.Abs(stimSite.GetDeltaRecall ()) > deltaRecallMinimum);
		}
	}
}