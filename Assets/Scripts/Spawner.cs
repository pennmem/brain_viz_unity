using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spawner : MonoBehaviour
{

    /// <summary>
    /// By default despawning is just destroying all children, but this is overridable.
    /// </summary>
	public virtual void Despawn()
	{
		while (gameObject.transform.childCount > 0)
		{
			DestroyImmediate (gameObject.transform.GetChild (0).gameObject);
		}
	}

    /// <summary>
    /// Spawners are united by this functionality.  They all take a subject name and make some children for themselves based on what subject it is.
    /// </summary>
    /// <param name="subjectName">Subject name.</param>
    /// <param name="average_brain">If set to <c>true</c> average brain.</param>
	public abstract IEnumerator Spawn (string subjectName, bool average_brain = false);

}
