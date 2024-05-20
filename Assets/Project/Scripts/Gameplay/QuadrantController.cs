using UnityEngine;

public interface IQuadrantController {
}

public class QuadrantController : IQuadrantController {
    private readonly IQuadrantView _view;

    public int quadrantId;
    public EffectCardView quadrantCardEffect; 

    public QuadrantController(IQuadrantView view) {
        _view = view;
    }
}