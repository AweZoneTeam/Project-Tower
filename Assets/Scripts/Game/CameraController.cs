using UnityEngine;
using System.Collections;


/// <summary>
/// Скрипт, которым я задаю поведение камеры, её эффекты, следование за персонажем и все механики движения
/// </summary>
public class CameraController : MonoBehaviour
{

    #region consts
    const float distanceToChar = -26.5f;
    const float sizeX= 81f;
    const float sizeY = 43f;
    const float fieldOfView = 87f;
    #endregion //consts

    #region enums
    public enum camMovX {movLeft=-2, stopLeft=-1, none=0, stopRight=1, movRight=2};
    public enum camMovY {movDown=-2,stopDown=-1, none=0, stopUp=1, movUp=2 };
    #endregion //enums

    #region parametres
    private GameObject character;//За каким персонажем камера следует
    private Rigidbody2D rigid;
    private Vector2 camSize = new Vector2 (sizeX,sizeY);//размер камеры, используемый для подсчёта нормального движения внутри комнаты
    private Transform camWindow;//Окошечко, выйдя за пределы которого персонаж двигает камеру.
    private AreaClass currentArea;
    private int movX = 0, movY=0;//Есть ли у камеры ограничения на передвижение вдоль направление движущегося персонажа
    #endregion //parametres

    public void Start()
    {
        //Инициализация
        camWindow = transform.FindChild("MoveRoom");
        if (GameObject.FindGameObjectWithTag(Tags.player) != null)
        {
            character = GameObject.FindGameObjectWithTag(Tags.player);
            rigid = character.GetComponent<Rigidbody2D>();
        }
        currentArea = SpFunctions.GetCurrentArea();
    }

    public void FixedUpdate()
    {
        if (rigid.velocity.x > 0)
        {
            if (character.transform.position.x > camWindow.position.x + camWindow.localScale.x)
            {
                movX = (int)camMovX.movRight;
            }
        }
    }

}
