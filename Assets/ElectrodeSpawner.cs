using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectrodeSpawner : Spawner
{
	public GameObject electrodeIndicatorPrefab;

	private static Dictionary<string, GameObject> atlasParents = new Dictionary<string, GameObject> ();

	private const float NEARBY_THRESHHOLD = 5f;
	private const float TOO_CLOSE = 0.001f;
	private const float MICRO_JITTER = 1f;
	private const float MICRO_SHRINK = 0.3f;
	private const int MICRO_CLUSTER_THRESHHOLD = 15;

	public override IEnumerator Spawn(string subjectName)
	{
		CoroutineWithData getElectrodeCSVReader = new CoroutineWithData (this, GetElectrodeCSVReader(subjectName));
		yield return getElectrodeCSVReader.coroutine;
		System.IO.TextReader reader = (System.IO.TextReader)getElectrodeCSVReader.result;

		using(reader)
		{
			Dictionary<string, Electrode> electrodes = new Dictionary<string, Electrode> ();
			atlasParents = new Dictionary<string, GameObject> ();

			reader.ReadLine (); //discard the first line, which contains column names
			string line;
			while ((line = reader.ReadLine()) != null)
			{
				GameObject indicator = Instantiate (electrodeIndicatorPrefab);
				Electrode electrode = indicator.GetComponent<Electrode> ();

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

			///////orientation and micro detection////////
			List <GameObject> micros = new List<GameObject>();
			foreach (Electrode orientMe in electrodes.Values)
			{
				List<Collider> nearbies = new List<Collider>(Physics.OverlapSphere (orientMe.transform.position, NEARBY_THRESHHOLD));
				List<Collider> too_close = new List<Collider>(Physics.OverlapSphere(orientMe.transform.position, TOO_CLOSE));
				List<Collider> remove_us = new List<Collider> ();
				foreach (Collider too_close_colider in too_close)
					remove_us.Add (too_close_colider);
				foreach (Collider nearbie in nearbies)
				{
					if (!nearbie.tag.Equals ("electrode"))
						remove_us.Add (nearbie);
				}
				
				foreach (Collider too_close_collider in remove_us)
					nearbies.Remove(too_close_collider);
				
				if (electrodes.ContainsKey(orientMe.GetOrientTo()))
					orientMe.gameObject.transform.LookAt (electrodes [orientMe.GetOrientTo ()].transform);
				else
				{
					if (nearbies.Count > 0)
					{
						orientMe.gameObject.transform.LookAt (nearbies [0].transform.position);
					}
				}

				//point "S" and "G" towards the center of the brain
				if (orientMe.GetContactType ().Contains ("G") || orientMe.GetContactType ().Contains ("S"))
					orientMe.transform.LookAt (gameObject.transform);

				orientMe.transform.Rotate (new Vector3 (90, 0, 0));

				//////these are micros
				if (too_close.Count > MICRO_CLUSTER_THRESHHOLD)
				{
					orientMe.MarkMicro ();
					micros.Add (orientMe.gameObject);
				}
					
			}

			foreach (GameObject micro in micros)
			{
				micro.transform.position = micro.transform.position + new Vector3 (Random.Range (-MICRO_JITTER, MICRO_JITTER), Random.Range (-MICRO_JITTER, MICRO_JITTER), Random.Range (-MICRO_JITTER, MICRO_JITTER));
				micro.transform.localScale = micro.transform.localScale * MICRO_SHRINK;
				micro.GetComponent<Renderer> ().material.color = Color.black;
			}
		}
	}

	private IEnumerator GetElectrodeCSVReader(string subjectName)
	{
		CoroutineWithData fileRequest = new CoroutineWithData (this, ObjSpawner.FileRequest (subjectName, "electrode_coordinates.csv"));
		yield return fileRequest.coroutine;
		string csvText = System.Text.Encoding.Default.GetString((byte[])fileRequest.result);
		yield return new System.IO.StringReader(csvText);
	}

	public static Dictionary<string, GameObject> GetAtlasParentDict()
	{
		return atlasParents;
	}
}