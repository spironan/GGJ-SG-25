using UnityEngine;
using System.Collections.Generic;

public class TutorialUIManager : MonoBehaviour
{
    [SerializeField] private GameObject tutorialUIPrefab;
    [SerializeField] private List<TutorialData> tutorialData = new List<TutorialData>();

    private List<TutorialUI> tutorials = new List<TutorialUI>();

    public bool HasTutorial(int levelIndex)
    {
        return levelIndex >= 0 && levelIndex < tutorialData.Count && tutorialData[levelIndex].IsTutorial;
    }

    public TutorialUI TryCreateMessage(int levelIndex, Bounds levelBounds)
    {
        if (levelIndex < 0 && levelIndex >= tutorialData.Count)
        {
            return null;
        }

        TutorialUI instance = Instantiate(tutorialUIPrefab).GetComponent<TutorialUI>();
        instance.transform.SetParent(transform);

        instance.transform.position = new Vector3(levelBounds.center.x, levelBounds.min.y);

        instance.name = "Tutorial_" + tutorials.Count;
        tutorials.Add(instance);
        instance.Initialize(tutorialData[levelIndex]);
        return instance;
    }
}

[System.Serializable]
public class TutorialData
{
    [SerializeField] private string message;
    [SerializeField] private bool isTutorial = false;
    public string Message { get { return message; } }
    public bool IsTutorial { get { return isTutorial; } }
}