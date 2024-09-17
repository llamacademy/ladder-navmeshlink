using UnityEngine;

namespace LlamAcademy.AI
{
    public static class AnimatorConstants
    {
        public static int VELOCITY_X = Animator.StringToHash("velocityX");
        public static int VELOCITY_Y = Animator.StringToHash("velocityY");
        public static int MOVE = Animator.StringToHash("move");
        public static int IS_ON_LADDER = Animator.StringToHash("isOnLadder");
        public static int LADDER_DIRECTION = Animator.StringToHash("ladderDirection");
    }
}
