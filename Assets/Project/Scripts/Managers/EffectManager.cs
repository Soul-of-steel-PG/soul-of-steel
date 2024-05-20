using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public interface IEffectManager {
    void CellsSelected(List<Vector2> cellsSelected);
    bool GetDoubleMovementEffectActive();
    void SetDoubleMovementEffectActive(bool isActive);
}

public class EffectManager : MonoBehaviourSingleton<EffectManager>, IEffectManager {
    public int effectTurn;
    private MineEffectController _mineEffectController;

    private BarrierEffectController _barrierEffectController;

    public event Action OnAllEffectsFinishedEvent;
    public event Action<Vector2, bool> OnSelectedCellEvent; //when selecting a single cell
    public event Action<List<Vector2>> OnCellsSelectedEvent; //when all cells have been selected

    #region DoubleMovementEffectController

    private DoubleMovementEffectController _doubleMovementEffectController;
    public bool doubleMovementEffectActive { get; set; }

    public void SetDoubleMovementEffectActive(bool isActive)
    {
        doubleMovementEffectActive = isActive;
    }

    private void ActivateDoubleMovement(int originId)
    {
        _doubleMovementEffectController = new DoubleMovementEffectController();
        _doubleMovementEffectController.Activate(originId);
    }

    #endregion

    #region GravitationalImpulseEffectController

    private GravitationalImpulseEffectController _gravitationalImpulseEffectController;
    public bool gravitationalImpulseEffectActive { get; set; }

    public void SetGravitationalImpulseEffectActive(bool isActive)
    {
        gravitationalImpulseEffectActive = isActive;
    }

    private void ActivateGravitationalImpulse(int originId)
    {
        _gravitationalImpulseEffectController = new GravitationalImpulseEffectController();
        _gravitationalImpulseEffectController.Activate(originId);
    }

    private void DeactivateGravitationalImpulse(int originId)
    {
        _gravitationalImpulseEffectController ??= new GravitationalImpulseEffectController();
        _gravitationalImpulseEffectController.Activate(originId);
    }

    #endregion

    #region GFABoosterCardController

    private GFABoosterEffectController _GFABoosterEffectController;

    private void ActivateGFABoosterEffectController(int originId)
    {
        _GFABoosterEffectController = new GFABoosterEffectController();
        _GFABoosterEffectController.Activate(originId);
    }

    private void DeactivateGFABoosterEffectController(int originId)
    {
        _GFABoosterEffectController ??= new GFABoosterEffectController();
        _GFABoosterEffectController.Deactivate();
    }

    #endregion

    #region OakShieldController

    private OakShieldEffectController _oakShieldEffectController;
    public bool oakShieldEffectActive { get; set; }

    public void SetOakShieldEffectActive(bool isActive)
    {
        oakShieldEffectActive = isActive;
    }

    private void ActivateOakShield(int originId)
    {
        _oakShieldEffectController = new OakShieldEffectController();
        _oakShieldEffectController.Activate(originId);
    }

    #endregion

    #region Teleport

    private TeleportEffectController _teleportEffectController;

    private void ActivateTeleportEffectController(int originId)
    {
        _teleportEffectController = new TeleportEffectController();
        _teleportEffectController.Activate(originId);
    }

    #endregion

    #region RadarSabotage

    private RadarSabotageEffectController _radarSabotageEffectController;
    public bool isRadarSabotageActive { get; set; } = false;
    public int radarSabotageRoundsCount = 0;

    private void ActivateRadarSabotageEffectController(int originId)
    {
        Debug.Log("Activating radar sabotage");
        _radarSabotageEffectController = new RadarSabotageEffectController();
        _radarSabotageEffectController.Activate(originId);
    }

    private void DeactivateRadarSabotageEffectController(int originId)
    {
        Debug.Log("Deactivating radar sabotage");
        _radarSabotageEffectController ??= new RadarSabotageEffectController();
        _radarSabotageEffectController.Deactivate(originId);
    }

    public void IncrementRadarSabotageRoundsCount()
    {
        if (isRadarSabotageActive)
        {
            Debug.Log("radarSabotageRoundsCount " + radarSabotageRoundsCount);
            if (radarSabotageRoundsCount <= 1)
            {
                ActivateRadarSabotageEffectController(GameManager.Instance.LocalPlayerInstance.PlayerController
                    .GetPlayerId());
                radarSabotageRoundsCount++;
            }
            else
            {
                DeactivateRadarSabotageEffectController(GameManager.Instance.LocalPlayerInstance.PlayerController
                    .GetPlayerId());
            }
        }
    }

    #endregion

    public void WaitForEffectCardAnimation()
    {
        StartCoroutine(WaitForSecondsCoroutine());
    }

    private IEnumerator WaitForSecondsCoroutine()
    {
        yield return new WaitForSeconds(1f);
        GameManager.Instance.LocalPlayerInstance.PlayerController.SetDoingEffect(false);
        OnAllEffectsFinished();
    }

    public void PutMines(int originId, int amount)
    {
        _mineEffectController = new MineEffectController(amount);
        _mineEffectController.Activate(originId);
    }

    public void PutBarrier(int originId, int amount)
    {
        _barrierEffectController = new BarrierEffectController(amount);
        _barrierEffectController.Activate(originId);
    }

    public void CellsSelected(List<Vector2> cellsSelected)
    {
        OnCellsSelectedEvent?.Invoke(cellsSelected);
    }

    public bool GetDoubleMovementEffectActive()
    {
        return doubleMovementEffectActive;
    }

    public void CellSelected(Vector2 index, bool select)
    {
        OnSelectedCellEvent?.Invoke(index, select);
    }

    public void OnAllEffectsFinished()
    {
        OnAllEffectsFinishedEvent?.Invoke();
    }

    public void GetEffect(int effectId, int originId)
    {
        switch (effectId)
        {
            case 0:
                PutMines(originId, 3);
                break;
            case 6:
                ActivateDoubleMovement(originId);
                break;
            case 26:
                ActivateGravitationalImpulse(originId);
                break;
            case 23:
                ActivateGFABoosterEffectController(originId);
                break;
            case 34:
                ActivateTeleportEffectController(originId);
                break;
            case 35:
                ActivateOakShield(originId);
                break;
            case 36:
                PutBarrier(originId, 1);
                break;
            case 38:
                ActivateRadarSabotageEffectController(originId);
                break;
        }
    }

    public void RemoveEffect(int effectId, int originId)
    {
        switch (effectId)
        {
            case 26:
                DeactivateGravitationalImpulse(originId);
                break;
            case 23:
                DeactivateGFABoosterEffectController(originId);
                break;
            case 38:
                DeactivateRadarSabotageEffectController(originId);
                break;
        }
    }
}