using UnityEngine;

public abstract class BeatableObjectScript : MonoBehaviour
{
    public bool canHit = true;

    public int maxHitCount;
    [HideInInspector] public int conHitCount = 0;

    public abstract void Hit();

    public abstract void Break();
}
