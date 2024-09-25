using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayetController : MonoBehaviour
{
    //플레이어의 움직임 속도를 설정하는 변수
    [Header("Player Movement")]
    public float moveSpeed = 5.0f;  //이동 속도
    public float jumpForce = 5.0f;  //점프 힘

    //카메라 설정 변수
    [Header("Camera Settings")]
    public Camera firstPersonCamera;            //1인칭 카메라
    public Camera thirdCamera;                  //3인칭 카메라
    public float mouseSenesitivity = 2.0f;      //마우스 감도

    public float radius = 5.0f;         //3인칭 카메라와 플레이어 간의 거리
    public float yMinLimit = -90;       //카메라 수직 회전 최소각
    public float yMaxLimit = 90;        //카메라 수직 회전 최대각

    private float theta = 0.0f;         //카메라의 수평 회전 각도
    private float phi = 0.0f;           //카메라의 수직 회전 각도
    private float targetVerticalRotation = 0.0f; //목표 수직 회전 각도
    private float verticalRoatationSpeed = 240f;    //수직 회전 각도

    //내부 변수들
    private bool isFirstPerson = true;  //1인칭 모드 인지 여부
    private bool isGrounded;    //플레이어가 땅에 있는지 여부
    private Rigidbody rb;       //플레이어의 Rigidbody

    void Start()
    {
        rb = GetComponent<Rigidbody>();     //rigidbody 컴포넌트를 가져온다.

        Cursor.lockState = CursorLockMode.Locked;   //마우스 커서를 잠그고 숨긴다.
        SetupCameras();
        SetActiveCamera();
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleRotation();
    }

    

    //카메라 초기 위치 및 회전을 설정하는 함수
    void SetupCameras()
    {
        firstPersonCamera.transform.localPosition = new Vector3(0.0f, 0.6f, 0.0f);      //1인칭 카메라 위치
        firstPersonCamera.transform.localRotation = Quaternion.identity;                //1인칭 카메라 회전 초기화
    }

    //카메라 및 캐릭터 회전 처리하는 함수
    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSenesitivity;        //마우스 좌우입력
        float mouseY = Input.GetAxis("Mouse Y") * mouseSenesitivity;        //마우스 상하입력

        //수평 회전(theta)값
        theta += mouseX;    //마우스 입력값 추가
        theta = Mathf.Repeat(theta, 360.0f);    //각도 값이 360을 넘지 않도록 조정 (0~360 | 361 -> 1)

        //수직 회전 처리
        targetVerticalRotation -= mouseY;
        targetVerticalRotation = Mathf.Clamp(targetVerticalRotation,yMinLimit,yMaxLimit); //수직 회전 제한
        phi = Mathf.MoveTowards(phi, targetVerticalRotation,  verticalRoatationSpeed * Time.deltaTime);

        //플레이어 회전 (캐릭터가 수평으로만 회전)
        transform.rotation = Quaternion.Euler(0.0f, theta, 0.0f);

        firstPersonCamera.transform.localRotation = Quaternion.Euler(phi, 0.0f, 0.0f);  //1인칭 카메라 수직 회전
    }

    //플레이어 점프를 처리하는 함수
    void HandleJump()
    {
        //점프 버튼을 누르고 땅에 있을때
        if (Input.GetKeyUp(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);     //위쪽으로 힘을 가해 점프
            isGrounded = false;
        }
    }

    //플레이어의 이동을 처리하는 함수
    void HandleMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");     //좌우 입력 (-1 ~ 1)
        float moveVertical = Input.GetAxis("Vertical");         //앞뒤 입력 (1 ~ -1)

        //캐릭터 기준으로 이동
        Vector3 movement = transform.right * moveHorizontal + transform.forward * moveVertical;
        rb.MovePosition(rb.position + movement * moveSpeed * Time.deltaTime);   //물리기반 이동
    }


    //플레이어가 땅에 닿아있는지 감지
    private void OnCollisionStay(Collision collision)
    {
        isGrounded = true;      //충돌 중이면 플레이어는 땅에 있다.
    }
}