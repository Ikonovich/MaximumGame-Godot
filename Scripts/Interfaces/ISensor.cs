using Godot;
using System;

interface ISensor {

    void TargetDetected(Area target);

    void TargetGone(Area target);

}