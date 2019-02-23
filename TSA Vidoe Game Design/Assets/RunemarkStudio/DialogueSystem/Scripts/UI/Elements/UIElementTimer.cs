using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Runemark.DialogueSystem.UI
{
    [AddComponentMenu("Runemark Dialogue System/UI/Elements/Timer")]
    public class UIElementTimer : UIElement
	{
		public Slider Slider;
		public Text Label;

        public override void Set<T>(T value)
        {
            if (typeof(T) == typeof(float))
            {
                float time = System.Convert.ToSingle(value);
                Slider.maxValue = time;
                Slider.value = time;
                Label.text = time.ToString("0.00") + " sec";
            }
            else
                base.Set<T>(value);
        }

        public override void UpdateValue<T>(T value)
        {
            if (typeof(T) == typeof(float))
            {
                float time = System.Convert.ToSingle(value);
                Slider.value = time;
                Label.text = time.ToString("0.00") + " sec";      
            }
            else
                base.UpdateValue<T>(value);
        } 

	}
}
