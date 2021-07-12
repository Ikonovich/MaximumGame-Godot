using System;
using Godot;


interface IWeaponized {

    Spatial CurrentTarget { get; set; }

    void Shoot();

}