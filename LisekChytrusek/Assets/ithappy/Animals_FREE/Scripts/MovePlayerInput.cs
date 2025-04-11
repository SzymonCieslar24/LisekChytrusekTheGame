using UnityEngine;
using System.Collections;

namespace Controller
{
    [RequireComponent(typeof(CreatureMover))]
    public class MovePlayerInput : MonoBehaviour
    {
        [Header("Random Movement Settings")]
        [SerializeField] private float directionChangeInterval = 2f;
        [SerializeField] private float jumpProbability = 0.1f;
        [SerializeField] private float runProbability = 0.0f;

        [Header("Camera")]
        [SerializeField] private PlayerCamera m_Camera;

        private CreatureMover m_Mover;

        private Vector2 m_Axis;
        private bool m_IsRun;
        private bool m_IsJump;

        private Vector3 m_Target;
        private Vector2 m_MouseDelta;
        private float m_Scroll;

        private float m_Timer;

        private void Awake()
        {
            m_Mover = GetComponent<CreatureMover>();
        }

        private void Update()
        {
            GatherInput();
            SetInput();
        }

        public void GatherInput()
        {
            m_Timer += Time.deltaTime;

            // Zmiana kierunku co kilka sekund
            if (m_Timer >= directionChangeInterval)
            {
                m_Axis = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                m_IsRun = Random.value < runProbability;
                m_IsJump = Random.value < jumpProbability;

                m_MouseDelta = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                m_Scroll = Random.Range(-1f, 1f);

                m_Timer = 0f;
            }

            m_Target = (m_Camera == null) ? Vector3.zero : m_Camera.Target;
        }

        public void BindMover(CreatureMover mover)
        {
            m_Mover = mover;
        }

        public void SetInput()
        {
            if (m_Mover != null)
            {
                m_Mover.SetInput(in m_Axis, in m_Target, in m_IsRun, m_IsJump);
            }

            if (m_Camera != null)
            {
                m_Camera.SetInput(in m_MouseDelta, m_Scroll);
            }
        }
    }
}