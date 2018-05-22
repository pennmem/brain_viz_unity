using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultisubjectSelection : Spawner
{

	public ElectrodeSpawner electrodeSpawner;
	public GameObject subjectSelector;

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

	public void SingleSelect(int index, bool select)
	{
		selectors[index].selectionIndicator.enabled = select;
		electrodeSpawner.ShowElectrodesBySubject (selectors[index].GetName(), select);
		last_clicked = index;
	}

	public void UniqueSelect(int index)
	{
		Debug.Log ("unqiue");
		for (int i = 0; i < selectors.Length; i++)
		{
			if (i == index)
				SingleSelect (i, true);
			else
				SingleSelect (i, false);
		}
	}

	public void RangeSelect(int index)
	{
		Debug.Log ("range");
		if (index >= last_clicked)
		{
			for (int i = 0; i <= index; i++)
				SingleSelect (i, true);
		}
		else
		{
			for (int i = index; i >= index; i--)
				SingleSelect (i, true);
		}
	}
}