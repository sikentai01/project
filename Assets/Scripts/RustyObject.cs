// RustyObject.cs
using UnityEngine;
public class RustyObject : MonoBehaviour
{
    private bool isCleaned = false;

    public void Clean()
    {
        if (isCleaned) return;
        Debug.Log("錆が取れた！");
        isCleaned = true;
        // アニメーションや状態変化処理
    }
}
