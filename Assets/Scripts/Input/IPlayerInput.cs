using System;
using UnityEngine;

namespace Input
{
    public interface IPlayerInput
    {
        event Action<Vector3> OnMouseInput;
    }
}