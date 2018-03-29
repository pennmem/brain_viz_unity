using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class StimSiteSpawner : Spawner
{
	public string stimSiteCSVPath;
	public GameObject stimSiteIndicatorPrefab;

	public override void Spawn(string subjectName)
	{
		using(System.IO.TextReader reader = GetStimSiteCSVReader())
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
	

	private System.IO.TextReader GetStimSiteCSVReader()
	{
		string csvText = System.IO.File.ReadAllText (stimSiteCSVPath);
		return new System.IO.StringReader(csvText);
	}
}