using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Dumpling67.Gameplay.ThirdPerson;
using Dumpling67.Core;
using Dumpling67.Managers;

namespace Dumpling67.Bootstrap
{
    /// <summary>
    /// Минимальный playable уровень из кода (для быстрой проверки без ручной сборки сцены).
    /// Повесь на пустой GO в пустой сцене, нажми Play.
    /// </summary>
    public class SceneBootstrap : MonoBehaviour
    {
        [SerializeField] bool buildOnStart = true;
        [SerializeField] bool createManagers = true;
        [SerializeField] int collectibleCount = 8;
        [SerializeField] float arenaSize = 24f;
        [Header("Mobile touch UI")]
        [SerializeField] bool buildMobileUI = true;

        private void Start()
        {
            if (buildOnStart) Build();
        }

        [ContextMenu("Build Test Level")]
        public void Build()
        {
            // Floor
            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor";
            floor.transform.localScale = new Vector3(arenaSize / 10f, 1f, arenaSize / 10f);
            floor.GetComponent<Renderer>().material.color = new Color(0.15f, 0.12f, 0.1f);

            // Light
            if (FindObjectOfType<Light>() == null)
            {
                var lightGo = new GameObject("Directional Light");
                var light = lightGo.AddComponent<Light>();
                light.type = LightType.Directional;
                light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            }

            // Managers
            if (createManagers && GameManager.Instance == null)
            {
                var gmGo = new GameObject("GameManager");
                var gm = gmGo.AddComponent<GameManager>();
                gm.economy = gmGo.AddComponent<EconomyManager>();
                // Остальные менеджеры — по необходимости
            }

            if (FindObjectOfType<GameAudio>() == null)
                new GameObject("GameAudio").AddComponent<GameAudio>();

            // Player
            var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.tag = "Player";
            player.transform.position = new Vector3(0f, 1f, 0f);
            Object.Destroy(player.GetComponent<CapsuleCollider>());

            var cc = player.AddComponent<CharacterController>();
            cc.height = 2f;
            cc.radius = 0.4f;
            cc.center = new Vector3(0f, 1f, 0f);

            var visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            visual.name = "Visual";
            visual.transform.SetParent(player.transform, false);
            visual.transform.localPosition = new Vector3(0f, 1f, 0f);
            Object.Destroy(visual.GetComponent<Collider>());
            visual.GetComponent<Renderer>().material.color = new Color(0.92f, 0.86f, 0.75f);

            var motor = player.AddComponent<ThirdPersonPlayerController>();
            var anim = player.AddComponent<CharacterAnimDriver>();
            anim.visual = visual.transform;
            anim.useProcedural = true;
            var juice = player.AddComponent<JuiceFeedback>();
            juice.playerVisual = visual.transform;
            motor.juice = juice;
            motor.animDriver = anim;

            // Camera
            Camera cam = Camera.main;
            if (cam == null)
            {
                var camGo = new GameObject("Main Camera");
                cam = camGo.AddComponent<Camera>();
                camGo.tag = "MainCamera";
                camGo.AddComponent<AudioListener>();
            }
            var tpc = cam.gameObject.GetComponent<ThirdPersonCamera>();
            if (tpc == null) tpc = cam.gameObject.AddComponent<ThirdPersonCamera>();
            tpc.target = player.transform;
            tpc.distance = 7f;
            juice.cameraTransform = cam.transform;

            // Collectibles
            for (int i = 0; i < collectibleCount; i++)
            {
                float angle = i * (360f / collectibleCount) * Mathf.Deg2Rad;
                float r = arenaSize * 0.35f;
                var pelmen = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                pelmen.name = $"Pelmen_{i}";
                pelmen.transform.position = new Vector3(Mathf.Cos(angle) * r, 0.5f, Mathf.Sin(angle) * r);
                pelmen.transform.localScale = Vector3.one * 0.6f;
                pelmen.GetComponent<Renderer>().material.color = new Color(0.95f, 0.75f, 0.3f);
                var col = pelmen.GetComponent<SphereCollider>();
                col.isTrigger = true;
                pelmen.AddComponent<CollectiblePelmen>();
            }

            // Finish zone
            var finish = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            finish.name = "FinishZone";
            finish.transform.position = new Vector3(0f, 0.1f, arenaSize * 0.4f);
            finish.transform.localScale = new Vector3(3f, 0.2f, 3f);
            finish.GetComponent<Renderer>().material.color = new Color(0.2f, 0.8f, 0.3f);
            var fcol = finish.GetComponent<CapsuleCollider>();
            if (fcol != null) Object.Destroy(fcol);
            var fsc = finish.AddComponent<SphereCollider>();
            fsc.isTrigger = true;
            fsc.radius = 1.5f;
            finish.AddComponent<FinishZone>();

            // Hazard
            var hazard = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hazard.name = "HazardZone";
            hazard.transform.position = new Vector3(arenaSize * 0.25f, 0.5f, 0f);
            hazard.transform.localScale = new Vector3(2f, 1f, 2f);
            hazard.GetComponent<Renderer>().material.color = new Color(0.8f, 0.15f, 0.15f);
            var hcol = hazard.GetComponent<BoxCollider>();
            hcol.isTrigger = true;
            hazard.AddComponent<HazardZone>();

            // Mobile UI
            if (buildMobileUI)
                BuildMobileUI(player, cam);

            Debug.Log("[SceneBootstrap] Test level built. Move with WASD / joystick, look with mouse / touch pad.");
        }

        private void BuildMobileUI(GameObject player, Camera cam)
        {
            // EventSystem
            if (FindObjectOfType<EventSystem>() == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<EventSystem>();
                es.AddComponent<StandaloneInputModule>();
            }

            // Canvas
            var canvasGo = new GameObject("MobileUI");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGo.AddComponent<GraphicRaycaster>();

            // Joystick (left)
            var joyGo = new GameObject("VirtualJoystick");
            joyGo.transform.SetParent(canvasGo.transform, false);
            var joyRt = joyGo.AddComponent<RectTransform>();
            joyRt.anchorMin = new Vector2(0f, 0f);
            joyRt.anchorMax = new Vector2(0f, 0f);
            joyRt.pivot = new Vector2(0f, 0f);
            joyRt.anchoredPosition = new Vector2(120f, 120f);
            joyRt.sizeDelta = new Vector2(200f, 200f);
            var joyImg = joyGo.AddComponent<Image>();
            joyImg.color = new Color(1f, 1f, 1f, 0.15f);
            var joy = joyGo.AddComponent<VirtualJoystick>();

            // Look pad (right)
            var lookGo = new GameObject("LookTouchPad");
            lookGo.transform.SetParent(canvasGo.transform, false);
            var lookRt = lookGo.AddComponent<RectTransform>();
            lookRt.anchorMin = new Vector2(1f, 0f);
            lookRt.anchorMax = new Vector2(1f, 1f);
            lookRt.pivot = new Vector2(1f, 0.5f);
            lookRt.anchoredPosition = Vector2.zero;
            lookRt.sizeDelta = new Vector2(400f, 0f);
            var lookImg = lookGo.AddComponent<Image>();
            lookImg.color = new Color(1f, 1f, 1f, 0.05f);
            var lookPad = lookGo.AddComponent<LookTouchPad>();

            // Wire to player / camera
            var motor = player.GetComponent<ThirdPersonPlayerController>();
            if (motor != null) motor.joystick = joy;
            var tpc = cam.GetComponent<ThirdPersonCamera>();
            if (tpc != null) tpc.lookPad = lookPad;

            // Toast
            var toastGo = new GameObject("ToastUI");
            toastGo.transform.SetParent(canvasGo.transform, false);
            var toastRt = toastGo.AddComponent<RectTransform>();
            toastRt.anchorMin = new Vector2(0.5f, 1f);
            toastRt.anchorMax = new Vector2(0.5f, 1f);
            toastRt.pivot = new Vector2(0.5f, 1f);
            toastRt.anchoredPosition = new Vector2(0f, -40f);
            toastRt.sizeDelta = new Vector2(400f, 60f);
            var toastCg = toastGo.AddComponent<CanvasGroup>();
            toastCg.alpha = 0f;
            var toastTxtGo = new GameObject("Text");
            toastTxtGo.transform.SetParent(toastGo.transform, false);
            var txt = toastTxtGo.AddComponent<Text>();
            txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = Color.white;
            txt.fontSize = 24;
            var txtRt = toastTxtGo.GetComponent<RectTransform>();
            txtRt.anchorMin = Vector2.zero;
            txtRt.anchorMax = Vector2.one;
            txtRt.offsetMin = Vector2.zero;
            txtRt.offsetMax = Vector2.zero;
            var toast = toastGo.AddComponent<ToastUI>();
            // ToastUI fields are private serialized — will work if assigned via reflection or public in real file
        }
    }
}
