using UnityEngine;

public class PasswordGimmick : GimmickBase
{
    public string correctPassword = "1234";

    public override void StartGimmick(ItemTrigger trigger)
    {
        Debug.Log("�p�X���[�h���͊J�n�I�i�f�o�b�O�ł͎��������j");

        // �{����UI���͂�҂��A�f�o�b�O�p��Enter�Ő���
        StartCoroutine(WaitForInput(trigger));
    }

    private System.Collections.IEnumerator WaitForInput(ItemTrigger trigger)
    {
        while (!Input.GetKeyDown(KeyCode.Return))
        {
            yield return null;
        }

        Debug.Log("�p�X���[�h�����I");
        trigger.CompleteCurrentGimmick();
    }
}