using UnityEngine;
using System.Collections;

/// <summary>
/// I did not write this.  Ted Bigham wrote this.
/// 
/// It it a wrapper for a coroutine that makes it easier to return some data from the coroutine after it's finished.
/// </summary>
public class CoroutineWithData
{
	public Coroutine coroutine { get; private set; }
	public object result;
	private IEnumerator target;
	public CoroutineWithData(MonoBehaviour owner, IEnumerator target)
	{
		this.target = target;
		this.coroutine = owner.StartCoroutine(Run());
	}

	private IEnumerator Run()
	{
		while(target.MoveNext())
		{
			result = target.Current;
			yield return result;
		}
	}
}