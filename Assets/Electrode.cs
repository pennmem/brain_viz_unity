using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Electrode : MonoBehaviour
{

	private string contact_name;
	private string contact_type;
	private string orientTo;

	public void Initialize(string new_contact_name, string new_contact_type, float new_x, float new_y, float new_z, string new_orient_to)
	{
		contact_name = new_contact_name;
		gameObject.name = contact_name;
		contact_type = new_contact_type;
		gameObject.transform.position = new Vector3 (new_x, new_y, new_z);
		orientTo = new_orient_to;
	}

	public string GetOrientTo()
	{
		return orientTo;
	}
}
