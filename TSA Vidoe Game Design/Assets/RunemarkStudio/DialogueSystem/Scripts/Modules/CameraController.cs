using System.Collections.Generic;
using UnityEngine;

#if CINEMACHINE
using Cinemachine;
#endif

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using System;
#endif

namespace Runemark.DialogueSystem
{
    /// <summary>
    /// Multipurpose camera control.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class CameraController
    {
        public enum CameraType
        {
            Basic,
#if CINEMACHINE
            Cinemachine
#endif
        }

        public bool Enable = false;
        public CameraType Type = CameraType.Basic;
        public List<Behaviour> CameraList = new List<Behaviour>();
        public int DefaultIndex = -1;
        
        Behaviour _activeCamera;
        Behaviour _mainCamera;
        
        public void Init()
        {
            if (Type == CameraType.Basic)
                _mainCamera = Camera.main;
#if CINEMACHINE
            else if (Type == CameraType.Cinemachine)
            {
                float priority = - Mathf.Infinity;
                foreach (var vCam in GameObject.FindObjectsOfType<CinemachineVirtualCamera>())
                {                    
                    if (vCam.Priority >= priority)
                    {
                        priority = vCam.Priority;
                        _mainCamera = vCam;
                    }
                }
            }
#endif
            Reset();
        }


        /// <summary>
        /// Set the camera with this given index to active.
        /// </summary>
        /// <param name="index"></param>
        public void Set(int index)
        {
            if (!Enable) return;

            // If the index is invalid, set it to the default one.
            if (index < 0 || index > CameraList.Count)
                index = DefaultIndex;

            // If active camera exists, disable it.
            if (_activeCamera != null) _activeCamera.enabled = false;        

            // If the index is still invalid, use the main camera
            if (index < 0) _activeCamera = _mainCamera;
            else _activeCamera = CameraList[index];

            // If the activated camera is exists, enable it.
            if (_activeCamera != null)
                _activeCamera.enabled = true;
        }

        /// <summary>
        /// Resets all camera to off, and the main camera to on.
        /// </summary>
        public void Reset()
        {
            if (_mainCamera != null)
            {
                _mainCamera.enabled = true;
                _activeCamera = _mainCamera;
            }

            if (Enable)
            {
                for (int cnt = 0; cnt < CameraList.Count; cnt++)
                    CameraList[cnt].enabled = false;
            }
        }



#if UNITY_EDITOR
        ReorderableList _cameraList;

        public void OnEnable()
        {
            _cameraList = new ReorderableList(CameraList, typeof(Behaviour), true, true, true, true);
            _cameraList.drawHeaderCallback = drawHeaderCallback;
            _cameraList.drawElementCallback = drawElementCallback;
            _cameraList.onAddCallback = onAddCallback;
        }

        public void DrawInspectorGUI()
        {
            Enable = EditorGUILayout.Toggle("Use Custom Camera(s)", Enable);

            if (Enable)
            {
                EditorGUI.indentLevel++;
#if CINEMACHINE
                var type = (CameraType)EditorGUILayout.EnumPopup("Camera Type", Type);
                if (type != Type)
                {
                    Type = type;
                    CameraList.Clear();
                }
#else
                // If only the basic camera is accessable force to use it.
                if (Type != CameraType.Basic)
                {
                    Type = CameraType.Basic;
                    CameraList.Clear();
                }
#endif

                DefaultIndex = EditorGUILayout.IntSlider("Default Camera Index", DefaultIndex, -1, CameraList.Count - 1);
                EditorGUILayout.HelpBox("Default camera is the one that is used when the Text Node returns -1 as camera index. If the default camera index is -1, it uses the main camera.", MessageType.Info);
                EditorGUI.indentLevel--;

                _cameraList.DoLayoutList();               
            }
        }

        protected virtual void drawHeaderCallback(Rect rect)
        {
            EditorGUI.LabelField(rect, "Cameras");
        }

        void drawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            Behaviour b = CameraList[index];
            if (Type == CameraType.Basic)
            {
                DrawCameraField<Camera>(rect, ref b);
                CameraList[index] = b;
            }
#if CINEMACHINE
            else if (Type == CameraType.Cinemachine)
            {
                DrawCameraField<CinemachineVirtualCameraBase>(rect, ref b);
                CameraList[index] = b;
            }
#endif
        }

        void DrawCameraField<T>(Rect rect, ref Behaviour value) where T : Behaviour
        {
            if (value != null)
            {
                Type type = value.GetType();
                if (type != typeof(T) && !type.IsSubclassOf(typeof(T))) return;
            }

            T stored = (value != null) ? (T)value : null;
            T c = (T)EditorGUI.ObjectField(rect, stored, typeof(T), true);
            value = c;
        }

        void onAddCallback(UnityEditorInternal.ReorderableList list)
        {
            ReorderableList.defaultBehaviours.DoAddButton(list);
        }
#endif


    }
}
