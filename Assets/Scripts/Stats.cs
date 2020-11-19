using UnityEngine;

public class Stats : MonoBehaviour
{
    public float hp;
    public float maxHpBase;
    public float eRadius = 10;
    public bool destructible;

    private float maxHp;

    // Start is called before the first frame update
    void Start()
    {
        maxHp = maxHpBase;
        hp = maxHp;
    }
}
