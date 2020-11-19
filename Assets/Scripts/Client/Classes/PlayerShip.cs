using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip
{
    public string UserId { get; set; }
    public string Mesh { get; set; }
    public float Mass { get; set; }
    public float Hull { get; set; }
    public float Shields { get; set; }
    public float MaxSpeed { get; set; }
    public float MaxAfterburnerSpeed { get; set; }
    public float Acceleration { get; set; }
    public float AfterburnerAcceleration { get; set; }
    public float AfterburnerTime { get; set; }
    public float Pitch { get; set; }
    public float Roll { get; set; }
    public float Yaw { get; set; }
    public GunType Guns { get; set; }
    public Module[] Modules { get; set; }

    public PlayerShip()
    {

    }

}
