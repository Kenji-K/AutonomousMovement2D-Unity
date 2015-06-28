using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kensai.Util.Extensions;

namespace Kensai.AutonomousMovement {
    public class WallAvoidance2D : SteeringBehaviour2D {
        public float feelerLength = 1; //TODO -> Decidir si deberia tener longitud dependiendo de su velocidad
        public bool DrawGizmos = true;
        private Vector2[] feelers;
        private Vector2 calculatedVelocity;

        private const float SIDE_FEELER_PROPORTION = 0.75f;
        private const float SIDE_FEELER_ANGLE = 45;

        void Reset() {
            if (World2D.Instance != null) {
                Weight = World2D.Instance.DefaultSettings.WallAvoidanceWeight;
                Probability = World2D.Instance.DefaultSettings.WallAvoidanceProb;
                feelerLength = World2D.Instance.DefaultSettings.MainWallDetectionFeelerLength;
                DrawGizmos = World2D.Instance.drawGizmos;
            }
        }

        static Vector2[] CreateFeelers(SteeringAgent2D agent, float feelerLength) {
            var feelers = new Vector2[3];
            feelers[0] = agent.transform.TransformPoint(Vector2.up * feelerLength);
            feelers[1] = agent.transform.TransformPoint(Quaternion.AngleAxis(-SIDE_FEELER_ANGLE, Vector3.back) * Vector2.up * feelerLength * SIDE_FEELER_PROPORTION);
            feelers[2] = agent.transform.TransformPoint(Quaternion.AngleAxis(SIDE_FEELER_ANGLE, Vector3.back) * Vector2.up * feelerLength * SIDE_FEELER_PROPORTION);
            return feelers;
        }

        public override Vector2 GetVelocity() {
            if (DrawGizmos) { 
                feelers = CreateFeelers(agent, feelerLength);
            }
            calculatedVelocity = GetVelocity(agent, feelerLength);
            return calculatedVelocity;
        }

        public static Vector2 GetVelocity(SteeringAgent2D agent, float feelerLength) {
            var feelers = CreateFeelers(agent, feelerLength);

            Vector2 steeringForce = Vector2.zero;


            foreach (var feeler in feelers) {
                int currentWallIndex = 0;
                int closestWallIndex = -1;
                float distToThisIP = 0f;
                float distToClosestIP = float.MaxValue;
                Vector2 point = Vector2.zero;
                Vector2 closestIP = Vector2.zero;

                Vector2 feelerSteeringForce = Vector2.zero;

                foreach (var wall in World2D.Instance.Walls) {
                    try {
                        Vector2 feelerFrom = agent.transform.position;
                        Vector2 feelerTo = feeler;
                        //if (MathfExtensions.SegmentIntersection2D(feelerFrom, feelerTo, wall.From, wall.To, out distToThisIP, out point)) {
                        if (MathfExtensions.LineSegementsIntersect(feelerFrom, feelerTo, wall.From, wall.To, out point)) {
                            distToThisIP = (feelerFrom - point).magnitude;
                            if (distToThisIP < distToClosestIP) {
                                distToClosestIP = distToThisIP;
                                closestWallIndex = currentWallIndex;
                                closestIP = point;
                            }
                        }
                        currentWallIndex++;
                    } catch {
                        //Do nothing
                    }
                } //Next wall

                if (closestWallIndex >= 0) {
                    Vector2 overShoot = (feeler - closestIP) * agent.Rigidbody2D.velocity.magnitude;
                    var closestWall = World2D.Instance.Walls[closestWallIndex];
                    var feelerDirection = feeler - agent.Rigidbody2D.position;
                    if (Vector2.Angle(feelerDirection, closestWall.Normal) > 90) {
                        feelerSteeringForce = closestWall.Normal * overShoot.magnitude;
                    } else {
                        feelerSteeringForce = closestWall.InverseNormal * overShoot.magnitude;
                    }
                }

                steeringForce += feelerSteeringForce;
            } //Next feeler

            return steeringForce;
        }

        void OnDrawGizmos() {
            if (DrawGizmos) {
                if (feelers != null && feelers.Count() > 0) {
                    Gizmos.color = Color.green;
                    foreach (var feeler in feelers) {
                        Gizmos.DrawLine(agent.Rigidbody2D.position, feeler);
                    }
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(agent.Rigidbody2D.position, agent.Rigidbody2D.position + calculatedVelocity);
                }
            }
        }

        public override int CalculationOrder {
            get { return 1; }
        }
    }
}
