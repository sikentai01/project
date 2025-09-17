using UnityEngine;

public class PasswordGimmick : GimmickBase
{
    public string correctPassword = "1234";

    public override void StartGimmick(ItemTrigger trigger)
    {
        Debug.Log("パスワード入力開始！（デバッグでは自動成功）");

        // 本来はUI入力を待つが、デバッグ用はEnterで成功
        StartCoroutine(WaitForInput(trigger));
    }

    private System.Collections.IEnumerator WaitForInput(ItemTrigger trigger)
    {
        while (!Input.GetKeyDown(KeyCode.Return))
        {
            yield return null;
        }

        Debug.Log("パスワード成功！");
        trigger.CompleteCurrentGimmick();
    }
}