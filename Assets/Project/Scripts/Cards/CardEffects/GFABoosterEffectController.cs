using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGFABoosterEffectController : IEffectController
{
    void Deactivate();
}

public class GFABoosterEffectController : EffectController, IGravitationalImpulseEffectController
{
    int extraDamage = 2;
    public override void Activate(int originId)
    {
        IPlayerController playerController = GameManager.Instance.LocalPlayerInstance.PlayerController;
        playerController.SetExtraDamage(extraDamage);
    }

    public void Deactivate()
    {
        IPlayerController playerController = GameManager.Instance.LocalPlayerInstance.PlayerController;
        playerController.SetExtraDamage(0);
    }
}
