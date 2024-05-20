using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHandPanel
{
    void ResetAnimationReferenceParent();
    GameObject GetGo();
    Transform GetAnimationReference();
}

public class HandPanel : MonoBehaviour, IHandPanel
{
    [SerializeField] private bool isMiddle;
    public Transform animationReference;

    void Start()
    {
        if (isMiddle) GameManager.Instance.MiddlePanel = this;
        else GameManager.Instance.HandPanel = this;
    }

    public void ResetAnimationReferenceParent()
    {
        animationReference.SetParent(transform.parent);
    }

    public GameObject GetGo()
    {
        return gameObject;
    }

    public Transform GetAnimationReference()
    {
        return animationReference;
    }
}