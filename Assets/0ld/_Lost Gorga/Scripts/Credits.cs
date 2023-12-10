using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Lost_Gorga.Old
{
    public class Credits : MonoBehaviour
    {
        RectTransform credits;

        [SerializeField] float speed = 1, maxHeight = 5;
        // Start is called before the first frame update
        void Start()
        {
            credits = GetComponent<RectTransform>();
        }

        // Update is called once per frame
        void Update()
        {
            if (credits.position.y < maxHeight)
            {
                credits.Translate(Vector3.up * speed * Time.deltaTime);
            }
        }
    }
}
