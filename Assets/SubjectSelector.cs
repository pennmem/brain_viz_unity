using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubjectSelector : MonoBehaviour
{
	public UnityEngine.UI.Image selectionIndicator;
	public UnityEngine.UI.Text textDisplay;

	private string subjectName;
	private MultisubjectSelection multisubjectSelection;
	private int index;

	public void Initialize(MultisubjectSelection newMultisubjectSelection, int newIndex, string newName)
	{
		multisubjectSelection = newMultisubjectSelection;
		index = newIndex;
		subjectName = newName;
	}

	public string GetName()
	{
		return subjectName;
	}

	public void OnClick()
	{
		selectionIndicator.enabled = !selectionIndicator.enabled;
		if (Input.GetKeyDown (KeyCode.LeftShift) || Input.GetKeyDown (KeyCode.RightShift))
			multisubjectSelection.RangeSelect (index, selectionIndicator.enabled);
		else if (Input.GetKeyDown (KeyCode.LeftControl) || Input.GetKeyDown (KeyCode.RightControl))
			multisubjectSelection.AdditiveSelect (index, selectionIndicator.enabled);
		else
			multisubjectSelection.SingleSelect (index, selectionIndicator.enabled);
	}
}
