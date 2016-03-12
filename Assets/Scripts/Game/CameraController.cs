using UnityEngine;
using System.Collections;


/// <summary>
/// Скрипт, которым я задаю поведение камеры, её эффекты, следование за персонажем и все механики движения
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
    private Rigidbody2D rigid;
    private Vector2 camSize = new Vector2 (sizeX,sizeY);//размер камеры, используемый для подсчёта нормального движения внутри комнаты
    private Transform camWindow;//Окошечко, выйдя за пределы которого персонаж двигает камеру.
    private AreaClass currentArea;
    private Camera cam;
    public Rect roomCoords;
    public int movX = 0, movY=0;//Есть ли у камеры ограничения на передвижение вдоль направление движущегося персонажа
    public GameObject g;
    public Vector3 vect;
    #endregion //parametres

    public void Start()
    {
        //Инициализация
        camWindow = transform.FindChild("CamWindow");
        if (GameObject.FindGameObjectWithTag(Tags.player) != null)
        {
            character = GameObject.FindGameObjectWithTag(Tags.player);
            rigid = character.GetComponent<Rigidbody2D>();
        }
        currentArea = SpFunctions.GetCurrentArea();
        roomCoords = new Rect(currentArea.position.x - currentArea.size.x / 2+sizeX/2,
                    currentArea.position.y - currentArea.size.y / 2+sizeY/2,
                    currentArea.size.x-sizeX,
                    currentArea.size.y-sizeY);

        cam = GetComponent<Camera>();
        transform.position = new Vector3(transform.position.x, character.transform.position.y+offsetY, character.transform.position.z + offsetZ);
    }

    public void FixedUpdate()
    {
        vect = cam.WorldToViewportPoint(g.transform.position);
        Vector2 spotPosition = new Vector2(transform.position.x + offsetX, transform.position.y + offsetY);
        #region howToHorizontalMove 
        if ((character.transform.position.x < camWindow.position.x - camWindow.localScale.x / 2)
            && (spotPosition.x > currentArea.position.x - (currentArea.size.x - camSize.x) / 2))
        {
            movX = (int)camMovX.movLeft;
        }
        else if ((character.transform.position.x > camWindow.position.x + camWindow.localScale.x / 2)
           && (spotPosition.x < currentArea.position.x + (currentArea.size.x - camSize.x) / 2))
        {
            movX = (int)camMovX.movRight;
        }
        else
        {
            movX = (int)camMovX.stop;
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
        transform.position = new Vector3(Mathf.Clamp(transform.position.x + movX * (distance.x - camWindow.localScale.x/2),roomCoords.xMin,roomCoords.xMax),
                                         Mathf.Clamp(transform.position.y + movY * distance.y,roomCoords.yMin,roomCoords.yMax),
                                         transform.position.z);
        /*transform.position = Vector3.Lerp(transform.position, newPosition, camSpeed*Time.deltaTime);
        transform.position=new Vector3(Mathf.Clamp(transform.position.x, roomCoords.xMin, roomCoords.xMax),
                                       Mathf.Clamp(transform.position.y, roomCoords.yMin, roomCoords.yMax),
                                       transform.position.z);*/
        #endregion //Move
    }

    public void ChangeRoom()
    {
        Transform trans = character.transform;
        currentArea = SpFunctions.GetCurrentArea();
        transform.position = new Vector3(currentArea.position.x,currentArea.position.y,trans.position.z+offsetZ);
        roomCoords = new Rect(currentArea.position.x - currentArea.size.x / 2+sizeX/2,
                            currentArea.position.y - currentArea.size.y / 2+sizeY/2,
                            currentArea.size.x-sizeX,
                            currentArea.size.y-sizeY);
    }

}
