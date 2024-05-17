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
        GameManager.Instance.LocalPlayerInstance.PlayerController.SetDoingEffect(true);

        IPlayerView enemyPlayer = GameManager.Instance.PlayerList.Find(p => p.PlayerController.GetPlayerId() != originId);
        enemyPlayer.GetPv().RPC("RpcHideBoard", RpcTarget.AllBuffered, originId); 

        EffectManager.Instance.WaitForEffectCardAnimation();
    }

    public void Deactivate(int originId)
    {
        IPlayerView enemyPlayer = GameManager.Instance.PlayerList.Find(p => p.PlayerController.GetPlayerId() != originId);
        enemyPlayer.GetPv().RPC("RpcShowBoard", RpcTarget.AllBuffered, originId);
    }
}
