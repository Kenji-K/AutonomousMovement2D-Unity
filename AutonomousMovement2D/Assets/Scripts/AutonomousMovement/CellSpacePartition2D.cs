using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kensai.AutonomousMovement {
    public class CellSpacePartition2D {
        public List<Cell2D> Cells { get; set; }

        float SpaceWidth;
        float SpaceHeight;

        float SpaceOriginX;
        float SpaceOriginY;

        float CellSizeX;
        float CellSizeY;

        int NumCellsX;
        int NumCellsY;

        public CellSpacePartition2D(float spaceWidth, float spaceHeight, int numCellsX, int numCellsY) {
            Init(spaceWidth, spaceHeight, numCellsX, numCellsY, 0, 0);
        }

        public CellSpacePartition2D(float spaceWidth, float spaceHeight, int numCellsX, int numCellsY, float spaceOriginX, float spaceOriginY) {
            Init(spaceWidth, spaceHeight, numCellsX, numCellsY, spaceOriginX, spaceOriginY);
        }

        private void Init(float spaceWidth, float spaceHeight, int numCellsX, int numCellsY, float spaceOriginX, float spaceOriginY) {
            var epsilon = 0.1f;
            SpaceWidth = spaceWidth + epsilon * 2;
            SpaceHeight = spaceHeight - epsilon * 2;
            SpaceOriginX = spaceOriginX - epsilon;
            SpaceOriginY = spaceOriginY - epsilon;
            NumCellsX = numCellsX;
            NumCellsY = numCellsY;

            CellSizeX = spaceWidth / numCellsX;
            CellSizeY = spaceHeight / numCellsY;

            Cells = new List<Cell2D>();

            for (int j = 0; j < numCellsY; j++) {
                for (int i = 0; i < numCellsX; i++) {
                    var newCell = new Cell2D(
                        new Vector2(spaceOriginX + i * CellSizeX, spaceOriginY + j * CellSizeY),
                        new Vector2(spaceOriginX + (i + 1) * CellSizeX, spaceOriginY + (j + 1) * CellSizeY));
                    Cells.Add(newCell);
                    
                }
            }
        }

        private int IndexFromPosition(Vector2 position) {
            var x = position.x;
            var y = position.y;
            int index = Mathf.FloorToInt((x - SpaceOriginX) / CellSizeX) + 
                        Mathf.FloorToInt((y - SpaceOriginY) / CellSizeY) * NumCellsX;

            if (index >= Cells.Count) index = Cells.Count - 1;

            return index;
        }

        public Cell2D CellFromPosition(Vector2 position) {
            return Cells[IndexFromPosition(position)];
        }

        public void EmptyCells() {
            foreach (var cell in Cells) {
                cell.Members.Clear();
            }
        }

        public void UpdateEntity(SteeringAgent2D entity, Vector2 previousPosition) {
            Vector2 currentPosition = entity.GetComponent<Rigidbody2D>().position;

            int prevIndex = IndexFromPosition(previousPosition);
            int curIndex = IndexFromPosition(currentPosition);

            if (prevIndex != curIndex) {
                Cells[prevIndex].Members.Remove(entity);
                Cells[curIndex].Members.Add(entity);
            }
        }

        public void AddEntity(SteeringAgent2D entity) {
            int index = IndexFromPosition(entity.transform.position);
            Cells[index].Members.Add(entity);
        }
    }
}
