mergeInto(LibraryManager.library, {

    TriggerHapticFeedback: function (stylePtr) {
        var style = UTF8ToString(stylePtr);
        if (window.Telegram && window.Telegram.WebApp && window.Telegram.WebApp.HapticFeedback) {
            try {
                window.Telegram.WebApp.HapticFeedback.impactOccurred(style);
            } catch (e) {
                console.log("[TMA Haptic error]", e);
            }
        } else {
            console.log("[TMA Haptic Mock]: " + style);
        }
    },

    ShowTelegramMainButton: function (textPtr) {
        var text = UTF8ToString(textPtr);
        if (window.Telegram && window.Telegram.WebApp && window.Telegram.WebApp.MainButton) {
            var mb = window.Telegram.WebApp.MainButton;
            mb.setText(text);
            mb.show();
            mb.onClick(function () {
                if (typeof SendMessage === "function") {
                    SendMessage("TelegramWebAppBridge", "OnMainButtonClick", "");
                }
            });
        } else {
            console.log("[TMA MainButton Mock] show: " + text);
        }
    },

    HideTelegramMainButton: function () {
        if (window.Telegram && window.Telegram.WebApp && window.Telegram.WebApp.MainButton) {
            window.Telegram.WebApp.MainButton.hide();
        } else {
            console.log("[TMA MainButton Mock] hide");
        }
    },

    TelegramExpand: function () {
        if (window.Telegram && window.Telegram.WebApp) {
            try { window.Telegram.WebApp.expand(); } catch (e) {}
        }
    },

    TelegramOpenInvoice: function (urlPtr) {
        var url = UTF8ToString(urlPtr);
        if (window.Telegram && window.Telegram.WebApp && window.Telegram.WebApp.openInvoice) {
            window.Telegram.WebApp.openInvoice(url, function (status) {
                if (status === "paid" && typeof SendMessage === "function") {
                    SendMessage("TelegramWebAppBridge", "OnStarsPaymentSuccess", "500");
                }
            });
        } else {
            console.log("[TMA Invoice Mock]", url);
            if (typeof SendMessage === "function") {
                SendMessage("TelegramWebAppBridge", "OnStarsPaymentSuccess", "500");
            }
        }
    },

    TelegramShareUrl: function (urlPtr, textPtr) {
        var url = UTF8ToString(urlPtr);
        var text = UTF8ToString(textPtr);
        if (window.Telegram && window.Telegram.WebApp && window.Telegram.WebApp.openTelegramLink) {
            var shareUrl = "https://t.me/share/url?url=" + encodeURIComponent(url) + "&text=" + encodeURIComponent(text);
            window.Telegram.WebApp.openTelegramLink(shareUrl);
        } else {
            console.log("[TMA Share Mock]", text, url);
        }
    },

    // --- Yandex ---
    YandexReady: function () {
        if (window.ysdk && window.ysdk.features && window.ysdk.features.LoadingAPI) {
            window.ysdk.features.LoadingAPI.ready();
        } else {
            console.log("[Yandex Mock] Ready");
        }
    },

    YandexShowRewarded: function () {
        if (window.ysdk && window.ysdk.adv) {
            window.ysdk.adv.showRewardedVideo({
                callbacks: {
                    onRewarded: function () {
                        if (typeof SendMessage === "function")
                            SendMessage("YandexGamesBridge", "OnYandexRewarded", "");
                    },
                    onClose: function () {
                        if (typeof SendMessage === "function")
                            SendMessage("YandexGamesBridge", "OnYandexRewardedClose", "");
                    }
                }
            });
        } else {
            console.log("[Yandex Mock] Rewarded");
            if (typeof SendMessage === "function")
                SendMessage("YandexGamesBridge", "OnYandexRewarded", "");
        }
    },

    YandexShowFullscreen: function () {
        if (window.ysdk && window.ysdk.adv) {
            window.ysdk.adv.showFullscreenAdv({ callbacks: {} });
        } else {
            console.log("[Yandex Mock] Fullscreen");
        }
    },

    YandexSetLeaderboardScore: function (boardPtr, score) {
        var board = UTF8ToString(boardPtr);
        if (window.ysdk && window.ysdk.getLeaderboards) {
            window.ysdk.getLeaderboards().then(function (lb) {
                return lb.setLeaderboardScore(board, score);
            }).catch(function (e) { console.log(e); });
        } else {
            console.log("[Yandex Mock] LB", board, score);
        }
    },

    // --- VK ---
    VkShowRewarded: function () {
        if (window.vkBridge) {
            window.vkBridge.send("VKWebAppShowNativeAds", { ad_format: "reward" })
                .then(function () {
                    if (typeof SendMessage === "function")
                        SendMessage("VkGamesBridge", "OnVkRewarded", "");
                })
                .catch(function (e) { console.log(e); });
        } else {
            console.log("[VK Mock] Rewarded");
            if (typeof SendMessage === "function")
                SendMessage("VkGamesBridge", "OnVkRewarded", "");
        }
    },

    VkShare: function (linkPtr, textPtr) {
        var link = UTF8ToString(linkPtr);
        var text = UTF8ToString(textPtr);
        if (window.vkBridge) {
            window.vkBridge.send("VKWebAppShare", { link: link }).catch(function () {});
        } else {
            console.log("[VK Mock] Share", text, link);
        }
    }
});
