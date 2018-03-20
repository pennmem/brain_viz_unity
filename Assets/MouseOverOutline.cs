using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOverOutline : MonoBehaviour
{
	public cakeslice.Outline outline;

	void Start()
	{
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
