using UnityEngine;
using UnityEngine.UI;

public class CameraMoveTest : MonoBehaviour
{
    [Header("카메라 이동 포인트")]
    public Transform pointFloor1;
    public Transform pointFloor2;
    public float moveSpeed = 2.5f;

    [Header("카메라 확대 설정")]
    public float fovFull = 60f;   // 전체 보기
    public float fovZoom = 40f;   // 확대 보기
    public float fovLerpSpeed = 3f;

    [Header("버튼 연결")]
    public Button btn1F;
    public Button btn2F;
    public Color normalColor = Color.white;
    public Color selectedColor = new Color(0.85f, 0.85f, 0.85f);

    private Transform targetPoint;
    private Camera mainCam;
    private float targetFov;
    private bool isMoving = false;

    void Start()
    {
        mainCam = GetComponent<Camera>();
        // 시작 시 target 지정 안 함 (자동 이동 방지)
        targetFov = fovFull;
    }

    void Update()
    {
        if (isMoving && targetPoint != null)
        {
            // 위치 보간 이동
            transform.position = Vector3.Lerp(transform.position, targetPoint.position, Time.deltaTime * moveSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetPoint.rotation, Time.deltaTime * moveSpeed);
            mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, targetFov, Time.deltaTime * fovLerpSpeed);
        }
    }

    public void MoveToFloor1()
    {
        targetPoint = pointFloor1;
        targetFov = fovZoom;
        isMoving = true;
        ChangeButtonColor(btn1F, btn2F);
        Debug.Log("1층 시점으로 확대 이동");
    }

    public void MoveToFloor2()
    {
        targetPoint = pointFloor2;
        targetFov = fovZoom;
        isMoving = true;
        ChangeButtonColor(btn2F, btn1F);
        Debug.Log("2층 시점으로 확대 이동");
    }

    void ChangeButtonColor(Button selected, Button unselected)
    {
        var sel = selected.colors;
        sel.normalColor = selectedColor;
        selected.colors = sel;

        var unsel = unselected.colors;
        unsel.normalColor = normalColor;
        unselected.colors = unsel;
    }
}


