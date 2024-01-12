using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 테스트용 가짜 인벤토리

public class InventoryFake : MonoBehaviour
{
    public static InventoryFake Instance;

    public PlayerStat status;

    private void Awake()
    {
        // { 싱글톤
        if (null == Instance)
        {
            Instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        // } 싱글톤
    }

    public bool CheckOneValue(int value1_, int value2_)
    {
        // 소지중인 별의 총 개수 체크
        if (value1_ == 7777777)
        {
            if (StarManager.starManager.getStarCount >= value2_) { return true; }
        }
        // 들고 있는 물건 체크
        else if (value1_ != 7777777)
        {
            if (value1_ == status.GetRightGrabbableID() || value1_ == status.GetLeftGrabbableID()) { return true; }
        }
        else { /* Do Nothing */ }

        return false;
    }

    public bool CheckTwoValue(int value1_, int value2_)
    {
        // 들고 있는 물건 체크
        if (value1_ == status.GetRightGrabbableID() && value2_ == status.GetLeftGrabbableID()) { return true; }
        // 반대로도 체크
        else if (value2_ == status.GetRightGrabbableID() && value1_ == status.GetLeftGrabbableID()) { return true; }
        else { return false; }
    }
}
