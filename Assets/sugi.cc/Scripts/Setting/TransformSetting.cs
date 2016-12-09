using UnityEngine;
using System.Collections;

namespace sugi.cc
{
    public class TransformSetting : MonoBehaviour
    {
        [Header("sync materialProperties server to client")]
        public bool sync;
        public Space space = Space.Self;
        [SerializeField]
        string settingFilePath = "TransformSetting.json";
        Setting setting;

        void Start()
        {
            setting = new Setting(transform, space);
            SettingManager.AddSettingMenu(setting, settingFilePath);
            if (sync)
                setting.SetSyncable();
        }

        [System.Serializable]
        public class Setting : SettingManager.Setting
        {
            public Vector3 position;
            public Vector3 rotation;
            public Vector3 scale;

            Transform target;
            Space space;

            public Setting(Transform transform, Space space)
            {
                target = transform;
                this.space = space;
                if (space == Space.Self)
                {
                    position = transform.localPosition;
                    rotation = transform.localRotation.eulerAngles;
                    scale = transform.localScale;
                }
                else
                {
                    position = transform.position;
                    rotation = transform.rotation.eulerAngles;
                    scale = transform.localScale;//lossyScale(readOnly)
                }
            }
            protected override void OnLoad()
            {
                base.OnLoad();
                ApplySetting();
            }
            public override void OnGUIFunc()
            {
                base.OnGUIFunc();
                ApplySetting();
            }

            void ApplySetting()
            {
                if (space == Space.Self)
                {
                    target.localPosition = position;
                    target.localRotation = Quaternion.Euler(rotation);
                    target.localScale = scale;
                }
                else
                {
                    target.position = position;
                    target.rotation = Quaternion.Euler(rotation);
                    target.localScale = scale;
                }
            }
        }
    }
}