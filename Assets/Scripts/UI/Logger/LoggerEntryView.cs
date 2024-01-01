using TMPro;
using UnityEngine;

namespace UI
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