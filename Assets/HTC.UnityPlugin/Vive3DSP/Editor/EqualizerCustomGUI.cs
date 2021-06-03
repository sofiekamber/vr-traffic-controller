/**
 * The MIT License (MIT)
 * 
 * Copyright (c) 2014, Unity Technologies
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;
using UnityEditor;
using System;
using System.Runtime.InteropServices;
using MathHelpers;

namespace HTC.UnityPlugin.Vive3DSP
{
    public class EqualizerCustomGUI
    {
        private float[] paramEQFreq = new float[7];
        private float[] paramEQGain = new float[7];
        private float[] paramEQ_Q = new float[5];
        private float[] coefs = new float[35];
        private bool[] filterIsEnable = new bool[7];

        private GCHandle pinnedParamEQFreq;
        private GCHandle pinnedParamEQGain;
        private GCHandle pinnedParamEQ_Q;
        private GCHandle pinnedCoefs;
        private GCHandle pinnedParamIsEnable;
        private bool useLogScale;

        public EqualizerCustomGUI()
        {
            pinnedParamEQFreq = GCHandle.Alloc(paramEQFreq, GCHandleType.Pinned);
            pinnedParamEQGain = GCHandle.Alloc(paramEQGain, GCHandleType.Pinned);
            pinnedParamEQ_Q = GCHandle.Alloc(paramEQ_Q, GCHandleType.Pinned);
            pinnedCoefs = GCHandle.Alloc(coefs, GCHandleType.Pinned);
            pinnedParamIsEnable = GCHandle.Alloc(filterIsEnable, GCHandleType.Pinned);
        }

        ~EqualizerCustomGUI()
        {
            pinnedParamEQFreq.Free();
            pinnedParamEQGain.Free();
            pinnedParamEQ_Q.Free();
            pinnedCoefs.Free();
            pinnedParamIsEnable.Free();
        }

        private void setParamEQFreq(float[] freq)
        {
            for (int idx = 0; idx < 7; idx++)
            {
                paramEQFreq[idx] = freq[idx];
            }
        }

        private void setParamEQGain(float[] gain)
        {
            for (int idx = 0; idx < 7; idx++)
            {
                paramEQGain[idx] = gain[idx];
            }
        }

        private void setParamEQ_Q(float[] Q)
        {
            for (int idx = 0; idx < 5; idx++)
            {
                paramEQ_Q[idx] = Q[idx];
            }
        }
        private void setFilterIsEnable(bool[] nable)
        {
            for (int idx = 0; idx < 7; idx++)
            {
                filterIsEnable[idx] = nable[idx];
            }
        }
        private void DrawSpectrum(Rect r, bool useLogScale, float[] data, float dB_range, float samplerate, float col_r, float col_g, float col_b, float col_a, float gainOffset_dB)
        {
            float xscale = (float)(data.Length - 2) * 2.0f / samplerate;
            float yscale = 1.0f / dB_range;
            AudioCurveRendering.DrawCurve(
                r,
                delegate (float x)
                {
                    double f = GUIHelpers.MapNormalizedFrequency((double)x, samplerate, useLogScale, true) * xscale;
                    int i = (int)Math.Floor(f);
                    double h = data[i] + (data[i + 1] - data[i]) * (f - i);
                    double mag = (h > 0.0) ? (20.0f * Math.Log10(h) + gainOffset_dB) : -120.0;
                    return (float)(yscale * mag);
                },
                new Color(col_r, col_g, col_b, col_a));
        }

        public void DrawBandSplitMarker(Rect r, float x, float w, bool highlight, Color color)
        {
            if (highlight)
                w *= 2.0f;

            EditorGUI.DrawRect(new Rect(r.x + x - w, r.y, 2 * w, r.height), color);
        }

        protected static Color ScaleAlpha(Color col, float blend)
        {
            return new Color(col.r, col.g, col.b, col.a * blend);
        }

        private void DrawFilterCurve(
            Rect r,
            float[] coeffs,
            bool[] isEnable,
            Color color,
            bool useLogScale,
            bool filled,
            double masterGain,
            double samplerate,
            double magScale)
        {
            AudioCurveRendering.AudioCurveEvaluator d = delegate (float x)
                {
                    IntPtr pParamEQFreq = pinnedParamEQFreq.AddrOfPinnedObject();
                    IntPtr pParamEQGain = pinnedParamEQGain.AddrOfPinnedObject();
                    IntPtr pParamEQ_Q = pinnedParamEQ_Q.AddrOfPinnedObject();
                    IntPtr pParamIsEnable = pinnedParamIsEnable.AddrOfPinnedObject();
                    Vive3DSPAudio.native3dsp.Set3DSPUICurveParametric((float)samplerate, useLogScale, pParamIsEnable, pParamEQFreq, pParamEQGain, pParamEQ_Q);

                    float[] outY = new float[1];
                    Vive3DSPAudio.native3dsp.Get3DSPUICurveValue(outY, x);
                    return (outY[0] + (float)masterGain) * (float)magScale;
                };

            if (filled)
                AudioCurveRendering.DrawFilledCurve(r, d, color);
            else
                AudioCurveRendering.DrawCurve(r, d, color);
        }

        private bool DrawControl(Rect r, bool[] isEnable, float samplerate)
        {
            r = AudioCurveRendering.BeginCurveFrame(r);

            float thr = 4.0f;
            bool changed = false;

            if (Event.current.type == EventType.Repaint)
            {
                //float blend = plugin.IsPluginEditableAndEnabled() ? 1.0f : 0.5f;
                float blend = 1.0f;

                // Mark bands (low, medium and high bands)
                for (int band_idx = 0; band_idx < 7; band_idx++)
                {
                    DrawBandSplitMarker(r, (float)GUIHelpers.MapNormalizedFrequency(paramEQFreq[band_idx], samplerate, useLogScale, false) * r.width, thr, true, new Color((float)band_idx / 6.0f, (float)band_idx / 6.0f, (float)band_idx / 6.0f, blend));
                }

                const float dbRange = 20.0f;
                const float magScale = 1.0f / dbRange;

                // Draw filled curve
                DrawFilterCurve(
                    r,
                    coefs,
                    isEnable,
                    ScaleAlpha(AudioCurveRendering.kAudioOrange, blend),
                    useLogScale,
                    false,
                    1.8f,
                    samplerate,
                    magScale);

                GUIHelpers.DrawFrequencyTickMarks(r, samplerate, useLogScale, new Color(1.0f, 1.0f, 1.0f, 0.3f * blend));
                GUIHelpers.DrawDbTickMarks(r, 17.0f, magScale, new Color(1.0f, 1.0f, 1.0f, 0.3f * blend), new Color(1.0f, 1.0f, 1.0f, 0.3f * blend));
            }

            AudioCurveRendering.EndCurveFrame();
            return changed;
        }

        public void DrawParamEQGUI(float[] freq, float[] gain, float[] Q, bool[] isEnable)
        {
            useLogScale = true;
            setParamEQFreq(freq);
            setParamEQGain(gain);
            setParamEQ_Q(Q);
            setFilterIsEnable(isEnable);

            GUILayout.Space(5f);
            Rect r = GUILayoutUtility.GetRect(200, 100, GUILayout.ExpandWidth(true));
            DrawControl(r, isEnable, (float)AudioSettings.outputSampleRate);
        }
    }
}