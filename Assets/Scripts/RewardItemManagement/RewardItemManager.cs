using System;
using System.Collections.Generic;
using Events;
using Pooling;
using SceneManagement;
using Toolkit.Singletons;
using UnityEngine;

namespace RewardItemManagement
{
    public class RewardItemManager : SingletonBehaviour<RewardItemManager>
    {
        private readonly Dictionary<int, RewardItemRuntimeData> m_RuntimeData = new();

        private Dictionary<int, RewardItem> m_RewardItemsById;

        private void Awake()
        {
            if (!SetupInstance())
            {
                return;
            }

            GEM.AddListener<RewardItemEvent>(OnRequestItemRuntimeData, channel: (int)RewardItemEventType.RequestData);
            GEM.AddListener<RewardItemEvent>(OnRequestItemWithID, channel: (int)RewardItemEventType.RequestItemWithId);
            
            GEM.AddListener<SceneEvent>(OnUnloadScene, channel: (int)SceneEventType.Unload);
        }
        
        private void OnDisable()
        {
            GEM.RemoveListener<RewardItemEvent>(OnRequestItemRuntimeData, channel: (int)RewardItemEventType.RequestData);
            GEM.RemoveListener<RewardItemEvent>(OnRequestItemWithID, channel: (int)RewardItemEventType.RequestItemWithId);
            
            GEM.RemoveListener<SceneEvent>(OnUnloadScene, channel: (int)SceneEventType.Unload);
        }

        private void OnRequestItemRuntimeData(RewardItemEvent evt)
        {
            var rewardItem = evt.RewardItem;
            var runtimeData = GetOrCreateRuntimeData(rewardItem);
            evt.RuntimeData = runtimeData;
            evt.result = EventResult.Positive;
        }

        private void OnRequestItemWithID(RewardItemEvent evt)
        {
            evt.RuntimeData = GetOrCreateRuntimeData(evt.Id);
            evt.RewardItem = evt.RuntimeData.RewardItem;
            evt.result = EventResult.Positive;
        }

        private RewardItem GetItemWithId(int id)
        {
            if (m_RewardItemsById.TryGetValue(id, out var item))
            {
                return item;
            }

            throw new Exception($"No item exists with the given id: {id}");
        }

        private RewardItemRuntimeData GetRuntimeDataWithId(int id)
        {
            if (m_RuntimeData.TryGetValue(id, out var runtimeData))
                return runtimeData;

            throw new MissingReferenceException($"No runtime data exists with id {id}");
        }

        private RewardItemRuntimeData GetOrCreateRuntimeData(RewardItem item, bool create = true)
        {
            if (!m_RuntimeData.TryGetValue(item.Id, out var runtimeData))
            {
                if (!create)
                {
                    throw new Exception(
                        $"No runtime data associated with '{item.name}' found and create is not allowed");
                    return null;
                }

                runtimeData = AutoPool<RewardItemRuntimeData>.Get();
                runtimeData.Initialize(item);
                m_RuntimeData[item.Id] = runtimeData;
            }

            return runtimeData;
        }

        private RewardItemRuntimeData GetOrCreateRuntimeData(int itemId, bool create = true)
        {
            var item = GetItemWithId(itemId);
            return GetOrCreateRuntimeData(item, create);
        }

        private void ReleaseRuntimeData(RewardItem item)
        {
            var runtimeData = GetOrCreateRuntimeData(item, false);

            if (runtimeData == null) return;

            
            runtimeData.Dispose();
            AutoPool<RewardItemRuntimeData>.Release(runtimeData);
        }
        
        private void OnUnloadScene(SceneEvent evt)
        {
            if (evt.SceneId != SceneId.Barbeque) return;
            
            foreach (var rewardItemPair in m_RewardItemsById)
            {
                ReleaseRuntimeData(rewardItemPair.Value);
            }
        }
    }
}