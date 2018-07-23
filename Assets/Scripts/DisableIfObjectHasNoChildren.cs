using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// If one of the DC/HCP brains has no objects, disable the button to toggle that brain.
/// </summary>
public class DisableIfObjectHasNoChildren : MonoBehaviour
{
	public GameObject objectPotentiallyWithoutChildren;

	void Start ()
	{
		if (objectPotentiallyWithoutChildren.transform.childCount == 0)
			gameObject.SetActive (false);
	}
}
