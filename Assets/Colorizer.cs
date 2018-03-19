using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colorizer : MonoBehaviour
{
	private Renderer[] GetRenderers()
	{
		return GetComponentsInChildren<Renderer> ();
	}
		
	public void RandomizeColors()
	{
		foreach (Renderer renderer in GetRenderers())
		{
			renderer.material.color = new Color (Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0f, 1f));
		}
	}

	public void ColorEverythingWhite()
	{
		foreach (Renderer renderer in GetRenderers())
		{
			renderer.material.color = new Color (1, 1, 1);
		}
	}

	public void MakeTransparent()
	{
		foreach (Renderer renderer in GetRenderers())
		{
			renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 0.5f);
		}
	}

	public void MakeOpaque()
	{
		foreach (Renderer renderer in GetRenderers())
		{
			renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 1f);
		}
	}
}