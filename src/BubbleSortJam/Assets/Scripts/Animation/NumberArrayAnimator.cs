using UnityEngine;
using System.Collections.Generic;

public class NumberArrayAnimator : MonoBehaviour
{
    [SerializeField] private GameObject elementPrefab;

    private List<NumberElementAnimator> elements = new List<NumberElementAnimator>();

    public int Count { get { return elements.Count; } }

    public void Initialize(List<int> values, float spacing)
    {
        foreach(int value in values)
        {
            NumberElementAnimator element = CreateElement(value);
            element.transform.localPosition = new Vector2((elements.Count - 1) * spacing, 0.0f);
        }
    }

    public void PlayArrayCompleteParticles()
    {
        for(int i = 0; i < elements.Count; ++i)
        {
            elements[i].PlayTopIntenseParticles();
        }

        if(elements.Count > 0)
        {
            elements[0].PlayLeftParticles();
            elements[elements.Count - 1].PlayRightParticles();
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
