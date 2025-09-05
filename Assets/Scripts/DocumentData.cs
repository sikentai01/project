using UnityEngine;

[CreateAssetMenu(fileName = "NewDocument", menuName = "Game/Document")]
public class DocumentData : ScriptableObject
{
    public string documentID;
    public string title;
    [TextArea(3, 10)]
    public string body;
}