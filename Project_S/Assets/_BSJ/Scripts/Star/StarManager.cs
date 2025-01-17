using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class StarManager : MonoBehaviour
{
    public static StarManager starManager;

    // 현재 까지 획득한 총 스타의 갯수
    public int getStarCount;

    [SerializeField] private GameObject star;
    [SerializeField] private GameObject starGold;
    [SerializeField] private ParticleSystem starGoldVFX;
    [SerializeField] private ParticleSystem starAppearVFX01;
    [SerializeField] private ParticleSystem starDisAppearVFX;
    [SerializeField] private Vector3 starObjectiveScale = new Vector3(1.5f, 1.5f, 1.5f);

    private DG.Tweening.Sequence StarAppearSequence;

    private void Awake()
    {
        starManager = this;
        starGoldVFX = starGold.GetComponent<ParticleSystem>();

        star.gameObject.SetActive(false);
    }

    /// <summary>
    /// 별을 생성하는 두트윈 시퀀스.
    /// </summary>
    private void CallStarSequence()
    {
        star.gameObject.SetActive(true);
        starGoldVFX.Play();

        StarAppearSequence = DOTween.Sequence().SetAutoKill(false).
            Append(starGold.transform.DOScale(starObjectiveScale, 0.15f)).
            Append(starGold.transform.DOShakeScale(0.5f));
    }

    /// <summary>
    /// 별을 돌리는 두트윈 시퀀스
    /// </summary>
    private void SpiralStarSequence()
    {
        StarAppearSequence = DOTween.Sequence().SetAutoKill(false).
            Append(starGold.transform.DOSpiral(3f, Vector3.forward, SpiralMode.ExpandThenContract, 0.75f, 5f)).
            Join(starGold.transform.DOScale(new Vector3(0.25f, 0.25f, 0.25f), 3f)).OnComplete(() =>
            {
                starDisAppearVFX.Play();
                starAppearVFX01.Stop();
                star.gameObject.SetActive(false);
            });
    }

    /// <summary>
    /// 별을 생성하는 코루틴.
    /// </summary>
    /// <returns></returns>
    public IEnumerator CallStar()
    {
        // 획득한 별의 갯수 증가
        getStarCount++;

        yield return new WaitForSeconds(2f);
        starAppearVFX01.Play();

        yield return new WaitForSeconds(0.3f);
        CallStarSequence();

        yield return new WaitForSeconds(0.8f);
        SpiralStarSequence();
    }

    /// <summary>
    /// 처음 입장 시 별의 갯수를 초기화해주는 메서드.
    /// </summary>
    public void InitStar()
    {
        getStarCount = 0;

        // 퍼즐이 클리어 된 갯수로 별의 갯수 변경
        foreach (bool isClear in PuzzleManager.instance.puzzles)
        {
            if (isClear)
            {
                getStarCount += 1;
            }
        }
    }
}
