# Чеклист — Telegram Mini App (Dumpling 67)

## Бот и хостинг

- [ ] Бот создан через @BotFather, команда `/newapp` или Bot Settings → Menu Button
- [ ] Web App URL указывает на **HTTPS** хостинг WebGL (GitHub Pages / Cloudflare / свой CDN)
- [ ] CORS и правильные MIME: `.br` / `.wasm` / `.data`
- [ ] В шаблоне подключён `https://telegram.org/js/telegram-web-app.js`

## Unity / код

- [ ] Объект сцены называется **точно** `TelegramWebAppBridge`
- [ ] jslib лежит в `Assets/Dumpling67/Plugins/WebGL/`
- [ ] `Expand()` на старте, haptic на прыжке / squish
- [ ] MainButton — опционально для «Играть снова» / «Забрать награду»
- [ ] Stars: invoice URL с **вашего** бэкенда бота (не хардкодить прод-ключ в клиент)

## UX в TMA

- [ ] Viewport: full height после expand
- [ ] Цвета header/bg совпадают с игрой (`#120e0c`)
- [ ] Не полагаться на hover — только тап
- [ ] Размер билда: чем меньше, тем лучше (мобильный 4G)

## Share / рефералка

- [ ] `ShareRun` открывает `t.me/share/url` с deep-link `?start=ref_XXX`
- [ ] Бэкенд бота пишет `start` payload и выдаёт бонус один раз

## Безопасность

- [ ] `initData` валидируется **на сервере** (HMAC), не доверять user id с клиента
- [ ] Покупки Stars подтверждать webhook’ом бота, монеты начислять после `paid`
