using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // ���ǵ� ���� ����
    [SerializeField]
    private float walkSpeed;    // �ӵ�
    [SerializeField]
    private float runSpeed;
    private float applySpeed;   // �Ȱų�, �޸��ų� �ӵ� �־���

    [SerializeField]
    private float jumpForce;

    // ���� ����
    private bool isRun = false;
    private bool isGround = true;   // true�� ��쿡�� ���� �����ϰ�

    // �� ���� ����
    private CapsuleCollider capsuleCollider;

    // �ΰ���
    [SerializeField]
    private float lookSensitivity;  // ī�޶� �ΰ���

    // ī�޶� �Ѱ�
    [SerializeField]
    private float cameraRotationLimit;  // ī�޶� ���� �� ����
    private float currentCameraRotaionX = 0;

    // �ʿ��� ������Ʈ
    [SerializeField]
    private Camera theCamera;

    private Rigidbody playerRb;

    private float x = 2, y = 2, z = 2;

    Animator anim;

    public GameObject key;

    public GameObject txt;
    public GameObject btn;

    // Start is called before the first frame update
    void Start()
    {
        //     theCamera = FindObjectOfType<Camera>(); // player ���� �ִ� ī�޶� �ҷ�����
        capsuleCollider = GetComponent<CapsuleCollider>();
        playerRb = GetComponent<Rigidbody>();
        applySpeed = walkSpeed; // �⺻���� �ȴ¼ӵ�
                                //        jump = new Vector3(0f, jumph, 0f);
        anim = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        IsGround();
        TryJump();
        TryRun();   // �ٴ��� �ȴ��� Ȯ�� -> �ݵ�� Move() �տ�
        Move();
        CameraRotation();
        CharacterRotation();

    }
    
    private void IsGround()
    {
        Vector3 v = new Vector3(0, 1, 0);
        isGround = Physics.Raycast(transform.position+v, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
//        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
        // �����ġ���� ����������� ����Ÿ���ŭ ������ ��� ��
    }
    
    private void TryJump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGround == true)
        {
            Jump();
        }
    }

    private void Jump()
    {
        playerRb.velocity = transform.up * jumpForce;   // ���� �������� jumpForce��ŭ ���� �����ϰ� ����ž� 
            // �÷��̾ ���� �����̴� �ӵ�
    }

    private void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Running();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            RunningCancle();
        }
    }

    private void Running()
    {
        isRun = true;
        applySpeed = runSpeed;
    }

    private void RunningCancle()
    {
        isRun = false;
        applySpeed = walkSpeed;
    }

    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal");    // X: �¿� / ������ ����Ű: 1, ���� ����Ű: -1
        float _moveDirZ = Input.GetAxisRaw("Vertical");  // Z: ����,��

        Vector3 dir = new Vector3(_moveDirX, 0, _moveDirZ);     // �ִϸ��̼� ����

        Vector3 _moveHorizontal = transform.right * _moveDirX;    // (1, 0, 0)�� Vector3�� �⺻������ ����. �̰ſ� *1(������ ����Ű:1) �ϸ� (1, 0, 0), + -1(���� ����Ű:-1)�ϸ� (-1, 0, 0)
        Vector3 _moveVertical = transform.forward * _moveDirZ;
        
        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;  // (1,0,0)+(0,0,1)=(1,0,1)=2 �ε� normalized�ϸ� (0.5, 0, 0.5)=1��

        playerRb.MovePosition(transform.position + _velocity * Time.deltaTime); // 1�ʵ��� _velocity ��ŭ �����̰ڴ�

        if (dir.x != 0 || dir.y != 0 || dir.z != 0)
        {
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }
    }

    private void CharacterRotation()
    {
        // �¿� ĳ���� ȸ��
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _charcaterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        playerRb.MoveRotation(playerRb.rotation * Quaternion.Euler(_charcaterRotationY));    // �츮�� ���� ���Ͱ��� Quternion���� ����(����Ƽ�� ���������� Quternion��(���� ���� xyz�� ǥ��������..))
        
    }

    private void CameraRotation()
    {
        // ���� ī�޶� ȸ��
        float _xRotation = Input.GetAxisRaw("Mouse Y"); // ���콺�� 2����(Y�� ���Ʒ� -> �� ���)
        float _cameraRotationX = _xRotation * lookSensitivity;  // ������� õõ�� (Ȯ �ö��� �ʰ�)
        currentCameraRotaionX -= _cameraRotationX;  // ���콺 ���� �ø��� ī�޶� ���� ��
        currentCameraRotaionX = Mathf.Clamp(currentCameraRotaionX, -cameraRotationLimit, cameraRotationLimit);   // ī�޶� �þ� ���� ����

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotaionX, 0f, 0f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Small_Item")
        {
            
            x = x - 0.4f;
            y = y - 0.4f;
            z = z - 0.4f;

            transform.localScale = new Vector3(x, y, z);
            walkSpeed = 2;
            jumpForce = 4;

            collision.gameObject.SetActive(false);
        }

        if (collision.gameObject.tag == "Big_Item")
        {
            x = x + 0.4f;
            y = y + 0.4f;
            z = z + 0.4f;

            transform.localScale = new Vector3(x, y, z);
            walkSpeed = 6;
            jumpForce = 5;

            collision.gameObject.SetActive(false);
        }

        if(collision.gameObject.tag == "Key")
        {
            collision.gameObject.SetActive(false);
            key.SetActive(true);
        }

        if (collision.gameObject.tag == "UIsetAcitve")
        {
            txt.SetActive(true);
            btn.SetActive(true);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.tag == "UIsetAcitve")
        {
            txt.SetActive(false);
            btn.SetActive(false);
        }
    }

}
