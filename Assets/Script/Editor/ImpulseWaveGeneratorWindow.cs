using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;

public class ImpulseWaveGeneratorWindow : EditorWindow
{
    private enum WaveType
    {
        Sine,
        Triangle,
        SquareSmooth,
        PunchRecoil,
        SymmetricalRumble
    }

    private CinemachineImpulseSource targetSource;

    private WaveType waveType = WaveType.Sine;
    private float duration = 0.35f;
    private bool useFrequencyHz = true;
    private float frequencyHz = 12f;
    private int cycles = 4;

    private float amplitude = 1f;
    private float attackRatio = 0.05f;
    private float decayPower = 2f;
    private bool invertStart = false;
    private int rumbleSeed = 42;
    private int samplesPerCycle = 8;

    private AnimationCurve generatedCurve = new AnimationCurve();
    private bool autoApply = false;

    [MenuItem("ShopCrafter/Tools/Impulse Wave Generator")]
    public static void ShowWindow()
    {
        ImpulseWaveGeneratorWindow window = GetWindow<ImpulseWaveGeneratorWindow>("Impulse Wave");
        window.minSize = new Vector2(360f, 520f);
    }

    private void OnEnable()
    {
        RegenerateCurve();
    }

    private void OnSelectionChange()
    {
        if (Selection.activeGameObject != null &&
            Selection.activeGameObject.TryGetComponent(out CinemachineImpulseSource src))
        {
            targetSource = src;
            Repaint();
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(6);

        targetSource = (CinemachineImpulseSource)EditorGUILayout.ObjectField(
            "Target Impulse Source",
            targetSource,
            typeof(CinemachineImpulseSource),
            true
        );

        EditorGUILayout.Space(6);
        DrawPresets();
        EditorGUILayout.Space(6);

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Paramètres de la Wave", EditorStyles.boldLabel);

        waveType = (WaveType)EditorGUILayout.EnumPopup("Forme", waveType);
        duration = Mathf.Max(0.02f, EditorGUILayout.FloatField("Durée (s)", duration));

        EditorGUILayout.BeginHorizontal();
        useFrequencyHz = EditorGUILayout.Toggle("Utiliser Fréquence (Hz)", useFrequencyHz);
        EditorGUILayout.EndHorizontal();

        if (useFrequencyHz)
        {
            frequencyHz = EditorGUILayout.Slider("Fréquence (Hz)", frequencyHz, 1f, 60f);
            int computedCycles = Mathf.Max(1, Mathf.RoundToInt(duration * frequencyHz));
            EditorGUILayout.LabelField($"Cycles complets calculés : {computedCycles} (symétrie garantie)", EditorStyles.miniLabel);
        }
        else
        {
            cycles = EditorGUILayout.IntSlider("Nombre de cycles", cycles, 1, 30);
            EditorGUILayout.LabelField($"Fréquence équivalente : {(cycles / duration):0.0} Hz", EditorStyles.miniLabel);
        }

        amplitude = EditorGUILayout.Slider("Amplitude", amplitude, 0.05f, 1f);
        attackRatio = EditorGUILayout.Slider("Attack (Montée)", attackRatio, 0f, 0.5f);
        decayPower = EditorGUILayout.Slider("Decay (Amortissement)", decayPower, 0f, 6f);
        invertStart = EditorGUILayout.Toggle("Inverser polarité (+/-)", invertStart);

        if (waveType == WaveType.SymmetricalRumble)
        {
            EditorGUILayout.BeginHorizontal();
            rumbleSeed = EditorGUILayout.IntField("Seed", rumbleSeed);
            if (GUILayout.Button("Random", GUILayout.Width(65)))
            {
                rumbleSeed = Random.Range(0, 99999);
            }
            EditorGUILayout.EndHorizontal();
        }

        samplesPerCycle = EditorGUILayout.IntSlider("Précision (points/cycle)", samplesPerCycle, 4, 16);
        EditorGUILayout.EndVertical();

        if (EditorGUI.EndChangeCheck())
        {
            RegenerateCurve();
            if (autoApply && targetSource != null)
            {
                ApplyToTarget();
            }
        }

        EditorGUILayout.Space(8);

        EditorGUILayout.LabelField("Aperçu de la courbe (normalisée 0 → 1)", EditorStyles.boldLabel);
        EditorGUILayout.CurveField(generatedCurve, Color.cyan, new Rect(0f, -1.1f, 1f, 2.2f), GUILayout.Height(130f));

        EditorGUILayout.Space(8);

        autoApply = EditorGUILayout.ToggleLeft("Appliquer automatiquement à chaque modification", autoApply);

        EditorGUILayout.Space(4);

        EditorGUI.BeginDisabledGroup(targetSource == null);
        GUI.backgroundColor = new Color(0.45f, 0.85f, 0.55f);
        if (GUILayout.Button("Appliquer à l'ImpulseSource", GUILayout.Height(30)))
        {
            ApplyToTarget();
        }
        GUI.backgroundColor = Color.white;

        EditorGUI.BeginDisabledGroup(!Application.isPlaying);
        if (GUILayout.Button("Tester le Screen Shake (Play Mode)", GUILayout.Height(26)))
        {
            ApplyToTarget();
            targetSource.GenerateImpulse();
        }
        EditorGUI.EndDisabledGroup();
        EditorGUI.EndDisabledGroup();
    }

    private void DrawPresets()
    {
        EditorGUILayout.LabelField("Presets rapides", EditorStyles.miniBoldLabel);
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Impact sec"))
        {
            ApplyPreset(WaveType.PunchRecoil, 0.18f, 16f, 1f, 0f, 3f);
        }
        if (GUILayout.Button("Micro Hit"))
        {
            ApplyPreset(WaveType.Sine, 0.12f, 25f, 0.8f, 0f, 2.2f);
        }
        if (GUILayout.Button("Explosion"))
        {
            ApplyPreset(WaveType.SymmetricalRumble, 0.65f, 14f, 1f, 0.05f, 2.5f);
        }
        if (GUILayout.Button("Rumble"))
        {
            ApplyPreset(WaveType.Triangle, 0.8f, 18f, 0.6f, 0.15f, 0.5f);
        }

        EditorGUILayout.EndHorizontal();
    }

    private void ApplyPreset(WaveType type, float dur, float freq, float amp, float attack, float decay)
    {
        waveType = type;
        duration = dur;
        useFrequencyHz = true;
        frequencyHz = freq;
        amplitude = amp;
        attackRatio = attack;
        decayPower = decay;
        RegenerateCurve();

        if (autoApply && targetSource != null)
        {
            ApplyToTarget();
        }
    }

    private void RegenerateCurve()
    {
        int totalCycles = useFrequencyHz
            ? Mathf.Max(1, Mathf.RoundToInt(duration * frequencyHz))
            : Mathf.Max(1, cycles);

        int totalSteps = totalCycles * samplesPerCycle;
        Keyframe[] keys = new Keyframe[totalSteps + 1];
        float polarity = invertStart ? -1f : 1f;

        System.Random rng = new System.Random(rumbleSeed);
        float[] halfCycleAmps = new float[totalCycles * 2];
        for (int i = 0; i < halfCycleAmps.Length; i++)
        {
            halfCycleAmps[i] = 0.55f + (float)rng.NextDouble() * 0.45f;
        }

        for (int i = 0; i <= totalSteps; i++)
        {
            float t = (float)i / totalSteps;
            float env = EvaluateEnvelope(t);
            float wave = 0f;

            if (i == 0 || i == totalSteps)
            {
                wave = 0f;
            }
            else
            {
                float phase = t * totalCycles;
                float cycleFrac = phase - Mathf.Floor(phase);

                switch (waveType)
                {
                    case WaveType.Sine:
                        wave = Mathf.Sin(phase * Mathf.PI * 2f);
                        break;

                    case WaveType.Triangle:
                        wave = 1f - 4f * Mathf.Abs(Mathf.Round(cycleFrac - 0.25f) - (cycleFrac - 0.25f));
                        break;

                    case WaveType.SquareSmooth:
                        wave = Mathf.Sign(Mathf.Sin(phase * Mathf.PI * 2f)) * Mathf.Pow(Mathf.Abs(Mathf.Sin(phase * Mathf.PI * 2f)), 0.35f);
                        break;

                    case WaveType.PunchRecoil:
                        wave = Mathf.Sin(phase * Mathf.PI * 2f) * Mathf.Exp(-t * 3.5f);
                        break;

                    case WaveType.SymmetricalRumble:
                        int halfIndex = Mathf.Clamp(Mathf.FloorToInt(phase * 2f), 0, halfCycleAmps.Length - 1);
                        wave = Mathf.Sin(phase * Mathf.PI * 2f) * halfCycleAmps[halfIndex];
                        break;
                }
            }

            float finalVal = wave * env * amplitude * polarity;
            keys[i] = new Keyframe(t, finalVal);
        }

        generatedCurve = new AnimationCurve(keys);

        for (int i = 0; i < generatedCurve.length; i++)
        {
            if (waveType == WaveType.Triangle)
            {
                AnimationUtility.SetKeyLeftTangentMode(generatedCurve, i, AnimationUtility.TangentMode.Linear);
                AnimationUtility.SetKeyRightTangentMode(generatedCurve, i, AnimationUtility.TangentMode.Linear);
            }
            else
            {
                AnimationUtility.SetKeyLeftTangentMode(generatedCurve, i, AnimationUtility.TangentMode.Auto);
                AnimationUtility.SetKeyRightTangentMode(generatedCurve, i, AnimationUtility.TangentMode.Auto);
            }
        }
    }

    private float EvaluateEnvelope(float t)
    {
        float attack = attackRatio > 0.001f ? Mathf.Clamp01(t / attackRatio) : 1f;
        float decay = decayPower > 0.001f ? Mathf.Pow(1f - t, decayPower) : 1f;
        return attack * decay;
    }

    private void ApplyToTarget()
    {
        if (targetSource == null) return;

        Undo.RecordObject(targetSource, "Apply Generated Impulse Wave");
        targetSource.ImpulseDefinition.ImpulseShape = CinemachineImpulseDefinition.ImpulseShapes.Custom;
        targetSource.ImpulseDefinition.CustomImpulseShape = new AnimationCurve(generatedCurve.keys);
        targetSource.ImpulseDefinition.ImpulseDuration = duration;
        EditorUtility.SetDirty(targetSource);
    }
}