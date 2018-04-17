using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class StimSiteSpawner : Spawner
{
	public GameObject stimSiteIndicatorPrefab;

	public override IEnumerator Spawn(string subjectName)
	{
		CoroutineWithData getStimSiteCSVReader = new CoroutineWithData (this, GetStimSiteCSVReader(subjectName));
		yield return getStimSiteCSVReader.coroutine;
		using(System.IO.StringReader reader = (System.IO.StringReader)getStimSiteCSVReader.result)
		{
			List<StimSite> stimSites = new List<StimSite> ();

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
					float.Parse (values [5]),
					float.Parse (values [6]),
					float.Parse (values [7])
				);

				stimSites.Add (stimSite);
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
}