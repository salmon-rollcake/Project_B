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
            spawnPipeScript = FindObjectOfType<SpawnPipe>();
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
                    Debug.Log("점수 획득");
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
        /// 게임 오버 처리
        /// </summary>
        private void GameOver()
        {
            isGameOver = true;

            // 게임 시간 정지 (Time.timeScale = 0으로 모든 Update 멈춤)
            Time.timeScale = 0f;

            // SpawnPipe 스크립트 비활성화 (장애물 생성 멈춤)
            if (spawnPipeScript != null)
            {
                spawnPipeScript.enabled = false;
            }
        }
    }
}