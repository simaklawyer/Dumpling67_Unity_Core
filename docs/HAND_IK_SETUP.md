# Hand IK Setup

## Humanoid
1. Avatar = Humanoid
2. Animator layer: IK Pass = On
3. Add `AnimatorHandIK` on the same GO as Animator
4. Assign hand targets / look target

## Generic / non-humanoid (пельмень)
1. Hierarchy: boneA (shoulder) → boneB (elbow) → boneC (hand)
2. Add `TwoBoneIK` per arm, assign bones + target + pole
3. Add `HandIkController` — auto-reach to tag `Interactable` within radius

## Auto-reach
- CollectiblePelmen should have tag Interactable (or set interactableMask)
- HandIkController blends weight toward nearest left/right targets
