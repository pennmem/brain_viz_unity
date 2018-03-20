﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StimSite : MonoBehaviour
{
	private string subject_id;
	private string contact_name;
	private string experiment;
	private float deltarec;
	private bool enhancement;

	public float negativeStimBlueThreshhold = -60f;
	public float positiveStimRedThreshhold = 60f;

	public void Initialize
	(
		string new_subject_id,
		string new_contact_name,
		string new_experiment,
		float new_deltarec,
		bool new_enhancement,
		float new_fs_x,
		float new_fs_y,
		float new_fs_z
	)
	{
		subject_id = new_subject_id;
		contact_name = new_contact_name;
		experiment = new_experiment;
		deltarec = new_deltarec;
		enhancement = new_enhancement;
		gameObject.transform.position = new Vector3 (new_fs_x, new_fs_y, new_fs_z);

		gameObject.GetComponent<Renderer> ().material.color = new Color
		(
			1 - Mathf.Clamp (deltarec, negativeStimBlueThreshhold, 0) / negativeStimBlueThreshhold,
			1 - Mathf.Max(Mathf.Clamp (deltarec, 0, positiveStimRedThreshhold) / positiveStimRedThreshhold, Mathf.Clamp (deltarec, negativeStimBlueThreshhold, 0) / negativeStimBlueThreshhold),
			1 - Mathf.Clamp (deltarec, 0, positiveStimRedThreshhold) / positiveStimRedThreshhold
		);
	}

}
