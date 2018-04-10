﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewportToggler : MonoBehaviour
{
	float toggledValue = 0.8f;

	public void ToggleViewport(bool toggled)
	{
		if (toggled)
			GetComponent<Camera> ().rect = new Rect (0, 0, toggledValue, 1);
		else
			GetComponent<Camera> ().rect = new Rect (0, 0, 1, 1);
	}

}
