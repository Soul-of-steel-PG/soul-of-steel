using DG.Tweening;
using UnityEngine;

public interface IEquipmentCardController : ICardController {
    void InitCard(int id, string cardName, string cardDescription, int scrapCost, int scrapRecovery,
        int shieldValue, Sprite imageSource, CardType type);

    void EquipCardAnimation(bool isMine);

    void GetEffect();
    void RemoveEffect();
}

public class EquipmentCardController : CardController, IEquipmentCardController {
    private readonly IEquipmentCardView _view;
    private int _shieldValue;

    public EquipmentCardController(IEquipmentCardView view, IGameManager gameManager, IUIManager uiManager) : base(view,
        gameManager,
        uiManager)
    {
        _view = view;
    }

    public override CardType GetCardType()
    {
        return Type;
    }

    public void InitCard(int id, string cardName, string cardDescription, int scrapCost, int scrapRecovery,
        int shieldValue, Sprite imageSource, CardType type)
    {
        _shieldValue = shieldValue;
        base.InitCard(id, cardName, cardDescription, scrapCost, scrapRecovery, imageSource, type);
    }

    public void EquipCardAnimation(bool isMine)
    {
        Transform t = _view.GetGameObject().transform;
        EquipmentPanel endPanel =
            isMine ? GameManager.Instance.myEquipmentPanel : GameManager.Instance.enemyEquipmentPanel;

        Vector3 endPos = endPanel.transform.position;

        GameManager.Instance.LocalPlayerInstance._inAnimation = true;
        t.DOMove(endPos, 0.5f).OnComplete(() => {
            GameManager.Instance.LocalPlayerInstance._inAnimation = false;

            _view.DestroyGo(t.gameObject);
        });
    }

    public void GetEffect()
    {
        EffectManager.Instance.GetEffect(Id, GameManager.Instance.LocalPlayerInstance.PlayerController.GetPlayerId());
    }

    public void RemoveEffect()
    {
        EffectManager.Instance.RemoveEffect(Id,
            GameManager.Instance.LocalPlayerInstance.PlayerController.GetPlayerId());
    }
}