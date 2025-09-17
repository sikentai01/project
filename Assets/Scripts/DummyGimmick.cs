using UnityEngine;

public class DummyGimmick : GimmickBase
{
    public override void StartGimmick(ItemTrigger trigger)
    {
        Debug.Log("�_�~�[�M�~�b�N�J�n�IEnter�������Ă�������");
        StartCoroutine(WaitForInput(trigger));
    }

    private System.Collections.IEnumerator WaitForInput(ItemTrigger trigger)
    {
        // Enter���������܂ő҂�
        while (!Input.GetKeyDown(KeyCode.Return))
        {
            yield return null;
        }

        Debug.Log("�_�~�[�M�~�b�N�����I");
        trigger.CompleteCurrentGimmick();
    }
}