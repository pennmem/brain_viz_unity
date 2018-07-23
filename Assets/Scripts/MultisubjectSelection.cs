using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultisubjectSelection : Spawner
{

	public ElectrodeSpawner electrodeSpawner; //the electrode spawner which will spawn all subjects
    public GameObject subjectSelector; //the UI element to spawn for each subject (i. e. little rectangle with "R1234E" in it that can change color when selected)

	private SubjectSelector[] selectors;
	private int last_clicked = 0;

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

    //called by SubjectSelectors
	public void SingleSelect(int index, bool select)
	{
		Select (index, select);
		last_clicked = index;
	}

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