using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TextMeshProUGUI text;

    public void Initialize(TutorialData data)
    {
        text.text = data.Message;
    }
}