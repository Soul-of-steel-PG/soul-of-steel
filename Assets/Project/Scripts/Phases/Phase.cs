using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public interface IPhase {
    IEnumerator Start();
    IEnumerator End();
}

public abstract class Phase : IPhase {
    protected readonly IMatchView matchView;

    protected Phase(IMatchView matchView) {
        this.matchView = matchView;
    }

    public virtual IEnumerator Start() {
        yield break;
    }

    public virtual IEnumerator End() {
        yield break;
    }
}