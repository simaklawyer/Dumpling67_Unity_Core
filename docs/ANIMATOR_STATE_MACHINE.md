# Animator State Machine (PelmenPlayer)

Создаётся меню: **Dumpling67 → Create Player Animator Controller**

## Parameters (CharacterAnimDriver)
| Name | Type |
|------|------|
| Speed | Float |
| MotionSpeed | Float |
| Grounded | Bool |
| VerticalVelocity | Float |
| IsMoving | Bool |
| IsSprinting | Bool |
| Jump | Trigger |
| Land | Trigger |

## States
- **Idle** (default)
- **Locomotion** (blend by Speed / IsSprinting)
- **Jump** ← AnyState on Jump trigger
- **Fall** ← Jump when VerticalVelocity < 0
- **Land** ← Fall when Grounded, then → Idle

## Transitions
- Idle ↔ Locomotion via IsMoving
- Any → Jump (Jump trigger)
- Jump → Fall (VerticalVelocity < 0)
- Fall → Land (Grounded)
- Land → Idle (exit time)

Клипы подставляются вручную в слоты состояний после импорта моделей.
