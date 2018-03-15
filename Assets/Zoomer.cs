using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoomer : MonoBehaviour
{
	public float zoomFactor = 100;

	void Update ()
	{
		gameObject.transform.position = gameObject.transform.position + new Vector3 (0, 0, Input.GetAxis ("Mouse ScrollWheel") * zoomFactor * Time.deltaTime);
	}
}
