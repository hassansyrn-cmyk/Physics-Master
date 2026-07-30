using System;
namespace PhysicsMaster.Core {
    [Serializable] public sealed class LevelData {
        public int id, chapter, parStrokes;
        public string titleEn, titleAr, objective, hazard;
        public float timeLimit, inkLimit, ballX, ballY, goalX, goalY, gravity, friction, bounciness;
        public bool movingObstacle, reverseGravity;
    }
}
