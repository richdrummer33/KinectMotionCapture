using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggeredExplosion : MonoBehaviour
{
    public Rigidbody toExplode;
    public List<GameObject> explosionFx;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Floor")
        {
            toExplode.AddForce((Random.insideUnitSphere*0.5f + Vector3.up * 0.6f) * 20f, ForceMode.Impulse);

            int i = Mathf.RoundToInt(Random.RandomRange(0, explosionFx.Count - 1));
            Instantiate(explosionFx[i], toExplode.transform.position, Quaternion.identity);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
