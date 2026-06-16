using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MyBird
{
    /// <summary>
    /// 게임 플레이 중 점수 등 UI를 관리하는 스크립트
    /// GameManager와 연동하여 실시간으로 점수를 표시합니다.
    /// </summary>
    public class PlayUI : MonoBehaviour
    {
        // ========== Inspector 설정 변수 ==========
        [SerializeField] private TextMeshProUGUI scoreText;        // 점수를 표시할 Text 컴포넌트 (TextMeshPro)

        // ========== 내부 변수 ==========
        private GameManager gameManager;                           // GameManager 참조
        private int lastDisplayedScore = -1;                       // 마지막으로 표시한 점수 (변화 감지용)

        private void Start()
        {
            // GameManager 싱글톤 참조
            gameManager = GameManager.Instance;

            // scoreText가 할당되지 않았으면 자동으로 찾기
            if (scoreText == null)
            {
                scoreText = GetComponent<TextMeshProUGUI>();
                if (scoreText == null)
                {
                    scoreText = FindFirstObjectByType<TextMeshProUGUI>();
                }
            }

            // 초기 점수 표시
            UpdateScoreDisplay();
        }

        private void Update()
        {
            // 매 프레임마다 점수 변화를 확인하고 UI 업데이트
            UpdateScoreDisplay();
        }

        /// <summary>
        /// 점수 표시를 업데이트합니다.
        /// 점수가 변경되었을 때만 텍스트를 갱신하여 효율성을 높입니다.
        /// </summary>
        private void UpdateScoreDisplay()
        {
            if (gameManager == null || scoreText == null)
                return;

            // 현재 점수가 마지막 표시 점수와 다르면 업데이트
            if (gameManager.score != lastDisplayedScore)
            {
                lastDisplayedScore = gameManager.score;
                scoreText.text = gameManager.score.ToString();
            }
        }

        /// <summary>
        /// 외부에서 점수 업데이트를 호출할 때 사용합니다.
        /// (GameManager의 공개 메서드로 호출 가능)
        /// </summary>
        public void OnScoreChanged()
        {
            UpdateScoreDisplay();
        }
    }
}