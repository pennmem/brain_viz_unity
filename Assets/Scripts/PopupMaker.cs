using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Instantiates a popup prefab for when brain world objecs are clicked.
/// </summary>
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
