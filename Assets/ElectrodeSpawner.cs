using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectrodeSpawner : MonoBehaviour
{
	public string electrodeCSVPath;
	public GameObject electrodeIndicatorPrefab;

	void Awake ()
	{
		using(var reader = new System.IO.StreamReader(electrodeCSVPath))
		{
			List<Electrode> electrodes = new List<Electrode> ();
			Dictionary<string, GameObject> atlasParents = new Dictionary<string, GameObject> ();

			reader.ReadLine (); //discard the first line, which contains column names
			while (!reader.EndOfStream)
			{
				GameObject indicator = Instantiate (electrodeIndicatorPrefab);
				Electrode electrode = indicator.GetComponent<Electrode> ();

				string line = reader.ReadLine();
				string[] values = line.Split(',');

				string atlas = values [5];
				if (!atlasParents.ContainsKey (atlas))
				{

				}

				electrode.Initialize
				(
					values[0],
					values[1],
					float.Parse(values[2]),
					float.Parse(values[3]),
					float.Parse(values[4])
				);

				electrodes.Add (electrode);
				indicator.transform.parent = gameObject.transform;
			}
		}
	}

}
