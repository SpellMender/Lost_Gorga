using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogSpawner : MonoBehaviour
{
    float timer;
    float maxTime = 0.25f;

    [SerializeField]
    GameObject log;

    // Start is called before the first frame update
    void Start()
    {
        timer = maxTime;
    }

    // Update is called once per frame
    void Update()
    {
        LogInterval();
    }

    public void LogInterval()
    {
        if (timer <= 0)
        {
            SpawnLog();
            timer = maxTime;
        }

        timer -= Time.deltaTime;
    }

    void SpawnLog()
    {
        GameObject newLog = Instantiate(log);
        newLog.transform.position = transform.position;
    }

}
