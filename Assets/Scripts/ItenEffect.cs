using UnityEngine;

public abstract class ItemEffect : MonoBehaviour
{
    public abstract void Execute(ItemBehaviour item);
}