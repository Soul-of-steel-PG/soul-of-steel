using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRadarSabotageEffectController: IEffectController
{

}

public class RadarSabotageEffectController : EffectController, IRadarSabotageEffectController
{
    public override void Activate(int originId)
    {
        EffectManager effectManager = EffectManager.Instance;

        GameManager.Instance.LocalPlayerInstance.PlayerController.SetDoingEffect(true);

        IPlayerView enemyPlayer = GameManager.Instance.PlayerList.Find(p => p.PlayerController.GetPlayerId() != originId);

        if (enemyPlayer == null)
        {
            effectManager.WaitForEffectCardAnimation();
            return;
        }

        enemyPlayer.GetPv().RPC("RpcHideBoard", RpcTarget.AllBuffered, originId);

        effectManager.isRadarSabotageActive = true;
        effectManager.WaitForEffectCardAnimation();
    }

    public void Deactivate(int originId)
    {
        EffectManager.Instance.isRadarSabotageActive = false;
        EffectManager.Instance.radarSabotageRoundsCount = 0;
        IPlayerView enemyPlayer = GameManager.Instance.PlayerList.Find(p => p.PlayerController.GetPlayerId() != originId);
        enemyPlayer.GetPv().RPC("RpcShowBoard", RpcTarget.AllBuffered, originId);
    }
}
