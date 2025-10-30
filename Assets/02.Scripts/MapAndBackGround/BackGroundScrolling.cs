using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundScrolling : MonoBehaviour
{
    [SerializeField] float speed;

    public new Transform camera;

    public GameObject[] sprites; //3°³
    private float halfScreenSizeX;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        halfScreenSizeX = Camera.main.orthographicSize * Camera.main.aspect; 
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;

        float cameraLeftEdge = camera.position.x - halfScreenSizeX;
        float spriteRightEdge = sprites[0].GetComponent<SpriteRenderer>().bounds.max.x;

        if (spriteRightEdge < cameraLeftEdge)
        {
            sprites[0].transform.position = sprites[2].transform.position + Vector3.right * halfScreenSizeX * 2;

            //½º¿Ò
            GameObject temp = sprites[0];
            sprites[0] = sprites[1];
            sprites[1] = sprites[2];
            sprites[2] = temp;
        }
    }
}
