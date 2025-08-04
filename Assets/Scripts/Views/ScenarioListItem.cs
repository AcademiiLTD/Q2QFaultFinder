using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScenarioListItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _labelText;

    public void PopulateItem(string newText)
    {
        _labelText.text = newText;
    }
}
