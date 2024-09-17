using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System;
using System.Collections.Generic;
using Unity.AI.Navigation;

namespace LlamAcademy.AI
{
    public enum OffMeshLinkMoveMethod
    {
        Teleport,
        NormalSpeed,
        Parabola,
        Curve,
        Ladder
    }

    [RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
    public class AgentLinkMover : MonoBehaviour
    {
        [field: SerializeField] public OffMeshLinkMoveMethod DefaultMoveMethod { get; private set; } = OffMeshLinkMoveMethod.Parabola;
        [field: SerializeField] public AnimationCurve Curve { get; private set; } = new ();
        [field: SerializeField] [field: Range(0, 5)] public float ParabolaHeight { get; private set; } = 8f;

        [SerializeField] private List<LinkTraversalConfiguration> NavMeshLinkTraversalTypes = new ();
        [SerializeField] private LadderAnimationInfo LadderMountInfo;

        public delegate void LinkEvent(OffMeshLinkMoveMethod moveMethod);
        public event LinkEvent OnLinkStart;
        public event LinkEvent OnLinkEnd;

        private NavMeshAgent Agent;
        private Animator Animator;

        private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            Animator = GetComponent<Animator>();
        }

        private IEnumerator Start()
        {
            Agent.autoTraverseOffMeshLink = false;
            while (true)
            {
                if (!Agent.isOnOffMeshLink)
                {
                    yield return null;
                    continue;
                }

                OffMeshLinkData offMeshLinkData = Agent.currentOffMeshLinkData;
                NavMeshLink link = (NavMeshLink)Agent.navMeshOwner;
                int areaType = link.area;

                if (Vector3.Distance(offMeshLinkData.endPos, Agent.destination) <
                    Vector3.Distance(offMeshLinkData.startPos, Agent.destination))
                {
                    LinkTraversalConfiguration configuration =
                        NavMeshLinkTraversalTypes.Find((type) => type.AreaType == areaType);

                    if (configuration is { MoveMethod: OffMeshLinkMoveMethod.NormalSpeed } ||
                        (configuration == null && DefaultMoveMethod == OffMeshLinkMoveMethod.NormalSpeed))
                    {
                        OnLinkStart?.Invoke(OffMeshLinkMoveMethod.NormalSpeed);
                        yield return StartCoroutine(MoveAtNormalSpeed());
                        OnLinkEnd?.Invoke(OffMeshLinkMoveMethod.NormalSpeed);
                    }
                    else if (configuration is { MoveMethod: OffMeshLinkMoveMethod.Parabola } ||
                             (configuration == null && DefaultMoveMethod == OffMeshLinkMoveMethod.Parabola))
                    {
                        OnLinkStart?.Invoke(OffMeshLinkMoveMethod.Parabola);
                        yield return StartCoroutine(MoveParabola(Vector3.Distance(offMeshLinkData.startPos, offMeshLinkData.endPos) / Agent.speed));
                        OnLinkEnd?.Invoke(OffMeshLinkMoveMethod.Parabola);
                    }
                    else if (configuration is { MoveMethod: OffMeshLinkMoveMethod.Curve } ||
                             (configuration == null && DefaultMoveMethod == OffMeshLinkMoveMethod.Curve))
                    {
                        OnLinkStart?.Invoke(OffMeshLinkMoveMethod.Curve);
                        yield return StartCoroutine(MoveCurve(Vector3.Distance(offMeshLinkData.startPos, offMeshLinkData.endPos) / Agent.speed));
                        OnLinkEnd?.Invoke(OffMeshLinkMoveMethod.Curve);
                    }
                    else if (configuration is { MoveMethod: OffMeshLinkMoveMethod.Teleport } ||
                             (configuration == null && DefaultMoveMethod == OffMeshLinkMoveMethod.Teleport))
                    {
                        OnLinkStart?.Invoke(OffMeshLinkMoveMethod.Teleport);
                        OnLinkEnd?.Invoke(OffMeshLinkMoveMethod.Teleport);
                    }
                    else if (configuration is { MoveMethod: OffMeshLinkMoveMethod.Ladder } ||
                             (configuration == null && DefaultMoveMethod == OffMeshLinkMoveMethod.Ladder))
                    {
                        OnLinkStart?.Invoke(OffMeshLinkMoveMethod.Ladder);
                        yield return StartCoroutine(MoveLadder(offMeshLinkData));
                        OnLinkEnd?.Invoke(OffMeshLinkMoveMethod.Ladder);
                    }
                }

                Agent.CompleteOffMeshLink();
                yield return null;
            }
        }

        private IEnumerator MoveAtNormalSpeed()
        {
            OffMeshLinkData data = Agent.currentOffMeshLinkData;
            Vector3 endPosition = data.endPos + Vector3.up * Agent.baseOffset;
            yield return StartCoroutine(MoveToTargetAtNormalSpeed(endPosition, Agent.stoppingDistance));
        }

        private IEnumerator MoveToTargetAtNormalSpeed(Vector3 target, float threshold = 0.01f)
        {
            while (Vector3.Distance(Agent.transform.position, target) > threshold)
            {
                Agent.transform.position = Vector3.MoveTowards(Agent.transform.position, target, Agent.speed * Time.deltaTime                );
                yield return null;
            }
        }

        private IEnumerator MoveParabola(float duration)
        {
            OffMeshLinkData data = Agent.currentOffMeshLinkData;
            Vector3 startPosition = Agent.transform.position;
            Vector3 endPosition = data.endPos + Vector3.up * Agent.baseOffset;
            float normalizedTime = 0.0f;
            while (normalizedTime < 1.0f)
            {
                float yOffset = ParabolaHeight * (normalizedTime - normalizedTime * normalizedTime);
                Agent.transform.position = Vector3.Lerp(startPosition, endPosition, normalizedTime) + yOffset * Vector3.up;
                normalizedTime += Time.deltaTime / duration;
                yield return null;
            }
        }

        private IEnumerator MoveCurve(float duration)
        {
            OffMeshLinkData data = Agent.currentOffMeshLinkData;
            Vector3 startPosition = Agent.transform.position;
            Vector3 endPosition = data.endPos + Vector3.up * Agent.baseOffset;
            float normalizedTime = 0.0f;
            while (normalizedTime < 1.0f)
            {
                float yOffset = Curve.Evaluate(normalizedTime);
                Agent.transform.position = Vector3.Lerp(startPosition, endPosition, normalizedTime) + yOffset * Vector3.up;
                normalizedTime += Time.deltaTime / duration;
                yield return null;
            }
        }

        private IEnumerator MoveLadder(OffMeshLinkData data)
        {
            Animator.applyRootMotion = false;
            Agent.updatePosition = false;
            Agent.updateRotation = false;

            float ladderDirection = data.endPos.y - data.startPos.y;
            Vector3 ladderStartPosition = ladderDirection > 0 ?
                data.startPos
                : new Vector3(data.endPos.x, data.startPos.y, data.endPos.z);

            Vector3 ladderEndPosition = ladderDirection > 0 ?
                new Vector3(ladderStartPosition.x, data.endPos.y, ladderStartPosition.z)
                : data.endPos;

            Vector3 ladderForward = ladderDirection > 0 ?
                (new Vector3(data.endPos.x, 0, data.endPos.z) - new Vector3(data.startPos.x, 0, data.startPos.z)).normalized
                : (new Vector3(data.startPos.x, 0, data.startPos.z) - new Vector3(data.endPos.x, 0, data.endPos.z)).normalized;

            // move agent to starting ladder position
            yield return StartCoroutine(MoveToTargetAtNormalSpeed(ladderStartPosition));
            // look at ladder forward
            yield return StartCoroutine(LookAtTargetForward(ladderForward));

            // play animation to mount ladder & wait for the agent to be mounted
            Animator.SetBool(AnimatorConstants.IS_ON_LADDER, true);
            Animator.SetFloat(AnimatorConstants.LADDER_DIRECTION, ladderDirection);
            yield return null;
            yield return new WaitForSeconds(ladderDirection > 0
                ? LadderMountInfo.BottomLadderMount != null ? LadderMountInfo.BottomLadderMount.length : 0
                : LadderMountInfo.TopLadderMount != null ? LadderMountInfo.TopLadderMount.length : 0
            );

            // move towards the end position at reduced agent speed
            yield return StartCoroutine(MoveOnLadder(ladderStartPosition, ladderEndPosition));

            // play animation to dismount ladder & wait for the agent to be dismounted
            Animator.SetBool(AnimatorConstants.IS_ON_LADDER, false);
            Animator.SetFloat(AnimatorConstants.LADDER_DIRECTION, 0);
            yield return new WaitForSeconds(ladderDirection > 0
                ? LadderMountInfo.TopLadderDismount != null ? LadderMountInfo.TopLadderDismount.length : 0
                : LadderMountInfo.BottomLadderDismount != null ? LadderMountInfo.BottomLadderDismount.length : 0
            );

            // re-enable the root motion update position & rotation
            Animator.applyRootMotion = true;
            Agent.updatePosition = false;
            Agent.updateRotation = true;
        }

        private IEnumerator MoveOnLadder(Vector3 ladderStartPosition, Vector3 ladderEndPosition)
        {
            float normalizedTime = 0;
            float speed = (ladderStartPosition - ladderEndPosition).magnitude / Agent.speed / 6f;

            while (normalizedTime < 1)
            {
                Agent.transform.position = Vector3.Lerp(ladderStartPosition, ladderEndPosition, normalizedTime);
                normalizedTime += Time.deltaTime * speed;
                yield return null;
            }

            Agent.transform.position = ladderEndPosition;
        }

        private IEnumerator LookAtTargetForward(Vector3 forward)
        {
            Quaternion targetRotation = Quaternion.LookRotation(forward);
            Quaternion startRotation = Agent.transform.rotation;

            float time = 0;
            while (time < 1)
            {
                Agent.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, time);
                time += Time.deltaTime * 4;
                yield return null;
            }

            Agent.transform.rotation = targetRotation;
        }

        [Serializable]
        public class LinkTraversalConfiguration
        {
            public OffMeshLinkMoveMethod MoveMethod;
            public int AreaType;
        }
    }
}
