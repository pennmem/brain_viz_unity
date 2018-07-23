using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Zooms the camera is and out.  And moves the brain up and down.
/// </summary>
public class Zoomer : MonoBehaviour
{
	public float zoomFactor = 100;
	public float elevationFactor = 100f;

	void Update ()
	{
		gameObject.transform.position = gameObject.transform.position + new Vector3 (0, 0, Input.GetAxis ("Mouse ScrollWheel") * zoomFactor * Time.deltaTime);

		float elevation = Input.GetAxis ("Elevation") * elevationFactor * Time.deltaTime;
		gameObject.transform.position = gameObject.transform.position + new Vector3 (0, elevation, 0);
	}
}
