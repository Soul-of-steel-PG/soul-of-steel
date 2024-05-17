using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOakShieldEffectController : IEffectController
{
}

public class OakShieldEffectController : EffectController, IOakShieldEffectController
{
    public override void Activate(int originId)
    {
        EffectManager.Instance.SetOakShieldEffectActive(true);
        GameManager.Instance.LocalPlayerInstance.PlayerController.SetDoingEffect(true);
        EffectManager.Instance.WaitForEffectCardAnimation();
    }
}
