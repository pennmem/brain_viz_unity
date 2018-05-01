using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StimSlider : MonoBehaviour
{
	public StimSiteSpawner stimSiteSpawner;
	public UnityEngine.UI.Text sliderText;
	public UnityEngine.UI.Slider slider;

	public void OnValueChanged()
	{
		float value = slider.value;
		sliderText.text = ">" + value.ToString ("F1") + "%";
		stimSiteSpawner.ShowStimSitesByDeltaRecall (value);
	}

}