using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableIfObjectHasNoChildren : MonoBehaviour
{
	public GameObject objectPotentiallyWithoutChildren;

	void Start ()
	{
		if (objectPotentiallyWithoutChildren.transform.childCount == 0)
			gameObject.SetActive (false);
	}
}
