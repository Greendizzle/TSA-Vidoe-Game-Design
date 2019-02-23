using UnityEngine;
using UnityEngine.UI;

namespace Runemark.DialogueSystem.UI
{
    public static class UnityUIUtility
    {
        /// <summary>
        /// Set the ui text safe. Handling if the text is null.
        /// Also this turns of the text gameobject if the value is empty.
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="text"></param>
        public static void SetText(Text text, string value)
        {
            if (text != null)
            {
                text.gameObject.SetActive(value != "");
                text.text = value;
            }
        }

        /// <summary>
        /// Set the ui image sprite safe. Handling if the image is null.
        /// Also this turns of the image gameobject if the sprite is null.
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="text"></param>
        public static void SetImage(Image image, Sprite sprite)
        {
            if (image != null)
            {
                image.gameObject.SetActive(sprite != null);
                image.sprite = sprite;
            }
        }
    }

}