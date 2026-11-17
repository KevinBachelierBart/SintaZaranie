using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Behavior : MonoBehaviour
{
    [Header("INPUT SYSTEM")]
    [SerializeField]
    private InputActionAsset m_inputAction;
    private InputAction m_move;
    private InputAction m_dash;

    //[Header("SCRIPTS RELATED")]

    [Header("Rigidbody 2D")]
    private Rigidbody2D m_rigidbody2D;

    [Header("ANIMATION & SR")]
    private Animator m_animator;
    private SpriteRenderer m_sp;

    [Header("DESIGN")]
    [SerializeField] private bool m_canMove;
    [SerializeField] private Vector2 m_moveAmt;
    [SerializeField] private float m_playerSpeed;

    [Space(4)]//Dashing
    private bool m_isDashing;
    [SerializeField] private int m_dashForce;
    [SerializeField] private float m_dashEndThreshold = 2f;

    [Space(4)]
    [Tooltip("+ cette valeur est élevée, plus le personnage ralentit rapidement.")]
    [SerializeField] private float waterDrag = 1.5f;//+ cette valeur est élevée, plus le personnage ralentit rapidement.

    [Header("DEBUG--------------------------------------------------")]
    public float magnitude;
    public float magnitudeBeforeDash;
    void Start()
    {
        //Inputs
        m_move = m_inputAction.FindAction("Move");
        m_dash = m_inputAction.FindAction("Dash");

        //RigidBody
        m_rigidbody2D = GetComponent<Rigidbody2D>();

        //SR
        m_sp = GetComponent<SpriteRenderer>();

    }

    void Update()
    {
        Moving();

        //Dash
        if (m_dash.WasPressedThisFrame())
        {
            Dashing(m_moveAmt);
        }
        if (m_isDashing) 
        {
            if (m_rigidbody2D.linearVelocity.magnitude < magnitudeBeforeDash)
            {
                m_isDashing = false;//Je sais desormais quand finit le dash mais je sais plus ce que je voulais en faire
            }
        }


        magnitude = m_rigidbody2D.linearVelocity.magnitude;
    }

    void Moving()
    {
        if (m_isDashing)
            return;


        m_moveAmt = Vector2.zero;//pour éviter de conserver l'ancienne direction du joueur.

        if (m_canMove)
        {
            //MOVEMENT
            m_moveAmt = m_move.ReadValue<Vector2>().normalized;
            /*Vector2 calculatedVelocity = m_rigidbody2D.linearVelocity;
            calculatedVelocity.x = moveAmt.x * m_playerSpeed;
            calculatedVelocity.y = moveAmt.y * m_playerSpeed;
            m_rigidbody2D.linearVelocity = calculatedVelocity;*/


            if (m_moveAmt.x > 0)
            {
                m_sp.flipX = true;
            }
            else
            {
                m_sp.flipX = false;
            }
        }

        

        Vector2 calculatedVelocity = m_moveAmt * m_playerSpeed;
        //LLM
        m_rigidbody2D.linearVelocity = Vector2.Lerp(m_rigidbody2D.linearVelocity, calculatedVelocity, waterDrag * Time.deltaTime);
    }

    void Dashing(Vector2 VectorMoveAmt)
    {
        m_isDashing = true;


        magnitudeBeforeDash = m_rigidbody2D.linearVelocity.magnitude;
        
        m_rigidbody2D.AddForce(VectorMoveAmt * m_dashForce, ForceMode2D.Impulse);

        
    }
}
