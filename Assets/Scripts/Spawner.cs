using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spawner : MonoBehaviour
{

	public virtual void Despawn()
	{
		while (gameObject.transform.childCount > 0)
		{
			DestroyImmediate (gameObject.transform.GetChild (0).gameObject);
		}
	}

	public abstract IEnumerator Spawn (string subjectName, bool average_brain = false);

}
