# NavMeshAgent Ladder Movement 

In this tutorial repository and [associated video](https://youtu.be/8eNk_5oRxbg) you will learn how to set up OffMeshLinks via the NavMeshLink component to add ladders into your game!
NavMeshAgents will correctly position themselves, mount the ladder, climb the ladder, and dismount the ladder in a relatively smooth sequence.

[![Youtube Tutorial](./Video%20Screenshot.jpg)](https://youtu.be/8eNk_5oRxbg)

## Quickstart
> **WARNING**: This repository **does not ship with ladder animations**. In the tutorial video I am using [Protofactor's Ultimate Animation Collection]() (affiliate link) ladder movement. Any of your preferred ladder animations will work but may require some tweaks.

The following assumes you have skipped the tutorial video altogether.

1. Open the `Assets/Scenes/SampleScene.unity` in a modern Unity Editor (Built with Unity 6).
2. Import your preferred ladder animations. If you import Protofactor's, the Animator will be automatically configured.
3. Attach ladder bottom/top mount/dismount clips to the `LadderAnimationInfo` on `The Dude` GameObject 
   1. Update the Animator Controller on `The Dude` to use your appropriate bottom/top mount/dismount and climb up/down clips. The Animator expects 6 clips.
4. Click play
5. Send `The Dude` near the ladder by left clicking.
6. Send `The Dude` to the top of the ladder by left clicking on top of the structure.
7. Send `The Dude` to the bottom of the ladder by left clicking back on the floor.

## Key Components
`AgentLinkMover.cs` - a heavily modified version of what Unity provided as a demo script with the Navigation Components back in 2019. This is how the scripted ladder climbing sequence is performed.
`LadderAnimationInfo.cs` - a struct to hold the clips so we don't have to introspect the Animator and can wait the appropriate amount of time before lerping the climb up/down position.
`PlayerMovement.cs` - Synchronizes the Root Motion Animation & NavMeshAgent as done in AI Series 42. Also listens for mouse clicks to set the NavMeshAgent destination.

## Further Resources
1. [AI Series Part 1](https://youtu.be/aHFSDcEQuzQ) - Left Click to Move a NavMeshAgent & Setting up the Navigation Components (installation can be done via Package Manager now).
2. [AI Series Part 2](https://youtu.be/dpJUc_BpChw) - Introduction to NavMeshLinks
3. [AI Series 17](https://youtu.be/PD6VFD1a21g) - AgentLinkMover implementation
4. [AI Series 42](https://youtu.be/uAGjKxH4sDQ) - Root Motion & NavMeshAgents
5. [8 Inverse Kinematic Solutions Compared](https://youtu.be/jOp52NQMcTo) - Some Unity IK Solutions compared and recommended.   

## Patreon Supporters
Have you been getting value out of these tutorials? Do you believe in LlamAcademy's mission of helping everyone make their game dev dream become a reality? Consider becoming a Patreon supporter and get your name added to this list, as well as other cool perks.
Head over to https://patreon.com/llamacademy to show your support.

### Phenomenal Supporter Tier
* YOUR NAME HERE!

### Tremendous Supporter Tier
* YOUR NAME HERE!

### Awesome Supporter Tier
* Mustafa
* Ivan
* Iffy Obelus 
* Snedden
* YOUR NAME HERE!

### Supporters
* AudemKay
* Matt Sponholz
* Tarik Ahmed
* Elijah Singer
* Bruno Bozic
* Josh Meyer
* Ben
* Christiaan van Steenwijk
* StrangeSwelter
* ChimeraDev
* Lukas Wolfe
* Jason Hansen
* angell
* Warspawn
* Ewald Schulte
* Wendy Whitner
* Ralevum
* YOUR NAME HERE!

## Other Projects
Interested in other AI Topics in Unity, or other tutorials on Unity in general? 

* [Check out the LlamAcademy YouTube Channel](https://youtube.com/c/LlamAcademy)!
* [Check out the LlamAcademy GitHub for more projects](https://github.com/llamacademy)

## Requirements
* Should work on any modern version of Unity. Built with Unity 6.
* [Navigation Components](https://docs.unity3d.com/Manual/NavMesh-BuildingComponents.html) - Can be installed from the Package Manager in modern versions of Unity.
