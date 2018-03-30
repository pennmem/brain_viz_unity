﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spawner : MonoBehaviour
{

	public void Despawn()
	{
		while (gameObject.transform.childCount > 0)
		{
			DestroyImmediate (gameObject.transform.GetChild (0).gameObject);
		}
	}

	public abstract void Spawn (string subjectName);

}
