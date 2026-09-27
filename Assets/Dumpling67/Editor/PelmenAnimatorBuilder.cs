#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Dumpling67.EditorTools
{
    /// <summary>
    /// Меню: Dumpling67 → Create Player Animator Controller
    /// Создаёт State Machine под CharacterAnimDriver.
    /// </summary>
    public static class PelmenAnimatorBuilder
    {
        private const string DefaultPath = "Assets/Dumpling67/Animations/PelmenPlayer.controller";

        [MenuItem("Dumpling67/Create Player Animator Controller")]
        public static void CreatePlayerAnimator()
        {
            EnsureFolder("Assets/Dumpling67");
            EnsureFolder("Assets/Dumpling67/Animations");

            var controller = AnimatorController.CreateAnimatorControllerAtPath(DefaultPath);

            controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
            controller.AddParameter("MotionSpeed", AnimatorControllerParameterType.Float);
            controller.AddParameter("Grounded", AnimatorControllerParameterType.Bool);
            controller.AddParameter("VerticalVelocity", AnimatorControllerParameterType.Float);
            controller.AddParameter("IsMoving", AnimatorControllerParameterType.Bool);
            controller.AddParameter("IsSprinting", AnimatorControllerParameterType.Bool);
            controller.AddParameter("Jump", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Land", AnimatorControllerParameterType.Trigger);

            var root = controller.layers[0].stateMachine;
            root.entryPosition = new Vector3(-200, 0, 0);
            root.anyStatePosition = new Vector3(-200, 80, 0);
            root.exitPosition = new Vector3(600, 0, 0);

            var idle = root.AddState("Idle", new Vector3(50, 0, 0));
            var loco = root.AddState("Locomotion", new Vector3(50, 80, 0));
            var jump = root.AddState("Jump", new Vector3(300, 0, 0));
            var fall = root.AddState("Fall", new Vector3(300, 80, 0));
            var land = root.AddState("Land", new Vector3(300, 160, 0));

            root.defaultState = idle;

            // Idle <-> Locomotion
            var toLoco = idle.AddTransition(loco);
            toLoco.AddCondition(AnimatorConditionMode.If, 0, "IsMoving");
            toLoco.hasExitTime = false;
            toLoco.duration = 0.1f;

            var toIdle = loco.AddTransition(idle);
            toIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsMoving");
            toIdle.hasExitTime = false;
            toIdle.duration = 0.1f;

            // Any -> Jump
            var anyJump = root.AddAnyStateTransition(jump);
            anyJump.AddCondition(AnimatorConditionMode.If, 0, "Jump");
            anyJump.hasExitTime = false;
            anyJump.duration = 0.05f;

            // Jump -> Fall when VerticalVelocity < 0
            var jumpFall = jump.AddTransition(fall);
            jumpFall.AddCondition(AnimatorConditionMode.Less, 0f, "VerticalVelocity");
            jumpFall.hasExitTime = false;
            jumpFall.duration = 0.05f;

            // Fall -> Land on Grounded
            var fallLand = fall.AddTransition(land);
            fallLand.AddCondition(AnimatorConditionMode.If, 0, "Grounded");
            fallLand.hasExitTime = false;
            fallLand.duration = 0.05f;

            // Land -> Idle
            var landIdle = land.AddTransition(idle);
            landIdle.hasExitTime = true;
            landIdle.exitTime = 0.9f;
            landIdle.duration = 0.1f;

            AssetDatabase.SaveAssets();
            Selection.activeObject = controller;
            EditorGUIUtility.PingObject(controller);
            Debug.Log("[PelmenAnimatorBuilder] Controller created at " + DefaultPath +
                      ". Assign clips to states Idle/Locomotion/Jump/Fall/Land.");
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            var parts = path.Split('/');
            string cur = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = cur + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(cur, parts[i]);
                cur = next;
            }
        }
    }
}
#endif
