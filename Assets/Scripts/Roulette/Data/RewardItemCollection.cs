using System.Collections.Generic;
using UnityEngine;

namespace Roulette
{
    public class RewardItemCollection : ScriptableObject
    {
        public List<RewardItemData> Items = new List<RewardItemData>();
    }
}