# SETUP GUIDE — Dumpling67 Unity Core

## Требования

- Unity **2022.3 LTS** или **Unity 6**
- Template: **3D (URP)** или **URP**
- Платформы: WebGL (основная), Android/iOS опционально

## Импорт

1. Создай новый URP-проект.
2. Скопируй папки:
   - `Assets/Dumpling67` → `Assets/Dumpling67`
   - `Assets/WebGLTemplates` → `Assets/WebGLTemplates`
3. (Опционально) скопируй `Packages/manifest.json` зависимости, если нужны.

## Первый запуск

1. Открой пустую сцену (или File → New Scene).
2. Hierarchy → Create Empty → назови `Bootstrap`.
3. Add Component → `SceneBootstrap`.
4. Play.

`SceneBootstrap` создаст:
- Player (CharacterController + ThirdPersonPlayerController + ...)
- Camera + ThirdPersonCamera
- Floor, collectibles, finish/hazard zones
- UI (joystick, toast, ...)
- Managers (GameManager, Economy, ...)

## Ручная сборка

См. `docs/THIRD_PERSON_SETUP.md`.

## Animator

Меню: **Dumpling67 → Create Player Animator Controller**
(см. `docs/ANIMATOR_STATE_MACHINE.md`)

## WebGL

- Player Settings → Resolution and Presentation → WebGL Template = Dumpling67
- См. `docs/PUBLISHING_YANDEX.md` и `docs/PUBLISHING_TELEGRAM.md`
