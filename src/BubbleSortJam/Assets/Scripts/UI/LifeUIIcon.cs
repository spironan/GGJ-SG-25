using UnityEngine;

public class LifeUIIcon : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject Highlight;
    [SerializeField] private GameObject DisabledParticles;

    private bool isEnabled = true;

    public void SetEnabled(bool enabled)
    {
        if (isEnabled == enabled)
        {
            return;
        }


        isEnabled = enabled;

        if(isEnabled)
        {
            Highlight?.SetActive(true);
            DisabledParticles?.SetActive(false);
        }
        else
        {
            Highlight?.SetActive(false);
            DisabledParticles?.SetActive(true);
        }
    }
}