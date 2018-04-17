using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{

	public Vector3 spinCenter;
	public float keySpinFactor = 30f;
	public float mouseSpinFactor = 30f;

	private Vector2 lastMousePos = Vector2.zero;

	void Update ()
	{
		float ySpin = Input.GetAxis ("Horizontal") * keySpinFactor * Time.deltaTime;
		float xSpin = Input.GetAxis ("Vertical") * keySpinFactor * Time.deltaTime;
		float zSpin = Input.GetAxis ("Roll") * keySpinFactor * Time.deltaTime;
		gameObject.transform.RotateAround (spinCenter, new Vector3 (0, 1, 0), ySpin);
		gameObject.transform.RotateAround (spinCenter, new Vector3 (1, 0, 0), xSpin);
		gameObject.transform.RotateAround (spinCenter, new Vector3 (0, 0, 1), zSpin);

		Vector2 mousePos = Input.mousePosition;
		if (Input.GetMouseButtonDown (0))
			lastMousePos = mousePos;
		if (Input.GetMouseButton (0))
		{
			if (mousePos.x < Screen.width * Camera.main.rect.width)
			{
				float Xchange = mousePos.x - lastMousePos.x;
				float Ychange = mousePos.y - lastMousePos.y;

				gameObject.transform.RotateAround (spinCenter, new Vector3 (0, 1, 0), Xchange * mouseSpinFactor * Time.deltaTime);
				gameObject.transform.RotateAround (spinCenter, new Vector3 (1, 0, 0), Ychange * mouseSpinFactor * Time.deltaTime);

				lastMousePos = mousePos;
			}
		}


	}
}
