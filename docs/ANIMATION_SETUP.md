# Animation Setup

## CharacterAnimDriver
- Assign motor (ThirdPersonPlayerController), Animator, visual Transform
- Param names: Speed, MotionSpeed, Grounded, VerticalVelocity, Jump, Land, IsMoving, IsSprinting
- Procedural fallback: bob + lean without clips (works out of the box)

## Create Animator Controller
Menu: **Dumpling67 → Create Player Animator Controller**
(see ANIMATOR_STATE_MACHINE.md)

## Clips needed
Idle, Walk, Run, Jump, Fall, Land, Collect (optional)
