using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
		gameObject.transform.localScale = new Vector3 (-1, 1, 1);
	}
}
