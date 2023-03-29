using System.Collections.Generic;
using UnityEngine;

public interface IPauseHandler
{
    void Pause(PauseMode pauseMode);
    void Unpause();
}

public enum PauseMode : short
{
    None,
    OnGame,
    Start
}

public class PauseHandler : MonoBehaviour, IPauseHandler
{
    protected PauseMode _mode;
    public void Pause(PauseMode pauseMode)
    {
        if (pauseMode == PauseMode.None)
            throw new System.InvalidOperationException();
        _mode = pauseMode;
        OnPaused(pauseMode);
    }

    public void Unpause()
    {
        OnUnpause();
        _mode = PauseMode.None;
    }

    protected virtual void OnPaused(PauseMode mode) { }
    protected virtual void OnUnpause() { }
}

public class GameProgressScaleController
{
    private static List<IPauseHandler> _items =
        new List<IPauseHandler>();

    public static void Subscribe(IPauseHandler obj)
    {
        _items.Add(obj);
    }

    public static void Unsubscribe(IPauseHandler obj)
    {
        _items.Remove(obj);
    }

    public static void Unpause()
    {
        foreach (var item in _items)
            item.Unpause();
    }

    public static void Pause(PauseMode pauseMode = PauseMode.OnGame)
    {
        foreach (var item in _items)
            item.Pause(pauseMode);
    }
}