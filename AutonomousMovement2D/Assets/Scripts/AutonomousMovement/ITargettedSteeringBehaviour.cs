using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kensai.AutonomousMovement {
    public interface ITargettedSteeringBehaviour {
        SteeringAgent2D TargetAgent1 { get; set; }
        SteeringAgent2D TargetAgent2 { get; set; }
    }
}
