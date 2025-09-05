using UnityEngine;

public class DoorAnimationController : MonoBehaviour
{
    // インスペクターからキャラクターのAnimatorを割り当てる
    public Animator characterAnimator;

    // プレイヤーがコライダー内にいるかどうかのフラグ
    private bool playerIsNearDoor = false;

    // トリガーコライダーに入ったときに呼び出される
    void OnTriggerEnter2D(Collider2D other)
    {
        // プレイヤーに"Player"タグがついているか確認
        if (other.CompareTag("Player"))
        {
            playerIsNearDoor = true;
            Debug.Log("プレイヤーがドアの近くに入りました"); // ★これを追加
        }
    }

    // トリガーコライダーから出たときに呼び出される
    void OnTriggerExit2D(Collider2D other)
    {
        // プレイヤーに"Player"タグがついているか確認
        if (other.CompareTag("Player"))
        {
            playerIsNearDoor = false;
            Debug.Log("プレイヤーがドアから離れました"); // ★これを追加
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // プレイヤーがドアの近くにいて、エンターキーが押されたかチェック
        if (playerIsNearDoor && Input.GetKeyDown(KeyCode.Return))
        {
            // Animatorにトリガーをセットして、アニメーションを再生
            characterAnimator.SetTrigger("OpenGateTrigger");
        }
    }
}
