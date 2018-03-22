using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BrainWorldMonobehavior : MonoBehaviour
{
	protected abstract string InfoString();

	void OnMouseOver()
	{
		InfoText.SetText (InfoString ());
	}

}
