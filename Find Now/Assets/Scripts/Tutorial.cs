using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Tutorial : MonoBehaviour
{
    [SerializeField] Image Hand;
    [SerializeField] TextMeshProUGUI InformationTxt;
    [SerializeField] Image informationtxtbg;

    private Vector3 startPos;

    void Start()
    {
        informationtxtbg.gameObject.SetActive(false);
        Hand.gameObject.SetActive(false);
        startPos = Hand.rectTransform.position; // Save center pos
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(4f);
        informationtxtbg.gameObject.SetActive(true);
        Hand.gameObject.SetActive(true);

        // --- First Instruction (X movement both sides) ---
        InformationTxt.text = "Scroll Left and Right to Rotate the island";
        Sequence xSeq = DOTween.Sequence();
        xSeq.Append(Hand.rectTransform.DOMoveX(startPos.x + 100, 1f)) // move right
            .Append(Hand.rectTransform.DOMoveX(startPos.x - 100, 1f)) // move left
            .Append(Hand.rectTransform.DOMoveX(startPos.x, 0.7f))       // back to center
            .SetLoops(1); // repeat twice

        yield return xSeq.WaitForCompletion();

        // --- Second Instruction (Y movement both sides) ---
        InformationTxt.text = "Scroll Up and Down to zoom in and out";
        Sequence ySeq = DOTween.Sequence();
        ySeq.Append(Hand.rectTransform.DOMoveY(startPos.y + 100, 1f)) // move up
            .Append(Hand.rectTransform.DOMoveY(startPos.y - 100, 1f)) // move down
            .Append(Hand.rectTransform.DOMoveY(startPos.y, 0.7f))       // back to center
            .SetLoops(1);

        yield return ySeq.WaitForCompletion();
        Hand.gameObject.SetActive(false);
        // --- Third Instruction ---
        InformationTxt.text = "Find Object and Click On it";
        yield return new WaitForSeconds(1.5f);

        informationtxtbg.gameObject.SetActive(false);
        Hand.gameObject.SetActive(false);
    }
}
