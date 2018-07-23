using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Makes the viewport not take up the right side of the screen when the options panel is there.
/// </summary>
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
