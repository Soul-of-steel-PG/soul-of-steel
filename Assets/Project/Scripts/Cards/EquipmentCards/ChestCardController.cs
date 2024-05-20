using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public interface IChestCardController : IEquipmentCardController {
}

public class ChestCardController : EquipmentCardController, IChestCardController {
    private readonly IChestCardView _view;

    public ChestCardController(IChestCardView view, IGameManager gameManager, IUIManager uiManager) : base(view,
        gameManager,
        uiManager)
    {
        _view = view;
    }
}