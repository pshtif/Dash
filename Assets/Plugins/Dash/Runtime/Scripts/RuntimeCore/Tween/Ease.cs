/*
 *  Tween library for Unity
 * 
 *	Copyright 2011-2021 Peter @sHTiF Stefcek. All rights reserved.
 *
 *	Ported from Genome2D framework (https://github.com/pshtif/Genome2D/)
 *	
 *	Easings
 */

using UnityEngine;
using System.Collections;
using System;

namespace Dash
{
	public enum EaseType
	{
		LINEAR,
		CUBIC_IN,
		CUBIC_OUT,
		CUBIC_INOUT,
		QUAD_IN,
		QUAD_OUT,
		QUAD_INOUT,
		QUART_IN,
		QUART_OUT,
		QUART_INOUT,
		QUINT_IN,
		QUINT_OUT,
		QUINT_INOUT,
		EXPO_IN,
		EXPO_OUT,
		EXPO_INOUT,
		SINE_IN,
		SINE_OUT,
		SINE_INOUT,
		BACK_IN,
		BACK_OUT,
		BACK_INOUT,
		BOUNCE_IN,
		BOUNCE_OUT,
		BOUNCE_INOUT,
		
	}

	public class EaseFunctions
	{
		static public float defaultPeriod = 0f;
		static public float defaultAmplitude = 1.70158f;

		static public float Evaluate(float p_time, float p_duration, EaseType p_easing)
		{
			switch (p_easing)
			{
				case EaseType.LINEAR:
					return Linear(p_time, p_duration);
				case EaseType.CUBIC_IN:
					return CubicIn(p_time, p_duration);
				case EaseType.CUBIC_OUT:
					return CubicOut(p_time, p_duration);
				case EaseType.CUBIC_INOUT:
					return CubicInOut(p_time, p_duration);
				case EaseType.QUAD_IN:
					return QuadIn(p_time, p_duration);
				case EaseType.QUAD_OUT:
					return QuadOut(p_time, p_duration);
				case EaseType.QUAD_INOUT:
					return QuadInOut(p_time, p_duration);
				case EaseType.QUART_IN:
					return QuartIn(p_time, p_duration);
				case EaseType.QUART_OUT:
					return QuartOut(p_time, p_duration);
				case EaseType.QUART_INOUT:
					return QuartInOut(p_time, p_duration);
				case EaseType.QUINT_IN:
					return QuintIn(p_time, p_duration);
				case EaseType.QUINT_OUT:
					return QuintOut(p_time, p_duration);
				case EaseType.QUINT_INOUT:
					return QuintInOut(p_time, p_duration);
				case EaseType.EXPO_IN:
					return ExpoIn(p_time, p_duration);
				case EaseType.EXPO_OUT:
					return ExpoOut(p_time, p_duration);
				case EaseType.EXPO_INOUT:
					return ExpoInOut(p_time, p_duration);
				case EaseType.SINE_IN:
					return SineIn(p_time, p_duration);
				case EaseType.SINE_OUT:
					return SineOut(p_time, p_duration);
				case EaseType.SINE_INOUT:
					return SineInOut(p_time, p_duration);
				case EaseType.BACK_IN:
					return BackIn(p_time, p_duration, defaultAmplitude);
				case EaseType.BACK_OUT:
					return BackOut(p_time, p_duration, defaultAmplitude);
				case EaseType.BACK_INOUT:
					return BackInOut(p_time, p_duration, defaultAmplitude);
				case EaseType.BOUNCE_IN:
					return BounceIn(p_time, p_duration);
				case EaseType.BOUNCE_OUT:
					return BounceOut(p_time, p_duration);
				case EaseType.BOUNCE_INOUT:
					return BounceInOut(p_time, p_duration);
				default:
					Debug.LogError("Unknown easing state!");
					break;
			}

			return 0.0f;
		}
		
		static public float Linear(float p_time, float p_duration)
		{
			return p_time/p_duration;
		}

		static public float CubicIn(float p_time, float p_duration)
		{
			return (p_time /= p_duration) * p_time * p_time;
		}

		static public float CubicOut(float p_time, float p_duration)
		{
			return (p_time = p_time / p_duration - 1.0f) * p_time * p_time + 1.0f;
		}

		static public float CubicInOut(float p_time, float p_duration)
		{
			return (p_time /= p_duration * 0.5f) < 1.0
				? 0.5f * p_time * p_time * p_time
				: 0.5f * ((p_time -= 2f) * p_time * p_time + 2.0f);
		}

		static public float QuadIn(float p_time, float p_duration)
		{
			return (p_time /= p_duration) * p_time;
		}

		static public float QuadOut(float p_time, float p_duration)
		{
			return -(p_time /= p_duration) * (p_time - 2.0f);
		}

		static public float QuadInOut(float p_time, float p_duration)
		{
			return (p_time /= p_duration * 0.5f) < 1.0
				? 0.5f * p_time * p_time
				: -0.5f * (--p_time * (p_time - 2.0f) - 1.0f);
		}

		static public float QuartIn(float p_time, float p_duration)
		{
			return (p_time /= p_duration) * p_time * p_time * p_time;
		}

		static public float QuartOut(float p_time, float p_duration)
		{
			return -((p_time = p_time / p_duration - 1.0f) * p_time * p_time * p_time - 1.0f);	
		}

		static public float QuartInOut(float p_time, float p_duration)
		{
			return (p_time /= p_duration * 0.5f) < 1.0f
				? 0.5f * p_time * p_time * p_time * p_time
				: -0.5f * ((p_time -= 2f) * p_time * p_time * p_time - 2.0f);
		}

		static public float QuintIn(float p_time, float p_duration)
		{
			return (p_time /= p_duration) * p_time * p_time * p_time * p_time;
		}

		static public float QuintOut(float p_time, float p_duration)
		{
			return (p_time = p_time / p_duration - 1.0f) * p_time * p_time * p_time * p_time + 1.0f;
		}

		static public float QuintInOut(float p_time, float p_duration)
		{
			return (p_time /= p_duration * 0.5f) < 1.0f
				? 0.5f * p_time * p_time * p_time * p_time * p_time
				: 0.5f * ((p_time -= 2f) * p_time * p_time * p_time * p_time + 2.0f);
		}

		static public float ExpoIn(float p_time, float p_duration)
		{
			return p_time != 0f ? Mathf.Pow(2.0f, 10.0f * (p_time / p_duration - 1.0f)) : 0.0f;
		}

		static public float ExpoOut(float p_time, float p_duration)
		{
			return p_time == p_duration ? 1f : -Mathf.Pow(2.0f, -10.0f *  p_time / p_duration) + 1.0f;
		}

		static public float ExpoInOut(float p_time, float p_duration)
		{
			if (p_time == 0.0f)
				return 0.0f;
			if (p_time == p_duration)
				return 1f;
			return (p_time /= p_duration * 0.5f) < 1.0
				? 0.5f * Mathf.Pow(2.0f, 10.0f * (p_time - 1.0f))
				: 0.5f * (-Mathf.Pow(2.0f, -10.0f * --p_time) + 2.0f);
		}

		static public float SineIn(float p_time, float p_duration)
		{
			return -Mathf.Cos(p_time / p_duration * 1.57079637f) + 1.0f;
		}

		static public float SineOut(float p_time, float p_duration)
		{
			return Mathf.Sin(p_time / p_duration * 1.57079637f);
		}

		static public float SineInOut(float p_time, float p_duration)
		{
			return -0.5f * (Mathf.Cos(3.14159274f * p_time / p_duration) - 1.0f);
		}

		static public float CircleIn(float p_time, float p_duration)
		{
			return -(Mathf.Sqrt(1.0f - (p_time /= p_duration) * p_time) - 1.0f);
		}

		static public float CircleOut(float p_time, float p_duration)
		{
			return Mathf.Sqrt(1.0f - (p_time = p_time / p_duration - 1.0f) * p_time);
		}

		static public float CircleInOut(float p_time, float p_duration)
		{
			return (p_time /= p_duration * 0.5f) < 1.0f
				? (-0.5f * (Mathf.Sqrt(1.0f - p_time * p_time) - 1.0f))
				: 0.5f * (Mathf.Sqrt(1.0f - (p_time -= 2f) * p_time) + 1.0f);
		}

		static public float ElasticIn(float p_time, float p_duration, float p_period, float p_amplitude)
		{
			if (p_time == 0.0f)
				return 0.0f;
			if ((p_time /= p_duration) == 1.0)
				return 1f;
			if (p_period == 0.0)
				p_period = p_duration * 0.3f;
			float num1;
			if (p_amplitude < 1.0f)
			{
				p_amplitude = 1f;
				num1 = p_period / 4f;
			}
			else
			{
				num1 = p_period / 6.283185f * Mathf.Asin(1.0f / p_amplitude);
			}

			return -(p_amplitude * Mathf.Pow(2.0f, 10.0f * --p_time) *
			         Mathf.Sin((p_time * p_duration - num1) * 6.283185482f / p_period));
		}

		static public float ElasticOut(float p_time, float p_duration, float p_period, float p_amplitude)
		{

			if (p_time == 0.0f)
				return 0.0f;
			if ((p_time /= p_duration) == 1.0f)
				return 1.0f;
			if (p_period == 0.0f)
				p_period = p_duration * 0.3f;
			float num2;
			if (p_amplitude < 1.0f)
			{
				p_amplitude = 1.0f;
				num2 = p_period / 4.0f;
			}
			else
			{
				num2 = p_period / 6.283185f * Mathf.Asin(1.0f / p_amplitude);
			}

			return p_amplitude * Mathf.Pow(2.0f, -10.0f * p_time) *
				Mathf.Sin((p_time * p_duration - num2) * 6.283185482f / p_period) + 1.0f;
		}

		static public float ElasticInOut(float p_time, float p_duration, float p_period, float p_amplitude)
		{
			if (p_time == 0.0f)
				return 0.0f;
			if ((p_time /= p_duration * 0.5f) == 2.0f)
				return 1.0f;
			if (p_period == 0.0f)
				p_period = p_duration * 0.45f;
			float num3;
			if (p_amplitude < 1.0f)
			{
				p_amplitude = 1.0f;
				num3 = p_period / 4.0f;
			}
			else
			{
				num3 = p_period / 6.283185f * Mathf.Asin(1.0f / p_amplitude);
			}

			return p_time < 1.0f
				? -0.5f * (p_amplitude * Mathf.Pow(2.0f, 10.0f * --p_time) * Mathf.Sin((p_time * p_duration - num3) * 6.283185482f / p_period))
				: p_amplitude * Mathf.Pow(2.0f, -10.0f * --p_time) * Mathf.Sin((p_time * p_duration - num3) * 6.283185482f / p_period) * 0.5f + 1.0f;
		}

		static public float BackIn(float p_time, float p_duration, float p_amplitude)
		{
			return (p_time /= p_duration) * p_time * ((p_amplitude + 1.0f) * p_time - p_amplitude);
		}

		static public float BackOut(float p_time, float p_duration, float p_amplitude)
		{
			return (p_time = p_time / p_duration - 1.0f) * p_time * ((p_amplitude + 1.0f) * p_time + p_amplitude) + 1.0f;
		}

		static public float BackInOut(float p_time, float p_duration, float p_amplitude)
		{
			return (p_time /= p_duration * 0.5f) < 1.0f
				? 0.5f * (p_time * p_time * (((p_amplitude *= 1.525f) + 1.0f) * p_time - p_amplitude))
				: 0.5f * ((p_time -= 2f) * p_time * (((p_amplitude *= 1.525f) + 1.0f) * p_time + p_amplitude) + 2.0f);
		}

		static public float BounceIn(float p_time, float p_duration)
		{
			return 1f - BounceOut(p_duration - p_time, p_duration);
		}

		static public float BounceOut(float p_time, float p_duration)
		{
			if ((p_time /= p_duration) < 0.363636374473572)
				return 121.0f / 16.0f * p_time * p_time;
			if (p_time < 0.727272748947144f)
				return (121.0f / 16.0f * (p_time -= 0.5454546f) * p_time + 0.75f);
			return p_time < 0.909090936183929f 
				? (121.0f / 16.0f * (p_time -= 0.8181818f) * p_time + 15.0f / 16.0f) 
				: (121.0f / 16.0f * (p_time -= 0.9545454f) * p_time + 63.0f / 64.0f);
		}

		static public float BounceInOut(float p_time, float p_duration)
		{
			return p_time < p_duration * 0.5f
				? BounceIn(p_time * 2.0f, p_duration) * 0.5f
				: BounceOut(p_time * 2.0f - p_duration, p_duration) * 0.5f + 0.5f;
		}
	}
	
    public class Bounce
    {
        /*
        static public float EaseIn(float p_t)
        {
            throw new System.InvalidOperationException("Not implemented yet.");
        }
        /**/

        static public float EaseOut(float p_t)
        {
            if (p_t < (1 / 2.75))
            {
                return (7.5625f * p_t * p_t);
            }
            else if (p_t < (2 / 2.75))
            {
                return (7.5625f * (p_t -= (1.5f / 2.75f)) * p_t + .75f);
            }
            else if (p_t < (2.5 / 2.75))
            {
                return (7.5625f * (p_t -= (2.25f / 2.75f)) * p_t + .9375f);
            }
            else
            {
                return (7.5625f * (p_t -= (2.625f / 2.75f)) * p_t + .984375f);
            }
        }
        /*
        static public float EaseInOut(float p_t)
        {
            if (p_t < .5)
            {
                return EaseIn(p_t * 2) * .5f;
            }
            else
            {
                return EaseOut(p_t * 2 - 1) * .5f + .5f;
            }
        }
        /**/
    }

    public class Cubic
    {
        static public float EaseIn(float p_t)
        {
		    return p_t * p_t * p_t;
	    }

        static public float EaseOut(float p_t)
        {
		    return ((p_t -= 1) * p_t * p_t + 1);
	    }

        static public float EaseInOut(float p_t)
        {
		    if ((p_t *= 2) < 1) {
			    return .5f * p_t * p_t * p_t;
		    }else {
			    return .5f * ((p_t -= 2) * p_t * p_t + 2);
		    }
	    }
    }

    public class Back
    {
        static public float DRIVE = 1.70158f;

	    static public float EaseIn(float p_t)
        {
		    return p_t* p_t * ((DRIVE + 1) * p_t - DRIVE);
	    }

        static public float EaseOut(float p_t)
        {
		    return ((p_t -= 1) * p_t * ((DRIVE + 1) * p_t + DRIVE) + 1);
	    }

        static public float EaseInOut(float p_t)
        {
		    float s = DRIVE * 1.525f;
		    if ((p_t*=2) < 1) return 0.5f * (p_t* p_t * (((s) + 1) * p_t - s));
		    return .5f * ((p_t -= 2) * p_t * (((s) + 1) * p_t + s) + 2);
	    }	
    }

    public class Expo
    {
        static public float EaseIn(float p_t)
        {
		    return p_t == 0 ? 0 : Mathf.Pow(2, 10 * (p_t - 1));
	    }

        static public float EaseOut(float p_t)
        {
		    return p_t == 1 ? 1 : (1 - Mathf.Pow(2, -10 * p_t));
	    }

        static public float EaseInOut(float p_t)
        {
		    if (p_t == 0 || p_t == 1) return p_t;

		    if ((p_t *= 2.0f) < 1.0) {
			    return 0.5f * Mathf.Pow(2, 10 * (p_t - 1));
		    }
		    return .5f * (2 - Mathf.Pow(2, -10 * --p_t));
	    }	
    }

    public class Quad
    {
        static public float EaseIn(float p_t)
        {
		    return p_t * p_t;
        }

        static public float EaseOut(float p_t)
        {
		    return -p_t * (p_t - 2);
	    }

        static public float EaseInOut(float p_t)
        {
		    p_t *= 2;
		    if (p_t< 1) {
			    return .5f * p_t * p_t;
		    }
		    return -.5f * ((p_t - 1) * (p_t - 3) - 1);
	    }
    }

    public class Quart
    {
        static public float EaseIn(float p_t)
        {
		    return p_t * p_t * p_t * p_t;
        }

        static public float EaseOut(float p_t)
        {
		    return -((p_t -= 1) * p_t * p_t * p_t - 1);
	    }
    
        static public float EaseInOut(float p_t)
        {
		    p_t *= 2;
		    if (p_t< 1) {
			    return .5f * p_t * p_t * p_t * p_t;
		    }
		    return -.5f * ((p_t -= 2) * p_t * p_t * p_t - 2);
	    }
    }

    public class Quint
    {
	    static public float EaseIn(float p_t)
        {
		    return p_t * p_t * p_t * p_t * p_t;
	    }

        static public float EaseOut(float p_t)
        {
		    return ((p_t -= 1) * p_t * p_t * p_t * p_t + 1);
	    }

        static public float EaseInOut(float p_t)
        {
		    p_t *= 2;
		    if (p_t< 1) {
			    return .5f * p_t * p_t * p_t * p_t * p_t;
		    }
		    return .5f * ((p_t -= 2) * p_t * p_t * p_t * p_t + 2);
	    }	
    }

    public class Sine
    {
        static public float EaseIn(float p_t)
        {
		    return -Mathf.Cos(p_t* (Mathf.PI / 2));
	    }

        static public float EaseOut(float p_t)
        {
		    return Mathf.Sin(p_t* (Mathf.PI / 2));
	    }

        static public float EaseInOut(float p_t)
        {
		    return -0.5f * (Mathf.Cos(Mathf.PI* p_t) - 1);
	    }
    }
}
