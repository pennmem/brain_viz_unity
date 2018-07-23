using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The script for the window that pops up when you click a brain world object.
/// </summary>
public class PopupWindow : MonoBehaviour
{
	public GameObject destroyMe;
	public MonoBehaviour disableMe;

	public void OnClose()
	{
		Destroy (destroyMe);
		disableMe.enabled = false;
	}
}
