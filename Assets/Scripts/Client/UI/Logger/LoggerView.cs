using System.Collections.Generic;
using UnityEngine;

namespace Common.UI.Logger
{
    public sealed class LoggerView : UiView
    {
        [SerializeField] private RectTransform _entriesContainer;
        [SerializeField] private LoggerEntryView _entryPrefab;

        [SerializeField] private float _padding;

        private List<RectTransform> _entriesList;
        private Vector3 _offset;

        public override void Initialize()
        {
            base.Initialize();
            _entriesList = new List<RectTransform>(10);
            _offset = new Vector2(0f, -((RectTransform)_entryPrefab.transform).rect.height + _padding);
        }

        public override void Finite()
        {
            for (int i = 0; i < _entriesList.Count; i++)
            {
                Destroy(_entriesList[i].gameObject);
            }

            _entriesList.Clear();
            _entriesList = null;
        }

        public void AddEntry(string message)
        {
            LoggerEntryView newEntry = Instantiate(_entryPrefab, _entriesContainer);
            RectTransform entryTransform = ((RectTransform)newEntry.transform);
            entryTransform.anchoredPosition = Vector2.zero;
            newEntry.SetText(message);

            for (int i = 0; i < _entriesList.Count; i++)
            {
                _entriesList[i].localPosition += _offset;
            }
            
            _entriesList.Add(entryTransform);
        }
    }
}