using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class StimSiteSpawner : MonoBehaviour
{
	public string stimSiteCSVPath;
	public GameObject stimSiteIndicatorPrefab;

	void Awake ()
	{
		using(var reader = new System.IO.StreamReader(stimSiteCSVPath))
		{
			List<StimSite> stimSites = new List<StimSite> ();

			reader.ReadLine (); //discard the first line, which contains column names
			while (!reader.EndOfStream)
			{
				GameObject indicator = Instantiate (stimSiteIndicatorPrefab);
				StimSite stimSite = indicator.GetComponent<StimSite> ();

				string line = reader.ReadLine();
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
	

	void Update ()
	{
		
	}
}
