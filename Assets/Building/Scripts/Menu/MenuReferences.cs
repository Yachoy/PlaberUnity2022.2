using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuReferences : MonoBehaviour
{
    [Header("Choise")]
    public Button menuBtn;
    public Button backToSelectGroupBtn;
    public InputField serachField;

    public GameObject scrollGroup;
    public GameObject contentGroup;

    public GameObject scrollMaster;
    public GameObject contentMaster;

    [Header("Settings")]
    public Button settingsBtn;
    public Image settingsBg;
    public Toggle aligmentTg;
    
    [Header("Other")]
    private Camera cam;
    private bool openMenu = false;


    public Vector3 close = new Vector3(1270, 0, 0); // позиция, в которой он закрыт
    public Vector3 open = new Vector3(662, 0, 0); // позиция, в которой он открыт
    private float CWopen = 0.695f; // camera width state open
    private float CWclose = 1f; // camera width state close


    void Start()
    {
        menuBtn.onClick.AddListener(click);
        cam = Camera.main;

        Rect r = cam.rect;
        r.width = 1f;
        cam.rect = r;
        transform.position = close;

        settingsBtn.onClick.AddListener( ()=>{
            settingsBg.gameObject.SetActive(!settingsBg.gameObject.activeSelf);
        });
        settingsBg.gameObject.SetActive(false);
    }



    private void click() => openMenu = !openMenu;

    private void FixedUpdate()
    {
        RectTransform tr = transform as RectTransform;
        if (!openMenu)
        {
            if (tr.localPosition.x < close.x)
            {
                transform.Translate(close * Time.fixedDeltaTime * 2);
                Rect r = cam.rect;
                r.width = CWclose * tr.localPosition.x / close.x;
                cam.rect = r;
            }
            else
                tr.localPosition = close; // выравнивание
        }
        else
        {
            if (tr.localPosition.x > open.x)
            {
                transform.Translate(-open * Time.fixedDeltaTime * 2);
                Rect r = cam.rect;
                r.width = CWopen * tr.localPosition.x / open.x;
                cam.rect = r;
            }
            else
                tr.localPosition = open; // выравнивание
        }
    }

}
