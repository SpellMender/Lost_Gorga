using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRandom : MonoBehaviour
{
    [SerializeField] GameObject coin, gem, heart;

    List<GameObject> spawnpoints = new List<GameObject>();
    List<GameObject> spawnables = new List<GameObject>();

    Vector3 pos0;
    Vector3 pos1;
    Vector3 pos2;

    // Start is called before the first frame update
    void Start()
    {
        spawnables.Add(coin);
        spawnables.Add(gem);
        spawnables.Add(heart);

        spawnpoints.Add(Instantiate(spawnables[0]));
        spawnpoints.Add(Instantiate(spawnables[1]));
        spawnpoints.Add(Instantiate(spawnables[2]));

        pos0 = new Vector3(transform.position.x - 5, transform.position.y);
        pos1 = new Vector3(transform.position.x, transform.position.y);
        pos2 = new Vector3(transform.position.x + 5, transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        Spawn();
    }

    void Spawn()
    {
        // Spawn random collectibles at empty spawn points
        if (spawnpoints[0] == null) { StartCoroutine(spawnRandom(0)); }
        if (spawnpoints[1] == null) { StartCoroutine(spawnRandom(1)); }
        if (spawnpoints[2] == null) { StartCoroutine(spawnRandom(2)); }

        // Make sure they are at the correct positions
        if (spawnpoints[0] != null && spawnpoints[0].transform.position != pos0) spawnpoints[0].transform.position = pos0;
        if (spawnpoints[1] != null && spawnpoints[1].transform.position != pos1) spawnpoints[1].transform.position = pos1;
        if (spawnpoints[2] != null && spawnpoints[2].transform.position != pos2) spawnpoints[2].transform.position = pos2;

        // Make sure they are active in the scene
        if (spawnpoints[0] != null && !spawnpoints[0].activeSelf) spawnpoints[0].SetActive(true);
        if (spawnpoints[1] != null && !spawnpoints[1].activeSelf) spawnpoints[1].SetActive(true);
        if (spawnpoints[2] != null && !spawnpoints[2].activeSelf) spawnpoints[2].SetActive(true);
    }

    IEnumerator spawnRandom(int atIndex)
    {
        spawnpoints[atIndex] = Instantiate(spawnables[Random.Range(0, 3)]);
        yield return null;
    }
}
