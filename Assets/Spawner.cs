using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spawner : MonoBehaviour
{

	public void Despawn()
	{
		for (int i = 0; i < gameObject.transform.childCount; i++)
		{
			Destroy (gameObject.transform.GetChild (i));
		}
	}

	public abstract void Spawn (string subjectName);

}
