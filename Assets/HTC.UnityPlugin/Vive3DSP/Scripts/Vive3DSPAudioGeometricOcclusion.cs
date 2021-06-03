//====================== Copyright 2017-2020, HTC.Corporation. All rights reserved. ======================
using UnityEngine;
using System;

namespace HTC.UnityPlugin.Vive3DSP
{
    [AddComponentMenu("VIVE/3DSP_AudioOcclusion/Geometric")]
    [HelpURL("https://hub.vive.com/storage/3dsp/vive_3dsp_audio_sdk_unity_plugin.html#audio-occlusion-settings")]
    public class Vive3DSPAudioGeometricOcclusion : MonoBehaviour
    {
        public OccGeometryMode occlusionGeometry = OccGeometryMode.Box;

        public bool OcclusionEffect
        {
            set { occlusionEffect = value; }
            get { return occlusionEffect; }
        }
        [SerializeField]
        private bool occlusionEffect = true;

        public OccMaterial OcclusionMaterial
        {
            set { occlusionMaterial = value; }
            get { return occlusionMaterial; }
        }
        [SerializeField]
        private OccMaterial occlusionMaterial = OccMaterial.Curtain;

        public OccComputeMode OcclusionComputeMode
        {
            set { occlusionComputeMode = value; }
            get { return OcclusionComputeMode; }
        }
        [SerializeField]
        private OccComputeMode occlusionComputeMode = OccComputeMode.VeryLight;

        public float OcclusionIntensity
        {
            set { occlusionIntensity = value; }
            get { return occlusionIntensity; }
        }
        [SerializeField]
        private float occlusionIntensity = 1.5f;

        public float HighFreqAttenuation
        {
            set { highFreqAttenuation = value; }
            get { return highFreqAttenuation; }
        }
        [SerializeField]
        private float highFreqAttenuation = -50.0f;

        public float LowFreqAttenuationRatio
        {
            set { lowFreqAttenuationRatio = value; }
            get { return lowFreqAttenuationRatio; }
        }
        [SerializeField]
        private float lowFreqAttenuationRatio = 0.0f;

        public Vector3 Position
        {
            get { return pos; }
            set { if (pos != value) pos = value; }
        }
        private Vector3 pos = Vector3.zero;

        public VIVE_3DSP_OCCLUSION_PROPERTY OcclusionPorperty
        {
            set { occProperty = value; }
            get { return occProperty; }
        }
        private VIVE_3DSP_OCCLUSION_PROPERTY occProperty;

        public IntPtr OcclusionObject
        {
            get { return _occObj; }
            set { _occObj = value; }
        }
        private IntPtr _occObj = IntPtr.Zero;

        public Vector3 OcclusionCenter
        {
            set { occlusionCenter = value; }
            get { return occlusionCenter; }
        }
        [SerializeField]
        private Vector3 occlusionCenter = Vector3.zero;

        public float OcclusionRadius
        {
            get { return occlusionRadius; }
            set { occlusionRadius = value; }
        }
        [SerializeField]
        private float occlusionRadius = 1.0f;

        public Vector3 OcclusionSize
        {
            set { occlusionSize = value; }
            get { return occlusionSize; }
        }
        [SerializeField]
        private Vector3 occlusionSize = Vector3.one;

        public float OcclusionHeight
        {
            set { occlusionHeight = value; }
            get { return occlusionHeight; }
        }
        [SerializeField]
        private float occlusionHeight = 1.0f;

        public Vector3 OcclusionRotation
        {
            set { occlusionRotation = value; }
            get { return occlusionRotation; }
        }
        [SerializeField]
        private Vector3 occlusionRotation = Vector3.zero;

        void Awake()
        {
            InitOcclusion();
        }
        
        void OnEnable()
        {
            if (InitOcclusion())
            {
                if (occlusionEffect)
                    Vive3DSPAudio.EnableOcclusion(_occObj);
                else
                    Vive3DSPAudio.DisableOcclusion(_occObj);
            }
            Vive3DSPAudio.UpdateAudioListener();
            Update();
        }

        void OnDisable()
        {
            Vive3DSPAudio.DisableOcclusion(_occObj);
        }

        void Update()
        {
            if (occlusionGeometry == OccGeometryMode.Sphere)
            {
                var radius = (Mathf.Max(Mathf.Abs(transform.lossyScale.x), Mathf.Abs(transform.lossyScale.y), Mathf.Abs(transform.lossyScale.z)) * occlusionRadius) / 2;
                occProperty.radius = radius;
                occProperty.rotation = transform.rotation;
                occProperty.height = 0.0f;
            }
            else if (occlusionGeometry == OccGeometryMode.Box)
            {
                occProperty.radius = 0.0f;
                occProperty.rotation = transform.rotation;
                occProperty.height = 0.0f;
            }
            else
            {
                var radius = (Mathf.Max(Mathf.Abs(transform.lossyScale.x), Mathf.Abs(transform.lossyScale.z)) * occlusionRadius) / 2;
                occProperty.radius = radius;
                occProperty.rotation = transform.rotation * Quaternion.Euler(occlusionRotation);
                occProperty.height = transform.lossyScale.y * OcclusionHeight;
            }
            occProperty.density = occlusionIntensity;
            occProperty.material = occlusionMaterial;
            occProperty.position = (transform.position + transform.rotation * Vector3.Scale(occlusionCenter, transform.lossyScale));
            occProperty.size = Vector3.Scale(transform.lossyScale, OcclusionSize);
            occProperty.rotation = transform.rotation;
            occProperty.rhf = highFreqAttenuation;
            occProperty.lfratio = lowFreqAttenuationRatio;
            occProperty.mode = occlusionGeometry;
            occProperty.computeMode = occlusionComputeMode;
            Vive3DSPAudio.UpdateOcclusion(_occObj, occlusionEffect, OcclusionPorperty);
        }
        
        private bool InitOcclusion()
        {
            if (_occObj == IntPtr.Zero)
            {
                _occObj = Vive3DSPAudio.CreateGeometricOcclusion(this);
                if (occlusionEffect && this.enabled)
                {
                    Vive3DSPAudio.EnableOcclusion(_occObj);
                }
                else
                {
                    Vive3DSPAudio.DisableOcclusion(_occObj);
                }
            }
            return _occObj != IntPtr.Zero;
        }

        void OnDrawGizmosSelected()
        {
            if (occlusionGeometry == OccGeometryMode.Sphere)
            {
                Gizmos.color = Color.black;
                var posUpdate = transform.position + transform.rotation * Vector3.Scale(occlusionCenter, transform.lossyScale);
                float maxScaleVal = Mathf.Max(Mathf.Abs(transform.lossyScale.x), Mathf.Abs(transform.lossyScale.y), Mathf.Abs(transform.lossyScale.z));
                Vector3 scaleVec = new Vector3(maxScaleVal, maxScaleVal, maxScaleVal);
                Gizmos.matrix = Matrix4x4.TRS(posUpdate, transform.rotation, scaleVec);
                Gizmos.DrawWireSphere(Vector3.zero, occlusionRadius / 2);
            }
            else if (occlusionGeometry == OccGeometryMode.Box)
            {
                Gizmos.color = Color.black;
                var posUpdate = transform.position + transform.rotation * Vector3.Scale(occlusionCenter, transform.lossyScale);
                Gizmos.matrix = Matrix4x4.TRS(posUpdate, transform.rotation, transform.lossyScale);
                Gizmos.DrawWireCube(Vector3.zero, occlusionSize);
            }
            else
            {
                Gizmos.color = Color.black;
                Vector3 centerShift = new Vector3(0, occlusionHeight, 0);
                Vector3 occCenter = Vector3.zero;

                Matrix4x4 defaultMatrix = Gizmos.matrix;

                Vector3[] pointArray = new Vector3[4];
                Vector3[] topArray = new Vector3[4];
                Vector3[] btmArray = new Vector3[4];

                Quaternion rot = transform.rotation * Quaternion.Euler(occlusionRotation);

                for (int i = 0; i < 2; i++)
                {
                    if (i == 0)
                        occCenter = occlusionCenter + centerShift;
                    else
                        occCenter = occlusionCenter - centerShift;

                    Vector3 posUpdate = transform.position + rot * Vector3.Scale(occCenter, transform.lossyScale);
                    float maxScaleVal = Mathf.Max(Mathf.Abs(transform.lossyScale.x), Mathf.Abs(transform.lossyScale.z));
                    Vector3 scaleVec = new Vector3(maxScaleVal, maxScaleVal, maxScaleVal);
                    Gizmos.matrix = Matrix4x4.TRS(posUpdate, rot, scaleVec);

                    DrawCircle(pointArray);

                    if (i == 0)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            topArray[j] = rot * pointArray[j] * maxScaleVal + posUpdate;
                        }
                    }
                    else
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            btmArray[j] = rot * pointArray[j] * maxScaleVal + posUpdate;
                        }
                    }
                }

                Gizmos.matrix = defaultMatrix;

                for (int j = 0; j < 4; j++)
                {
                    Gizmos.DrawLine(topArray[j], btmArray[j]);
                }
            }
        }

        void DrawCircle(Vector3[] pointArray)
        {
            Vector3 beginPoint = Vector3.zero;
            Vector3 firstPoint = Vector3.zero;
            int pointCnt = 0;
            for (int theta_cnt = 0; theta_cnt < 40; theta_cnt++)
            {
                float theta = (2 * Mathf.PI / 40) * theta_cnt;

                float x = (occlusionRadius / 2) * Mathf.Cos(theta);
                float z = (occlusionRadius / 2) * Mathf.Sin(theta);
                Vector3 endPoint = new Vector3(x, 0, z);
                if (theta == 0)
                {
                    firstPoint = endPoint;
                }
                else
                {
                    Gizmos.DrawLine(beginPoint, endPoint);
                }

                if (theta_cnt % 10 == 0)
                {
                    pointArray[pointCnt] = endPoint;
                    pointCnt++;
                }

                beginPoint = endPoint;
            }

            Gizmos.DrawLine(firstPoint, beginPoint);

        }
    }
}

