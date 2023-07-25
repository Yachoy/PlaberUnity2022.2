using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject elementOfScroll; // ������, ������� ������������ ������� ������� � ui ������
    [SerializeField] private GameObject elementOfGroup; // ������, ������� ������������ ������� ������ � ui ������
    [SerializeField] private MenuReferences refer; // ������ �� ������� ��������� + �������� �������� ����
    [SerializeField] private string tagObjects = "obj"; // ��� RayCast, �� Update, ����� �������.
    [SerializeField] private Canvas canvas;
    [Header("PATH")]
    [SerializeField] private string imgPath = "PreviewIMG/"; // �������� ��� ������ ������� 
    [SerializeField] private string prefabPath = "Prefab/"; 
    [SerializeField] private string pathGroupsSprites = "dataOfGroups/"; // �������� ��� �����
    private Camera Cam;

    private Dictionary<string, List<InfoPrefab>> groups = 
        new Dictionary<string, List<InfoPrefab>>(); // ���� �������, �� ����������� ������ ������ ������ ��������

    private List<GameObject> instantiateElements = new List<GameObject>(); // ����� ��� ����� ������ �������� 

    void Start()
    {

        Cam = Camera.main;
        generateGroups(pathGroupsSprites); // ���������� ������ �����
        preparePrefabsOfGroups(imgPath, prefabPath); // ������ ������� � ��������� groups

        refer.backToSelectGroupBtn.onClick.AddListener(onClickBackToGroup);
        refer.backToSelectGroupBtn.gameObject.SetActive(false);

        //canvas.GetComponent<CanvasScaler>().referenceResolution =
        // new Vector2(Screen.width, Screen.height);
        
    }

    

    public void generateGroups(string pathGroups)
    {
        Debug.Log("Load prefabs from: " + pathGroups);

        Sprite[] objects = Resources.LoadAll<Sprite>(pathGroups);
        if (objects.Length == 0) return;

        foreach (Sprite obj in objects)
        {
            GameObject _temp = Instantiate(elementOfGroup, refer.contentGroup.transform);
            UIItem sett = _temp.GetComponent<UIItem>();
            sett.imgIcon.sprite = obj;
            sett.text.text = obj.name;
            sett.btn.onClick.AddListener(
                () => onSelectGroup(sett)
            );
            groups.Add(obj.name, new List<InfoPrefab>());
        }

    }

    private void onClickBackToGroup()
    {
        refer.scrollMaster.gameObject.SetActive(false);
        refer.scrollGroup.gameObject.SetActive(true);
        refer.backToSelectGroupBtn.gameObject.SetActive(false);
    }

    public void onSelectGroup(UIItem target)
    {
        refer.backToSelectGroupBtn.gameObject.SetActive(true);
        refer.scrollMaster.gameObject.SetActive(true);
        refer.scrollGroup.gameObject.SetActive(false);

        foreach(GameObject obj in instantiateElements)
        {
            Destroy(obj);
        }

        foreach(InfoPrefab data in groups[target.text.text])
        {
            GameObject _el = Instantiate(elementOfScroll, refer.contentMaster.transform);
            instantiateElements.Add(_el);

            UIItem srcs = _el.GetComponent<UIItem>();

            string name = data.header;
            string pathImg = "ImgEmpty";

            pathImg = imgPath + "/" + name;

            Sprite img = Resources.Load<Sprite>(
                 pathImg
            );

            if (img == null)
            {
                Errore("(err import) Can't find img in path: ");

                img = Resources.Load<Sprite>("ImgEmpty");
            }

            srcs.text.text =  name;
            srcs.imgIcon.sprite = img;
            srcs.btn.GetComponent<BuildingComponent>().modelName = name;
        }


        
        //srcs.text.text = data.autoFindImgByNamePrefab ? name : data.header;
        // srcs.img.sprite = img;

        // srcs.btn.GetComponent<BuildingComponent>().modelName = _obj.name;

    }

    private void preparePrefabsOfGroups(string pathFolderImg, string pathFolderPrefabs)
    {
        Debug.Log("Load prefabs from: " + pathFolderPrefabs);

        Object[] objects = Resources.LoadAll(pathFolderPrefabs);
        if (objects.Length == 0) return;
        foreach (Object obj in objects)
        {
            GameObject _obj = (GameObject)obj;

            InfoPrefab data = _obj.GetComponent<InfoPrefab>();
            data.header = _obj.name;
            if (data.owner == "none")
            {
                Errore("untrigger prefab owner: "+_obj.name);
            }
            bool r = groups.TryGetValue(data.owner, out List<InfoPrefab> el);

            if (r)
                el.Add(data);
            else
                Errore("not found owner: "+data.owner);

        }
    }




    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit raycastHit;
            Ray ray = Cam.ScreenPointToRay(Input.mousePosition);
            //if ( Physics.Raycast(ray, out raycastHit, Mathf.Infinity) && raycastHit.transform != null)

        }
        // Debug.Log(new Vector2(Screen.width, Screen.height));
        // scale factor on Canvas broke this way.
        // refer.open.x = Screen.width - 100 ;
        // refer.close.x = Screen.width + 500;
    }

    static void Errore(string val)
    {
        Debug.Log("[Menu] " + val);
    }
}
