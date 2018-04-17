using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
