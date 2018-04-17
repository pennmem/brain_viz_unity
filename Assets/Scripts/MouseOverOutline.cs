using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOverOutline : MonoBehaviour
{
	public cakeslice.Outline outline;
	public cakeslice.Outline clickOutline;
	public GameObject popupInfoPrefab;

	void Start()
	{
		outline.enabled = false;
	}

	void OnMouseEnter()
	{
		outline.enabled = true;
	}

	void OnMouseExit()
	{
		outline.enabled = false;
	}

	void OnMouseOver()
	{
		if (Input.GetMouseButtonDown (0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
		{
			clickOutline.enabled = true;
			SpawnInfoWindow ();
		}
	}

	private void SpawnInfoWindow()
	{
		GameObject infoWindowCanvas = Instantiate (popupInfoPrefab);
		GameObject infoWindow = infoWindowCanvas.transform.GetChild(0).gameObject;
		infoWindow.GetComponentInChildren<UnityEngine.UI.Text> ().text = gameObject.GetComponent<BrainWorldMonobehavior> ().InfoString ();
		infoWindow.GetComponentInChildren<PopupWindow> ().disableMe = clickOutline;
		infoWindow.GetComponent<RectTransform> ().anchorMin = new Vector2 (Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
		infoWindow.GetComponent<RectTransform> ().anchorMax = infoWindow.GetComponent<RectTransform> ().anchorMin + infoWindow.GetComponent<RectTransform> ().anchorMax;
		if (infoWindow.GetComponent<RectTransform> ().anchorMax.x > 1)
		{
			float offset = infoWindow.GetComponent<RectTransform> ().anchorMax.x - 1;
			infoWindow.GetComponent<RectTransform> ().anchorMax -= new Vector2 (offset, 0);
			infoWindow.GetComponent<RectTransform> ().anchorMin -= new Vector2 (offset, 0);
		}
		if (infoWindow.GetComponent<RectTransform> ().anchorMax.y > 1)
		{
			float offset = infoWindow.GetComponent<RectTransform> ().anchorMax.y - 1;
			infoWindow.GetComponent<RectTransform> ().anchorMax -= new Vector2 (0, offset);
			infoWindow.GetComponent<RectTransform> ().anchorMin -= new Vector2 (0, offset);
		}
		infoWindow.GetComponent<RectTransform> ().offsetMin = infoWindow.GetComponent<RectTransform> ().offsetMax = Vector2.zero;
	}
}