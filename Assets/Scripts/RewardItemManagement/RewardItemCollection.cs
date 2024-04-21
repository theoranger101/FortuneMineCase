using System.Collections.Generic;
using UnityEngine;

namespace RewardItemManagement
{
    public class RewardItemCollection : ScriptableObject
    {
        public List<RewardItem> Items = new List<RewardItem>();
    }
}