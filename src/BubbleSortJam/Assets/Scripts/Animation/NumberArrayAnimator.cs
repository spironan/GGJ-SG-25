using UnityEngine;
using System.Collections.Generic;
using System;

public class NumberArrayAnimator : MonoBehaviour
{
    [SerializeField] private GameObject elementPrefab;

    private List<NumberElementAnimator> elements = new List<NumberElementAnimator>();

    public void Initialize(List<int> values, float spacing)
    {
        foreach(int value in values)
        {
            NumberElementAnimator element = CreateElement(value);
            element.transform.localPosition = new Vector2((elements.Count - 1) * spacing, 0.0f);
        }
    }

    private NumberElementAnimator CreateElement(int value)
    {
        NumberElementAnimator element = Instantiate(elementPrefab).GetComponent<NumberElementAnimator>();
        element.name = "ElementAnimator_" + elements.Count;
        element.SetValue(value);

        element.transform.SetParent(transform);
        elements.Add(element);

        return element;
    }
}
