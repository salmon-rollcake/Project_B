using UnityEngine;
using UnityEngine.UI;

namespace MyBird
{
    /// <summary>
    /// 플레이어의 이동, 점프, 회전을 관리하는 스크립트
    /// 대기(Ready) 상태와 게임 진행 상태를 구분하여 관리합니다.
    /// </summary>
    public class Player : MonoBehaviour
    {
        // ========== 상태 열거형 ==========
        /// <summary>
        /// 플레이어의 현재 상태를 나타냄
        /// Ready: 게임 시작 전 대기 상태 (중력 미적용, 고도 유지)
        /// Playing: 게임 진행 중인 상태 (정상 중력 적용)
        /// </summary>
        private enum PlayerState
        {
            Ready,    // 게임 시작 전 준비 상태
            Playing   // 게임 진행 중
        }

        // ========== Inspector 설정 변수 ==========
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float maxJumpRotation = 40f;
        [SerializeField] private float maxFallRotation = -90f;
        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private float moveSpeed = 5f;

        // ========== 내부 변수 ==========
        private Rigidbody2D rb;
        private float targetRotationZ;
        private PlayerState currentState = PlayerState.Ready;  // 초기 상태: Ready
        private bool canControl = true;                        // 플레이어 조작 가능 여부

        public GameObject readyUI;
        public GameObject gameoverUI;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            targetRotationZ = 0f;

            // Ready 상태에서 중력 비활성화
            rb.gravityScale = 0f;
        }

        private void Update()
        {
            HandleInput();
            HandleMovement();
            UpdateRotation();
        }

        /// <summary>
        /// 입력을 처리하고 상태를 관리합니다.
        /// </summary>
        private void HandleInput()
        {
            // 조작이 불가능하면 입력 처리 안 함
            if (!canControl)
                return;

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                // Ready 상태에서 입력받으면 Playing 상태로 전환
                if (currentState == PlayerState.Ready)
                {
                    StartGame();
                }
                else if (currentState == PlayerState.Playing)
                {
                    Jump();
                }
            }
        }

        /// <summary>
        /// 게임을 시작하고 Playing 상태로 전환합니다.
        /// </summary>
        private void StartGame()
        {
            currentState = PlayerState.Playing;
            // 중력 활성화 (일반적인 중력값으로 복원)
            rb.gravityScale = 1f;
            Jump(); // 첫 점프 실행
        }

        /// <summary>
        /// 좌우 이동을 관리합니다.
        /// Playing 상태에서만 수평 속도를 적용합니다.
        /// </summary>
        private void HandleMovement()
        {
            // 조작이 불가능하면 이동 중지
            if (!canControl)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                return;
            }

            if (currentState == PlayerState.Playing)
            {
                rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
            }
        }

        /// <summary>
        /// 점프를 실행합니다.
        /// </summary>
        private void Jump()
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        /// <summary>
        /// 현재 Y 속도에 따라 플레이어의 회전을 업데이트합니다.
        /// </summary>
        private void UpdateRotation()
        {
            // Ready 상태에서는 회전 적용 안 함
            if (currentState == PlayerState.Ready)
            {
                // 회전값을 0으로 초기화
                targetRotationZ = 0f;
            }
            else if (currentState == PlayerState.Playing)
            {
                // 현재 Y 속도로 회전 각도 결정
                if (rb.linearVelocity.y > 0)
                {
                    // 상승 중: +40도까지 회전
                    targetRotationZ = Mathf.Lerp(0f, maxJumpRotation, rb.linearVelocity.y / jumpForce);
                }
                else
                {
                    // 낙하 중: -90도까지 회전
                    targetRotationZ = Mathf.Lerp(0f, maxFallRotation, -rb.linearVelocity.y / (jumpForce * 2f));
                }
            }

            // 부드러운 회전
            Quaternion currentRotation = transform.rotation;
            float currentZ = currentRotation.eulerAngles.z;

            // eulerAngles가 0-360 범위이므로 -180~180 범위로 변환
            if (currentZ > 180f)
                currentZ -= 360f;

            currentZ = Mathf.Lerp(currentZ, targetRotationZ, Time.deltaTime * rotationSpeed);

            transform.rotation = Quaternion.Euler(
                transform.rotation.eulerAngles.x,
                transform.rotation.eulerAngles.y,
                currentZ
            );
        }

        /// <summary>
        /// 충돌 감지: Check 포인트와 Enemy 충돌체를 감지합니다.
        /// </summary>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            // GameManager 인스턴스가 있으면 충돌 이벤트 전달
            if (GameManager.Instance != null)
            {
                if (collision.CompareTag("Check"))
                {
                    GameManager.Instance.OnPlayerCheckCollision(collision.gameObject);
                }
                else if (collision.CompareTag("Enemy"))
                {
                    GameManager.Instance.OnPlayerEnemyCollision();
                }
            }
        }

        /// <summary>
        /// 게임 오버 상태로 플레이어를 설정합니다.
        /// 플레이어 조작을 불가능하게 하고 모든 이동 속도를 0으로 설정합니다.
        /// </summary>
        public void OnGameOver()
        {
            // 플레이어 조작 불가능 설정
            canControl = false;

            // 모든 이동 속도를 0으로 설정 (수평, 수직 모두)
            rb.linearVelocity = Vector2.zero;
        }
    }
}