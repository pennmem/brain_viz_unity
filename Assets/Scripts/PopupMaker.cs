using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupMaker : MonoBehaviour
{
	public GameObject popupInfoPrefab;

	public GameObject MakePopup()
	{
		GameObject popup = Instantiate (popupInfoPrefab);
		popup.transform.parent = gameObject.transform;
		return popup;
	}

}
