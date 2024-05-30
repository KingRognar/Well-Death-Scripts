using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class AnimationsDB_Scr : MonoBehaviour
{
    public static AnimationsDB_Scr instance;

    public AnimationCurve testCurve;

    private static AnimationCurve rotationAnimCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;

        testCurve = new AnimationCurve(new Keyframe(0f, 0f, 1f, 1f), new Keyframe(1f, .3f, 0.2f, 1f, .3f, .1f));
    }

    /// <summary>
    /// Task анимации, выполняющий движение по дуге до цели
    /// </summary>
    /// <param name="transToAnim">Transform анимируемого объекта</param>
    /// <param name="animator">Animator анимируемого объекта</param>
    /// <param name="targetPos">Конечная позиция</param>
    public async Task DBMovementAnim(Transform transToAnim, Animator animator, Vector3 targetPos)
    {
        // TODO: сделать улучшенную анимацию падения и анимацию передвижения под занятой клеткой + убрать хардкод переменные

        Vector3 startPos = transToAnim.position;
        float movementSpeed = 3f;
        float arcHeight = 1f;

        Vector2 startXZ = new Vector2(transToAnim.position.x, transToAnim.position.z);
        Vector2 nextXZ = startXZ;
        Vector2 targetXZ = new Vector2(targetPos.x, targetPos.z);
        float dist = Vector2.Distance(startXZ, targetXZ);

        _ = DBRotateAnim(transToAnim.GetChild(0).GetChild(0), targetPos); // TODO: поменять

        animator.Play("Move State");
        float temp = 0;
        while (temp != 1)
        {
            if (destroyCancellationToken.IsCancellationRequested)
                return;

            nextXZ = Vector2.MoveTowards(nextXZ, targetXZ, movementSpeed * Time.deltaTime);
            temp = Mathf.MoveTowards(temp, 1, movementSpeed * Time.deltaTime);
            float baseY = Mathf.Lerp(startPos.y, targetPos.y, temp);
            float arc = arcHeight * Vector2.Distance(nextXZ, startXZ) * Vector2.Distance(nextXZ, targetXZ) / (0.25f * dist * dist);
            transToAnim.position = new Vector3(nextXZ.x, baseY + arc, nextXZ.y);
            await Task.Yield();
        }
        animator.Play("Idle State");
    }
    /// <summary>
    /// Task анимации, выполняющий падение до цели
    /// </summary>
    /// <param name="transToAnim">Transform анимируемого объекта</param>
    /// <param name="animator">Animator анимируемого объекта</param>
    /// <param name="targetPos">Конечная позиция</param>
    public async Task DBFallAnim(Transform transToAnim, Animator animator, Vector3 targetPos)
    {
        // TODO: сделать анимацию для намеренного и непреднамеренного падения
        Vector3 startPos = transToAnim.position;
        float fallingDistance = startPos.y - targetPos.y;
        float movementSpeed = 9f / fallingDistance;

        animator.Play("Fall State");
        float temp = 0;
        while (temp != 1)
        {
            if (destroyCancellationToken.IsCancellationRequested)
                return;

            transToAnim.position = Vector3.Lerp(startPos, targetPos, temp); // TODO: мб добавить кривую
            temp = Mathf.MoveTowards(temp, 1, movementSpeed * Time.deltaTime);
            await Task.Yield();
        }

        animator.Play("Idle State");
        transToAnim.position = targetPos;
    }
    public async Task DBRaiseAnim(Transform transToAnim, Vector3 targetPos)
    {
        Vector3 startPos = transToAnim.localPosition;
        float movementSpeed = 4f;

        float temp = 0;
        while (temp != 1)
        {
            if (destroyCancellationToken.IsCancellationRequested)
                return;

            transToAnim.localPosition = Vector3.Lerp(startPos, targetPos, temp);
            temp = Mathf.MoveTowards(temp, 1, movementSpeed * Time.deltaTime);
            await Task.Yield();
        }

        transToAnim.localPosition = targetPos;
    }

    public async Task DBAttcakAnim(Transform transToAnim, Animator animator, Vector3 targetPos, CreatureSoundController_Scr soundController)
    {
        Vector3 startPos = transToAnim.position;
        Vector3 dif = targetPos - startPos;
        float animationSpeed = 2f;
        AnimationCurve animCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, -0.3f), new Keyframe(0.7f, 0.3f), new Keyframe(1f, 0f));
        float nextAnimTime = animCurve.keys[1].time;

        _ = DBRotateAnim(transToAnim.GetChild(0).GetChild(0), targetPos); // TODO: поменять

        soundController.PlaySwipe();

        animator.Play("AttackPrep State");
        bool isPlayingNextAnim = false;
        float temp = 0;
        while (temp != 1)
        {
            if (destroyCancellationToken.IsCancellationRequested)
                return;

            transToAnim.position = startPos + dif * animCurve.Evaluate(temp);

            if (!isPlayingNextAnim && temp >= nextAnimTime)
            {
                animator.Play("Attack State");
                isPlayingNextAnim = true;
            }

            temp = Mathf.MoveTowards(temp, 1, animationSpeed * Time.deltaTime);
            await Task.Yield();
        }
        transToAnim.position = startPos;
        animator.Play("Idle State");
    }
    public async Task DBAttackPrepAnim(Transform transToAnim, Animator animator, Vector3 targetPos, CreatureSoundController_Scr soundController, CancellationToken token)
    {
        Vector3 startPos = transToAnim.position;
        Vector3 dif = targetPos - startPos;
        float animationSpeed = 2f;
        AnimationCurve animCurve = new AnimationCurve(new Keyframe(0f, 0f, 1f, 1f), new Keyframe(1f, .3f, 0.2f, 1f, .3f, .1f));

        _ = DBRotateAnim(transToAnim.GetChild(0).GetChild(0), targetPos);

        animator.Play("AttackPrep State");
        float temp = 0;
        while (temp != 1)
        {
            if (destroyCancellationToken.IsCancellationRequested || token.IsCancellationRequested)
            {
                transform.position = startPos;
                return;
            }    


            transToAnim.position = startPos - dif * animCurve.Evaluate(temp);

            temp = Mathf.MoveTowards(temp, 1, animationSpeed * Time.deltaTime);
            await Task.Yield();
        }

        transform.position = startPos;
    }
    public async Task DBAttackOnlyAnim(Transform transToAnim, Animator animator, Vector3 targetPos, CreatureSoundController_Scr soundController, CancellationToken token)
    {

        Vector3 startPos = transToAnim.position;
        Vector3 dif = targetPos - startPos;
        float animationSpeed = 4f;
        AnimationCurve animCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.3f, 0.3f), new Keyframe(1f, 0f));

        _ = DBRotateAnim(transToAnim.GetChild(0).GetChild(0), targetPos);

        animator.Play("Attack State");
        float temp = 0;
        while (temp != 1)
        {
            if (destroyCancellationToken.IsCancellationRequested || token.IsCancellationRequested)
            {
                transform.position = startPos;
                return;
            }


            transToAnim.position = startPos + dif * animCurve.Evaluate(temp);

            temp = Mathf.MoveTowards(temp, 1, animationSpeed * Time.deltaTime);
            await Task.Yield();
        }

        transform.position = startPos;
        animator.Play("Idle State");
    }


    public async Task DBGetHitAnim(Transform transToAnim, Animator animator, Vector3 targetPos, CreatureSoundController_Scr soundController)
    {
        Vector3 startPos = transToAnim.position;
        Vector3 dif = targetPos - startPos;
        float animationSpeed = 6.6f;
        AnimationCurve animCurve = new AnimationCurve(new Keyframe(0f, 0f),  new Keyframe(0.5f, -0.3f), new Keyframe(1f, 0f));
        float nextAnimTime = animCurve.keys[1].time;

        _ = DBRotateAnim(transToAnim.GetChild(0).GetChild(0), targetPos); // TODO: поменять
        //soundController.PlayHit();

        bool isPlayingNextAnim = false;
        float temp = 0;
        while (temp != 1)
        {
            if (destroyCancellationToken.IsCancellationRequested)
                return;

            transToAnim.position = startPos + dif * animCurve.Evaluate(temp);

            if (!isPlayingNextAnim && temp >= nextAnimTime)
            {
                animator.Play("GetHit State");
                isPlayingNextAnim = true;
            }

            temp = Mathf.MoveTowards(temp, 1, animationSpeed * Time.deltaTime);
            await Task.Yield();
        }
        transToAnim.position = startPos;
        animator.Play("Idle State");
    }
    public async Task DBParryAnim(Transform transToAnim, Animator animator, Vector3 targetPos, CreatureSoundController_Scr soundController)
    {
        Vector3 startPos = transToAnim.position;
        Vector3 dif = targetPos - startPos;
        float animationSpeed = 6.6f;
        AnimationCurve animCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, -0.1f), new Keyframe(1f, 0f));
        float nextAnimTime = animCurve.keys[1].time;

        _ = DBRotateAnim(transToAnim.GetChild(0).GetChild(0), targetPos); // TODO: поменять
        //soundController.PlayHit();

        bool isPlayingNextAnim = false;
        float temp = 0;
        while (temp != 1)
        {
            if (destroyCancellationToken.IsCancellationRequested)
                return;

            transToAnim.position = startPos + dif * animCurve.Evaluate(temp);

            if (!isPlayingNextAnim && temp >= nextAnimTime)
            {
                animator.Play("Parry State");
                isPlayingNextAnim = true;
            }

            temp = Mathf.MoveTowards(temp, 1, animationSpeed * Time.deltaTime);
            await Task.Yield();
        }
        transToAnim.position = startPos;
        animator.Play("Idle State");
    }
    
    public void DBResetAnim(Transform transToAnim, Animator animator, Vector3 targetPos)
    {
        transToAnim.position = targetPos;
        animator.Play("Idle State");
    }

    public async Task DBRotateAnim(Transform trasToAnim, Vector3 targetPos)
    {
        // TODO: изменить targetPos на direction
        // TODO: причесать метод

        Quaternion startQuat = trasToAnim.rotation;
        Quaternion targetQuat;
        float rotationSpeed = 5;
        float degree = 0f;
        float sign = 1f;

        Vector3 moveDirectionLeveled = new Vector3(targetPos.x - trasToAnim.position.x, 0, targetPos.z - trasToAnim.position.z);

        sign = Mathf.Sign(Vector3.Dot(trasToAnim.right, moveDirectionLeveled));
        degree = Vector3.Angle(trasToAnim.forward, moveDirectionLeveled);
        trasToAnim.RotateAround(trasToAnim.position, Vector3.up, (degree + Random.Range(-10f, 10f)) * sign);
        targetQuat = trasToAnim.rotation;

        float temp = 0;
        while (temp != 1)
        {
            if (destroyCancellationToken.IsCancellationRequested)
                return;

            temp = Mathf.MoveTowards(temp, 1, rotationSpeed * Time.deltaTime);
            trasToAnim.rotation = Quaternion.Lerp(startQuat, targetQuat, rotationAnimCurve.Evaluate(temp));
            await Task.Yield();
        }
    }
}
