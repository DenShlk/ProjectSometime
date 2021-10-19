using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


[RequireComponent(typeof(TextMeshProUGUI))]
[ExecuteInEditMode]
public class PlayerLifesUISetter : MonoBehaviour
{
    public PlayerBehaviour playerState;

    public string PrefixText;
    public string SuffixText;

    private TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (playerState)
        {
            text.text = PrefixText + playerState.lifes + SuffixText;
        }
    }

}
