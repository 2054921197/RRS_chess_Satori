using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Card_controller : MonoBehaviour
{
    public int a = 2;
    private SpriteRenderer spr;

    private int type;//0为主站者所用卡牌，否则为敌人
    private int num;//编号,即图中的位置
    [SerializeField] private int card_id;//是哪张牌，在场景切换时由主站者选择
    [SerializeField] private int order;//在队列里的攻击顺序

    [SerializeField] private bool defense = false;//是否在战斗中，用于判断是否生成碰撞体
    [SerializeField] private bool attacked = false;//本轮是否已攻击过
    [SerializeField] private bool back = true;//是否已完成动作归位
    [SerializeField] private bool dead = false;//是否已死亡
    [SerializeField] private bool destroy = false;//是否接受战斗系统发回的销毁信号
    [SerializeField] private bool pause = false;//是否接受战斗系统发回的销毁信号

    [SerializeField] private string Describe;
    [SerializeField] private int Level;
    [SerializeField] private int ATK;
    [SerializeField] private int HP;

    private int Level_limit;
    private int ATK_limit;
    private int HP_limit;

    [SerializeField] private Vector2 location;

    //private GameObject root ;
//    private GameObject ui_prefab;
    private GameObject Describe_board;
    private GameObject Level_board;
    private GameObject ATK_board;
    private GameObject HP_board;

    private GameObject canv;
    private GameObject Describe_obj;
    private GameObject Level_obj;
    private GameObject ATK_obj;
    private GameObject HP_obj;

    private Text Describe_text;
    private Text Level_text;
    private Text ATK_text;
    private Text HP_text;

    public int sb;
    public string nimabi;
    public float cnm = 0.1f;
    public Vector2 size1;
    public Vector2 size2;

    private string picturepath = "Card_face/";
    private void Card_face_loading(GameObject g,string s)
    {
        nimabi = s;
        Texture2D img = Resources.Load(picturepath + s) as Texture2D;
        if (img != null)
        {
            Sprite sp = Sprite.Create(img, new Rect(0, 0, img.width, img.height), new Vector2(0.5f, 0.5f));
            g.GetComponent<SpriteRenderer>().sprite = sp;
        }
        else { /*异常*/}
    }
    public void Load()
    {
        spr = gameObject.GetComponent<SpriteRenderer>();
        //加载图片
        //Texture2D img = Resources.Load(picturepath + card_id.ToString()) as Texture2D;
        Texture2D img = Resources.Load(picturepath + "background") as Texture2D;
        if (img != null)
        {
            Sprite sp = Sprite.Create(img, new Rect(0, 0, img.width, img.height), new Vector2(0.5f, 0.5f));
            gameObject.GetComponent<SpriteRenderer>().sprite = sp;
            //transform.localScale = new Vector2(0.5f, 0.5f);
            gameObject.GetComponent<BoxCollider2D>().transform.position = transform.position;
            gameObject.GetComponent<BoxCollider2D>().size = gameObject.GetComponent<SpriteRenderer>().bounds.size;
            size1 = gameObject.GetComponent<BoxCollider2D>().transform.position;
            size2 = GetComponent<BoxCollider2D>().size;
        }
        else { /*异常*/}

        //设置组件

        Level_board = gameObject.transform.GetChild(1).gameObject;
        Card_face_loading(Level_board, "level");
        //Level_board.transform.localScale = new Vector2(0.5f, 0.5f);
        Level_board.transform.position = new Vector2(transform.position.x - (spr.bounds.size.x - Level_board.GetComponent<SpriteRenderer>().bounds.size.x) / 2,
            transform.position.y+(spr.bounds.size.y - Level_board.GetComponent<SpriteRenderer>().bounds.size.y) / 2);

        ATK_board = gameObject.transform.GetChild(2).gameObject;
        Card_face_loading(ATK_board, "ATK");
        //ATK_board.transform.localScale = new Vector2(0.5f, 0.5f);
        ATK_board.transform.position = new Vector2(transform.position.x - (spr.bounds.size.x - ATK_board.GetComponent<SpriteRenderer>().bounds.size.x) / 2,
            transform.position.y - (spr.bounds.size.y - ATK_board.GetComponent<SpriteRenderer>().bounds.size.y) / 2);

        HP_board = gameObject.transform.GetChild(3).gameObject;
        Card_face_loading(HP_board, "HP");
        //HP_board.transform.localScale = new Vector2(0.5f, 0.5f);
        HP_board.transform.position = new Vector2(transform.position.x + (spr.bounds.size.x - HP_board.GetComponent<SpriteRenderer>().bounds.size.x) / 2,
            transform.position.y - (spr.bounds.size.y - HP_board.GetComponent<SpriteRenderer>().bounds.size.y) / 2);

        Describe_board = gameObject.transform.GetChild(0).gameObject;
        Card_face_loading(Describe_board, "describe");
        //Describe_board.transform.localScale = new Vector2(0.5f, 0.5f);
        Describe_board.transform.position = new Vector2(transform.position.x,
            transform.position.y - (spr.bounds.size.y - Describe_board.GetComponent<SpriteRenderer>().bounds.size.y - 2* ATK_board.GetComponent<SpriteRenderer>().bounds.size.y ) / 2);

        canv = Instantiate(Resources.Load("Text/Canvas1") as GameObject);
        Describe_obj = canv.transform.GetChild(0).gameObject;
        Level_obj = canv.transform.GetChild(1).gameObject;
        ATK_obj = canv.transform.GetChild(2).gameObject;
        HP_obj = canv.transform.GetChild(3).gameObject;

        Describe_text = Describe_obj.GetComponent<Text>();
        Level_text = Level_obj.GetComponent<Text>();
        ATK_text = ATK_obj.GetComponent<Text>();
        HP_text = HP_obj.GetComponent<Text>();

        Describe_text.text = Describe;
        Level_text.text = Level.ToString();
        ATK_text.text = ATK.ToString();
        HP_text.text = HP.ToString();

        //设置初始数据
        Level_limit = Level;
        ATK_limit = ATK;
        HP_limit = HP;
        State_init();
    }
    void Start()
    {
        Invoke("Load",0.5f);
        Interrupt(0.7f, false);
        
    }

    // Update is called once per frame
    void Update()
    {
        Describe_text.text = Describe;
        Level_text.text = Level.ToString();
        ATK_text.text = ATK.ToString();
        HP_text.text = HP.ToString();

        Describe_text.transform.position =  Camera.main.WorldToScreenPoint(Describe_board.transform.position);
        Level_text.transform.position = Camera.main.WorldToScreenPoint(Level_board.transform.position);
        ATK_text.transform.position = Camera.main.WorldToScreenPoint(ATK_board.transform.position);
        HP_text.transform.position = Camera.main.WorldToScreenPoint(HP_board.transform.position);

        if (pause) return;

        if (destroy)
        {
            Invoke("Boom",0.1f);
        }

        if (attacked == true && !(System.Math.Abs(transform.position.y) < System.Math.Abs(location.y)))
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
            spr.sortingLayerName = "card_face";
            for(int j=0;j<4;j++)
            {
                gameObject.transform.GetChild(j).GetComponent<SpriteRenderer>().sortingLayerName = "card_information";
            }
            back = true;
        }
 
        Level_board.transform.position = new Vector2(transform.position.x - (spr.bounds.size.x - Level_board.GetComponent<SpriteRenderer>().bounds.size.x) / 2,
            transform.position.y + (spr.bounds.size.y - Level_board.GetComponent<SpriteRenderer>().bounds.size.y) / 2);
        
        ATK_board.transform.position = new Vector2(transform.position.x - (spr.bounds.size.x - ATK_board.GetComponent<SpriteRenderer>().bounds.size.x) / 2,
            transform.position.y - (spr.bounds.size.y - ATK_board.GetComponent<SpriteRenderer>().bounds.size.y) / 2);
        
        HP_board.transform.position = new Vector2(transform.position.x + (spr.bounds.size.x - HP_board.GetComponent<SpriteRenderer>().bounds.size.x) / 2,
            transform.position.y - (spr.bounds.size.y - HP_board.GetComponent<SpriteRenderer>().bounds.size.y) / 2);

        Describe_board.transform.position = new Vector2(transform.position.x,
            transform.position.y - (spr.bounds.size.y - Describe_board.GetComponent<SpriteRenderer>().bounds.size.y - ATK_board.GetComponent<SpriteRenderer>().bounds.size.y * 2) / 2);
    }
    private void Boom()
    {
        Debug.Log("I am" + card_id + " I 'm beeing destroyed soon");
        Destroy(canv);
        Destroy(gameObject);
    }
    public void State_init()
    {
        attacked = false;
        back = true;
        dead = false;
        defense = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        sb = 1000;
        Card_controller enemy = collision.gameObject.GetComponent<Card_controller>();
        HP = HP - enemy.Gs_atk;
        if (!(HP > 0))
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            dead = true;
            Debug.Log("I am" + card_id + " I'll die");
        }
        else
        {
            if (!defense && !back)
            {
                attacked = true;
                gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2((location.x - transform.position.x) / 2, (location.y - transform.position.y) / 2);
                Debug.Log("I am" + card_id + " I have already attacked but not back");
            }
            else
            {
                dedefense();
            } 
        }
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }
    private void dedefense()
    {
        defense = false;
        Debug.Log("I am" + card_id + " I have fnishied my defense");
    }
    public void Interrupt(float time, bool forever)
    {
        pause = true;
        if (!forever)
            Invoke("Interrupt_end", time);
    }
    private void Interrupt_end()
    {
        pause = false;
    }
    public int Gs_type
    {
        get { return type; }
        set { type = value; }
    }
    public int Gs_num
    {
        get { return num; }
        set { num = value; }
    }
    public int Gs_id
    {
        get { return card_id; }
        set { card_id = value; }
    }
    public int Gs_order
    {
        get { return order; }
        set { order = value; }
    }
    public string Gs_describe
    {
        get { return Describe; }
        set { Describe = value; }
    }
    public int Gs_level
    {
        get { return Level; }
        set { Level = value; }
    }
    public int Gs_atk
    {
        get { return ATK; }
        set { ATK = value; }
    }
    public int Gs_hp
    {
        get { return HP; }
        set { HP = value; }
    }
    public bool Gs_defense
    {
        get { return defense; }
        set { defense = value; }
    }
    public bool Gs_attacked
    {
        get { return attacked; }
        set { attacked = value; }
    }
    public bool Gs_back
    {
        get { return back; }
        set { back = value; }
    }
    public bool Gs_dead
    {
        get { return dead; }
        set { dead = value; }
    }
    public bool Gs_destroy
    {
        get { return destroy; }
        set { destroy = value; }
    }
    public void Set_lct(Vector2 l)
    { this.location = new Vector2(l.x, l.y); }
    public Vector2 Get_lct()
    { return this.location; }
}

