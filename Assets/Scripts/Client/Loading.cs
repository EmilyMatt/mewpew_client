using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public Object[] sprites;

    private SpriteRenderer image;
    private int idx = 0;
    private bool inc = true;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        sprites = Resources.LoadAll("Loading", typeof(Sprite));
        image = GetComponent<SpriteRenderer>();
        StartCoroutine(MainThread());
    }

    void LateUpdate()
    {
        if (transform.localScale == Vector3.zero)
            return;

        Transform camTransform = Camera.main.transform;
        transform.position = camTransform.position + camTransform.forward * 3;
        transform.rotation = camTransform.rotation;
    }

    IEnumerator MainThread()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.01f);

            if (transform.localScale == Vector3.zero)
                continue;

            image.sprite = (Sprite)sprites[idx];

            if (inc)
            {
                idx++;
                if (idx == sprites.Length - 1)
                    inc = false;
            }
            else
            {
                idx--;
                if (idx == 0)
                    inc = true;
            }
        }
    }
}
