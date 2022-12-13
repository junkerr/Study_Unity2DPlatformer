using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMove : MonoBehaviour
{

    public int nextAction;

    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsuleCollider;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        Invoke("ChangeAction", 3);
    }

    void FixedUpdate()
    {
        rigid.velocity = new Vector2(nextAction, rigid.velocity.y);

        // Platform Check
        float offset = 0.3f;
        Vector2 frontVector = new Vector2(rigid.position.x + (nextAction * offset), rigid.position.y);

        Debug.DrawRay(frontVector, Vector3.down, Color.green);
        RaycastHit2D rayHit = Physics2D.Raycast(frontVector, Vector3.down, 1f, LayerMask.GetMask("Platform"));
        if (rayHit.collider == null)
        {
            //Debug.Log("낭떠러지 체크");
            TurnMonster();
        }

    }

    void ChangeAction()
    {
        // Set Next Action
        nextAction = UnityEngine.Random.Range(-1, 2);

        // Sprite Animation
        anim.SetInteger("WalkSpeed", nextAction);

        // Flip Sprite, 방향 변경
        if (nextAction != 0)
        {
            //Debug.Log(nextAction);
            spriteRenderer.flipX = nextAction == 1;
        }

        Invoke("ChangeAction", 3); //5초뒤 ChangeAction 함수 호출;
    }

    void TurnMonster()
    {
        nextAction *= -1;
        spriteRenderer.flipX = nextAction == 1;
        CancelInvoke("ChangeAction");
        Invoke("ChangeAction", 3);
    }

    public void OnDamaged()
    {
        // Sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // Sprite FlipY
        spriteRenderer.flipY = true;

        // Collider Diasble
        capsuleCollider.enabled = false;

        // Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        // Destroy
        Invoke("DeActive", 5);
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }
}
