using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ThrowBodiesMM_Scr : MonoBehaviour
{
    [SerializeField] private GameObject bodyPref;

    private void Start()
    {
        ConstantlyFallingBodies();
    }

    private void DropBody()
    {
        Vector2 getRand = Random.insideUnitCircle.normalized * 5.5f;
        Vector3 randPos = new Vector3(getRand.x, 0.5f, getRand.y);
        Instantiate(bodyPref, randPos, Quaternion.identity);
    }
    private async void ConstantlyFallingBodies()
    {
        while (!destroyCancellationToken.IsCancellationRequested)
        {
            DropBody();
            await Task.Delay(Random.Range(1000, 7000));
        }
    }
}
