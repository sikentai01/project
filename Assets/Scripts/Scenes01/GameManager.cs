using UnityEngine;

public class GameManager : MonoBehaviour
{
    // シーンをまたいでアクセスするための静的なインスタンス
    public static GameManager instance;

    // ゲートが開いた状態かどうかを記録するフラグ
    public bool isGateOpen = false;

    // シングルトンパターンを実装
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // シーンが切り替わっても破棄されないようにする
        }
        else
        {
            Destroy(gameObject);
        }
    }
}