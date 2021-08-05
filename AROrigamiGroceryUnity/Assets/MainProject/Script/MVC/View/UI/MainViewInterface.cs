using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface MainViewInterface
{
    CanvasGroup CanvasGroup { get; }

    void Dispose();
}
