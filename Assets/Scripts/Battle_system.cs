using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Battle_system : MonoBehaviour
{
    // Start is called before the first frame update
    // private GameObject root;
    private GameObject prfb;
    private GameObject transmission_ob;
    private Data_transmission trans;
    // private Data_transmission transmission;
    private GameObject temporary;//初始化时的临时变量
    private GameObject temp;//战斗时的临时变量
    private Card_controller cc;
    private List<GameObject> player_queue = new List<GameObject>();
    private List<GameObject> enemy_queue = new List<GameObject>();
    private List<GameObject> offensive_queue;//攻方队列
    private List<GameObject> defensive_queue;//守方队列

    public Text round;
    public Text end;

    [SerializeField] private int adr = -1;//防守方下标
    [SerializeField] private Vector2 vct;
    private bool pause = false;
    private int num_of_round;

    public int sb = 1;
    public int of_0_id = 7;
    public Battle_system()
    {
        
    }
    void Start()
    {
        Debug.Log("Debug.start");

        num_of_round = 1;
        round.text = "Round" + num_of_round.ToString();
        round.gameObject.SetActive(true);
        Invoke("Close_round", 3);
        //root = GameObject.Find("GameObject");//找到顶级对象
        transmission_ob = GameObject.Find("Data_transmission");//找到从地下城界面和阵容选择界面传输来的信息载体
        //transmission_ob = root.transform.Find("Data_transmission").gameObject;//找到从地下城界面和阵容选择界面传输来的信息载体
        trans = transmission_ob.GetComponent<Data_transmission>();//获取信息载体的代码组件——Data_transmission类并生产对象
        Loading();
        offensive_queue = trans.of_or_def ? player_queue : enemy_queue;
        defensive_queue = trans.of_or_def ? enemy_queue : player_queue;
        Interrupt(4, false);
        adr = -1;
    }
    public void Loading()
    {
        int i;
        prfb = (GameObject)Resources.Load("Card/Card_controller");
        for (i=0;i<trans.p_amt;i++)//加载玩家的卡牌实体并入队
        {
            temporary = (GameObject)Instantiate(prfb);
            temporary.SetActive(true);
            temporary.GetComponent<Transform>().position = new Vector2(-17.8f + (i + 1) * 35.6f / (trans.p_amt + 1), -5f);
            cc = temporary.GetComponent<Card_controller>();
            cc.Set_lct(temporary.GetComponent<Transform>().position);
            cc.Gs_type = 0;
            cc.Gs_id = trans.ci.card_information[trans.Get_card(i)].ID;
            cc.Gs_describe = trans.ci.card_information[trans.Get_card(i)].Describe;
            cc.Gs_level = trans.ci.card_information[trans.Get_card(i)].Level;
            cc.Gs_atk = trans.ci.card_information[trans.Get_card(i)].ATK;
            cc.Gs_hp = trans.ci.card_information[trans.Get_card(i)].HP;
            cc.Gs_order = i;
            player_queue.Add(temporary);
        }
        for (i = 0; i < trans.e_amt; i++)//加载敌人的卡牌实体并入队
        {
            temporary = (GameObject)Instantiate(prfb);
            temporary.SetActive(true);
            temporary.GetComponent<Transform>().position = new Vector2(-17.8f + (i + 1) * 35.6f / (trans.e_amt + 1), 5f);
            cc = temporary.GetComponent<Card_controller>();
            cc.Set_lct(temporary.GetComponent<Transform>().position);
            cc.Gs_type = 1;
            cc.Gs_id = trans.ci.card_information[trans.Get_enemy(i)].ID;
            cc.Gs_describe = trans.ci.card_information[trans.Get_enemy(i)].Describe;
            cc.Gs_level = trans.ci.card_information[trans.Get_enemy(i)].Level;
            cc.Gs_atk = trans.ci.card_information[trans.Get_enemy(i)].ATK;
            cc.Gs_hp = trans.ci.card_information[trans.Get_enemy(i)].HP;
            cc.Gs_order = i;
            enemy_queue.Add(temporary);
        }

        //Locate(player_queue,-5f);
        //Locate(enemy_queue,5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (pause)
        {
            return;
        }
        if(player_queue.Count != 0 && enemy_queue.Count != 0)
        {
            //Debug.Log("111111111111111");
            int unattacked = player_queue.Count + enemy_queue.Count;
            for (int i = 0; i < player_queue.Count; i++)
            {
                if (player_queue[i].GetComponent<Card_controller>().Gs_attacked == true && player_queue[i].GetComponent<Card_controller>().Gs_back == true)
                {
                    unattacked--;
                }
            }
            for (int i = 0; i < enemy_queue.Count; i++)
            {
                if (enemy_queue[i].GetComponent<Card_controller>().Gs_attacked == true && enemy_queue[i].GetComponent<Card_controller>().Gs_back == true)
                {
                    unattacked--;
                }
            }
            sb = unattacked;
            if (unattacked == 0)
            {
                for (int i = 0; i < player_queue.Count; i++)
                {
                    player_queue[i].GetComponent<Card_controller>().State_init();
                }
                for (int i = 0; i < enemy_queue.Count; i++)
                {
                    enemy_queue[i].GetComponent<Card_controller>().State_init();
                }
                num_of_round++;
                round.text = "Round" + num_of_round.ToString();
                round.gameObject.SetActive(true);
                Invoke("Close_round", 3);
                Interrupt(4, false);
                Debug.Log("The queue has been inited.");
            }
            else
            {
            }
            if (pause) return;

            if ((adr != -1) && (offensive_queue[0].GetComponent<Card_controller>().Gs_dead == true||defensive_queue[adr].GetComponent<Card_controller>().Gs_dead == true
                || (offensive_queue[0].GetComponent<Card_controller>().Gs_attacked == true && offensive_queue[0].GetComponent<Card_controller>().Gs_back == false)))
            {
                int a = 0;
                if (defensive_queue[adr].GetComponent<Card_controller>().Gs_dead == true)
                {
                    temp = defensive_queue[adr];
                    a = temp.GetComponent<Card_controller>().Gs_order;
                    defensive_queue.RemoveAt(adr);
                    temp.GetComponent<Card_controller>().Gs_destroy = true;
                    Relocate(defensive_queue,a);
                    Debug.Log("Offenser changed because of defenser's death.");
                }

                if (offensive_queue[0].GetComponent<Card_controller>().Gs_dead == true)
                {
                    temp = offensive_queue[0];
                    a = temp.GetComponent<Card_controller>().Gs_order;
                    offensive_queue.RemoveAt(0);
                    temp.GetComponent<Card_controller>().Gs_destroy = true;
                    Relocate(offensive_queue,a);
                    Debug.Log("Offenser changed because of offenser's death.");
                }
                else if (offensive_queue[0].GetComponent<Card_controller>().Gs_attacked == true && offensive_queue[0].GetComponent<Card_controller>().Gs_back == false)
                {
                    temp = offensive_queue[0];
                    offensive_queue.RemoveAt(0);
                    offensive_queue.Add(temp);
                    Debug.Log("Offenser changed because the attack ended.");
                }

                adr = -1;
                List<GameObject> temp_queue = offensive_queue;
                offensive_queue = defensive_queue;
                defensive_queue = temp_queue;
                Debug.Log("Offenser change preparing");
                if (offensive_queue.Count!=0)
                {
                    Debug.Log("Offenser changing");
  
                      of_0_id = offensive_queue[0].GetComponent<Card_controller>().Gs_id;
                    Debug.Log("Offenser changed to" + offensive_queue[0].GetComponent<Card_controller>().Gs_id + ".");
                }   
                Interrupt(4, false);
            }
            else if (adr == -1)
            {
                System.Random rd = new System.Random();
                adr = (int)rd.Next(0, defensive_queue.Count);
                Debug.Log("offence_id = " + offensive_queue[0].GetComponent<Card_controller>().Gs_id + ",defense_id = " + defensive_queue[adr].GetComponent<Card_controller>().Gs_id);
                offensive_queue[0].GetComponent<BoxCollider2D>().enabled = true;
                defensive_queue[adr].GetComponent<BoxCollider2D>().enabled = true;
                defensive_queue[adr].GetComponent<Card_controller>().Gs_defense = true;
                offensive_queue[0].GetComponent<Card_controller>().Gs_back = false;
                offensive_queue[0].GetComponent<SpriteRenderer>().sortingLayerName = "battling_card_face";
                for (int j = 0; j < 4; j++)
                {
                    offensive_queue[0].transform.GetChild(j).GetComponent<SpriteRenderer>().sortingLayerName = "battling_card_information";
                }
                vct = new Vector2((defensive_queue[adr].GetComponent<Transform>().position.x - offensive_queue[0].GetComponent<Transform>().position.x) / 2,
                    (defensive_queue[adr].GetComponent<Transform>().position.y - offensive_queue[0].GetComponent<Transform>().position.y) / 2);
                offensive_queue[0].GetComponent<Rigidbody2D>().velocity = vct;
                Interrupt(0.5f, false);
                Debug.Log("Offenser " + offensive_queue[0].GetComponent<Card_controller>().Gs_id + "'s new enemy is" + adr);
            }
            if (pause)
            {
                Debug.Log("3333333333333333");
                return; 
            }
        }
        else if (enemy_queue.Count == 0)
        {
            Victory();
        }
        else if (player_queue.Count == 0)
        {
            Defeat();
        }
    }
    private void Close_round()
    {
        round.gameObject.SetActive(false);
    }
    private void Relocate(List<GameObject> l,int order)
    {
        for (int i = 0; i < l.Count; i++)
        {
            if(l[i].GetComponent<Card_controller>().Gs_order> order)
            {
                l[i].GetComponent<Card_controller>().Gs_order -= 1;
            }
            l[i].GetComponent<Transform>().position = new Vector2(-17.8f + (l[i].GetComponent<Card_controller>().Gs_order + 1) * 35.6f / (l.Count + 1), l[i].GetComponent<Transform>().position.y);
            l[i].GetComponent<Card_controller>().Set_lct(l[i].GetComponent<Transform>().position);
        }
    }
    private void Locate(List<GameObject> l,float f)
    {
        for (int i = 0; i < l.Count; i++)
        {
            l[i].GetComponent<Transform>().position = new Vector2(-17.8f + (i + 1) * 35.6f / (l.Count + 1), f);
            l[i].GetComponent<Card_controller>().Set_lct(l[i].GetComponent<Transform>().position);
        }
    }
    public void Interrupt(float time,bool forever)
    {
        pause = true;
        if (!forever)
        {
            Invoke("Interrupt_end", time);
        }
        /*for (int i = 0; i < offensive_queue.Count; i++)
        {

        }*/
    }
    private void Interrupt_end()
    {
        pause = false;
    }
    private void Victory()
    { 
        Interrupt(0.1f, true);
        end.text = "恭喜你获得了对战胜利\n按B键返回地下城";
        end.gameObject.SetActive(true);
    }
    private void Defeat()
    { 
        Interrupt(0.1f, true);
        end.text = "很遗憾，对战失败了\n按B键返回地下城";
        end.gameObject.SetActive(true);
    }
}
