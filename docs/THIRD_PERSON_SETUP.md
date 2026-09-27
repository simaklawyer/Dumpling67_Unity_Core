# Third-Person «как в Roblox» — сборка сцены

Это **основной** режим игры.

## 1. Игрок
1. Capsule (или модель) → Tag: **Player**
2. CharacterController + ThirdPersonPlayerController + CharacterAnimDriver + JuiceFeedback
3. Дочерний Visual для bob/lean

## 2. Камера
Main Camera → ThirdPersonCamera, target = игрок, distance ≈ 6.5

## 3. Ввод
ПК: WASD + мышь, Space, Shift.
Мобайл: VirtualJoystick + LookTouchPad на Canvas.

## 4. Звук
GO GameAudio → GameAudio (procedural beep без клипов).

## 5. Мир
Plane + CollectiblePelmen + HazardZone + FinishZone.

## 6. Быстрый старт
SceneBootstrap на пустой сцене → Play.
