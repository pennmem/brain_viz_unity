using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleSpawner : Spawner
{
	public GameObject oneToggleForEachChildOf;
	public GameObject toggle;
	public float spacing = 0.1f;

	public override void Spawn(string subjectName)
	{
		for (int i = 0; i < oneToggleForEachChildOf.transform.childCount; i++)
		{
			GameObject child = oneToggleForEachChildOf.transform.GetChild (i).gameObject;

			GameObject newToggle = Instantiate (toggle);
			RectTransform rectTransform = newToggle.GetComponent<RectTransform> ();
			rectTransform.anchorMin = new Vector2 (rectTransform.anchorMin.x, rectTransform.anchorMin.y - (i + 1) * spacing);
			rectTransform.anchorMax = new Vector2 (rectTransform.anchorMax.x, rectTransform.anchorMax.y - (i + 1) * spacing);
			newToggle.GetComponent<WorldObjectToggler> ().worldObject = child;
			newToggle.GetComponentInChildren<UnityEngine.UI.Text> ().text = child.name;
			newToggle.name = child.name + " toggle";
			newToggle.transform.SetParent (gameObject.transform, false);
		}
	}

}
