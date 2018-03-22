using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectrodeSpawner : MonoBehaviour
{
	public string electrodeCSVPath;
	public GameObject electrodeIndicatorPrefab;

	private static Dictionary<string, GameObject> atlasParents = new Dictionary<string, GameObject> ();

	private const float NEARBY_THRESHHOLD = 5f;

	void Awake ()
	{
		using(var reader = new System.IO.StreamReader(electrodeCSVPath))
		{
			Dictionary<string, Electrode> electrodes = new Dictionary<string, Electrode> ();

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
					GameObject newAtlasParent = new GameObject ();
					newAtlasParent.transform.parent = gameObject.transform;
					newAtlasParent.name = atlas;
					atlasParents.Add(atlas, newAtlasParent);
				}

				electrode.Initialize
				(
					values[0],
					values[1],
					float.Parse(values[2]),
					float.Parse(values[3]),
					float.Parse(values[4]),
					values[5],
					values[6]
				);

				electrodes.Add(values[0], electrode);
				indicator.transform.parent = atlasParents[atlas].transform;
			}

			foreach (Electrode orientMe in electrodes.Values)
			{
				if (electrodes.ContainsKey(orientMe.GetOrientTo()))
					orientMe.gameObject.transform.LookAt (electrodes [orientMe.GetOrientTo ()].transform);
				else
				{
					Collider[] nearbies = Physics.OverlapSphere (orientMe.transform.position, NEARBY_THRESHHOLD);
					if (nearbies.Length > 0)
					{
						orientMe.gameObject.transform.LookAt (nearbies [0].transform.position);
					}
				}
				orientMe.transform.Rotate (new Vector3 (90, 0, 0));
			}
		}
	}

	public static Dictionary<string, GameObject> GetAtlasParentDict()
	{
		return atlasParents;
	}
}
