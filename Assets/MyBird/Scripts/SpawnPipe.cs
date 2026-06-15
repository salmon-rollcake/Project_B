using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace MyBird
{
    /// <summary>
    /// 파이프 장애물을 생성 및 관리하는 스크립트
    /// 위아래 파이프 쌍을 시간 간격을 두고 생성하며,
    /// 높이는 스폰 포인트를 기준으로 랜덤하게 조절됩니다.
    /// </summary>
    public class SpawnPipe : MonoBehaviour
    {
        // ========== Inspector 설정 변수 ==========
        [SerializeField] private GameObject pipePrefab;           // 파이프 프리팹 (위아래 파이프 포함)
        [SerializeField] private Transform spawnPoint;            // 파이프 생성 기준점 (Y축 중심)
        [SerializeField] private float minSpawnInterval = 1.5f;   // 최소 생성 간격 (초)
        [SerializeField] private float maxSpawnInterval = 3.0f;   // 최대 생성 간격 (초)
        [SerializeField] private float heightRange = 2.0f;        // 높이 범위 (spawnPoint Y값 기준 ±)
        [SerializeField] private float spawnDistance = 30f;       // 화면 오른쪽에서 얼마나 떨어진 곳에서 생성할지
        [SerializeField] private float despawnDistance = -10f;    // 화면 왼쪽에서 얼마나 떨어진 곳에서 제거할지

        // ========== 내부 변수 ==========
        private Queue<GameObject> pipeQueue = new Queue<GameObject>(); // 생성된 파이프들을 순서대로 관리
        private Camera mainCamera;                                     // 메인 카메라 참조
        private float nextSpawnTime;                                   // 다음 파이프 생성 시간
        private float nextSpawnX;                                      // 다음 파이프 생성 X 위치

        private void Start()
        {
            // 메인 카메라 참조
            mainCamera = Camera.main;

            // 스폰 포인트가 할당되지 않았으면 자동 찾기
            if (spawnPoint == null)
            {
                spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint")?.transform;
            }

            // 필수 요소 확인
            if (pipePrefab != null && mainCamera != null && spawnPoint != null)
            {
                // 첫 번째 생성 시간 설정
                nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);

                // 다음 생성 X 위치 초기화
                float cameraRightEdge = mainCamera.ViewportToWorldPoint(new Vector3(1, 0.5f, 10)).x;
                nextSpawnX = cameraRightEdge + spawnDistance;
            }
        }

        private void Update()
        {
            // 필수 요소 확인
            if (pipePrefab == null || mainCamera == null || spawnPoint == null)
                return;

            // ========== 파이프 생성 ==========
            // 설정된 시간 간격으로 파이프 생성
            if (Time.time >= nextSpawnTime)
            {
                SpawnNewPipe();
                // 다음 생성 시간 설정 (랜덤 간격)
                nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
            }

            // 카메라의 왼쪽 끝 위치 계산
            float cameraLeftEdge = mainCamera.ViewportToWorldPoint(Vector3.zero).x;

            // ========== 불필요한 파이프 제거 ==========
            // 화면 왼쪽으로 완전히 벗어난 파이프를 제거하여 메모리 절약
            while (pipeQueue.Count > 0 && pipeQueue.Peek().transform.position.x < cameraLeftEdge + despawnDistance)
            {
                GameObject oldPipe = pipeQueue.Dequeue(); // Queue에서 제거
                Destroy(oldPipe);                         // 오브젝트 파괴
            }
        }

        /// <summary>
        /// 새로운 파이프 쌍을 생성하고 Queue에 추가합니다.
        /// </summary>
        private void SpawnNewPipe()
        {
            // 스폰 포인트의 Y값을 기준으로 높이를 랜덤하게 결정
            float randomHeightOffset = Random.Range(-heightRange, heightRange);
            float spawnY = spawnPoint.position.y + randomHeightOffset;

            // 파이프 프리팹을 인스턴스화
            GameObject newPipe = Instantiate(pipePrefab);

            // 파이프의 위치 설정 (X는 nextSpawnX, Y는 랜덤 높이, Z는 프리팹의 원래 위치 유지)
            newPipe.transform.position = new Vector3(
                nextSpawnX,
                spawnY,
                pipePrefab.transform.position.z
            );

            // 생성된 파이프를 Queue에 추가 (순서대로 관리)
            pipeQueue.Enqueue(newPipe);

            // 다음 생성 X 위치 업데이트 (적절한 간격 유지)
            nextSpawnX += 15f; // 파이프 간 거리 (필요시 변수로 변경 가능)
        }
    }
}