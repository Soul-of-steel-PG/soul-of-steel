using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public interface IBarrierEffectController : IEffectController {
}

public class BarrierEffectController : EffectController, IBarrierEffectController {
    public List<Vector2> BarrierCells { private set; get; }
    private readonly int _barrierAmount;

    public BarrierEffectController(int barrierAmount)
    {
        BarrierCells = new List<Vector2>();
        _barrierAmount = barrierAmount;
    }


    public override void Activate(int originId)
    {
        GameManager.Instance.LocalPlayerInstance.PlayerController.SetDoingEffect(true);
        EffectManager.Instance.OnCellsSelectedEvent += StopSettingBarrier;
        EffectManager.Instance.OnSelectedCellEvent += SetBarrier;

        (GameManager.Instance.PlayerList.Find(p => p.PlayerController.GetPlayerId() == originId) as PlayerView)
            .SelectCells(_barrierAmount);
    }

    private void SetBarrier(Vector2 index, bool select)
    {
        if (CellType.Normal != GameManager.Instance.BoardView.GetBoardStatus()[(int)index.y][(int)index.x]
                .CellController.GetCellType()) return;
        GameManager.Instance.BoardView.SetBoardStatusCellType(index, select ? CellType.Barrier : CellType.Normal);

        if (!GameManager.Instance.testing)
        {
            GameManager.Instance.LocalPlayerInstance.GetPv().RPC("RpcPutBarrier",
                RpcTarget.AllBuffered, (int)index.y, (int)index.x, true);
        }
    }

    private void StopSettingBarrier(List<Vector2> cellsSelected)
    {
        EffectManager.Instance.OnAllEffectsFinished();
        GameManager.Instance.LocalPlayerInstance.PlayerController.SetDoingEffect(false);

        EffectManager.Instance.OnCellsSelectedEvent -= StopSettingBarrier;
        EffectManager.Instance.OnSelectedCellEvent -= SetBarrier;
    }
}