# Art placeholders

В этом HANDOFF-пакете **нет** готовых моделей, текстур и анимационных клипов.

## Что нужно добавить

1. **Player model** (Humanoid preferred) — под `Assets/Dumpling67/Art/Characters/`
2. **Pelmen prefab** (collectible) — mesh + colliders
3. **Environment** — floor, props (low-poly / stylized)
4. **Animations**: Idle, Walk, Run, Jump, Fall, Collect, Win, Die
5. **UI sprites** — joystick, buttons, toast background, share card

## Рекомендации

- WebGL budget: см. `docs/WEBGL_PERFORMANCE_BUDGET.md`
- Анимации → Humanoid Avatar + Animator Controller (создаётся через Editor menu)
- Softbody / 2D-runner арт — только если возвращаетесь к frozen-системам

## Editor helpers

- `Dumpling67 → Create Player Animator Controller` — генерирует базовый state machine
