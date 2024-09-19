using UnityEngine;
namespace KoboldTools
{

    public enum EasingType
    {
        Linear,
        QuadraticIn,
        QuadraticInOut,
        QuadraticOut,
        CubicIn,
        CubicInOut,
        CubicOut,
        QuarticIn,
        QuarticInOut,
        QuarticOut,
        QuinticIn,
        QuinticInOut,
        QuinticOut,
        SinusoidalIn,
        SinusoidalInOut,
        SinusoidalOut,
        ExponentialIn,
        ExponentialInOut,
        ExponentialOut,
        CircularIn,
        CircularInOut,
        CirularOut,
        ElasticIn,
        ElasticInOut,
        ElasticOut,
        BounceIn,
        BounceInOut,
        BounceOut,
        BackIn,
        BackInOut,
        BackOut
    }

    /* 
     * Functions taken from Tween.js - Licensed under the MIT license
     * at https://github.com/sole/tween.js
     */
    public class Easing
    {
        public static float ease(EasingType easingType, float val)
        {
            switch (easingType)
            {
                case EasingType.BackIn:
                    return Back.In(val);
                case EasingType.BackInOut:
                    return Back.InOut(val);
                case EasingType.BackOut:
                    return Back.Out(val);
                case EasingType.BounceIn:
                    return Bounce.In(val);
                case EasingType.BounceInOut:
                    return Bounce.InOut(val);
                case EasingType.BounceOut:
                    return Bounce.Out(val);
                case EasingType.CircularIn:
                    return Circular.In(val);
                case EasingType.CircularInOut:
                    return Circular.InOut(val);
                case EasingType.CirularOut:
                    return Circular.Out(val);
                case EasingType.CubicIn:
                    return Cubic.In(val);
                case EasingType.CubicInOut:
                    return Cubic.InOut(val);
                case EasingType.CubicOut:
                    return Cubic.Out(val);
                case EasingType.ElasticIn:
                    return Elastic.In(val);
                case EasingType.ElasticInOut:
                    return Elastic.InOut(val);
                case EasingType.ElasticOut:
                    return Elastic.Out(val);
                case EasingType.ExponentialIn:
                    return Exponential.In(val);
                case EasingType.ExponentialInOut:
                    return Exponential.InOut(val);
                case EasingType.ExponentialOut:
                    return Exponential.Out(val);
                case EasingType.Linear:
                    return Linear(val);
                case EasingType.QuadraticIn:
                    return Quadratic.In(val);
                case EasingType.QuadraticInOut:
                    return Quadratic.InOut(val);
                case EasingType.QuadraticOut:
                    return Quadratic.Out(val);
                case EasingType.QuarticIn:
                    return Quartic.In(val);
                case EasingType.QuarticInOut:
                    return Quartic.InOut(val);
                case EasingType.QuarticOut:
                    return Quartic.Out(val);
                case EasingType.QuinticIn:
                    return Quintic.In(val);
                case EasingType.QuinticInOut:
                    return Quintic.InOut(val);
                case EasingType.QuinticOut:
                    return Quintic.Out(val);
                case EasingType.SinusoidalIn:
                    return Sinusoidal.In(val);
                case EasingType.SinusoidalInOut:
                    return Sinusoidal.InOut(val);
                case EasingType.SinusoidalOut:
                    return Sinusoidal.Out(val);
                default:
                    return val;
            }

        }

        public static float Linear(float k)
        {
            return k;
        }

        public class Quadratic
        {
            public static float In(float k)
            {
                return k * k;
            }

            public static float Out(float k)
            {
                return k * (2f - k);
            }

            public static float InOut(float k)
            {
                if ((k *= 2f) < 1f)
                    return 0.5f * k * k;
                return -0.5f * ((k -= 1f) * (k - 2f) - 1f);
            }
        };

        public class Cubic
        {
            public static float In(float k)
            {
                return k * k * k;
            }

            public static float Out(float k)
            {
                return 1f + ((k -= 1f) * k * k);
            }

            public static float InOut(float k)
            {
                if ((k *= 2f) < 1f)
                    return 0.5f * k * k * k;
                return 0.5f * ((k -= 2f) * k * k + 2f);
            }
        };

        public class Quartic
        {
            public static float In(float k)
            {
                return k * k * k * k;
            }

            public static float Out(float k)
            {
                return 1f - ((k -= 1f) * k * k * k);
            }

            public static float InOut(float k)
            {
                if ((k *= 2f) < 1f)
                    return 0.5f * k * k * k * k;
                return -0.5f * ((k -= 2f) * k * k * k - 2f);
            }
        };

        public class Quintic
        {
            public static float In(float k)
            {
                return k * k * k * k * k;
            }

            public static float Out(float k)
            {
                return 1f + ((k -= 1f) * k * k * k * k);
            }

            public static float InOut(float k)
            {
                if ((k *= 2f) < 1f)
                    return 0.5f * k * k * k * k * k;
                return 0.5f * ((k -= 2f) * k * k * k * k + 2f);
            }
        };

        public class Sinusoidal
        {
            public static float In(float k)
            {
                return 1f - Mathf.Cos(k * Mathf.PI / 2f);
            }

            public static float Out(float k)
            {
                return Mathf.Sin(k * Mathf.PI / 2f);
            }

            public static float InOut(float k)
            {
                return 0.5f * (1f - Mathf.Cos(Mathf.PI * k));
            }
        };

        public class Exponential
        {
            public static float In(float k)
            {
                return k == 0f ? 0f : Mathf.Pow(1024f, k - 1f);
            }

            public static float Out(float k)
            {
                return k == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * k);
            }

            public static float InOut(float k)
            {
                if (k == 0f)
                    return 0f;
                if (k == 1f)
                    return 1f;
                if ((k *= 2f) < 1f)
                    return 0.5f * Mathf.Pow(1024f, k - 1f);
                return 0.5f * (-Mathf.Pow(2f, -10f * (k - 1f)) + 2f);
            }
        };

        public class Circular
        {
            public static float In(float k)
            {
                return 1f - Mathf.Sqrt(1f - k * k);
            }

            public static float Out(float k)
            {
                return Mathf.Sqrt(1f - ((k -= 1f) * k));
            }

            public static float InOut(float k)
            {
                if ((k *= 2f) < 1f)
                    return -0.5f * (Mathf.Sqrt(1f - k * k) - 1);
                return 0.5f * (Mathf.Sqrt(1f - (k -= 2f) * k) + 1f);
            }
        };

        public class Elastic
        {
            public static float In(float k)
            {
                if (k == 0)
                    return 0;
                if (k == 1)
                    return 1;
                return -Mathf.Pow(2f, 10f * (k -= 1f)) * Mathf.Sin((k - 0.1f) * (2f * Mathf.PI) / 0.4f);
            }

            public static float Out(float k)
            {
                if (k == 0)
                    return 0;
                if (k == 1)
                    return 1;
                return Mathf.Pow(2f, -10f * k) * Mathf.Sin((k - 0.1f) * (2f * Mathf.PI) / 0.4f) + 1f;
            }

            public static float InOut(float k)
            {
                if ((k *= 2f) < 1f)
                    return -0.5f * Mathf.Pow(2f, 10f * (k -= 1f)) * Mathf.Sin((k - 0.1f) * (2f * Mathf.PI) / 0.4f);
                return Mathf.Pow(2f, -10f * (k -= 1f)) * Mathf.Sin((k - 0.1f) * (2f * Mathf.PI) / 0.4f) * 0.5f + 1f;
            }
        };

        public class Back
        {
            static float s = 1.70158f;
            static float s2 = 2.5949095f;

            public static float In(float k)
            {
                return k * k * ((s + 1f) * k - s);
            }

            public static float Out(float k)
            {
                return (k -= 1f) * k * ((s + 1f) * k + s) + 1f;
            }

            public static float InOut(float k)
            {
                if ((k *= 2f) < 1f)
                    return 0.5f * (k * k * ((s2 + 1f) * k - s2));
                return 0.5f * ((k -= 2f) * k * ((s2 + 1f) * k + s2) + 2f);
            }
        };

        public class Bounce
        {
            public static float In(float k)
            {
                return 1f - Out(1f - k);
            }

            public static float Out(float k)
            {
                if (k < (1f / 2.75f))
                {
                    return 7.5625f * k * k;
                }
                else if (k < (2f / 2.75f))
                {
                    return 7.5625f * (k -= (1.5f / 2.75f)) * k + 0.75f;
                }
                else if (k < (2.5f / 2.75f))
                {
                    return 7.5625f * (k -= (2.25f / 2.75f)) * k + 0.9375f;
                }
                else
                {
                    return 7.5625f * (k -= (2.625f / 2.75f)) * k + 0.984375f;
                }
            }

            public static float InOut(float k)
            {
                if (k < 0.5f)
                    return In(k * 2f) * 0.5f;
                return Out(k * 2f - 1f) * 0.5f + 0.5f;
            }
        };
    }
}