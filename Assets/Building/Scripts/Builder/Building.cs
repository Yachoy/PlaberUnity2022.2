
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class Building : MonoBehaviour
{

    struct DataObject
    {
        public Vector3 position;
        public Quaternion rotation;
        public string pathPrefab;

        public DataObject(Vector3 position, Quaternion rotation, string path) : this()
        {
            this.position = position;
            this.rotation = rotation;
            this.pathPrefab = path;
        }
    }

    private enum Align { Enabled, Disabled }

    private List<DataObject> AppendObject = new List<DataObject>();
    private List<GameObject> InstantiateHistory = new List<GameObject>();

    [SerializeField] private Align alignment = Align.Enabled; // выравнивание позиции, т.е. до целых чисел
    [SerializeField] private LayerMask layerMask; // фильтр, поверхность на которой тригерить RayCast для разрешения размещения префабов
    [SerializeField] private RectTransform IconsRect; // окно, в котором находятся иконки, что бы нельзя было за панелькой ставить объекты
    [SerializeField] private Color lockColor; // цвет, если невозможно установить
    [SerializeField] private Color unlockColor; // цвет, если возможно установить
    [SerializeField] private float heightOffset; // сдвиг по высоте для 3D, может понадобится, чтобы моделька не "висела в воздухе" или наоборот если они слишком "уходят под землю"

    // папки в Resources где лежат превью, оригинальные префабы и превью в картинках 
    [SerializeField] private string imgPath = "PreviewIMG";
    [SerializeField] private string prefabPath = "Prefab";
    [SerializeField] private static string previewPath = "Preview";

    private static bool _active;
    private static GameObject _lastTarget;
    private static LayerMask _ignoreMask;

    private static BuildingPreview target;

    private bool m_2d, canUse; // camera
    private static string curName; // last prefab file name
    private static float cutTime; // defender by spam? and trigger for BuildingPreview

    /// <summary>
    /// 'true' - если выбрана модель превью, но не установлена.
    /// </summary>
    public static bool isActive
    {
        get { return _active; }
    }

    /// <summary>
    /// Ссылка на последний установленный объект.
    /// </summary>
    public static GameObject lastTarget
    {
        get { return _lastTarget; }
    }

    /// <summary>
    /// Игнорируемые слои.
    /// </summary>
    public static LayerMask ignoreLayers
    {
        get { return _ignoreMask; }
    }

    /// <summary>
    /// Загрузка модели превью из Resources.
    /// </summary>
    /// <param name="objName">Имя целевого объекта.</param>
    public static void LoadPreview(string objName)
    {
        if (target != null) DestroyPreview();

        curName = objName;
        string path = previewPath + "/" + objName;
        BuildingPreview obj = Resources.Load<BuildingPreview>(path);

        if (obj)
        {
            _active = true;
            target = Instantiate(obj);
        }
        else Error(path);
    }

    /// <summary>
    /// Используется в связке с классом BuildingPreview.
    /// </summary>
    public static void ResetStatus()
    {
        cutTime = 0;
    }
    

    void Awake()
    {
        _active = false;
        _ignoreMask = ~layerMask;
        m_2d = Camera.main.orthographic;

        //GetComponent<Menu>().loadPrefabsFromResourcesToSCroll(imgPath, prefabPath);

    }

    // идея, возможно, хорошая, однако два ньюанса
    // 1) момент, когда центры перемекаються, или друг в друге (!canUse)
    // 2) подредактировать направление итоговых результатов 
    // 3) дергаеться объект после правок
    Vector3 GetMousePosition()
    {
        Vector3 pos = Vector3.zero;
        Collider col = target.GetComponent<Collider>();

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) pos = hit.point;

        if (alignment == Align.Enabled)
        {
            pos.x = Mathf.Round(pos.x);
            pos.z = Mathf.Round(pos.z);
            pos.y = Mathf.Round(pos.y) + heightOffset;
        }
        else
        {
            pos.y = pos.y + heightOffset;
        }

        if (!canUse)
        {
            //Debug.Log(col.attachedRigidbody);
        }


        Vector3 checkSide(Vector3 centerObject, Vector3 direction, GameObject ignoreSelf, float length)
        {
            RaycastHit[] hits = Physics.RaycastAll(centerObject, direction, length);
            foreach (RaycastHit hite in hits)
            {
                return hite.point;
            }
            return default;
        }
        
        float calculate(Vector3 dir, float length)
        {
            Vector3 temp = checkSide(
                pos, dir,
                target.gameObject, length
            );
            if (temp != Vector3.zero)
            {
                // return temp - pos;
                Vector3 dif = temp - pos;
                Debug.LogFormat("{0}: test,  length: {1}, dir: {2}", dif, length, dir);
                if (dir.x != 0)
                    return length - Mathf.Abs(dif.x) + 0.01f;


                if (dir.z != 0)
                    return length - Mathf.Abs(dif.z) + 0.01f;


            }
            return 0;
        }
        
        float lengthX = target.transform.position.x - col.bounds.min.x;
        float lengthY = target.transform.position.y - col.bounds.min.y;
        float lengthZ = target.transform.position.z - col.bounds.min.z;

        float PX = calculate(new Vector3(1, 0, 0), lengthX);
        float MX = calculate(new Vector3(-1, 0, 0), lengthX);
        float PZ = calculate(new Vector3(0, 0, 1), lengthZ);
        float MZ = calculate(new Vector3(0, 0, -1), lengthZ);

        pos -= new Vector3(PX, 0, PZ);
        pos += new Vector3(MX, 0, MZ);

        Debug.LogFormat("x+: {0}, x-: {1}, z+: {2}, z-: {3}",
            PX, MX, PZ, MZ
        );


        


        return pos;
    }

    bool CheckLayer() // доп. проверка при установки модели, чтобы нельзя было установить одну, внутри другой
    {
        int index = 0;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit) && !hit.collider.isTrigger) index = hit.collider.gameObject.layer;


        if (((1 << index) & ignoreLayers) != 0) // first expression??
        {
            return true;
        }

        return false;
    }

    // чтобы нельзя было спавнить друг в друге, находим, насколько они влазят в друг друга с помощью Ray cast
    Vector3 CheckRayCollisions(Vector3 position) 
    {
        return position;

        target.transform.position = position + new Vector3(0, 100, 0);
        RaycastHit YHit;
        if (Physics.Linecast(position, target.transform.position, out YHit, 1 << 2))
        {
            position = new Vector3(position.x, target.transform.position.y - YHit.distance, position.z);
        }

        target.transform.position = position + new Vector3(-100, 0, 0);
        RaycastHit Xhit;
        if (Physics.Linecast(position, target.transform.position, out Xhit, 1 << 2))
        {
            position = new Vector3(target.transform.position.x - Xhit.distance, position.y, position.z);
        }

        target.transform.position = position + new Vector3(0, 0, 100);
        RaycastHit Zhit;
        if (Physics.Linecast(position, target.transform.position, out Xhit, 1 << 2))
        {
            position = new Vector3(position.x, position.y, target.transform.position.z - Xhit.distance);
        }

        return position;
    }

    bool IsOverlap() // проеб после того, как префабы стали тоже получать маску layerMask
    {
        Vector2 mouse = Input.mousePosition;
        Vector3[] worldCorners = new Vector3[4];
        IconsRect.GetWorldCorners(worldCorners);

        if (mouse.x >= worldCorners[0].x && mouse.x < worldCorners[2].x
            && mouse.y >= worldCorners[0].y && mouse.y < worldCorners[2].y)
        {
            return true;
        }
        


        if (CheckLayer()) return true;
        return false;
    }

    Vector3 PositionCorrection(Vector3 position)
    {
         
        return position;
    }

    static void DestroyPreview()
    {
        Destroy(target.gameObject);
        _active = false;
    }

    void TargetStatus()
    {
        cutTime += Time.deltaTime;
        if (cutTime > .1f)
        {
            canUse = true;
            target.SetColor(unlockColor);
        }
        else
        {
            canUse = false;
            target.SetColor(lockColor);
        }
    }

    void TargetRotation()
    {
        if (!Input.GetKey(KeyCode.LeftControl) && Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            target.transform.Rotate(0, 90, 0);
        }
        if (!Input.GetKey(KeyCode.LeftControl) && Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            target.transform.Rotate(0, -90, 0);
        }
    }

    void createObjectByData(DataObject data)
    {
        GameObject obj = Resources.Load<GameObject>(data.pathPrefab);
        if (obj)
        {
            _lastTarget = Instantiate(obj, data.position, data.rotation) as GameObject;
            InstantiateHistory.Add(_lastTarget);
        }
    }

    void placementObject()
    {
        if (target == null) return;

        TargetStatus();

        Vector3 position = GetMousePosition();
        target.transform.position = CheckRayCollisions(position);

        TargetRotation();

        if (Input.GetMouseButtonDown(0) && !IsOverlap() && canUse)
        {
            string path = prefabPath + "/" + curName;
            GameObject obj = Resources.Load<GameObject>(path);
            if (obj)
            {
                _lastTarget = Instantiate(obj, target.transform.position, target.transform.rotation) as GameObject;
                _lastTarget.gameObject.layer = layerMask.value - 1; // кастыль, но какой
                _lastTarget.name = curName;

                AppendObject.Add(new DataObject(
                    _lastTarget.transform.position,
                    _lastTarget.transform.rotation,
                    path
                ));
                InstantiateHistory.Add(_lastTarget);
            }
            else Error(path);
            DestroyPreview();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            DestroyPreview();
        }
    }

    void Update()
    {

        placementObject();


        if (Input.GetKeyDown(KeyCode.Z)) // Undo
        {
            if (InstantiateHistory.Count > 0)
            {
                Destroy(InstantiateHistory[InstantiateHistory.Count - 1]);
                InstantiateHistory.RemoveAt(InstantiateHistory.Count - 1);
            }
        }
        if (Input.GetKeyDown(KeyCode.X)) // return last
        { 
            if (AppendObject.Count > 0 && AppendObject.Count - 1 >= InstantiateHistory.Count)
            {
                createObjectByData(AppendObject[InstantiateHistory.Count]);
            }
        }

    }

    static void Error(string val)
    {
        Debug.Log("[Building] указанного объекта не существует: Resources/" + val + ".prefab");
    }

    static void Errore(string val)
    {
        Debug.Log("[Building] " + val);
    }
}
