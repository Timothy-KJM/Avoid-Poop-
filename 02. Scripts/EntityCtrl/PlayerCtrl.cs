using System.Collections;
using System.Collections.Generic;
using _State;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    public Rigidbody2D m_rigidbody;
    public Transform m_transform;
    public float m_move_speed = 80.0f;
    public float m_axis_h;
    public Animator m_animator;

    // 상태 패턴을 위한 State와 Context 선언
    private IPlayerState m_stop_state, m_move_state, m_dead_state;
    private PlayerStateContext m_player_state_context;

    void Start()
    {
        m_player_state_context = new PlayerStateContext(this);

        m_stop_state = gameObject.AddComponent<PlayerStopState>();
        m_move_state = gameObject.AddComponent<PlayerMoveState>();
        m_dead_state = gameObject.AddComponent<PlayerDeadState>();

        m_player_state_context.Transition(m_stop_state);

        GameManager.game_state = GameManager.GameState.PLAYING; 
        m_rigidbody = this.gameObject.GetComponent<Rigidbody2D>();
        m_transform = this.gameObject.GetComponent<Transform>();
        m_animator = this.gameObject.GetComponent<Animator>();
    }

    public void StopPlayer()
    {
        m_player_state_context.Transition(m_stop_state);
    }

    public void MovePlayer()
    {
        m_player_state_context.Transition(m_move_state);
    }

    public void DeadPlayer()
    {
        m_player_state_context.Transition(m_dead_state);
    }

    public Quaternion PlayerDirection
    {
        get { return m_transform.rotation; }
        set { m_transform.rotation = value; }
    }

    public float AxisH
    {
        get { return m_axis_h; }
        set { m_axis_h = value; }
    }

    void Update()
    {
        m_axis_h = Input.GetAxis("Horizontal");
        if(m_axis_h > 0f)
            PlayerDirection = Quaternion.Euler(0f, 0f, 0f);
        else if(m_axis_h < 0f)
            PlayerDirection = Quaternion.Euler(0f, 180f, 0f);

        SetPlayerMoveAnimation();
    }

    void SetPlayerMoveAnimation()
    {
        if(GameManager.game_state != GameManager.GameState.DEAD)
        {
            if(m_axis_h == 0f)
                StopPlayer();
            else
                MovePlayer();
        }
    }

    void SetPlayerDeadAnimation()
    {
        DeadPlayer();
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.gameObject.CompareTag("POOP"))
        {
            SetPlayerDeadAnimation();
            this.gameObject.GetComponent<SECallCtrl>().Click();
        }
    }
}