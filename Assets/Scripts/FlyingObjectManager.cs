using UnityEngine;

public class FlyingObjectManager : MonoBehaviour
{
    [SerializeField]
    private int maxChildren = 50;
    private int currChildren = 0;

    [SerializeField]
    private GameObject Prefab;

    private void Update()
    {
        Vector3 pos = new Vector3(
                    Random.Range(0, Screen.width),
                    Random.Range(0, Screen.height),
                    0
                    );
        Quaternion rot = Quaternion.Euler(
                    0, 
                    0, 
                    Random.Range(0, 360)
                    );

        Vector3 WorldPos = Camera.main.ScreenToWorldPoint(pos);
        WorldPos.z = 0;

        if (currChildren < maxChildren)
        {
            Instantiate(
                Prefab,
                WorldPos,
                rot);
            currChildren++;
        }
    }
}
