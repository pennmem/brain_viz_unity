using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StimSite : BrainWorldMonobehavior
{
	private string subject_id;
	private string contact_name;
	private string experiment;
	private float deltarec;
	private bool enhancement;

	public float negativeStimBlueThreshhold = -50f;
	public float positiveStimRedThreshhold = 50f;

	private const float MAGNITUDE_CUTOFF = 100f;

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

		gameObject.name = subject_id + " " + experiment;

		if (gameObject.transform.position.sqrMagnitude > MAGNITUDE_CUTOFF * MAGNITUDE_CUTOFF)
		{
			gameObject.SetActive (false);
		}
	}

	public override string InfoString()
	{
		string enhancementWord = "enhancement";
		if (!enhancement)
			enhancementWord = "impairment";
		return "<b>Prior Stim Site:</b> " + deltarec.ToString ("###.##") + "% " + enhancementWord + "\n" + subject_id + " " + contact_name + " " + experiment;
	}
}
