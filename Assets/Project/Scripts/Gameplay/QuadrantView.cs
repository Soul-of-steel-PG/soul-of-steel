using UnityEngine;

public interface IQuadrantView {
}

public class QuadrantView : MonoBehaviour, IQuadrantView {
    private IQuadrantController _quadrantController;

    public IQuadrantController QuadrantController {
        get { return _quadrantController ??= new QuadrantController(this); }
    }
}