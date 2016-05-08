using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Скрипт, который задаёт поведение камеры, её эффекты, следование за персонажем и все механики движения
/// </summary>
public class CameraController : MonoBehaviour
{

    #region consts
    const float sizeX= 81f;
    const float sizeY = 43f;
    const float fieldOfView = 87f;
    const float offsetX = 0f;
    const float offsetY = -11f;
    const float offsetZ = -24.5f;
    const float camSpeed = 20f;
    #endregion //consts

    #region enums
    public enum camMovX {movLeft=-1, stop=0, movRight=1};
    public enum camMovY {movDown=-1, stop=0, movUp=1 };
    #endregion //enums

    #region parametres

    private GameObject character;//За каким персонажем камера следует
    private GameObject ambLight;
    private Rigidbody2D rigid;
    private Vector3 offsetPosition;
    private Vector2 camSize = new Vector2 (sizeX,sizeY);//размер камеры, используемый для подсчёта нормального движения внутри комнаты
    private Transform camWindow;//Окошечко, выйдя за пределы которого персонаж двигает камеру.
    private AreaClass currentArea;
    private Camera cam;
    public Rect roomCoords;
    public int movX = 0, movY=0;//Есть ли у камеры ограничения на передвижение вдоль направление движущегося персонажа
    private float deltaX, deltaY;
    public Vector3 vect;

    public List<BackgroundClass> backgroundList=new List<BackgroundClass>();

    #endregion //parametres

    public void FixedUpdate()
    {
        vect = cam.WorldToViewportPoint(character.transform.position);
        Vector2 spotPosition = new Vector2(transform.position.x + offsetPosition.x, transform.position.y + offsetPosition.y);
        Vector3 newPosition;
        #region howToHorizontalMove 
        movX = (int)camMovX.stop;
        if ((character.transform.position.x > camWindow.position.x + camWindow.localScale.x / 2)
            && (spotPosition.x < currentArea.position.x + (currentArea.size.x - camSize.x) / 2))
        {
            movX = (int)camMovX.movRight;
        }
        if ((character.transform.position.x < camWindow.position.x - camWindow.localScale.x / 2)
            && (spotPosition.x > currentArea.position.x - (currentArea.size.x - camSize.x) / 2))
        {
            movX = (int)camMovX.movLeft;
        }
        
        #endregion //howToHorizontalMove

        #region howToVerticalMove
        if ((character.transform.position.y > spotPosition.y)
            && (transform.position.y< currentArea.position.y + (currentArea.size.y - camSize.y) / 2))
        {
            movY = (int)camMovY.movUp;
        }
        else if ((character.transform.position.y < spotPosition.y)
            && (transform.position.y > currentArea.position.y - (currentArea.size.y - camSize.y) / 2))
        {
            movY = (int)camMovY.movDown;
        }
        else
        {
            movY = (int)camMovY.stop;
        }
        #endregion //howToVerticalMove

        #region Move
        Vector2 distance = new Vector2 (Mathf.Abs(character.transform.position.x - spotPosition.x),
                                        Mathf.Abs(character.transform.position.y - spotPosition.y));
        deltaX = Mathf.Clamp(transform.position.x + movX * (distance.x - camWindow.localScale.x/2-0.3f), roomCoords.xMin, roomCoords.xMax)-transform.position.x;
        deltaY = Mathf.Clamp(transform.position.y + movY * distance.y, roomCoords.yMin, roomCoords.yMax)-transform.position.y;
        if (movX * deltaX < 0f)
        {
            deltaX = 0f;
        }
        newPosition = new Vector3(transform.position.x + deltaX,
                                         transform.position.y + deltaY,
                                         transform.position.z);
        transform.position = Vector3.Lerp(transform.position, newPosition, camSpeed);
        Parallax(deltaX, deltaY);
        /*transform.position = Vector3.Lerp(transform.position, newPosition, camSpeed*Time.deltaTime);
        transform.position=new Vector3(Mathf.Clamp(transform.position.x, roomCoords.xMin, roomCoords.xMax),
                                       Mathf.Clamp(transform.position.y, roomCoords.yMin, roomCoords.yMax),
                                       transform.position.z);*/
        #endregion //Move
    }

    public void Initialize()
    {
        //Инициализация
        offsetPosition = new Vector3(offsetX, offsetY, offsetZ);
        camWindow = transform.FindChild("CamWindow");
        if (GameObject.FindGameObjectWithTag(Tags.player) != null)
        {
            character = GameObject.FindGameObjectWithTag(Tags.player);
            rigid = character.GetComponent<Rigidbody2D>();
        }
        if (transform.FindChild("AmbientLight")!= null)
        {
            ambLight = transform.FindChild("AmbientLight").gameObject;
        }
        currentArea = SpFunctions.GetCurrentArea();
        roomCoords = new Rect(currentArea.position.x - currentArea.size.x / 2 + sizeX / 2,
                    currentArea.position.y - currentArea.size.y / 2 + sizeY / 2,
                    currentArea.size.x - sizeX,
                    currentArea.size.y - sizeY);

        cam = GetComponent<Camera>();
        transform.position = new Vector3(transform.position.x, character.transform.position.y + offsetY, character.transform.position.z + offsetZ);
    }

    public void ChangeRoom(AreaClass prevArea, AreaClass nextArea)
    {
        if (nextArea.size.x == 0)
        {
            if (ambLight != null)
            {
                ambLight.SetActive(false);
            }
        }
        else
        {
            if (ambLight != null)
            {
                ambLight.SetActive(true);
            }
            Transform trans = character.transform;
            if (currentArea != nextArea)
            {
                currentArea = nextArea;
                Vector3 newPosition = new Vector3(trans.position.x, trans.position.y, nextArea.position.z+offsetZ);
                //deltaX = newPosition.x - transform.position.x;
                //deltaY = newPosition.y - transform.position.y;
                //Parallax(deltaX, deltaY);
                if (nextArea.position.z != prevArea.position.z)
                {
                    transform.position = newPosition;
                }
                roomCoords = new Rect(currentArea.position.x - currentArea.size.x / 2 + sizeX / 2,
                                    currentArea.position.y - currentArea.size.y / 2 + sizeY / 2,
                                    currentArea.size.x - sizeX,
                                    currentArea.size.y - sizeY);
            }
        }
    }

    void Parallax(float delX, float delY)
    {
        BackgroundClass background;
        Vector3 pos;
        for (int i = 0; i < backgroundList.Count; i++)
        {
            background = backgroundList[i];
            pos = background.background.transform.position;
            background.background.transform.position = new Vector3(pos.x - delX * background.prlxXScale, pos.y - delY * background.prlxYScale, pos.z);
        }
    }

    public void SetOffsetPosition(Vector2 _offset)
    {
        offsetPosition = Vector3.Lerp(offsetPosition,new Vector3(offsetX - _offset.x, offsetY - _offset.y, offsetZ),Time.deltaTime);
    }
}


/// <summary>
/// Специальный класс, используемый для создания бэкграунда и эффекта параллакса
/// </summary>
[System.Serializable]
public class BackgroundClass
{
    public string backgroundName;
    public GameObject background;
    public float prlxXScale, prlxYScale;//С какой относительной скоростью смещаются эти элементы при движении камеры (1 - с обычной скоростью, <1 - медленнее) 
}