using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOverOutline : MonoBehaviour
{
	private cakeslice.Outline outline;

	void Start()
	{
		outline = gameObject.GetComponent<cakeslice.Outline> ();
		outline.enabled = false;
	}

	void OnMouseEnter()
	{
		outline.enabled = true;
	}

	void OnMouseExit()
	{
		outline.enabled = false;
	}

}
