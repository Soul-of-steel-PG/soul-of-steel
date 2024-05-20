using System.Collections.Generic;
using UnityEngine;

public interface IPilotCardController : ICardController {
    void InitCard(int id, string cardName, string cardDescription, int scrapCost, int scrapRecovery, Sprite imageSource,
        int health, Movement defaultMovement, CardType type, int defaultDamage = 0);

    Movement GetDefaultMovement();
    int GetDefaultDamage();
    int GetHealth();
    void SelectAttack();
}

public class PilotCardController : CardController, IPilotCardController {
    private readonly IPilotCardView _view;

    [Header("Pilot Properties")] private int _health;
    private int _defaultDamage;
    private Movement _defaultMovement;
    [Space(20)] public Vector2 position;

    private List<Vector2> currentCellsShaded;

    public PilotCardController(IPilotCardView view, IGameManager gameManager, IUIManager uiManager) : base(view,
        gameManager,
        uiManager)
    {
        _view = view;
    }

    public void InitCard(int id, string cardName, string cardDescription, int scrapCost, int scrapRecovery,
        Sprite imageSource, int health, Movement defaultMovement, CardType type,
        int defaultDamage = 1)
    {
        _health = health;
        _defaultMovement = defaultMovement;
        _defaultDamage = defaultDamage;

        /* Init card method called at the end because I am calling SetCardUI from it,
           and in this class I am modifying the SetCardUI*/
        base.InitCard(id, cardName, cardDescription, scrapCost, scrapRecovery, imageSource, type);
    }

    public Movement GetDefaultMovement()
    {
        return _defaultMovement;
    }

    public int GetDefaultDamage()
    {
        return _defaultDamage;
    }

    public int GetHealth()
    {
        return _health;
    }

    protected override void SetCardUI()
    {
        _view.SetCardUI(CardName, CardDescription, ScrapCost, ImageSource, _health);
    }

    public override CardType GetCardType()
    {
        return Type;
    }

    public void SelectAttack()
    {
        currentCellsShaded = new List<Vector2>();
        currentCellsShaded.Clear();
        PlayerView currentPlayer = GameManager.Instance.LocalPlayerInstance;
        IBoardView currentBoardView = GameManager.Instance.BoardView;

        int direction = currentPlayer.PlayerController.GetCurrentDegrees();
        Vector2 cellToSelect = currentPlayer.PlayerController.GetCurrentCell();

        for (int i = 0; i < 1; i++)
        {
            switch (direction)
            {
                case 180:
                    cellToSelect.x -= 1;
                    break;
                case 0:
                    cellToSelect.x += 1;
                    break;
                case 90:
                    cellToSelect.y -= 1;
                    break;
                case 270:
                    cellToSelect.y += 1;
                    break;
            }

            Vector2 index = new(
                Mathf.Clamp(cellToSelect.x, 0, currentBoardView.BoardController.GetBoardCount() - 1),
                Mathf.Clamp(cellToSelect.y, 0, currentBoardView.BoardController.GetBoardCount() - 1));

            currentCellsShaded.Add(index);
            GameManager.Instance.BoardView.SetBoardStatusCellType(index, CellType.Shady);
        }

        GameManager.Instance.OnCellClickedEvent += currentPlayer.PlayerController.DoAttack;

        GameManager.Instance.OnLocalAttackDoneEvent += UnShadeCells;
    }

    public void UnShadeCells()
    {
        foreach (Vector2 cellIndex in currentCellsShaded)
        {
            if (GameManager.Instance.BoardView.GetBoardStatus()[(int)cellIndex.y][(int)cellIndex.x].CellController
                .GetIsMined())
            {
                GameManager.Instance.BoardView.SetBoardStatusCellType(cellIndex, CellType.Mined);
            }
            else if (GameManager.Instance.BoardView.GetBoardStatus()[(int)cellIndex.y][(int)cellIndex.x].CellController
                     .GetIsBarrier())
            {
                GameManager.Instance.BoardView.SetBoardStatusCellType(cellIndex, CellType.Barrier);
            }
            else if (GameManager.Instance.BoardView.GetBoardStatus()[(int)cellIndex.y][(int)cellIndex.x].CellController
                     .GetIsTower())
            {
                GameManager.Instance.BoardView.SetBoardStatusCellType(cellIndex, CellType.Tower);
            }
            else
            {
                GameManager.Instance.BoardView.SetBoardStatusCellType(cellIndex, CellType.Normal);
            }
        }

        GameManager.Instance.OnLocalAttackDoneEvent -= UnShadeCells;
    }
}