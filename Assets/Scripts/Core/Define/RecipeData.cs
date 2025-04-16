using System.Collections.Generic;
using UnityEngine;

public class RecipeData : Data
{
    public string result;
    public int workAmount;

    public bool canDoIfLackSkill;// if true, worker can do this recipe even if he/she lack skill from min level

    public string effectWork;//effect by skill of worker
    public float effectNegativeMultiEachPointFromMinLevel;//
    public float effectMultiEachPointLevel;//hiệu quả làm việc đối với mỗi điểm từ cấp độ tối thiểu
    public int requireMinLevel;
    public int requireMinLevelToLearn;
   

    public List<MatsRequired> matsList;
    public List<ToolQuality> orderToolQuality;
    public List<string> tags;
}

public class MatsRequired
{
    public string name;
    public int count;
}