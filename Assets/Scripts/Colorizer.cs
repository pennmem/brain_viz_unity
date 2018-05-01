using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colorizer : MonoBehaviour
{
	public void Initialize()
	{
		ColorEverythingWhite ();
		MakeOpaque ();
	}

	private Renderer[] GetRenderers()
	{
		return GetComponentsInChildren<Renderer> ();
	}
		
	public void RandomizeColors()
	{
		foreach (Renderer renderer in GetRenderers())
		{
			renderer.material.color = new Color (Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0f, 1f), renderer.material.color.a);
		}
	}

	public void ColorEverythingWhite()
	{
		foreach (Renderer renderer in GetRenderers())
		{
			renderer.material.color = new Color (1, 1, 1, renderer.material.color.a);
		}
	}

	public void MakeTransparent()
	{
		foreach (Renderer renderer in GetRenderers())
		{
			StandardShaderUtils.ChangeRenderMode(renderer.material, StandardShaderUtils.BlendMode.Transparent);
			renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 0.1f);
		}
	}

	public void MakeOpaque()
	{
		foreach (Renderer renderer in GetRenderers())
		{
			StandardShaderUtils.ChangeRenderMode(renderer.material, StandardShaderUtils.BlendMode.Opaque);
			renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 1f);
		}
	}
}

public static class StandardShaderUtils
{
	public enum BlendMode
	{
		Opaque,
		Cutout,
		Fade,
		Transparent
	}

	public static void ChangeRenderMode(Material standardShaderMaterial, BlendMode blendMode)
	{
		switch (blendMode)
		{
		case BlendMode.Opaque:
			standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
			standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
			standardShaderMaterial.SetInt("_ZWrite", 1);
			standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
			standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
			standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
			standardShaderMaterial.renderQueue = -1;
			break;
		case BlendMode.Cutout:
			standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
			standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
			standardShaderMaterial.SetInt("_ZWrite", 1);
			standardShaderMaterial.EnableKeyword("_ALPHATEST_ON");
			standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
			standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
			standardShaderMaterial.renderQueue = 2450;
			break;
		case BlendMode.Fade:
			standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			standardShaderMaterial.SetInt("_ZWrite", 0);
			standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
			standardShaderMaterial.EnableKeyword("_ALPHABLEND_ON");
			standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
			standardShaderMaterial.renderQueue = 3000;
			break;
		case BlendMode.Transparent:
			standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
			standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			standardShaderMaterial.SetInt("_ZWrite", 0);
			standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
			standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
			standardShaderMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
			standardShaderMaterial.renderQueue = 3000;
			break;
		}

	}
}