using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dongle : MonoBehaviour
{
    public int level;
    public bool isDrag;
    public bool isMerge;
    public ParticleSystem effect;
    public GameManager manager;
    Rigidbody2D rigid;
    CircleCollider2D circle;
    Animator anim;

    void Start()
    {

    }

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        circle = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        anim.SetInteger("Level", level);
    }

    void Update()
    {
        if (isDrag)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // X축 경계 설정
            float leftBoarder = -4.2f + transform.localScale.x / 2;
            float rightBoarder = 4.2f - transform.localScale.x / 2;
            if (mousePos.x < leftBoarder)
            {
                mousePos.x = leftBoarder;
            }
            else if (mousePos.x > rightBoarder)
            {
                mousePos.x = rightBoarder;
            }
            mousePos.y = 8;
            mousePos.z = 0;
            transform.position = Vector3.Lerp(transform.position, mousePos, 0.2f);
        }

    }

    public void Drag()
    {
        isDrag = true;
    }

    public void Drop()
    {
        isDrag = false;
        rigid.simulated = true;
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Dongle")
        {
            Dongle other = collision.gameObject.GetComponent<Dongle>();
            if (level == other.level && !isMerge && !other.isMerge && level < 7)
            {
                float meX = transform.position.x;
                float meY = transform.position.y;
                float otherX = other.transform.position.x;
                float otherY = other.transform.position.y;
                // 1. 내가 아래에 있을 때
                // 2. 동일한 높이일 때, 내가 오른쪽에 있을 때
                if (meY < otherY || (meY == otherY && meX > otherX))
                {
                    other.Hide(transform.position);
                    LevelUp();
                }
            }
        }
    }
    void Hide(Vector3 targetPos)
    {
        isMerge = true;
        rigid.simulated = false;
        circle.enabled = false;
        StartCoroutine(HideRoutine(targetPos));
    }
    IEnumerator HideRoutine(Vector3 targetPos)
    {
        int frameCount = 0;
        while (frameCount < 20)
        {
            frameCount++;
            transform.position = Vector3.Lerp(transform.position, targetPos, 0.5f);
            yield return null;
        }
        isMerge = false;
        gameObject.SetActive(false);
    }
    void LevelUp()
    {
        isMerge = true;
        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0;
        StartCoroutine(LevelUpRoutine());
    }
    IEnumerator LevelUpRoutine()
    {
        yield return new WaitForSeconds(0.2f);
        anim.SetInteger("Level", level + 1);
        EffectPlay();
        yield return new WaitForSeconds(0.3f);
        level++;
        manager.maxLevel = Mathf.Max(level, manager.maxLevel);
        isMerge = false;
    }
    void EffectPlay()
    {
        effect.transform.position = transform.position;
        effect.transform.localScale = transform.localScale;
        effect.Play();
    }
}
