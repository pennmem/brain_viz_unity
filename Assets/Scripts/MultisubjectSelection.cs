using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultisubjectSelection : Spawner
{

	public ElectrodeSpawner electrodeSpawner; //the electrode spawner which will spawn all subjects
    public GameObject subjectSelector; //the UI element to spawn for each subject (i. e. little rectangle with "R1234E" in it that can change color when selected)

	private SubjectSelector[] selectors;
	private int last_clicked = 0;

    /// <summary>
    /// Performs the electrode spawning for each subject by calling ElectrodeSpawner.SpawnAllSubjects, and creates a clickable rectangle field for flipping electrodes on and off in the multisubject selection box.
    /// </summary>
    /// <param name="subjectName">Subject name.</param>
    /// <param name="average_brain">If set to <c>true</c> average brain.</param>
	public override IEnumerator Spawn(string subjectName, bool average_brain = false)
	{
		yield return electrodeSpawner.SpawnAllSubjects (average_brain);
		List<string> subjects = electrodeSpawner.GetSubjectList ();
		selectors = new SubjectSelector[subjects.Count];
		Debug.Log (subjects.Count);
		for (int i = 0; i < selectors.Length; i++)
		{
			string subject = subjects[i];
			GameObject selectorObject = Instantiate (subjectSelector);
			SubjectSelector selector = selectorObject.GetComponent<SubjectSelector> ();
			selector.Initialize (this, i, subject);
			selector.transform.SetParent(this.transform, false);
			selector.transform.position = selector.transform.position - new Vector3 (0, 25, 0) * i;
			selectors [i] = selector;
		}
	}

	private void Select(int index, bool select)
	{
		selectors[index].selectionIndicator.enabled = select;
		electrodeSpawner.ShowElectrodesBySubject (selectors[index].GetName(), select);
	}

    /// <summary>
    /// called by SubjectSelectors (on a regular click)
    /// </summary>
    /// <param name="index">index of subject box</param>
    /// <param name="select">whether to select or deselect</param>
	public void SingleSelect(int index, bool select)
	{
		Select (index, select);
		last_clicked = index;
	}

    /// <summary>
    /// flip only one subject leaving the rest teh same (When "alt" is held some)
    /// </summary>
    /// <param name="index">Index.</param>
	public void UniqueSelect(int index)
	{
		for (int i = 0; i < selectors.Length; i++)
		{
			if (i == index)
			{
				Select (i, true);
			}
			else
				Select (i, false);
		}
		last_clicked = index;
	}

    /// <summary>
    /// select a range of subject (when "shift" is held down)
    /// </summary>
    /// <param name="index">Index.</param>
	public void RangeSelect(int index)
	{
		if (index >= last_clicked)
		{
			for (int i = last_clicked; i <= index; i++)
				Select (i, true);
		}
		else
		{
			for (int i = last_clicked; i >= index; i--)
				Select (i, true);
		}
		last_clicked = index;
	}
}