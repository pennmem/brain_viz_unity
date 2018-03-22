using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Electrode : MonoBehaviour
{

	private string contact_name;
	private string contact_type;
	private string atlas;
	private string orientTo;

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
			gameObject.GetComponent<Renderer> ().material.color = Color.green;
		if (atlas.Equals ("monopolar_dykstra"))
			gameObject.GetComponent<Renderer> ().material.color = Color.yellow;
		if (atlas.Equals ("bipolar_dykstra"))
			gameObject.GetComponent<Renderer> ().material.color = Color.magenta;
	}

	public string GetOrientTo()
	{
		return orientTo;
	}
}
