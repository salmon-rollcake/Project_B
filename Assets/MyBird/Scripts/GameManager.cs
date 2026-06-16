using UnityEngine;
using System.Collections.Generic;

namespace MyBird
{
    /// <summary>
    /// 게임 전체를 관리하는 스크립트
    /// 충돌 감지, 점수 획득, 게임 오버 등을 처리합니다.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        // ========== 싱글톤 인스턴스 ==========
        public static GameManager Instance { get; private set; }

        // ========== 내부 변수 ==========
        private bool isGameOver = false;                           // 게임 오버 여부
        private HashSet<GameObject> scoredCheckPoints = new();     // 이미 점수를 획득한 Check 포인트들
        private SpawnPipe spawnPipeScript;                         // SpawnPipe 스크립트 참조
        private Rigidbody2D playerRigidbody;                       // 플레이어 Rigidbody2D 참조
        private Player playerScript;                               // 플레이어 스크립트 참조
        public int score = 0;

        private void Awake()
        {
            // 싱글톤 패턴 구현
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // SpawnPipe 스크립트 찾기
            spawnPipeScript = FindFirstObjectByType<SpawnPipe>();

            // 플레이어 Rigidbody2D와 Player 스크립트 찾기
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                playerRigidbody = playerObject.GetComponent<Rigidbody2D>();
                playerScript = playerObject.GetComponent<Player>();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // 게임이 이미 오버되었으면 추가 처리 안 함
            if (isGameOver)
                return;

            // ========== Check 태그 처리: 점수 획득 ==========
            if (collision.CompareTag("Check"))
            {
                // 해당 체크 포인트에서 아직 점수를 획득하지 않았다면
                if (!scoredCheckPoints.Contains(collision.gameObject))
                {
                    score++;
                    Debug.Log("점수 획득. 현재 점수 : " + score);
                    scoredCheckPoints.Add(collision.gameObject); // HashSet에 추가하여 중복 방지
                }
            }

            // ========== Enemy 태그 처리: 게임 오버 ==========
            if (collision.CompareTag("Enemy"))
            {
                Debug.Log("게임 오버");
                GameOver();
            }
        }

        /// <summary>
        /// Player에서 Check 포인트 충돌 시 호출합니다.
        /// </summary>
        public void OnPlayerCheckCollision(GameObject checkPoint)
        {
            if (isGameOver)
                return;

            if (!scoredCheckPoints.Contains(checkPoint))
            {
                score++;
                Debug.Log("점수 획득. 현재 점수 : " + score);
                scoredCheckPoints.Add(checkPoint);
            }
        }

        /// <summary>
        /// Player에서 Enemy 충돌 시 호출합니다.
        /// </summary>
        public void OnPlayerEnemyCollision()
        {
            if (isGameOver)
                return;

            Debug.Log("게임 오버");
            GameOver();
        }

        /// <summary>
        /// 게임 오버 처리
        /// 플레이어 조작 불가능, 모든 이동 속도 0, 장애물 생성 멈춤
        /// </summary>
        private void GameOver()
        {
            isGameOver = true;

            // 플레이어 조작 불가능하게 설정
            if (playerScript != null)
            {
                playerScript.OnGameOver();
            }
            // 플레이어 스크립트가 없으면 직접 속도 설정
            else if (playerRigidbody != null)
            {
                playerRigidbody.linearVelocity = Vector2.zero;
            }

            // SpawnPipe 스크립트 비활성화 (장애물 생성 멈춤)
            if (spawnPipeScript != null)
            {
                spawnPipeScript.enabled = false;
            }
        }
    }
}