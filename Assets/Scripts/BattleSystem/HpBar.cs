using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField] private Text hpText;
    private int curHp;
    private int maxHp;

    public bool IsUpdating { get; private set; }

    // Set up the Hp information
    public void SetHp(float hpNormalized, int maxHp, int curHp)
    {
        transform.localScale = new Vector3(hpNormalized, 1f);
        this.maxHp = maxHp;
        this.curHp = curHp;

        if (hpText!= null)
        {
            hpText.text = this.curHp.ToString() + " / " + this.maxHp.ToString();
        }
        
    }

    // Hp bar animation
    public IEnumerator SetHpSmooth(float newHp, int curHpInt)
    {
        IsUpdating = true;
        float curHp = transform.localScale.x;
        float changeAmt = curHp - newHp;

        while (curHp - newHp > Mathf.Epsilon)
        {
            curHp -= changeAmt * Time.deltaTime;
            
            transform.localScale = new Vector3(curHp, 1f);
            yield return null;
        }
        transform.localScale = new Vector3(newHp, 1f);
        if (hpText != null)
        {
            hpText.text = curHpInt.ToString() + " / " + this.maxHp.ToString();
        } 

        IsUpdating = false;

    }
}
