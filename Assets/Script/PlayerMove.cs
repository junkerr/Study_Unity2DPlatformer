using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Assets.Script;

public class PlayerMove : MonoBehaviour
{

    public float maxSpeed;
    public float jumpPower;

    private GameManager gameManager;
    private AudioManager audioManager;

    private Rigidbody2D rigid;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private CapsuleCollider2D capsuleCollider;

    private float accelation;

    private Vector2 perp;
    private float rayDistance;
    private bool isSlope;
    private float maxAngle;
    private float angle;
    private bool isGround;


    [SerializeField]
    private bool isDamaged;

    //T GetMethod<T>()
    //{
    //    if (typeof(T) == typeof(GameManager))
    //    {
    //        return (T)Convert.ChangeType(gameManager, typeof(T));
    //    }

    //    return default(T);
    //}

    private void Awake()
    {
        accelation = 2f;
        maxSpeed = 10f;
        jumpPower = 20f;
        rayDistance = 1f;
        maxAngle = 46f;
        isGround = false;

        GlobalVariables.health = 3;

        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        isDamaged = false;

        //tileMap = GetComponent<Tilemap>();
    }

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();

        rigid.drag = 2f; // 공기저항
        rigid.gravityScale = 4f; // 중력
    }

    private void FixedUpdate()
    {
        // Move
        float xDirection = Input.GetAxisRaw("Horizontal"); // 0, -1, 1

        if (xDirection == 0)
        {
            rigid.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        }


        //Debug.DrawRay(rigid.position, Vector3.right * xDirection, Color.green);
        //RaycastHit2D frontRayHit = Physics2D.Raycast(rigid.position, Vector3.right * xDirection, 1f, LayerMask.GetMask("Platform"));
        //if (frontRayHit.collider != null)
        //{
        //    if (frontRayHit.collider is TilemapCollider2D)
        //    {
        //        //Debug.Log((frontRayHit.collider as TilemapCollider2D).tag);
        //    }
        //}

        RaycastHit2D downRayhit = Physics2D.Raycast(rigid.position, Vector3.down, rayDistance, LayerMask.GetMask("Platform")); // Platform Layer만 가져오도록



        // Slope Check
        if (downRayhit)
        {
            perp = Vector2.Perpendicular(downRayhit.normal).normalized; // A의 값에서 반시계 방향으로 90도 회전한 벡터 값을 반환

            // rayHit.normal => 충돌점 평면의 수직인 법선벡터,
            angle = Vector2.Angle(downRayhit.normal, Vector2.up);

            //Debug.Log($"rayHitPoint : {downRayhit.point}, rayHitNormal : {downRayhit.normal}");
            Debug.DrawLine(downRayhit.point, downRayhit.point + downRayhit.normal, Color.green);
            Debug.DrawLine(downRayhit.point, downRayhit.point + perp, Color.red);

            if (angle != 0)
            {
                isSlope = true;
            }
            else
            {
                isSlope = false;
            }
        }

        //Debug.Log($"isSlope: {isSlope}, isGround : {isGround}, angle : {angle}, MaxAngle : {maxAngle}");


        //rigid.AddForce(Vector2.right * xDirection * 2, ForceMode2D.Impulse);

        if (isSlope && isGround && !anim.GetBool("isJumping") && angle < maxAngle)
        {
            rigid.AddForce(perp * xDirection * accelation * -1f, ForceMode2D.Impulse);
            //rigid.velocity = perp * maxSpeed * xDirection * -1f;
        }
        else if (!isSlope && isGround && !anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.right * xDirection * accelation, ForceMode2D.Impulse);
            //rigid.velocity = new Vector2(xDirection * maxSpeed, 0);
        }
        else if (!isGround)
        {
            //rigid.AddForce(Vector2.right * xDirection * 2, ForceMode2D.Impulse);
            //Debug.Log($"velocity X : {rigid.velocity.x}, velocity Y : {rigid.velocity.y}");

            //rigid.velocity = new Vector2(xDirection * maxSpeed, rigid.velocity.y);
        }



        if (angle >= maxAngle)
        {
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        }


        MoveSpeedLimit();


        //RaycastHit2D downRayhit = Physics2D.Raycast(rigid.position, Vector3.down, rayDistance, LayerMask.GetMask("Platform")); // Platform Layer만 가져오도록

        // 경사면일경우 Down 지면 충돌체크 벡터 새로 그려줌
        Debug.DrawRay(rigid.position, -downRayhit.normal, Color.yellow);
        RaycastHit2D landingRayHit = Physics2D.Raycast(rigid.position, -downRayhit.normal, rayDistance, LayerMask.GetMask("Platform"));

        // Landing Platform 
        if (rigid.velocity.y <= 0)
        {
            // rayHit Distance를 경사면에서는 각도 돌려서 체크하도록 수정 필요
            //Debug.Log(rayHit.distance);

            if (landingRayHit)
            {
                if (landingRayHit.distance < 0.5f)
                {
                    anim.SetBool("isJumping", false);
                }
            }
        }

        if (isDamaged)
        {
            if (Time.frameCount % 2 == 0)
            {
                spriteRenderer.color = new Color(1, 1, 1, 1);
            }
            else
            {
                spriteRenderer.color = new Color(1, 1, 1, 0.4f);
            }
        }
    }


    private void MoveSpeedLimit()
    {
        if (rigid.velocity.x > maxSpeed)
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }

        if (rigid.velocity.x < maxSpeed * -1f)
        {
            rigid.velocity = new Vector2(maxSpeed * -1f, rigid.velocity.y);
        }

        if (rigid.velocity.y > maxSpeed)
        {
            rigid.velocity = new Vector2(rigid.velocity.x, maxSpeed);
        }

        if (rigid.velocity.y < maxSpeed * -1f)
        {
            rigid.velocity = new Vector2(rigid.velocity.x, maxSpeed * -1f);
        }
    }

    private void Update()
    {
        // Jump
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);

            audioManager.PlaySound(AudioManager.EPlayerAction.Jump);

            isGround = false;
        }

        if (Input.GetButtonUp("Horizontal"))
        {
            // Stop Speed
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        if (Input.GetButton("Horizontal"))
        {

            // 방향키 두개다 누르면 Input.GetAxisRaw("Horizontal") 이 0 을 return 함 다른 방식으로 수정 필요

            // Direction Sprite
            if (Input.GetAxisRaw("Horizontal") < 0)
            {
                spriteRenderer.flipX = true;
            }
            else if (Input.GetAxisRaw("Horizontal") > 0)
            {
                spriteRenderer.flipX = false;
            }

            //spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        // Animation
        if (Mathf.Abs(rigid.velocity.x) < 0.3f)
        {
            anim.SetBool("isWalking", false);
        }
        else
        {
            anim.SetBool("isWalking", true);
        }

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                //Attack
                OnAttack(collision.transform);
            }
            else
            {
                // Damaged
                OnDamaged(collision.transform.position);

            }
        }
        else if (collision.gameObject.tag == "Spike")
        {
            // Damaged
            OnDamaged(collision.transform.position);
        }
        else if (collision.gameObject.tag == "Floor")
        {
            isGround = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            // Point
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if (isBronze)
            {
                GlobalVariables.stagePoint += 50;
            }
            else if (isSilver)
            {
                GlobalVariables.stagePoint += 100;
            }
            else if (isGold)
            {
                GlobalVariables.stagePoint += 200;
            }

            // Deactive Item
            collision.gameObject.SetActive(false);
            audioManager.PlaySound(AudioManager.EPlayerAction.GetItem);
        }
        else if (collision.gameObject.tag == "Finish")
        {
            // NextStage
            gameManager.NextStage();
            audioManager.PlaySound(AudioManager.EPlayerAction.Finish);
        }

    }

    private void OnAttack(Transform monster)
    {
        // Point
        GlobalVariables.stagePoint += 100;

        // Reaction Force
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        // Enemy Die
        MonsterMove monsterMove = monster.GetComponent<MonsterMove>();
        monsterMove.OnDamaged();

        audioManager.PlaySound(AudioManager.EPlayerAction.Attack);

    }

    private void OnDamaged(Vector2 targetPos)
    {
        // Health Down
        isDamaged = true;

        gameManager.HealthDown();

        // Change Layer (Immortal Active)
        gameObject.layer = 8;

        // View Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // Reaction Force
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);

        // Anumation
        anim.SetTrigger("doDamaged");

        Invoke("OffDamaged", 0.8f);

        audioManager.PlaySound(AudioManager.EPlayerAction.Damaged);

    }

    private void OffDamaged()
    {
        isDamaged = false;
        gameObject.layer = 7;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void OnDie()
    {
        // Sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // Sprite FlipY
        spriteRenderer.flipY = true;

        // Collider Diasble
        capsuleCollider.enabled = false;

        // Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        audioManager.PlaySound(AudioManager.EPlayerAction.Die);
    }


    public void Resurrection()
    {
        //GlobalVariables.health = 3;
        isDamaged = false;
        gameObject.layer = 7;
        spriteRenderer.flipY = false;
        capsuleCollider.enabled = true;
        spriteRenderer.color = new Color(1, 1, 1, 1);
        transform.position = new Vector3(0, 0, -1);
        rigid.velocity = Vector2.zero;

        audioManager.PlaySound(AudioManager.EPlayerAction.Resurrection);
    }
}
