using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public interface IPhase
{
    IEnumerator Start();
}

public abstract class Phase : IPhase
{
    protected readonly IMatchView matchView;

    protected Phase(IMatchView matchView)
    {
        this.matchView = matchView;
        GameManager.Instance.CurrenPhase = this;
    }

    public virtual IEnumerator Start()
    {
        yield break;
    }

    public virtual void End()
    {
        matchView.StopPhase(this);
    }
}