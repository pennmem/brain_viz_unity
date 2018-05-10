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

	private Dictionary<string, Electrode> electrodes = new Dictionary<string, Electrode> ();
	private Dictionary<string, List<Electrode>> subjects_to_electrodes = new Dictionary<string, List<Electrode>> ();

	public override IEnumerator Spawn(string subjectName)
	{
		CoroutineWithData getElectrodeCSVReader = new CoroutineWithData (this, GetElectrodeFileReader(subjectName, "electrode_coordinates.csv"));
		yield return getElectrodeCSVReader.coroutine;
		System.IO.TextReader reader = (System.IO.TextReader)getElectrodeCSVReader.result;

		using(reader)
		{
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
					-float.Parse(values[2]), //this is weirdly flipped
					float.Parse(values[3]),
					float.Parse(values[4]),
					values[5],
					values[6]
				);

				electrodes.Add(values[0].ToUpper(), electrode);
				subjects_to_electrodes [subjectName].Add (electrode);
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

		getElectrodeCSVReader = new CoroutineWithData (this, GetElectrodeFileReader(subjectName, "target_selection_table"));
		yield return getElectrodeCSVReader.coroutine;
		reader = (System.IO.TextReader)getElectrodeCSVReader.result;

		using(reader)
		{
			reader.ReadLine (); //discard the first line, which contains column names
			string line;
			while ((line = reader.ReadLine ()) != null)
			{
				string[] values = line.Split(',');

				//foreach (string key in electrodes.Keys)
				//	Debug.Log (key);
				//Debug.Log (values [0]);
				if (electrodes.ContainsKey(values[0]))
				{
					Electrode thisElectrode = electrodes [values [0]];
					thisElectrode.SetSMEValues (float.Parse(values [12]), float.Parse(values [11]), float.Parse(values [10]), float.Parse(values [9]));
				}
				else
				{
					Debug.LogWarning("An electrode found in target_selection_table was missing: " + values[0]);
				}
			}
		}
	}

	private IEnumerator GetElectrodeFileReader(string subjectName, string filename)
	{
		CoroutineWithData fileRequest = new CoroutineWithData (this, RhinoRequestor.FileRequest (subjectName, filename));
		Debug.Log ("Electrode coordinates received.");
		yield return fileRequest.coroutine;
		string csvText = System.Text.Encoding.Default.GetString((byte[])fileRequest.result);
		yield return new System.IO.StringReader(csvText);
	}

	public static Dictionary<string, GameObject> GetAtlasParentDict()
	{
		return atlasParents;
	}

	public void ShowElectrodesByStat (bool pValueTrueTStatFalse, bool oneTenTrueHFAFalse, float value)
	{
		foreach (Electrode electrode in electrodes.Values)
		{
			if (!electrode.GetSMEValuesSet ())
				continue;

			float valueInQuestion = 0f;
			if (pValueTrueTStatFalse && oneTenTrueHFAFalse)
				valueInQuestion = electrode.GetPValue110 ();
			if (pValueTrueTStatFalse && !oneTenTrueHFAFalse)
				valueInQuestion = electrode.GetPValueHFA ();
			if (!pValueTrueTStatFalse && oneTenTrueHFAFalse)
				valueInQuestion = electrode.GetTStat110 ();
			if (!pValueTrueTStatFalse && !oneTenTrueHFAFalse)
				valueInQuestion = electrode.GetTStatHFA ();
			if (pValueTrueTStatFalse)
				electrode.gameObject.SetActive (valueInQuestion < value);
			else
				electrode.gameObject.SetActive (valueInQuestion > value);
		}
	}

	public void ShowElectrodesBySubject(string subject, bool show)
	{
		foreach (Electrode electrode in subjects_to_electrodes[subject])
			electrode.gameObject.SetActive (show);
	}
}