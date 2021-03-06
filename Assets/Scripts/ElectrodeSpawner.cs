﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectrodeSpawner : Spawner
{
	public GameObject electrodeIndicatorPrefab;

	private const float NEARBY_THRESHHOLD = 5f;
	private const float TOO_CLOSE = 0.001f;
	private const float MICRO_JITTER = 1f;
	private const float MICRO_SHRINK = 0.3f;
	private const int MICRO_CLUSTER_THRESHHOLD = 15;

	private Dictionary<string, List<Electrode>> subjects_to_electrodes = new Dictionary<string, List<Electrode>> ();
	private Dictionary<string, bool> subjects_enabled = new Dictionary<string, bool>();

	public List<string> GetSubjectList()
	{
		return new List<string>(subjects_to_electrodes.Keys);
	}

	public override void Despawn()
	{
		base.Despawn ();

		subjects_to_electrodes = new Dictionary<string, List<Electrode>> ();
		subjects_enabled = new Dictionary<string, bool> ();
	}

    /// <summary>
    /// This spawns the electrodes for the subject.
    /// </summary>
    /// <param name="subjectName">Subject name.</param>
    /// <param name="average_brain">If set to <c>true</c> average brain.</param>
	public override IEnumerator Spawn(string subjectName, bool average_brain = false)
	{
		subjects_to_electrodes.Add (subjectName, new List<Electrode> ());
		subjects_enabled.Add (subjectName, true);
		CoroutineWithData getElectrodeCSVReader = new CoroutineWithData (this, GetElectrodeFileReader(subjectName, "electrode_coordinates.csv"));
		yield return getElectrodeCSVReader.coroutine;
		System.IO.TextReader reader = (System.IO.TextReader)getElectrodeCSVReader.result;

		Dictionary<string, GameObject> atlasParents = new Dictionary<string, GameObject> ();
		Dictionary<string, Electrode> electrodes = new Dictionary<string, Electrode> ();

		reader.ReadLine (); //discard the first line, which contains column names
		string line;
		while ((line = reader.ReadLine()) != null)
		{
			string[] values = line.Split(',');

			string atlas = values [5];

			if (average_brain && !atlas.Equals ("avg.corrected"))
				continue;
			else if (!average_brain && !atlas.Equals ("ind.corrected"))
				continue;

			GameObject indicator = Instantiate (electrodeIndicatorPrefab);
			Electrode electrode = indicator.GetComponent<Electrode> ();

			if (!atlasParents.ContainsKey (atlas))
			{
				GameObject newAtlasParent = new GameObject ();
				newAtlasParent.transform.parent = gameObject.transform;
				newAtlasParent.name = atlas;
				atlasParents.Add(atlas, newAtlasParent);
			}

			electrode.Initialize
			(
				subjectName,
				values[0],
				values[1],
				-float.Parse(values[2]) * 0.02f, //this is weirdly flipped
				float.Parse(values[3]) * 0.02f, //and these are all in wrong units, so must be scaled
				float.Parse(values[4]) * 0.02f,
				values[5],
				StandardizeElectrodeName(atlas, subjectName, values[6])
			);

			string standardized_name = StandardizeElectrodeName (atlas, subjectName, values [0]);
			if (!electrodes.ContainsKey (standardized_name))
			{
				electrodes.Add (standardized_name, electrode);
			}
			else
			{
				Destroy (indicator);
				continue;
			}
			subjects_to_electrodes [subjectName].Add (electrode);
			indicator.transform.parent = atlasParents[atlas].transform;

			electrode.SetSMEValues
			(
				float.Parse(values [10]),
				float.Parse(values [9]),
				float.Parse(values [8]),
				float.Parse(values [7])
			);
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

	private IEnumerator GetElectrodeFileReader(string subjectName, string filename)
	{
		CoroutineWithData fileRequest = new CoroutineWithData (this, RhinoRequestor.ElectrodeRequest (subjectName));
		yield return fileRequest.coroutine;
		string csvText = (string)fileRequest.result;
		yield return new System.IO.StringReader(csvText);
	}

	public void ShowElectrodesByStat (bool pValueTrueTStatFalse, bool oneTenTrueHFAFalse, float value)
	{
		foreach (string subject in subjects_to_electrodes.Keys)
		{
			if (!subjects_enabled [subject])
				continue;
			List<Electrode> electrodes = subjects_to_electrodes [subject];
			foreach (Electrode electrode in electrodes)
			{
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
					electrode.gameObject.SetActive (valueInQuestion <= value);
				else
					electrode.gameObject.SetActive (valueInQuestion >= value);
			}
		}
	}

    /// <summary>
    /// These requests a lists of all subjects and spawns electrodes for each of them.
    /// </summary>
    /// <returns>The all subjects.</returns>
    /// <param name="average_brain">If set to <c>true</c> average brain.</param>
	public IEnumerator SpawnAllSubjects(bool average_brain)
	{
		CoroutineWithData subjectListRequest = new CoroutineWithData (this, RhinoRequestor.SubjectListRequest());
		yield return subjectListRequest.coroutine;
		string[] subjects = (string[])subjectListRequest.result;
		foreach (string subject in subjects)
		{
			Debug.Log ("Spawning electrodes of: " + subject);
			yield return Spawn (subject, average_brain: average_brain);
		}
	}

    /// <summary>
    /// Called from the UI, shows or hides a subject's electrodes.
    /// </summary>
    /// <param name="subject">Subject.</param>
    /// <param name="show">If set to <c>true</c> show.</param>
	public void ShowElectrodesBySubject(string subject, bool show)
	{
		foreach (Electrode electrode in subjects_to_electrodes [subject])
			electrode.gameObject.SetActive (show);
		subjects_enabled [subject] = show;
	}


	/// <summary>
	/// Deals with:
	/// 1. order
	/// 2. leading zeroes
	/// 3. capitalization
	/// </summary>
	/// <returns>The electrode name.</returns>
	/// <param name="electrodeName">Electrode name.</param>
	private string StandardizeElectrodeName(string atlas, string subjectName, string electrodeName)
	{
		return atlas + subjectName + StandardizeElectrodeNameOnly (electrodeName);
	}

	private string StandardizeElectrodeNameOnly (string electrodeName)
	{
		if (electrodeName.Equals (""))
			return "No electrode";

		if (electrodeName.Contains ("-"))
		{
			string[] twoparts = electrodeName.Split ('-');
			string firstpart = twoparts [0];
			string secondpart = twoparts [1];
			firstpart = StandardizeElectrodeNameOnly (firstpart);
			secondpart = StandardizeElectrodeNameOnly (secondpart);
			if (firstpart.CompareTo(secondpart) > 0)
				return firstpart + '-' + secondpart;
			else
				return secondpart + '-' + firstpart;
		}

		electrodeName = electrodeName.ToUpper ();

		if (electrodeName [electrodeName.Length - 2].Equals ("0"))
			electrodeName = electrodeName.Substring (0, electrodeName.Length - 2) + electrodeName [electrodeName.Length - 1];

		return electrodeName;
	}
}