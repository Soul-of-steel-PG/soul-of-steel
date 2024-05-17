using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDoubleMovementEffectController : IEffectController
{
}

public class DoubleMovementEffectController : EffectController, IDoubleMovementEffectController
{
    public override void Activate(int originId)
    {
        EffectManager.Instance.SetDoubleMovementEffectActive(true);
        GameManager.Instance.LocalPlayerInstance.PlayerController.SetDoingEffect(true);
        EffectManager.Instance.WaitForEffectCardAnimation();
    }
}
