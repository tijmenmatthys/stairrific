using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _doorsConnectedText;

    public void UpdateDoorsConnectedText(int amount)
    {
        _doorsConnectedText.text = $"{amount} doors connected";
    }
}
