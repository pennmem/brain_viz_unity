using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used by UI.
/// </summary>
public class WorldObjectToggler : MonoBehaviour
{
	public GameObject worldObject;

	public void Toggle()
	{
		worldObject.SetActive (!worldObject.activeSelf);
	}
}
