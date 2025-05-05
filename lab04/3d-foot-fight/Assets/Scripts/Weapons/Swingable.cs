using UnityEngine;

namespace Weapons
{
    public class Swingable : Weapon
    {
        [field: SerializeField] public float SwingTime { get; protected set; }
    }
}