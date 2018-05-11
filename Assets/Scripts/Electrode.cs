using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Electrode : BrainWorldMonobehavior
{

	private string contact_name;
	private string contact_type;
	private string atlas;
	private string orientTo;
	private bool isMicro = false;

	private bool SME_values_set = false;
	private float pvalue110 = 1f;
	private float tstat110 = 5f;
	private float pvalueHFA = 1f;
	private float tstatHFA = 5f;

	private const float POSITION_SCALING_FACTOR = 50f;

	public void Initialize(string new_contact_name, string new_contact_type, float new_x, float new_y, float new_z, string new_atlas, string new_orient_to)
	{
		contact_name = new_contact_name;
		gameObject.name = contact_name;
		contact_type = new_contact_type;
		atlas = new_atlas;
		gameObject.transform.position = new Vector3 (new_x * POSITION_SCALING_FACTOR, new_y * POSITION_SCALING_FACTOR, new_z * POSITION_SCALING_FACTOR);
		orientTo = new_orient_to;

		if (atlas.Equals ("monopolar_orig"))
			gameObject.GetComponent<Renderer> ().material.color = Color.grey;
		if (atlas.Equals ("monopolar_dykstra"))
			gameObject.GetComponent<Renderer> ().material.color = Color.black;
		if (atlas.Equals ("bipolar_dykstra"))
			gameObject.GetComponent<Renderer> ().material.color = Color.white;
	}

	public void SetSMEValues(float new_pvalue110, float new_tstat110, float new_pvalueHFA, float new_tstatHFA)
	{
		SME_values_set = true;
		pvalue110 = new_pvalue110;
		tstat110 = new_tstat110;
		pvalueHFA = new_pvalueHFA;
		tstatHFA = new_tstatHFA;
	}

	public bool GetSMEValuesSet()
	{
		return SME_values_set;
	}

	public float GetPValue110()
	{
		return pvalue110;
	}

	public float GetTStat110()
	{
		return tstat110;
	}

	public float GetPValueHFA()
	{
		return pvalueHFA;
	}

	public float GetTStatHFA()
	{
		return tstatHFA;
	}

	public string GetOrientTo()
	{
		return orientTo;
	}

	public string GetContactType()
	{
		return contact_type;
	}

	public void MarkMicro()
	{
		isMicro = true;
	}

	public override string InfoString ()
	{
		string contact_type_full = contact_type;
		switch (contact_type_full)
		{
			case "D":
				contact_type_full = "Depth";
				break;
			case "S":
				contact_type_full = "Strip";
				break;
			case "G":
				contact_type_full = "Grid";
				break;
		}
		string infoString = "<b>Contact:</b> " + contact_name + "\n" + contact_type_full + ", " + atlas + " atlas";
		if (isMicro)
		{
			infoString = infoString + ". This is a micro contact.";
		}

		if (SME_values_set)
			infoString = infoString + "\n" + "SME Stats: HFA p=" + pvalueHFA.ToString ("0.00") + ", HFA t=" +
			tstatHFA.ToString ("0.00") + ", 110Hz p=" + pvalue110.ToString ("0.00") + ", 110Hz t=" + tstat110.ToString ("0.00");
		else
			infoString = infoString + "\nSME data wasn't available for this electrode.";
		return infoString;
	}
}
