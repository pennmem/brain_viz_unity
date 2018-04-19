using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMESlider : MonoBehaviour
{
	public UnityEngine.UI.Slider slider;
	public UnityEngine.UI.Text valueText;
	public UnityEngine.UI.Dropdown whichStatDropdown;
	public UnityEngine.UI.Dropdown whichFreqDropdown;
	public ElectrodeSpawner electrodeSpawner;

	void Start()
	{
		SetTextToMatchValue ();
	}

	public void UpdateSliderRange()
	{
		if (whichStatDropdown.value == 0)
		{
			slider.maxValue = 1f;
			slider.minValue = 0f;
		}
		else
		{
			slider.maxValue = 5f;
			slider.minValue = -5f;
		}
	}

	public void SetTextToMatchValue()
	{
		string comparisonOperator = "<";
		if (whichStatDropdown.value == 1)
			comparisonOperator = ">";
		valueText.text = comparisonOperator+slider.value.ToString ("0.00");
	}

	public void UpdateShownElectrodes()
	{
		bool pValueTrueTStatFalse = whichStatDropdown.value == 0;
		bool oneTenTrueHFAFalse = whichFreqDropdown.value == 0;
		float value = slider.value;
		electrodeSpawner.ShowElectrodesByStat (pValueTrueTStatFalse, oneTenTrueHFAFalse, value);
	}
}