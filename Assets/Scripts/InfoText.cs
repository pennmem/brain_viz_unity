using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoText : MonoBehaviour
{
	private static InfoText instance;

	void Start ()
	{
		instance = this;	
	}

	public static void SetText(string text)
	{
		instance.GetComponent<UnityEngine.UI.Text> ().text = text;
	}
}
