using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace MyBird
{
    /// <summary>
    /// 무한 스크롤 바닥 시스템을 관리하는 스크립트
    /// 카메라 뷰포트를 기준으로 바닥을 동적으로 생성하고 제거하여
    /// 무한으로 앞으로 나아가는 느낌을 제공합니다.
    /// 바닥들은 자동으로 서로 이어붙고, 왼쪽 밖으로 벗어난 바닥은 제거됩니다.
    /// </summary>
    public class GroundRoll : MonoBehaviour
    {
        // ========== Inspector 설정 변수 ==========
        [SerializeField] private GameObject groundPrefab;      // 바닥 프리팹 (스폰할 오브젝트)
        [SerializeField] private float groundWidth = 10f;      // 한 개 바닥의 너비
        [SerializeField] private float groundOffsetY = 0f;     // 바닥의 Y 위치 오프셋
        [SerializeField] private float spawnMargin = 20f;      // 화면 오른쪽에서 얼마나 떨어진 곳에서 생성할지
        [SerializeField] private float despawnMargin = -10f;   // 화면 왼쪽에서 얼마나 떨어진 곳에서 제거할지

        // ========== 내부 변수 ==========
        private Queue<GameObject> groundQueue = new Queue<GameObject>(); // 생성된 바닥들을 순서대로 관리
        private Camera mainCamera;                                        // 메인 카메라 참조

        /// <summary>
        /// 초기화: 카메라 참조 및 초기 바닥들 생성
        /// </summary>
        private void Start()
        {
            // 메인 카메라 참조
            mainCamera = Camera.main;

            // 바닥 프리팹과 카메라가 있을 경우만 초기화 진행
            if (groundPrefab != null && mainCamera != null)
            {
                // 카메라의 왼쪽 끝 위치 계산
                float cameraLeftEdge = mainCamera.ViewportToWorldPoint(Vector3.zero).x;

                // 초기 바닥들을 카메라 뷰를 기준으로 생성 (화면 왼쪽에서 시작)
                float nextX = cameraLeftEdge;
                for (int i = 0; i < 5; i++)
                {
                    SpawnGround(nextX);
                    nextX += groundWidth;
                }
            }
        }

        /// <summary>
        /// 매 프레임 업데이트: 바닥 생성 및 제거 로직
        /// </summary>
        private void Update()
        {
            // 필수 요소 점검 (프리팹 없음 또는 카메라 없으면 작동 불가)
            if (groundPrefab == null || mainCamera == null)
                return;

            // 카메라의 오른쪽 끝 위치 계산 (Z 거리 10은 카메라의 월드 좌표 변환을 위함)
            float cameraRightEdge = mainCamera.ViewportToWorldPoint(new Vector3(1, 0.5f, 10)).x;

            // ========== 새로운 바닥 생성 ==========
            // 가장 오른쪽에 있는 바닥이 화면 오른쪽 경계(spawnMargin)에 가까워지면 새 바닥 생성
            // 이렇게 하면 바닥끼리 자동으로 이어붙음
            while (groundQueue.Count > 0 && groundQueue.Last().transform.position.x + groundWidth < cameraRightEdge + spawnMargin)
            {
                // 마지막 바닥의 오른쪽 끝에 새 바닥 생성
                float newX = groundQueue.Last().transform.position.x + groundWidth;
                SpawnGround(newX);
            }

            // 카메라의 왼쪽 끝 위치 계산
            float cameraLeftEdge = mainCamera.ViewportToWorldPoint(Vector3.zero).x;

            // ========== 불필요한 바닥 제거 ==========
            // 가장 왼쪽에 있는 바닥이 화면 왼쪽 경계(despawnMargin)를 완전히 넘어서면 제거
            // 이렇게 하면 메모리 효율을 높이면서도 필요한 바닥들만 유지함
            while (groundQueue.Count > 0 && groundQueue.Peek().transform.position.x + groundWidth < cameraLeftEdge + despawnMargin)
            {
                GameObject oldGround = groundQueue.Dequeue(); // Queue에서 제거
                Destroy(oldGround);                           // 오브젝트 파괴
            }
        }

        /// <summary>
        /// 새로운 바닥을 생성하고 Queue에 추가
        /// </summary>
        /// <param name="posX">생성할 바닥의 X 위치</param>
        private void SpawnGround(float posX)
        {
            // 프리팹을 인스턴스화
            GameObject newGround = Instantiate(groundPrefab);

            // 바닥의 위치를 설정 (X는 파라미터 값, Y는 오프셋 적용, Z는 프리팹의 원래 위치 유지)
            newGround.transform.position = new Vector3(
                posX,
                groundOffsetY,
                groundPrefab.transform.position.z
            );

            // 생성된 바닥을 Queue에 추가 (순서대로 관리)
            groundQueue.Enqueue(newGround);
        }
    }
}

