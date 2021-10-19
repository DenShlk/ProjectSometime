using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TimeReverseButtonUpdater : MonoBehaviour
{
    private InputGrabber inputGrabber;
    private Image image;

    private void Start()
    {
        inputGrabber = FindObjectOfType<InputGrabber>();
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if (inputGrabber)
        {
            image.fillAmount = inputGrabber.GetTimeReverseReloadProgress();
        }
    }
}
