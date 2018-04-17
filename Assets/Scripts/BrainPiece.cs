using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainPiece : BrainWorldMonobehavior
{

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public override string InfoString ()
	{
		string[] splitName = gameObject.name.Split ('.');
		string infoText = "<b>Brain piece: </b>";
		for (int i = 0; i < splitName.Length - 1; i++)
		{
			string namePiece = splitName [i];
			string fullVersion;
			switch (namePiece)
			{
			case "lh":
				fullVersion = "Left Hemisphere";
				break;
			case "rh":
				fullVersion = "Right Hemisphere";
				break;
			case "hcp":
				fullVersion = "HCP";
				break;
			default:
				fullVersion = namePiece;
				break;
			}
			infoText = infoText + " " + fullVersion;
		}
		return infoText;
	}
}
