//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

////csv에서 읽어와서 값이 초기화 되는 CharacterInfomation과 둘 중에 하나를 없애야 한다. 
////기획자의 요청으로 일단 csv에서 값을 읽어오는 방식이 아니라 해당 클래스를 이용해 인스펙트에서 커스텀 할수 있도록 구현
//[System.Serializable]
//public class CharacterInfo : MonoBehaviour
//{
//    //캐릭터 이름
//    [SerializeField]
//    private string character_Name;
//    public string Character_Name { get { return character_Name; } set { character_Name = value; } }

//    //캐릭터 hp 총량
//    [SerializeField]
//    private int player_HP;
//    public int P_player_HP { get { return player_HP; } set { player_HP = value; } }


//    //캐릭터 방어력
//    [SerializeField]
//    private int player_Def;
//    public int P_player_Def { get { return player_Def; } set { player_Def = value; } }


//    //캐릭터 Stamina 총량
//    [SerializeField]
//    private int player_Stamina;
//    public int P_player_Stamina { get { return player_Stamina; } set { player_Stamina = value; } }

//    // Stamina 자동회복 시간 
//    [SerializeField]
//    private int player_Stamina_Recovery_Time;
//    public int P_player_Stamina_Recovery_Time { get { return player_Stamina_Recovery_Time; } set { player_Stamina_Recovery_Time = value; } }

//    // Stamina 자동회복 값
//    [SerializeField]
//    private int player_Stamina_Recovery_Val;
//    public int P_player_Stamina_Recovery_Val { get { return player_Stamina_Recovery_Val; } set { player_Stamina_Recovery_Val = value; } }

//    //그로기값 최대치
//    [SerializeField]
//    private int player_Groggy;
//    public int P_player_Groggy { get { return player_Groggy; } set { player_Groggy = value; } }

//    // 그로기값 자동회복 시간 
//    [SerializeField]
//    private int player_Groggy_Recovery_Time;
//    public int P_player_Groggy_Recovery_Time { get { return player_Groggy_Recovery_Time; } set { player_Groggy_Recovery_Time = value; } }

//    // 그로기값 자동회복 값
//    [SerializeField]
//    private int player_Groggy_Recovery_Val;
//    public int P_player_Groggy_Recovery_Val { get { return player_Groggy_Recovery_Val; } set { player_Groggy_Recovery_Val = value; } }


//    //경직 상태에 빠지는 그로기값
//    [SerializeField]
//    private int player_Stagger_Groggy;
//    public int P_player_Stagger_Groggy { get { return player_Stagger_Groggy; } set { player_Stagger_Groggy = value; } }

//    //다운 상태에 빠지는 그로기값
//    [SerializeField]
//    private int player_Down_Groggy;
//    public int P_player_Down_Groggy { get { return player_Down_Groggy; } set { player_Down_Groggy = value; } }

//    //캐릭터 움직임 속도
//    [SerializeField]
//    private int player_MoveSpeed;
//    public int P_player_MoveSpeed { get { return player_MoveSpeed; } set { player_MoveSpeed = value; } }

//    //캐릭터 움직임 속도
//    [SerializeField]
//    private int player_RunSpeed;
//    public int P_player_RunSpeed { get { return player_RunSpeed; } set { player_RunSpeed = value; } }

//    [SerializeField]
//    private int player_MouseSpeed;
//    public int P_player_MouseSpeed { get { return player_MouseSpeed; } set { player_MouseSpeed = value; } }

//    [SerializeField]
//    private float player_RotSpeed;
//    public float P_player_RotSpeed { get { return player_RotSpeed; } set { player_RotSpeed = value; } }

//    [SerializeField]
//    private Vector2 player_UIPos;

//    public Vector2 P_player_UIPos { get { return player_UIPos; } set { player_UIPos = value; } }


//}
