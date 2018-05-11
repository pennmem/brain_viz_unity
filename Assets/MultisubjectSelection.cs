using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultisubjectSelection : MonoBehaviour
{

	public ElectrodeSpawner electrodeSpawner;
	public GameObject subjectSelector;

	private SubjectSelector[] selectors;

	public void SpawnAllFromList()
	{
		StartCoroutine (DoSpawnAllFromList ());
	}

	private IEnumerator DoSpawnAllFromList()
	{
		yield return electrodeSpawner.SpawnAllSubjects ();
		List<string> subjects = electrodeSpawner.GetSubjectList ();
		selectors = new SubjectSelector[subjects.Count];
		Debug.Log (subjects.Count);
		for (int i = 0; i < selectors.Length; i++)
		{
			string subject = subjects[i];
			GameObject selectorObject = Instantiate (subjectSelector);
			SubjectSelector selector = selectorObject.GetComponent<SubjectSelector> ();
			selector.Initialize (this, i, subject);
			selector.transform.parent = this.transform;
			selector.transform.position = selector.transform.position + new Vector3 (0, 25, 0);
		}
	}

	public void SingleSelect(int index, bool select)
	{
		electrodeSpawner.ShowElectrodesBySubject (selectors[index].GetName(), select);
	}

	public void AdditiveSelect(int index, bool select)
	{

	}

	public void RangeSelect(int index, bool select)
	{

	}
}