using TMPro;
using UnityEngine;

public class ObjectiveTracker : MonoBehaviour
{
    [SerializeField] TMP_Text objectiveText;
    [SerializeField] string startingObjective = "Encontrá la forma de abrir la reja";

    void Start()
    {
        SetObjective(startingObjective);
    }

    public void SetObjective(string text)
    {
        objectiveText.text = text;
    }
}