using UnityEngine;

namespace MyBird
{
    /// <summary>
    /// 메인 카메라가 플레이어를 추적하는 스크립트
    /// Y축은 고정하고 X축만 추적하며, X축에 오프셋을 적용합니다.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        // ========== Inspector 설정 변수 ==========
        [SerializeField] private Transform player;           // 추적할 플레이어의 Transform
        [SerializeField] private float smoothSpeed = 5f;     // 카메라 이동 부드러움 정도
        [SerializeField] private float offsetX = 1.5f;      // X축 오프셋 (음수면 플레이어 뒤쪽에 위치)

        // ========== 내부 변수 ==========
        private Vector3 initialPosition;                     // 카메라의 초기 위치 (Y, Z값 유지용)

        private void Start()
        {
            // 플레이어가 할당되지 않았으면 "Player" 태그로 자동 찾기
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player")?.transform;
            }

            initialPosition = transform.position;
        }

        private void LateUpdate()
        {
            if (player == null)
                return;

            // 플레이어의 X 위치에 오프셋을 적용하고, Y와 Z는 초기값 유지
            Vector3 targetPosition = new Vector3(
                player.position.x + offsetX,
                initialPosition.y,
                initialPosition.z
            );

            // 부드러운 카메라 이동
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
        }
    }
}
