# Экспертная оценка и стратегический анализ проекта Dumpling67

## 1. Общий вердикт и уровень готовности концепции

Представленный документ демонстрирует глубокое понимание узких мест текущей архитектуры на Android Native и четкое видение масштабирования продукта.

**Уровень проработки концепта:** *Высокий (Good / Advanced)*.

**Главный вопрос:** *Согласен ли я с решением о переходе на Unity?*
**Ответ:** **Да, абсолютно согласен.** Перевод Dumpling67 на Unity — это единственно верный шаг для трансформации локального Android-приложения в полноценный гиперказуальный/гибридный продукт с высокой окупаемостью (ROAS) и виральным охватом.

---

## 2. Почему переход на Unity критически необходим

### A. Ограничения Jetpack Compose Canvas vs Unity URP
* **Графика и шейдеры:** Эффекты VHS/glitch и squish вычисляются на UI Thread → FPS drop и нагрев.
* **Решение в Unity:** URP Post-Processing на GPU, стабильные 60 FPS.

### B. Физика деформации (Squish & Squash)
* Soft Body через SpringJoint2D / Sprite Skinning — тактильный juicy отклик.

### C. Cross-Platform & WebGL
* Яндекс Игры / VK Games / Telegram Mini Apps — виральный охват без установки.

---

## 3. Рекомендации

* WebGL-first, Telegram WebApp SDK (initData, Haptic, Share, Stars)
* ScriptableObjects для каталога, PlayerPrefs/JSON для сейва
* Бюджет билда < 15–20 MB compressed

---

## 4. Матрица сущностей Android → Unity

| Android Native | Unity C# |
| :--- | :--- |
| AppDatabase (Room) | ScriptableObjects + PlayerPrefs/JSON |
| PlayerInventoryEntity | PlayerSaveData |
| BlindBox67Catalog.kt | BlindBoxSO |
| VhsChromaticGlitchOverlay | URP Post-Processing / Shader |
| SpringSquishPelmenCharacter | SoftBody / SkinnedMesh + Springs |

---

## 5. Roadmap

```
[Фаза 1: Unity Core MVP]
   ├── SO + JSON save
   ├── Sensory Room (SoftBody 2D) / Runner
   └── URP Shaders (VHS, Glitch)

[Фаза 2: WebGL & Telegram First]
   ├── WebGL < 15 MB
   ├── Яндекс / VK / TMA SDK
   └── Retention D1/D7

[Фаза 3: Mobile & Monetization]
   ├── Ads / IAP
   ├── Google Play, RuStore, App Store
   └── Marketing Shorts / TikTok
```

---

## Заключение

Перевод на Unity — верный шаг. С фокусом на WebGL и Telegram Mini Apps проект имеет шансы стать успешным гибридно-казуальным тайтлом.
