using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kensai.Util;

namespace Kensai.AutonomousMovement {
    [ExecuteInEditMode]
    public class World2D : MonoBehaviour {
        public float worldSizeY;
        public float worldSizeX;

        public IntVector2 initialSpacePartition;

        public bool drawGizmos;
        public bool wrapAround;
        [Tooltip("These are the default values used on instantiation of steering agents and steering behaviours.")]
        public SteeringBehaviorSettings DefaultSettings = new SteeringBehaviorSettings();

        private static World2D _instance = null;
        public static World2D Instance {
            get { 
                if (_instance == null) {
                    _instance = GameObject.FindObjectOfType<World2D>();
                    DontDestroyOnLoad(_instance);
                }
                return _instance;
            }
            private set { _instance = value; }
        }

        private List<Wall2D> walls = new List<Wall2D>();
        public List<Wall2D> Walls {
            get { return walls; }
            set { walls = value; }
        }

        private List<CircleCollider2D> obstacles = new List<CircleCollider2D>();
        public List<CircleCollider2D> Obstacles {
            get { return obstacles; }
            set { obstacles = value; }
        }

        private List<SteeringAgent2D> agentList = new List<SteeringAgent2D>();
        public List<SteeringAgent2D> AgentList {
            get { return agentList; }
            set { agentList = value; }
        }

        private CellSpacePartition2D spacePartition = null;
        public CellSpacePartition2D SpacePartition {
            get { return spacePartition; }
            set { spacePartition = value; }
        }

        [ExecuteInEditMode]
        void Awake() {
            Application.targetFrameRate = -1;
            if (_instance != null) {
                Debug.Log("Destroying previous World2D instance.");
                Destroy(_instance);
            }

            _instance = this;
            DontDestroyOnLoad(this);

            if (initialSpacePartition.x > 0 && initialSpacePartition.y > 0) { 
                SpacePartition = new CellSpacePartition2D(worldSizeX, worldSizeY, initialSpacePartition.x, initialSpacePartition.y);
            }

            Obstacles = FindObjectsOfType<CircleCollider2D>().Where(o => o.tag == "Obstacle").ToList();
        }

        void OnDrawGizmos() {
            if (drawGizmos) {
                foreach (var wall in Walls) {
                    Gizmos.DrawLine(wall.From, wall.To);
                }

                if (spacePartition != null) { 
                    foreach (var cell in spacePartition.Cells) {
                        Gizmos.color = Color.green;
                        Gizmos.DrawWireCube(cell.Rect.center, new Vector3(cell.Rect.size.x, cell.Rect.size.y, 0.5f));
                    }
                }
            }
        }
    }

    [Serializable]
    public class SteeringBehaviorSettings {
        [Header("General Settings")]
        public bool SmoothHeadingOn = true;
        public int NumSamplesForSmoothing = 10;

        public float DefaultNeighborRadius = 2.5f;
        public float ArriveSlowingDistance = 1f;
        public float HidingDistanceFromObstacle = 1f;

        public float MinDetectionBoxLength = 1f;
        public float MainWallDetectionFeelerLength = 1f;

        [Header("Steering Behaviour Weights")]
        public float SeparationWeight = 1.0f;
        public float AlignmentWeight = 1.0f;
        public float CohesionWeight = 2.0f;
        public float ObstacleAvoidanceWeight = 10.0f;
        public float WallAvoidanceWeight = 10.0f;
        public float WanderWeight = 1.0f;
        public float SeekWeight = 1.0f;
        public float FleeWeight = 1.0f;
        public float ArriveWeight = 1.0f;
        public float PursuitWeight = 1.0f;
        public float OffsetPursuitWeight = 1.0f;
        public float InterposeWeight = 1.0f;
        public float HideWeight = 1.0f;
        public float EvadeWeight = 0.01f;
        public float FollowPathWeight = 0.05f;

        [Header("Steering Behaviour Probabilities")]
        [Range(0, 1)]
        public float WallAvoidanceProb = 0.5f;
        [Range(0, 1)]
        public float ObstacleAvoidanceProb = 0.5f;
        [Range(0, 1)]
        public float SeparationProb = 0.2f;
        [Range(0, 1)]
        public float AlignmentProb = 0.3f;
        [Range(0, 1)]
        public float CohesionProb = 0.6f;
        [Range(0, 1)]
        public float WanderProb = 0.8f;
        [Range(0, 1)]
        public float SeekProb = 0.8f;
        [Range(0, 1)]
        public float FleeProb = 0.6f;
        [Range(0, 1)]
        public float EvadeProb = 1.0f;
        [Range(0, 1)]
        public float HideProb = 0.8f;
        [Range(0, 1)]
        public float ArriveProb = 0.5f;
    }
}
