using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvokeOnEnter : MonoBehaviour
{

	public UnityEngine.UI.InputField inputField;
	public UnityEngine.UI.Button invokeMe;

	private void Start()
	{
		inputField.onEndEdit.AddListener(val =>
			{
				if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
					invokeMe.onClick.Invoke();
			});
	}

}
