﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Threading.Tasks;

namespace Utilities
{
	public class UtilityMethod {

		private static readonly System.Random _random = new System.Random();

		/// <summary>
		///  Load single sprite from multiple mode
		/// </summary>
		/// <param name="spriteArray"></param>
		/// <param name="spriteName"></param>
		/// <returns></returns>
		public static Sprite LoadSpriteFromMulti(Sprite[] spriteArray, string spriteName) {
			foreach (Sprite s in spriteArray) {
				
				if (s.name == spriteName) return s;
			}
			return null;
		}

		/// <summary>
        /// Clear every child gameobject
        /// </summary>
        /// <param name="parent"></param>
        public static void ClearChildObject(Transform parent) {
            foreach (Transform t in parent) {
                GameObject.Destroy(t.gameObject);
            }
        }

		/// <summary>
		///  Insert gameobject to parent
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="prefab"></param>
		/// <returns></returns>
		public static T CreateObjectToParent<T>(Transform parent, T prefab) where T : MonoBehaviour
		{
			GameObject item = GameObject.Instantiate(prefab.gameObject);
			item.transform.SetParent(parent);
			item.transform.localScale = Vector3.one;
			item.transform.position = new Vector3(item.transform.position.x, item.transform.position.y, 1);
			item.transform.localPosition = new Vector3(0, 0, 1);
			return item.GetComponent<T>();
		}

		public static GameObject FindObject(GameObject parent, string name) {
		     Transform[] trs= parent.GetComponentsInChildren<Transform>(true);
		     foreach(Transform t in trs){
		         if(t.name == name){
		              return t.gameObject;
		         }
		     }
		     return null;
		 }


		/// <summary>
		/// Rolls the dice, only return 1 or 0.
		/// </summary>
		/// <returns>The dice.</returns>
		public static int RollDice() {
			return Mathf.RoundToInt(Random.Range(0,1));
		}
		
		/// <summary>
		/// Possibilities the match.
		/// </summary>
		/// <returns><c>true</c>, if match was possibilityed, <c>false</c> otherwise.</returns>
		public static bool PercentageGame(float rate) {
			float testValue =Random.Range(0f ,1f);
			return ( rate >= testValue ) ? true : false;
		}

		public static T PercentageTurntable<T>(T[] p_group, float[] percent_array) {
			float percent = Random.Range(0f, 100f);
			float max = 100;

			for (int i = 0 ; i < percent_array.Length; i++) {
				float newMax = max - percent_array[i];
				if (max >= percent && newMax <= percent ) return p_group[i];

				max = newMax;
			}
			return default (T);
		}

		public static T PercentageTurntable<T>(T[] p_group, int[] percent_array) {
			float[] convertFloat = System.Array.ConvertAll(percent_array, s => (float)s);
			return PercentageTurntable<T>(p_group, convertFloat);
		}

		public static Vector3 ScaleToWorldSize(Vector3 p_vector3, int target_value) {
			return new Vector3( target_value / p_vector3.x, target_value / p_vector3.y, target_value / p_vector3.z );
		}


		 public static T SafeDestroy<T>(T obj) where T : Object {
			if (Application.isEditor)
				Object.DestroyImmediate(obj);
			else
				Object.Destroy(obj);
			
			return null;
		}
		
		public static T SafeDestroyGameObject<T>(T component) where T : Component
		{
			if (component != null)
				SafeDestroy(component.gameObject);
			return null;
		}

		public static T ParseEnum<T>(string value)	{
		    return (T) System.Enum.Parse(typeof(T), value, true);
		}

		public static async Task DoDelayWork(float p_delay, System.Action p_action)
		{
			await Task.Delay(System.TimeSpan.FromSeconds(p_delay));

			if (p_action != null)
				p_action();
		}

		public static int GetRandomNumber(int min, int max)
		{
			lock (_random) // synchronize
			{
				return (_random.Next(min, max));
			}
		}

		public static string Base64Encode(string plainText)
		{
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
			return System.Convert.ToBase64String(plainTextBytes);
		}

		public static string Base64Decode(string base64EncodedData)
		{
			var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
			return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
		}

		public static void SetSimpleBtnEvent(Button btn, System.Action eventCallback) {
			btn.onClick.RemoveAllListeners();
			btn.onClick.AddListener(() =>
			{
				eventCallback();
			});
		}

		public static string GenerateUniqueRandomString(int length) {
			string constant = SystemInfo.deviceUniqueIdentifier;

			return constant + "_" + GenerateRandomString(length);
		}

		public static string GenerateRandomString(int length) {
			var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			int stringLength = chars.Length;
			var stringChars = new char[length];
			var random = new System.Random();

			for (int i = 0; i < length; i++)
			{
				stringChars[i] = chars[random.Next(stringLength)];
			}

			return new System.String(stringChars);
		}

	}
}