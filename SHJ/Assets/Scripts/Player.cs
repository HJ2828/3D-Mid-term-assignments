using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // 스피드 조정 변수
    [SerializeField]
    private float walkSpeed;    // 속도
    [SerializeField]
    private float runSpeed;
    private float applySpeed;   // 걷거나, 달리거나 속도 넣어줌

    [SerializeField]
    private float jumpForce;

    // 상태 변수
    private bool isRun = false;
    private bool isGround = true;   // true일 경우에만 점프 가능하게

    // 땅 착지 여부
    private CapsuleCollider capsuleCollider;

    // 민감도
    [SerializeField]
    private float lookSensitivity;  // 카메라 민감도

    // 카메라 한계
    [SerializeField]
    private float cameraRotationLimit;  // 카메라 돌릴 때 각도
    private float currentCameraRotaionX = 0;

    // 필요한 컴포넌트
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
        //     theCamera = FindObjectOfType<Camera>(); // player 내에 있는 카메라 불러오기
        capsuleCollider = GetComponent<CapsuleCollider>();
        playerRb = GetComponent<Rigidbody>();
        applySpeed = walkSpeed; // 기본값은 걷는속도
                                //        jump = new Vector3(0f, jumph, 0f);
        anim = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        IsGround();
        TryJump();
        TryRun();   // 뛰는지 걷는지 확인 -> 반드시 Move() 앞에
        Move();
        CameraRotation();
        CharacterRotation();

    }
    
    private void IsGround()
    {
        Vector3 v = new Vector3(0, 1, 0);
        isGround = Physics.Raycast(transform.position+v, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
//        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
        // 어느위치에서 어느방향으로 어느거리만큼 레이저 쏘는 것
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
        playerRb.velocity = transform.up * jumpForce;   // 공중 방향으로 jumpForce만큼 위로 점프하게 만들거야 
            // 플레이어가 현재 움직이는 속도
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
        float _moveDirX = Input.GetAxisRaw("Horizontal");    // X: 좌우 / 오른쪽 방향키: 1, 왼쪽 방향키: -1
        float _moveDirZ = Input.GetAxisRaw("Vertical");  // Z: 정면,뒤

        Vector3 dir = new Vector3(_moveDirX, 0, _moveDirZ);     // 애니메이션 위해

        Vector3 _moveHorizontal = transform.right * _moveDirX;    // (1, 0, 0)이 Vector3에 기본적으로 있음. 이거에 *1(오른쪽 방향키:1) 하면 (1, 0, 0), + -1(왼쪽 방향키:-1)하면 (-1, 0, 0)
        Vector3 _moveVertical = transform.forward * _moveDirZ;
        
        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;  // (1,0,0)+(0,0,1)=(1,0,1)=2 인데 normalized하면 (0.5, 0, 0.5)=1됨

        playerRb.MovePosition(transform.position + _velocity * Time.deltaTime); // 1초동안 _velocity 만큼 움직이겠다

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
        // 좌우 캐릭터 회전
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _charcaterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        playerRb.MoveRotation(playerRb.rotation * Quaternion.Euler(_charcaterRotationY));    // 우리가 구한 벡터값을 Quternion으로 변경(유니티는 내부적으로 Quternion씀(보기 쉽게 xyz로 표현했지만..))
        
    }

    private void CameraRotation()
    {
        // 상하 카메라 회전
        float _xRotation = Input.GetAxisRaw("Mouse Y"); // 마우스는 2차원(Y는 위아래 -> 고개 들게)
        float _cameraRotationX = _xRotation * lookSensitivity;  // 어느정도 천천히 (확 올라가지 않게)
        currentCameraRotaionX -= _cameraRotationX;  // 마우스 위로 올리면 카메라도 위에 봐
        currentCameraRotaionX = Mathf.Clamp(currentCameraRotaionX, -cameraRotationLimit, cameraRotationLimit);   // 카메라 시야 각도 고정

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
