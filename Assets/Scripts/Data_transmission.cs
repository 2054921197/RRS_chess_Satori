using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Data_transmission : MonoBehaviour
{
    private int[] card_queue = new int[7];
    private int[] enemy_queue = new int[7];
    public Card_initializing ci;
    public int p_amt;
    public int e_amt;
    public bool of_or_def;//先手权，为true时我方先手，为false时我方后手
    //唯一的卡牌信息获取途径
    //从地下城转到战斗的流程：遭遇敌人（meeting）→进入阵容选择界面（choosing）→战斗（battling）
    //进入商店后的流程：
    //进入商店（shoping）→←进入卡牌购买界面（buying）
    //         ↓↑
    //进入卡牌升级界面（choosing）
    public void Card_choosing(int[] card_num)//选择的卡牌的ID序列，after choosing
    {
        int i;
        for (i = 0;i<p_amt;i++)
        {
            card_queue[i] = card_num[i];
        }
        for (; i < 7; i++)
        {
            card_queue[i] = 0;
        }
    }
    public void Enemy_setting(int[] enemy_num)//设置的敌人的ID序列,after meeting
    {
        int i = 0;
        for (i = 0; i < e_amt; i++)
        {
            enemy_queue[i] = enemy_num[i];
        }
        for (; i < 7; i++)
        {
            enemy_queue[i] = 0;
        }
    }
    public int Get_card(int a)//读得所选的卡牌的ID序列，before battling
    {
        if(a < 7 && a > -1)
            return card_queue[a];
        else
        {
            //异常
            return 3;
        }
    }
    public int Get_enemy(int a)//读得敌人的ID序列，before battling
    {
        if (a < 7 && a > -1)
            return enemy_queue[a];
        else
        {
            //异常
            return 3;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        ci = new Card_initializing();
        p_amt = 2;
        e_amt = 3;
        card_queue[0] = 0;
        card_queue[1] = 1;
        enemy_queue[0] = 3;
        enemy_queue[1] = 2;
        enemy_queue[2] = 4;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }//按E切换场景
    }
}
