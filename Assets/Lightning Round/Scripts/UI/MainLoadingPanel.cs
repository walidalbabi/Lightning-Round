using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainLoadingPanel : MonoBehaviour
{
    [SerializeField] private Slider _loadingSlider;
    [SerializeField] private TextMeshProUGUI _silderText;

    public void IncreaseLoadingBar(float amount)
    {
        if (_loadingSlider == null) return;

        _loadingSlider.value += amount;
        _silderText.text = _loadingSlider.value.ToString() +"%";

        if (_loadingSlider.value == 100f)
        {
            gameObject.SetActive(false);
        }
    }
}
