using UnityEngine;
using System.Collections;

/// <summary>
/// 玩家登陆数据类(demo 根据各自项目表结构来定义)
/// </summary>
public class ModuleProfile
{
    public int userId;
    public int level;
    public int exp;
    public int token;
    public int energy;
    public int energyLimit;
    public int lastEnergyChargedTime;
    public int registerTime;
    public int lastLoginTime;

    /// <summary>
    /// 特殊解析
    /// </summary>
    /// <param name="fieldName">字段名称</param>
    /// <param name="stringValue">数据</param>
    public void ParseField(string fieldName, string stringValue)
    {
        
    }
}
