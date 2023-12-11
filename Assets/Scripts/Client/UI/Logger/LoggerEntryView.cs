using TMPro;
using UnityEngine;

namespace Common.UI.Logger
{
    internal sealed class LoggerEntryView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        public void SetText(string text)
        {
            _text.text = text;
        }
    }
}