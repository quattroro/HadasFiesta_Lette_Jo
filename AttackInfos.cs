using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
///������ �۾�
///���� CSV�� �����͸� �ҷ��Ծ�����
///��ȹ�� ��û���� �ν����Ϳ��� ���� �����ϵ���
///��Ʈ���ͺ� ������Ʈ�� ���������� �����ϵ��� ��
/////////////////////////////////////////////////////////////////////

[CreateAssetMenu(fileName = "CharacterAttackInfoList", menuName = "Scriptable Object/CharacterAttackInfoList")]
public class AttackInfos : ScriptableObject
{
    public List<AttackInfo> attackInfoList;
}
