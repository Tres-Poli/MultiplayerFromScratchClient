using Riptide;
using UnityEngine;

namespace Messages
{
    public struct MoveInputMessage : IMessageSerializable
    {
        public Vector3 Point;

        public void Serialize(Message message)
        {
            message.AddFloat(Point.x);
            message.AddFloat(Point.y);
            message.AddFloat(Point.z);
        }

        public void Deserialize(Message message)
        {
            Vector3 newDirection;
            newDirection.x = message.GetFloat();
            newDirection.y = message.GetFloat();
            newDirection.z = message.GetFloat();

            Point = newDirection;
        }
    }
}