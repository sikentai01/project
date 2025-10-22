/// <summary>
/// セーブデータ読込後に、シーンオブジェクトを初期化したいスクリプト用。
/// GameBootstrap または BootLoader から自動で呼ばれる。
/// </summary>
public interface ISceneInitializable
{
    /// <summary>
    /// ロード完了後に1度呼ばれる初期化処理
    /// </summary>
    void InitializeSceneAfterLoad();
}