using System.Net;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "ClientConfig", menuName = "Configs/ClientConfig")]
    public sealed class ClientConfig : ScriptableObject
    {
        [field: SerializeField] public string Ip { get; private set; }
        [field: SerializeField] public int Port { get; private set; }

        [SerializeField, HideInInspector] private string _validatedIp;
        public string ValidatedIp => _validatedIp;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(Ip))
            {
                return;
            }

            if (IPAddress.TryParse(Ip, out IPAddress address))
            {
                _validatedIp = address.ToString();
            }
        }
    }
}