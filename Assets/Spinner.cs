using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{

	public Vector3 spinCenter;
	public float spinFactor = 1f;

	void Update ()
	{
		float ySpin = Input.GetAxis ("Horizontal") * spinFactor * Time.deltaTime;
		float xSpin = Input.GetAxis ("Vertical") * spinFactor * Time.deltaTime;
		gameObject.transform.RotateAround (spinCenter, new Vector3 (0, 1, 0), ySpin);
		gameObject.transform.RotateAround (spinCenter, new Vector3 (1, 0, 0), xSpin);
	}
}
