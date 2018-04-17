using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObjectToggler : MonoBehaviour
{

	public GameObject worldObject;

	public void Toggle()
	{
		worldObject.SetActive (!worldObject.activeSelf);
	}
}
