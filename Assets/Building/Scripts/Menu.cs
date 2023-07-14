using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject elementOfScroll; // Префаб, который представляет элемент в ui списке: Canvas -> Scroll View -> Viewport -> Content
    [SerializeField] private RectTransform scrollList; // элемент-контента в ui : Canvas -> Scroll View -> Viewport -> Content
    [SerializeField] private SettingsObjects settingsObjects;
    [SerializeField] private string tagObjects = "obj";
    private Camera Cam;
    private GameObject _lastTarger;

    public void loadPrefabsFromResourcesToSCroll(string pathFolderImg, string pathFolderPrefabs)
    {
        pathFolderPrefabs += "/";
        if (elementOfScroll == null)
        {
            Errore("на скрипте Building не поставлен префаб elementOfScroll, загрузка префабов в Scroll остановлена");
            return;
        }
        if (scrollList == null)
        {
            Errore("на скрипте Building не поставлен префаб scrollList, загрузка префабов в Scroll остановлена");
            return;
        }
        Debug.Log("Load prefabs from: " + pathFolderPrefabs);

        Object[] objects = Resources.LoadAll(pathFolderPrefabs);
        if (objects.Length == 0) return;
        foreach (Object obj in objects)
        {
            GameObject _obj = (GameObject)obj;

            InfoPrefab data = _obj.GetComponent<InfoPrefab>();
            GameObject _el = Instantiate(elementOfScroll, scrollList);

            DummyReferences srcs = _el.GetComponent<DummyReferences>();



            if (data != null)
            {
                string name = data.header;
                string pathImg = "ImgEmpty";

                if (data.autoFindImgByNamePrefab)
                    pathImg = pathFolderImg + "/" + _obj.name;

                if (data.autoFindImgByNamePrefab)
                    name = _obj.name;

                Sprite img = Resources.Load<Sprite>(
                    data.autoFindImgByNamePrefab ? pathImg : data.pathToImg
                );

                if (img == null)
                {
                    if (data.autoFindImgByNamePrefab)
                        Errore("(err import) Can't find img in path: " + pathImg);
                    else
                        Errore("(err import) Can't find img in path: " + data.pathToImg);

                    img = Resources.Load<Sprite>("ImgEmpty");
                }
                srcs.text.text = data.autoFindImgByNamePrefab ? name : data.header;
                srcs.img.sprite = img;

                srcs.btn.GetComponent<BuildingComponent>().modelName = _obj.name;
            }
            else
            {
                Errore("(err import) Prefab missing InfoPrefab component");
            }
        }
    }
   
    
    // Start is called before the first frame update
    void Start()
    {
        if(settingsObjects == null)
            Errore("Не передан компонент settingsObjects (панель с настройками, ссылки на элементы settings)");

        Cam = Camera.main;
        settingsObjects.xInput.onSubmit.AddListener(OnChacngeX);
        settingsObjects.yInput.onSubmit.AddListener(OnChacngeZ);
    }

    void OnChacngeX(string value)
    {

    }

    void OnChacngeZ(string value)
    {

    }

    void makeSelect(GameObject target)
    {
        if(target.tag == tagObjects)
        {
            _lastTarger = target;
            settingsObjects.setTX(target.transform.localScale.x.ToString());
            settingsObjects.setTY(target.transform.localScale.y.ToString());
            settingsObjects.setTSelect(target.name);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit raycastHit;
            Ray ray = Cam.ScreenPointToRay(Input.mousePosition);
            if (
                Physics.Raycast(ray, out raycastHit, Mathf.Infinity) && 
                raycastHit.transform != null
                )
                    makeSelect(raycastHit.transform.gameObject);
        }
    }

    static void Errore(string val)
    {
        Debug.Log("[Menu] " + val);
    }
}
