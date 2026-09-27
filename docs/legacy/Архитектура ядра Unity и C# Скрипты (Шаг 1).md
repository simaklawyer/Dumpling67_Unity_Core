# Dumpling67 Unity Migration: Шаг 1 — Базовая архитектура данных и ядра C#

## 1. Структура проекта в Unity (`Assets/`)

```
Assets/
├── Dumpling67/
│   ├── Scripts/
│   │   ├── Core/          # GameManager, GameEvents, SaveSystem
│   │   ├── Data/          # ScriptableObjects
│   │   ├── Managers/      # Economy, Gacha, Quest, Ads, Iap
│   │   ├── Gameplay/      # ThirdPerson + _Frozen2D
│   │   ├── Integrations/  # Yandex, Telegram, VK
│   │   ├── UI/
│   │   ├── Analytics/
│   │   └── Bootstrap/
│   ├── Editor/
│   ├── Plugins/WebGL/
│   ├── Shaders/
│   └── Art/
└── WebGLTemplates/Dumpling67/
```

## 2. Слой данных: ScriptableObjects

### 2.1 PelmenDataSO
- pelmenId, displayName, icon, rarity, baseValue, prefab

### 2.2 BlindBoxSO
- boxId, priceSoft/Hard, possibleRewards[], weights[], GetRandomDrop()

### 2.3 EconomyBalanceSO / QuestDataSO
- стартовые валюты, награды, типы квестов

## 3. Ядро: шина событий и сохранения

### 3.1 GameEvents
Статический event bus: OnCoinsChanged, OnPelmenUnlocked, OnBoxOpened, OnQuestProgress, OnToastTriggered, OnSaveRequested, Runner events.

### 3.2 SaveSystem
PlayerSaveData → JsonUtility → PlayerPrefs (ключ dumpling67_save_v2).  
Inventory как List&lt;InventoryEntry&gt; (JsonUtility не сериализует Dictionary).

## 4. Физика 2D SoftBody (Frozen)
SoftBodyPelmen2D — mesh + spring simulation, drag, squish burst.  
См. `_Frozen2D/README.md`.

## 5. Telegram Mini App Bridge

### 5.1 TelegramBridge.jslib
Haptic, MainButton, Expand, OpenInvoice, ShareUrl + Yandex/VK hooks.

### 5.2 TelegramWebAppBridge.cs
DllImport + mock в Editor. GameObject **должен** называться `TelegramWebAppBridge`.

Аналогично: `YandexGamesBridge`, `VkGamesBridge`.

## Чек-лист Шага 1

- [ ] URP проект, скопированы Assets/Dumpling67 и WebGLTemplates
- [ ] SceneBootstrap → Play: движение, сбор, finish/hazard
- [ ] GameManager создаёт bridges по точному имени GO
- [ ] Save/Load через PlayerPrefs
- [ ] Toast + ShareCard stubs
- [ ] Editor: Create Player Animator Controller
- [ ] Editor: Apply WebGL Build Settings
- [ ] WebGL template = Dumpling67

Актуальный HANDOFF и setup — в корне репозитория (`HANDOFF.md`, `SETUP_GUIDE.md`, `docs/*`).
