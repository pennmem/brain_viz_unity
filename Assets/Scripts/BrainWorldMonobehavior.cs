using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Brain world objects change the info string in the info box when you mouse over them.
/// </summary>
public abstract class BrainWorldMonobehavior : MonoBehaviour
{
    /// <summary>
    /// This returns the string to display for the brain world object.
    /// </summary>
	public abstract string InfoString();

	void OnMouseOver()
	{
		InfoText.SetText (InfoString ());
	}

}
