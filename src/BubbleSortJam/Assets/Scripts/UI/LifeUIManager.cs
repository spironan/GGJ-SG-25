using UnityEngine;
using System.Collections.Generic;

public class LifeUIManager : MonoBehaviour
{
    private GameObject iconTemplate = null;
    private List<LifeUIIcon> iconList = new List<LifeUIIcon>();

    private GameplayEventListener eventListener = new GameplayEventListener();

    private void Awake()
    {
        iconTemplate = transform.GetChild(0).gameObject;

        eventListener.Activate();
        eventListener.AddCallback(typeof(LifeChangedGameplayEvent), OnLifeChanged);
    }

    private void OnDestroy()
    {
        eventListener.Deactivate();
    }

    private void OnLifeChanged(BaseGameplayEvent baseEvent)
    {
        LifeChangedGameplayEvent usableEvent = (LifeChangedGameplayEvent)baseEvent;
        UpdateIcons(usableEvent.CurrentHealth);
    }

    private void UpdateIcons(int newIconCount)
    {
        for(int i = 0; i < iconList.Count; ++i)
        {
            iconList[i].SetEnabled(i < newIconCount);
        }
        for (int i = iconList.Count; i < newIconCount; ++i)
        {
            CreateIcon();
        }
    }

    private LifeUIIcon CreateIcon()
    {
        LifeUIIcon instance = Instantiate(iconTemplate).GetComponent<LifeUIIcon>();
        instance.name = "LifeIcon_" + iconList.Count;
        instance.transform.SetParent(transform, false);


        iconList.Add(instance);
        return instance;
    }
}