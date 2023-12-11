using System.Collections.Generic;
using DataStructures;
using UnityEngine;

namespace Common.UI
{
    [CreateAssetMenu(fileName = "ScreenConfig", menuName = "Configs/ScreenConfig")]
    public class ScreenConfig : ScriptableObject
    {
        [SerializeField] private SerializableDictionary<ScreenType, GameObject> _uiPrefabs;
        public IDictionary<ScreenType, GameObject> UiPrefabs => _uiPrefabs;
    }
}