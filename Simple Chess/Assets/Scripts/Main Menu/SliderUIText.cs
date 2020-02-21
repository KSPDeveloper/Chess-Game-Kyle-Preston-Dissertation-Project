using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderUIText : MonoBehaviour
{
    Text sliderText;
    public Slider sliderRef;
    void Start()
    {
        sliderText = GetComponent<Text>();
        sliderText.text = 5000.ToString();
        sliderRef.value = 5000;
    }

    public void textUpdate(Slider slider)
    {
        sliderText.text = slider.value.ToString();
        Stockfish.moveTime = int.Parse(sliderText.text);
    }
}
