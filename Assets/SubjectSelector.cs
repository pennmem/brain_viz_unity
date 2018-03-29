using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubjectSelector : MonoBehaviour
{
	public UnityEngine.UI.Text subjectNameInput;
	public Spawner[] spawners;

	public void Go()
	{
		string subjectName = subjectNameInput.text;
		foreach (Spawner spawner in spawners)
			spawner.Despawn ();
		foreach (Spawner spawner in spawners)
			spawner.Spawn (subjectName);
	}
}
