# Dumpling67 — HANDOFF для следующего ИИ / разработчика

**Дата среза:** 2026-09-26  
**Статус:** код-пакет ядра Unity (не полный `.unity` проект со сценами). Скрипты + WebGL template + docs.  
**Целевой движок:** Unity 2022.3 LTS / Unity 6, URP, WebGL-first.

---

## 1. Что это за продукт

Мем-казуалка **Dumpling 67** (пельмени, гача, раннер, коллекция).

**Исходник пользователя:** Android Kotlin + Jetpack Compose + Room (`Dumpling67-main.zip` у автора).
План: перенос на Unity для WebGL (Яндекс.Игры, Telegram Mini App, VK) + потенциально Android/iOS.

---

## 2. Структура пакета

```
Dumpling67_Unity_Core/
├── Assets/
│   ├── Dumpling67/
│   │   ├── Scripts/          # весь C# код
│   │   ├── Editor/           # меню + билдеры
│   │   ├── Plugins/WebGL/    # jslib
│   │   ├── Shaders/
│   │   └── Art/              # placeholder
│   └── WebGLTemplates/Dumpling67/
├── docs/
├── Packages/manifest.json
├── HANDOFF.md
├── README.md
├── SETUP_GUIDE.md
└── VERSION.txt
```

---

## 3. Быстрый старт (Play Mode без сцен)

1. Unity 2022.3 LTS или 6, URP template.
2. Скопировать `Assets/Dumpling67` и `Assets/WebGLTemplates`.
3. Пустая сцена → создать GO → добавить `SceneBootstrap` → Play.
   Bootstrap сам создаёт player, camera, floor, collectibles, UI, managers.

Или пошагово: `docs/THIRD_PERSON_SETUP.md`.

---

## 4. Архитектура (ключевые классы)

### Bootstrap
- `SceneBootstrap` — entry point, спавнит всё для тестового уровня.

### Core
- `GameManager` — singleton, state machine (Menu / Playing / Paused / GameOver).
- `SaveSystem` — PlayerPrefs + JSON serialization.
- `GameEvents` — static events (OnPelmenCollected, OnLevelComplete, ...).

### Gameplay / ThirdPerson (основной режим)
- `ThirdPersonPlayerController` — CharacterController + input (joystick / keyboard).
- `ThirdPersonCamera` — orbit / follow.
- `CharacterAnimDriver` — Animator params (Speed, IsGrounded, Jump, ...).
- `VirtualJoystick` + `LookTouchPad` — mobile input.
- `CollectiblePelmen`, `FinishZone`, `HazardZone`.
- `GameAudio`, `JuiceFeedback`.
- IK: `HandIkController`, `TwoBoneIK`, `AnimatorHandIK`.

### Managers
- `EconomyManager`, `GachaManager`, `QuestManager`, `AdsManager`, `IapManager`.

### Data (ScriptableObjects)
- `PelmenDataSO`, `BlindBoxSO`, `EconomyBalanceSO`, `QuestDataSO`.

### Integrations
- `YandexGamesBridge`, `TelegramWebAppBridge`, `VkGamesBridge`.
- `Plugins/WebGL/TelegramBridge.jslib`.

### Legacy / Frozen 2D
- `_Frozen2D/`: SoftBodyPelmen2D, RunnerController и т.д. — не основной путь.

---

## 5. Что уже работает в Play Mode

- Движение third-person (WASD + мышь / джойстик).
- Сбор пельменей, finish zone, hazard.
- Простая анимация (через AnimDriver).
- Toast UI, share card (stub).
- Метрики (console).
- Bootstrap создаёт полноценный тестовый уровень.

---

## 6. Что НЕ входит в пакет

- Готовые `.unity` сцены и prefabs (кроме того, что bootstrap генерирует runtime).
- Арт, модели, анимации (только README_ART + Editor builder для Animator Controller).
- Полный URP project settings / Quality / Input System asset.
- Реальные SDK Яндекса / Telegram (только bridge-обёртки).

---

## 7. Следующие шаги (рекомендуемый порядок)

1. Импорт + Play через SceneBootstrap — убедиться, что движение/сбор работает.
2. Создать Animator Controller через Editor menu `Dumpling67 → Create Player Animator Controller`.
3. Подключить реальные модели/анимации (см. ANIMATION_SETUP, ANIMATOR_STATE_MACHINE, HAND_IK_SETUP).
4. Собрать WebGL, проверить template + bridges (PUBLISHING_YANDEX, PUBLISHING_TELEGRAM).
5. Баланс гачи: Editor `GachaBalanceSimulator`.
6. Performance budget: `docs/WEBGL_PERFORMANCE_BUDGET.md`.

---

## 8. Важные заметки

- Код ориентирован на WebGL-first (mobile touch + desktop).
- Нет зависимости от Input System package (используется legacy Input + UI EventSystem).
- Softbody 2D и fever runner — experimental / frozen, не ломать основной third-person.
- Все менеджеры — singleton-ish через FindObjectOfType / static Instance где нужно.

---

## 9. Контакты / контекст

Пакет подготовлен как HANDOFF для продолжения разработки. Если продолжаешь — начинай с HANDOFF.md и SceneBootstrap.
