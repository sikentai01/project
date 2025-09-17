using UnityEngine;

public class DummyGimmick : GimmickBase
{
    public override void StartGimmick(ItemTrigger trigger)
    {
        Debug.Log("ダミーギミック開始！Enterを押してください");
        StartCoroutine(WaitForInput(trigger));
    }

    private System.Collections.IEnumerator WaitForInput(ItemTrigger trigger)
    {
        // Enterが押されるまで待つ
        while (!Input.GetKeyDown(KeyCode.Return))
        {
            yield return null;
        }

        Debug.Log("ダミーギミック成功！");
        trigger.CompleteCurrentGimmick();
    }
}