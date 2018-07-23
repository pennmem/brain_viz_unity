using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the script for the button for each subject in the multisubject selection box.
/// </summary>
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
		textDisplay.text = subjectName;
	}

	public string GetName()
	{
		return subjectName;
	}

	public void OnClick()
	{
		if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift))
			multisubjectSelection.RangeSelect (index);
		else if (Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl))
			multisubjectSelection.SingleSelect (index, !selectionIndicator.enabled);
		else
			multisubjectSelection.UniqueSelect (index);
	}
}