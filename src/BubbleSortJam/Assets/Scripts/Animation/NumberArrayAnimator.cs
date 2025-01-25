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

    public void PlaySwapAnimation(int elementIndex)
    {
        if(elementIndex < 0 || elementIndex + 1 >= elements.Count)
        {
            Debug.LogError("Invalid Element Index");
            return;
        }

        NumberElementAnimator lhs = elements[elementIndex];
        NumberElementAnimator rhs = elements[elementIndex + 1];

        int lhsValue = lhs.GetValue();
        int rhsValue = rhs.GetValue();

        lhs.PlaySwapAnimation(true, rhsValue);
        rhs.PlaySwapAnimation(false, lhsValue);
    }

    public void SetAllElementsState(NumberElementState state)
    {
        foreach(NumberElementAnimator element in elements)
        {
            element.UpdateState(state);
        }
    }

    public NumberElementAnimator FindElement(int elementIndex)
    {
        if (elementIndex < 0 || elementIndex >= elements.Count)
        {
            return null;
        }
        return elements[elementIndex];
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
