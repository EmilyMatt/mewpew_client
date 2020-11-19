using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    private Transform camTransform;
    private GameObject starMap;
    // Start is called before the first frame update
    void Start()
    {
        camTransform = Camera.main.transform;
        starMap = transform.Find("StarMap").gameObject;
        starMap.SetActive(false);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.M))
            starMap.SetActive(!starMap.activeSelf);

        if (!starMap.activeSelf)
            return;

        transform.position = camTransform.position + camTransform.forward*2;
        transform.rotation = camTransform.rotation;
    }

    void HUDEvent(string[] info)
    {
        Transform element = GameObject.Find(info[0]).transform;
        string myEvent = info[1];
    }
}
