using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGravitationalImpulseEffectController : IEffectController
{
    void Deactivate();
}

public class GravitationalImpulseEffectController : EffectController, IGravitationalImpulseEffectController
{
    public override void Activate(int originId)
    {
        EffectManager.Instance.SetGravitationalImpulseEffectActive(true);
    }

    public void Deactivate()
    {
        EffectManager.Instance.SetGravitationalImpulseEffectActive(false);
    }
}
