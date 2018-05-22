using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleSpawner : Spawner
{
	public GameObject toggle;
	public float spacing = 0.1f;

	public override IEnumerator Spawn(string subjectName, bool average_brain = false)
	{
		int i = 0;
		foreach(string atlas_name in ElectrodeSpawner.GetAtlasParentDict().Keys)
		{
			GameObject child = ElectrodeSpawner.GetAtlasParentDict()[atlas_name];

			GameObject newToggle = Instantiate (toggle);
			RectTransform rectTransform = newToggle.GetComponent<RectTransform> ();
			rectTransform.anchorMin = new Vector2 (rectTransform.anchorMin.x, rectTransform.anchorMin.y - (i + 1) * spacing);
			rectTransform.anchorMax = new Vector2 (rectTransform.anchorMax.x, rectTransform.anchorMax.y - (i + 1) * spacing);
			newToggle.GetComponent<WorldObjectToggler> ().worldObject = child;
			newToggle.GetComponentInChildren<UnityEngine.UI.Text> ().text = child.name;
			newToggle.name = child.name + " toggle";
			newToggle.transform.SetParent (gameObject.transform, false);
			if (!newToggle.name.Contains("bipolar"))
				newToggle.GetComponentInChildren<UnityEngine.UI.Toggle>().isOn = false;
			i++;
		}

		yield return null;
	}
}