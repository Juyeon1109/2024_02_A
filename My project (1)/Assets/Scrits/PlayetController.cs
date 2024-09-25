using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayetController : MonoBehaviour
{
    //�÷��̾��� ������ �ӵ��� �����ϴ� ����
    [Header("Player Movement")]
    public float moveSpeed = 5.0f;  //�̵� �ӵ�
    public float jumpForce = 5.0f;  //���� ��

    //ī�޶� ���� ����
    [Header("Camera Settings")]
    public Camera firstPersonCamera;            //1��Ī ī�޶�
    public Camera thirdCamera;                  //3��Ī ī�޶�
    public float mouseSenesitivity = 2.0f;      //���콺 ����

    public float radius = 5.0f;         //3��Ī ī�޶�� �÷��̾� ���� �Ÿ�
    public float yMinLimit = -90;       //ī�޶� ���� ȸ�� �ּҰ�
    public float yMaxLimit = 90;        //ī�޶� ���� ȸ�� �ִ밢

    private float theta = 0.0f;         //ī�޶��� ���� ȸ�� ����
    private float phi = 0.0f;           //ī�޶��� ���� ȸ�� ����
    private float targetVerticalRotation = 0.0f; //��ǥ ���� ȸ�� ����
    private float verticalRoatationSpeed = 240f;    //���� ȸ�� ����

    //���� ������
    private bool isFirstPerson = true;  //1��Ī ��� ���� ����
    private bool isGrounded;    //�÷��̾ ���� �ִ��� ����
    private Rigidbody rb;       //�÷��̾��� Rigidbody

    void Start()
    {
        rb = GetComponent<Rigidbody>();     //rigidbody ������Ʈ�� �����´�.

        Cursor.lockState = CursorLockMode.Locked;   //���콺 Ŀ���� ��װ� �����.
        SetupCameras();
        SetActiveCamera();
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleRotation();
    }

    

    //ī�޶� �ʱ� ��ġ �� ȸ���� �����ϴ� �Լ�
    void SetupCameras()
    {
        firstPersonCamera.transform.localPosition = new Vector3(0.0f, 0.6f, 0.0f);      //1��Ī ī�޶� ��ġ
        firstPersonCamera.transform.localRotation = Quaternion.identity;                //1��Ī ī�޶� ȸ�� �ʱ�ȭ
    }

    //ī�޶� �� ĳ���� ȸ�� ó���ϴ� �Լ�
    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSenesitivity;        //���콺 �¿��Է�
        float mouseY = Input.GetAxis("Mouse Y") * mouseSenesitivity;        //���콺 �����Է�

        //���� ȸ��(theta)��
        theta += mouseX;    //���콺 �Է°� �߰�
        theta = Mathf.Repeat(theta, 360.0f);    //���� ���� 360�� ���� �ʵ��� ���� (0~360 | 361 -> 1)

        //���� ȸ�� ó��
        targetVerticalRotation -= mouseY;
        targetVerticalRotation = Mathf.Clamp(targetVerticalRotation,yMinLimit,yMaxLimit); //���� ȸ�� ����
        phi = Mathf.MoveTowards(phi, targetVerticalRotation,  verticalRoatationSpeed * Time.deltaTime);

        //�÷��̾� ȸ�� (ĳ���Ͱ� �������θ� ȸ��)
        transform.rotation = Quaternion.Euler(0.0f, theta, 0.0f);

        firstPersonCamera.transform.localRotation = Quaternion.Euler(phi, 0.0f, 0.0f);  //1��Ī ī�޶� ���� ȸ��
    }

    //�÷��̾� ������ ó���ϴ� �Լ�
    void HandleJump()
    {
        //���� ��ư�� ������ ���� ������
        if (Input.GetKeyUp(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);     //�������� ���� ���� ����
            isGrounded = false;
        }
    }

    //�÷��̾��� �̵��� ó���ϴ� �Լ�
    void HandleMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");     //�¿� �Է� (-1 ~ 1)
        float moveVertical = Input.GetAxis("Vertical");         //�յ� �Է� (1 ~ -1)

        //ĳ���� �������� �̵�
        Vector3 movement = transform.right * moveHorizontal + transform.forward * moveVertical;
        rb.MovePosition(rb.position + movement * moveSpeed * Time.deltaTime);   //������� �̵�
    }


    //�÷��̾ ���� ����ִ��� ����
    private void OnCollisionStay(Collision collision)
    {
        isGrounded = true;      //�浹 ���̸� �÷��̾�� ���� �ִ�.
    }
}