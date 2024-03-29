using UnityEngine;
using TMPro;

namespace IMTC505.starter.SampleGame
{    
    public class GameManager : MonoBehaviour
    {
        [Tooltip("Distance to move before game starts.")]
        [Range(0.0f, 5f)]
        [SerializeField]
        private float startDistance;
        [Tooltip("Game time limit in seconds.")]
        [SerializeField]
        private int timeLimit = 120;

        [Tooltip("Points text.")]
        [SerializeField]
        private TextMeshProUGUI pointsText;
        [Tooltip("Time text.")]
        [SerializeField]
        private TextMeshProUGUI timeText;

        private enum GameState
            {
                initiated,
                stopped,
                started
            };
        
        private Transform _characterTransform;
        private GameState _gameState;
        private Vector3 _startPosition;
        private float _gameStartTime;
        private float _timeRemaining;
        private float _points;
        private float _maxPoints;

        // Start is called before the first frame update
        void Start()
        {
            CharacterController[] characterControllers = FindObjectsOfType<CharacterController>();
            if (characterControllers.Length != 1)
            {
                Debug.LogError("Expecting only one `CharacterController`");
            }

            // Setting up canvas and tracking the character controller.
            _characterTransform = characterControllers[0].transform;
            timeText.text = "Move to start";
            pointsText.text = "";
        }

        /// <summary>
        /// Update the points and clear object which scored the points. Used as a callback with the `GamePoint' objects.
        /// </summary>
        private void OnPointScored(GamePoint gamePoint)
        {
            _points += gamePoint.points;
            Destroy(gamePoint.gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            switch (_gameState)
            {
                case GameState.initiated:
                    // Setup when game-level starts
                    // The timer/game-level is not started until the character moves by startDistance
                    if ((_startPosition - _characterTransform.transform.position).magnitude > startDistance)
                    {
                        Debug.Log($"Game Started");
                        _gameState = GameState.started;
                        _gameStartTime = Time.fixedTime;
                        _timeRemaining = timeLimit;

                        foreach (GamePoint gamePoint in FindObjectsOfType<GamePoint>())
                        {
                            gamePoint.OnTriggerEnterAction += OnPointScored;
                            _maxPoints += gamePoint.points;
                        }
                    }
                    break;
                case GameState.started:
                    // Game code while game-level is running
                    
                    // Calculate the remaining time
                    _timeRemaining = timeLimit - (Time.fixedTime - _gameStartTime);

                    // Update the canvas text
                    timeText.text = $"Time remaining: {Mathf.FloorToInt(_timeRemaining / 60)}:{_timeRemaining % 60.0f}";
                    pointsText.text = $"Points: {_points}/{_maxPoints}";

                    // Decide if the game-level should end
                    if (_timeRemaining <= 0 || _points == _maxPoints)
                    {
                        _gameState = GameState.stopped;
                        if (_timeRemaining <= 0)
                        {
                            timeText.text += "\nTime out";
                        }
                        else
                        {
                            timeText.text += "\nMax Score Reached";
                        }
                        
                        foreach (GamePoint gamePoint in FindObjectsOfType<GamePoint>())
                        {
                            gamePoint.OnTriggerEnterAction -= OnPointScored;
                        }
                    }

                    break;
                case GameState.stopped:
                    break;
            }
        }
    }
}
