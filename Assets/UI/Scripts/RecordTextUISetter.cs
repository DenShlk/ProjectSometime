using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


[RequireComponent(typeof(TextMeshProUGUI))]
[ExecuteInEditMode]
public class RecordTextUISetter : MonoBehaviour
{

    public string PrefixText;
    public string SuffixText;

    private TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        text.text = PrefixText + ScoreManager.TopScore + SuffixText;
    }

}
