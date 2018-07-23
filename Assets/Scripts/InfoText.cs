using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Lets other classes access the info text box via its status member.
/// </summary>
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
