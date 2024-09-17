using UnityEngine;

namespace LlamAcademy.AI
{
    [System.Serializable]
    public struct LadderAnimationInfo
    {
        [field: SerializeField] public AnimationClip TopLadderMount { get; private set; }
        [field: SerializeField] public AnimationClip TopLadderDismount { get; private set; }
        [field: SerializeField] public AnimationClip BottomLadderMount { get; private set; }
        [field: SerializeField] public AnimationClip BottomLadderDismount { get; private set; }
    }
}
