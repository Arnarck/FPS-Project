using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpreadTest : MonoBehaviour
{
    public float range = 3f;
    public int max_iterations = 100;
    public GameObject prefab;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(generate_random_point());
        //StartCoroutine(generate_random_point_around_edges());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator generate_random_point()
    {
        int iterations = 0;
        while (iterations <= max_iterations)
        {
            float random_x = Random.Range(-range, range);
            float max_y = Mathf.Sqrt( Mathf.Pow(range, 2f) - Mathf.Pow(random_x, 2f) );
            float random_y = Random.Range(-max_y, max_y);

            Instantiate(prefab, new Vector3(random_x, 0f, random_y), Quaternion.identity);

            iterations++;
            yield return new WaitForSeconds(.1f);
        }
    }

    IEnumerator generate_random_point_around_edges()
    {
        int iterations = 0;
        while (iterations <= max_iterations)
        {
            float random_x = Random.Range(-range, range);
            float max_y = Mathf.Sqrt(Mathf.Pow(range, 2f) - Mathf.Pow(random_x, 2f));
            float random_y = Random.Range(0, 2) == 0 ? max_y : -max_y;

            Instantiate(prefab, new Vector3(random_x, 0f, random_y), Quaternion.identity);

            iterations++;
            yield return new WaitForSeconds(.1f);
        }
    }
}
