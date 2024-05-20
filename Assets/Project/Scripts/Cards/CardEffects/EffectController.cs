public interface IEffectController {
    void Activate(int targetId);
}

public abstract class EffectController : IEffectController {
    public abstract void Activate(int originId);
}