// RustyObject.cs
using UnityEngine;
public class RustyObject : MonoBehaviour
{
    private bool isCleaned = false;

    public void Clean()
    {
        if (isCleaned) return;
        Debug.Log("�K����ꂽ�I");
        isCleaned = true;
        // �A�j���[�V�������ԕω�����
    }
}
