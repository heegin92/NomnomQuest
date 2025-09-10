// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

using Ironcow.Synapse.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


#if USE_ADDRESSABLE
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.Networking;
#endif

namespace Ironcow.Synapse
{
    public static class ExtensionMethods
    {
#if !UNITY_6000_0_OR_NEWER
        /// <summary>
        /// 비동기 작업을 대기할 수 있게 변환하는 확장 메서드
        /// </summary>
        public static TaskAwaiter<bool> GetAwaiter(this AsyncOperation reqOp)
        {
            TaskCompletionSource<bool> tsc = new TaskCompletionSource<bool>();
            reqOp.completed += asyncOp => tsc.TrySetResult(reqOp.isDone);

            if (reqOp.isDone)
                tsc.TrySetResult(reqOp.isDone);

            return tsc.Task.GetAwaiter();
        }
#endif
#if !UNITY_EDITOR
        public static TaskAwaiter<UnityWebRequest.Result> GetAwaiter(this UnityWebRequestAsyncOperation reqOp)
        {
            TaskCompletionSource<UnityWebRequest.Result> tsc = new TaskCompletionSource<UnityWebRequest.Result>();
            reqOp.completed += asyncOp => tsc.TrySetResult(reqOp.webRequest.result);
            if (reqOp.isDone)
                tsc.TrySetResult(reqOp.webRequest.result);
            return tsc.Task.GetAwaiter();
        }
#endif
        /// <summary>
        /// ResourceRequest 작업을 비동기로 대기할 수 있게 변환하는 확장 메서드
        /// </summary>
        public static TaskAwaiter<UnityEngine.Object> GetAwaiter(this ResourceRequest reqOp)
        {
            TaskCompletionSource<UnityEngine.Object> tsc = new TaskCompletionSource<UnityEngine.Object>();
            reqOp.completed += asyncOp => tsc.TrySetResult(reqOp.asset);

            if (reqOp.isDone)
                tsc.TrySetResult(reqOp.asset);

            return tsc.Task.GetAwaiter();
        }

        /// <summary>
        /// 배열을 리스트로 변환하는 확장 메서드
        /// </summary>
        public static List<T> ToList<T>(this T[] array)
        {
            return new List<T>(array);
        }

        /// <summary>
        /// 배열에서 마지막 요소를 반환하는 확장 메서드
        /// </summary>
        public static T Last<T>(this T[] array)
        {
            var list = array.ToList<T>();
            return list.Last();
        }

        /// <summary>
        /// 리스트를 복제하는 확장 메서드
        /// </summary>
        public static List<T> Clone<T>(this List<T> list)
        {
            var retList = new List<T>();
            foreach (var item in list)
            {
                retList.Add(item);
            }
            return retList;
        }

        /// <summary>
        /// 리스트에서 마지막 요소를 제거하고 반환하는 확장 메서드
        /// </summary>
        public static T RemovePullLast<T>(this List<T> list)
        {
            var data = list.Last();
            list.Remove(data);
            return data;
        }

        /// <summary>
        /// 리스트에서 마지막 요소를 제거하는 확장 메서드
        /// </summary>
        public static void RemoveLast<T>(this List<T> list)
        {
            var idx = list.Count - 1;
            list.RemoveAt(idx);
        }

        /// <summary>
        /// 리스트에서 랜덤한 요소를 반환하는 확장 메서드
        /// </summary>
        public static T RandomValue<T>(this List<T> list)
        {
            var rand = Util.Random(0, list.Count);
            return list[rand];
        }

        /// <summary>
        /// 리스트에서 지정한 최대 범위 내에서 랜덤한 요소를 반환하는 확장 메서드
        /// </summary>
        public static T RandomValue<T>(this List<T> list, int max)
        {
            var rand = Util.Random(0, max);
            return list[rand];
        }

        /// <summary>
        /// 리스트에서 지정한 최소값과 최대값 사이에서 랜덤한 요소를 반환하는 확장 메서드
        /// </summary>
        public static T RandomValue<T>(this List<T> list, int min, int max)
        {
            var rand = Util.Random(min, max);
            return list[rand];
        }

        /// <summary>
        /// 배열에서 랜덤한 요소를 반환하는 확장 메서드
        /// </summary>
        public static T RandomValue<T>(this T[] array)
        {
            return array.ToList().RandomValue();
        }

        /// <summary>
        /// 리스트에서 랜덤한 요소를 제거하고 반환하는 확장 메서드
        /// </summary>
        public static T RandomPeek<T>(this List<T> list)
        {
            var rand = Util.Random(0, list.Count);
            var t = list[rand];
            list.RemoveAt(rand);
            return t;
        }

        /// <summary>
        /// MonoBehaviour 객체에서 RectTransform 컴포넌트를 반환하는 확장 메서드
        /// </summary>
        public static RectTransform rectTransform(this MonoBehaviour mono)
        {
            return mono.transform as RectTransform;
        }

        /// <summary>
        /// Texture2D를 Sprite로 변환하는 확장 메서드
        /// </summary>
        public static Sprite ToSprite(this Texture2D tex)
        {
            Rect rect = new Rect(0, 0, tex.width, tex.height);
            return Sprite.Create(tex, rect, new Vector2(0.5f, 0.5f));
        }

        /// <summary>
        /// Texture를 Sprite로 변환하는 확장 메서드
        /// </summary>
        public static Sprite ToSprite(this Texture tex)
        {
            return ToSprite((Texture2D)tex);
        }

        /// <summary>
        /// RenderTexture를 Texture2D로 변환하는 확장 메서드
        /// </summary>
        public static Texture2D ToTexture2D(this RenderTexture rTex)
        {
            Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.ARGB32, false, true);
            var old_rt = RenderTexture.active;
            RenderTexture.active = rTex;

            tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
            tex.Apply();

            RenderTexture.active = old_rt;
            return tex;
        }

        /// <summary>
        /// RenderTexture를 지정된 크기의 Texture2D로 변환하는 확장 메서드
        /// </summary>
        public static Texture2D ToTexture2D(this RenderTexture rTex, Vector2 size)
        {
            Texture2D tex = new Texture2D((int)size.x, (int)size.y, TextureFormat.ARGB32, false, true);
            var old_rt = RenderTexture.active;
            RenderTexture.active = rTex;

            int destX = rTex.width / 2 - (int)size.x / 2;
            int destY = rTex.height / 2 - (int)size.y / 2;
            tex.ReadPixels(new Rect(destX, destY, size.x, size.y), 0, 0);
            tex.Apply();

            RenderTexture.active = old_rt;
            return tex;
        }

        /// <summary>
        /// RenderTexture를 지정된 영역의 Texture2D로 변환하는 확장 메서드
        /// </summary>
        public static Texture2D ToTexture2D(this RenderTexture rTex, Rect rect)
        {
            Texture2D tex = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.ARGB32, false, true);
            var old_rt = RenderTexture.active;
            RenderTexture.active = rTex;

            tex.ReadPixels(rect, 0, 0);
            tex.Apply();

            RenderTexture.active = old_rt;
            return tex;
        }

        /// <summary>
        /// Texture2D를 디코딩하여 압축을 푼 텍스쳐로 반환하는 확장 메서드
        /// </summary>
        public static Texture2D DeCompress(this Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
        }

        /// <summary>
        /// Dictionary에서 첫 번째 값을 반환하는 확장 메서드
        /// </summary>
        public static T First<V, T>(this Dictionary<V, T> dic)
        {
            var keys = new List<V>(dic.Keys);
            return dic[keys[0]];
        }

        /// <summary>
        /// 파일 경로에서 파일 이름을 추출하는 확장 메서드
        /// </summary>
        public static string FullFileName(this string path)
        {
            return path.Split('/').Last();
        }

        /// <summary>
        /// 파일 경로에서 파일 이름을 추출하는 확장 메서드
        /// </summary>
        public static string FileName(this string path)
        {
            return path.Split('/').Last().Split('.')[0];
        }

        /// <summary>
        /// 파일 경로에서 확장자를 추출하는 확장 메서드
        /// </summary>
        public static string Extension(this string path)
        {
            return path.Split('/').Last().Split('.').Last().ToLower();
        }

        /// <summary>
        /// 파일 경로에서 자산 유형을 추출하는 확장 메서드
        /// </summary>
        public static string AssetType(this string path)
        {
            return path.Split('/')[3];
        }

        /// <summary>
        /// 리스트에서 첫 번째 요소를 반환하는 확장 메서드
        /// </summary>
        public static T First<T>(this List<T> list)
        {
            return list[0];
        }

        /// <summary>
        /// AssetBundleCreateRequest의 완료를 대기할 수 있게 변환하는 확장 메서드
        /// </summary>
        public static TaskAwaiter<AssetBundle> GetAwaiter(this AssetBundleCreateRequest instruction)
        {
            TaskCompletionSource<AssetBundle> tsc = new TaskCompletionSource<AssetBundle>();
            instruction.completed += asyncOp => tsc.TrySetResult(instruction.assetBundle);

            if (instruction.isDone)
                tsc.TrySetResult(instruction.assetBundle);

            return tsc.Task.GetAwaiter();
        }

        /// <summary>
        /// 리스트의 각 요소에 대해 비동기 작업을 수행하는 확장 메서드
        /// </summary>
        public static async Task ForEachAsync<T>(this List<T> list, Func<T, Task> func)
        {
            foreach (var value in list)
            {
                await func(value);
            }
        }

        /// <summary>
        /// MonoBehaviour에서 RectTransform을 반환하는 확장 메서드
        /// </summary>
        public static RectTransform RectTransform(this MonoBehaviour mono)
        {
            return mono.transform as RectTransform;
        }

        /// <summary>
        /// GameObject의 Transform을 RectTransform으로 변환하여 반환합니다.
        /// </summary>
        public static RectTransform RectTransform(this GameObject gameObject)
        {
            return gameObject.transform as RectTransform;
        }

        /// <summary>
        /// Transform을 RectTransform으로 변환하여 반환합니다.
        /// </summary>
        public static RectTransform RectTransform(this Transform transform)
        {
            return transform as RectTransform;
        }

        /// <summary>
        /// Animator의 Transform을 RectTransform으로 변환하여 반환합니다.
        /// </summary>
        public static RectTransform RectTransform(this Animator animator)
        {
            return animator.transform as RectTransform;
        }

        /// <summary>
        /// Canvas의 Transform을 RectTransform으로 변환하여 반환합니다.
        /// </summary>
        public static RectTransform RectTransform(this Canvas canvas)
        {
            return canvas.transform as RectTransform;
        }

        /// <summary>
        /// Collider2D의 Transform을 RectTransform으로 변환하여 반환합니다.
        /// </summary>
        public static RectTransform RectTransform(this Collider2D collider)
        {
            return collider.transform as RectTransform;
        }

        /// <summary>
        /// 정수를 천 단위로 쉼표를 추가하여 포맷팅합니다.
        /// </summary>
        public static string ToComma(this int val)
        {
            return string.Format("{0:#,##0}", val);
        }

        /// <summary>
        /// long 값을 천 단위로 쉼표를 추가하여 포맷팅합니다.
        /// </summary>
        public static string ToComma(this long val)
        {
            return string.Format("{0:#,##0}", val);
        }

        /// <summary>
        /// 초 단위의 정수를 시:분:초 형식으로 변환하여 반환합니다.
        /// </summary>
        public static string ToTime(this int val)
        {
            var second = val % 60;
            var minute = val / 60 % 60;
            var hour = val / 360;
            return hour > 0 ? string.Format("{0:00}:{1:00}:{2:00}", hour, minute, second) : string.Format("{0:00}:{1:00}", minute, second);
        }

        /// <summary>
        /// float 값을 초 단위로 변환하여 시:분:초 형식의 문자열로 반환합니다.
        /// </summary>
        public static string ToTime(this float val)
        {
            return ((int)val).ToTime();
        }

        /// <summary>
        /// 비트 수를 계산하여 정수로 반환합니다.
        /// </summary>
        public static int BitToInt(this int val)
        {
            int count = 0;
            int bit = val;
            while (bit != 0)
            {
                bit = bit >> 1;
                count++;
            }
            return count;
        }

        /// <summary>
        /// 비트가 1인 위치를 리스트로 반환합니다.
        /// </summary>
        public static List<int> BitToIntList(this int val)
        {
            int count = 0;
            int bit = val;
            List<int> list = new List<int>();

            while (bit != 0)
            {
                if ((1 & bit) == 1)
                {
                    list.Add(count);
                }
                bit = bit >> 1;
                count++;
            }
            return list;
        }

        /// <summary>
        /// 벡터의 방향(normal)을 각도로 변환하여 반환합니다.
        /// </summary>
        public static float NormalToAngle(this Vector3 normal)
        {
            return (Mathf.Atan2(normal.y, normal.x) * 180 / Mathf.PI) + 90;
        }

        /// <summary>
        /// 리스트에서 주어진 키들을 포함하는 항목을 필터링하여 반환합니다.
        /// </summary>
        public static List<string> FindAllKey(this List<string> list, List<string> keys)
        {
            var retList = new List<string>(list);
            keys.ForEach(obj => retList.RemoveAll(str => !str.ToLower().Contains(obj.ToLower())));
            return retList;
        }

#if USE_ADDRESSABLE
        /// <summary>
        /// 주소 가능한 리소스에서 주어진 키들을 포함하는 항목을 필터링하여 반환합니다.
        /// </summary>
        public static List<string> FindKey(this IResourceLocator locator, List<string> keys)
        {
            var list = locator.Keys.ToList().ConvertObjectListToStringList();
            return list.FindAllKey(keys);
        }
#endif

        /// <summary>
        /// 객체 리스트를 문자열 리스트로 변환하여 반환합니다.
        /// </summary>
        public static List<string> ConvertObjectListToStringList(this List<object> list)
        {
            List<string> retList = new List<string>();
            foreach (var key in list)
            {
                retList.Add(key.ToString());
            }
            return retList;
        }

        /// <summary>
        /// 문자열 내 '{' 문자의 개수를 세어 열의 개수를 반환합니다.
        /// </summary>
        public static int GetColumnCount(this string str)
        {
            return str.Split('{').Length - 1;
        }

        /// <summary>
        /// 리스트의 항목들을 파이('|' 구분자)로 구분한 문자열로 반환합니다.
        /// </summary>
        public static string ToDataString(this List<string> list)
        {
            string ret = "";
            list.ForEach(obj => ret += obj + "|");

            if (ret.Length > 0)
            {
                return ret.Substring(0, ret.Length - 1);
            }
            else
            {
                return ret;
            }
        }

        /// <summary>
        /// 정수 리스트를 파이('|' 구분자)로 구분한 문자열로 반환합니다.
        /// </summary>
        public static string ToDataString(this List<int> list)
        {
            string ret = "";
            list.ForEach(obj => ret += obj + "|");
            if (ret.Length > 0)
            {
                return ret.Substring(0, ret.Length - 1);
            }
            else
            {
                return ret;
            }
        }

        /// <summary>
        /// 실수 리스트를 파이('|' 구분자)로 구분한 문자열로 반환합니다.
        /// </summary>
        public static string ToDataString(this List<float> list)
        {
            string ret = "";
            list.ForEach(obj => ret += obj + "|");
            if (ret.Length > 0)
            {
                return ret.Substring(0, ret.Length - 1);
            }
            else
            {
                return ret;
            }
        }

        /// <summary>
        /// Collider2D 리스트를 주어진 타입 T로 변환하여 반환합니다.
        /// </summary>
        public static List<T> ToList<T>(this List<Collider2D> colliders)
        {
            List<T> list = new List<T>();

            foreach (Collider2D col in colliders)
            {
                if (col.TryGetComponent(out T obj))
                {
                    list.Add(obj);
                }
            }

            return list;
        }

        /// <summary>
        /// 부모 Transform의 모든 자식 GameObject에 대해 주어진 액션을 실행합니다.
        /// </summary>
        public static void ForEachChild(this Transform parent, Action<GameObject> action)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                action.Invoke(parent.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// 주어진 컴포넌트를 추가하거나 이미 있는 컴포넌트를 반환합니다.
        /// </summary>
        public static T AddOrGetComponent<T>(this Component component) where T : Component
        {
            var t = component.GetComponent<T>();
            if (t == null) t = component.gameObject.AddComponent<T>();
            return t;
        }

        /// <summary>
        /// 주어진 컴포넌트를 추가하거나 이미 있는 컴포넌트를 반환합니다.
        /// </summary>
        public static Component AddOrGetComponent(this Component component, Type type)
        {
            var t = component.GetComponent(type);
            if (t == null) t = component.gameObject.AddComponent(type);
            return t;
        }

        /// <summary>
        /// 주어진 컴포넌트를 추가하거나 이미 있는 컴포넌트를 반환합니다.
        /// </summary>
        public static Component AddOrGetComponent(this GameObject component, Type type)
        {
            var t = component.GetComponent(type);
            if (t == null) t = component.gameObject.AddComponent(type);
            return t;
        }

        /// <summary>
        /// 문자열을 UTF8 바이트 배열로 변환하여 반환합니다.
        /// </summary>
        public static byte[] ToDataArray(this string str)
        {
            return UTF8Encoding.UTF8.GetBytes(str);
        }

        /// <summary>
        /// 리스트를 랜덤하게 섞은 후 반환합니다.
        /// </summary>
        public static List<T> Shuffle<T>(this List<T> values)
        {
            System.Random rnd = new System.Random();
            var shuffled = values.OrderBy(_ => rnd.Next()).ToList();

            return shuffled;
        }

        /// <summary>
        /// 문자열을 Vector3로 변환하여 반환합니다.
        /// </summary>
        public static Vector3 ToVector3(this string str)
        {
            if (str[0] == '(' && str.Last() == ')')
            {
                var pos = str.Substring(1, str.Length - 2).Split(',');
                return new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
            }
            return Vector3.zero;
        }

        /// <summary>
        /// Vector2를 Vector2Int로 변환하여 반환합니다.
        /// </summary>
        public static Vector2Int ToVector2Int(this Vector2 vector)
        {
            return new Vector2Int((int)vector.x, (int)vector.y);
        }

        /// <summary>
        /// MonoBehaviour가 붙은 GameObject의 활성 상태를 설정합니다.
        /// </summary>
        public static void SetActive(this MonoBehaviour mono, bool isActive)
        {
            mono.gameObject.SetActive(isActive);
        }


        /// <summary>
        /// 큰 숫자를 포맷팅하여 반환합니다.
        /// </summary>
        public static string ToBigint(this long number)
        {
            long retLong = 0;
            while (number > 99)
            {
                retLong = number % 100;
                number /= 100;
            }
            retLong += number * 100;
            return $"{retLong:#,##0}";
        }

        /// <summary>
        /// 인덱스에서 다음 인덱스를 계산하여 반환합니다.
        /// </summary>
        public static int Next(this int idx, int max, bool isEqual = true, int min = 0)
        {
            return Util.Next(idx, min, max, isEqual);
        }

        /// <summary>
        /// 인덱스에서 이전 인덱스를 계산하여 반환합니다.
        /// </summary>
        public static int Prev(this int idx, int max, bool isEqual = true, int min = 0)
        {
            return Util.Prev(idx, min, max, isEqual);
        }

        /// <summary>
        /// 문자열의 첫 번째 문자를 대문자로 변경해줍니다.
        /// </summary>
        public static string ToFirstUpper(this string str)
        {
            var first = str.Substring(0, 1);
            first.ToUpper();
            return first + str.Substring(1, str.Length - 1);
        }

        public static void Forget(this Task task)
        {
            // 예외는 무시하지만, 로깅도 가능
            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    Debug.LogException(t.Exception);
                }
            });
        }

        public static void Forget(this Task task, Action<Exception> onError)
        {
            task.ContinueWith(t =>
            {
                if (t.IsFaulted && t.Exception != null)
                {
                    onError?.Invoke(t.Exception);
                }
            });
        }


        public static Vector2 ToVector2(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.y);
        }

        public static Vector3 ToVector3(this Vector2 vector)
        {
            return new Vector3(vector.x, vector.y, 0);
        }

        public static int GetInstanceIdOnGO(this Collision col)
        {
            return col.gameObject.GetInstanceID();
        }

        public static int GetInstanceIdOnGO(this Collision2D col)
        {
            return col.gameObject.GetInstanceID();
        }

        public static int GetInstanceIdOnGO(this Collider2D col)
        {
            return col.gameObject.GetInstanceID();
        }

        public static int GetInstanceIdOnGO(this Collider col)
        {
            return col.gameObject.GetInstanceID();
        }

        public static T GetInstance<T>(this Collision obj) where T : SynapseBehaviour
        {
            if (Register.TryGetInstance<T>(obj.GetInstanceIdOnGO(), out var target))
            {
                return target;
            }
            return null;
        }

        public static T GetInstance<T>(this Collision2D obj) where T : SynapseBehaviour
        {
            if (Register.TryGetInstance<T>(obj.GetInstanceIdOnGO(), out var target))
            {
                return target;
            }
            return null;
        }

        public static T GetInstance<T>(this Collider2D obj) where T : SynapseBehaviour
        {
            if (Register.TryGetInstance<T>(obj.GetInstanceIdOnGO(), out var target))
            {
                return target;
            }
            return null;
        }

        public static T GetInstance<T>(this Collider obj) where T : SynapseBehaviour
        {
            if (Register.TryGetInstance<T>(obj.GetInstanceIdOnGO(), out var target))
            {
                return target;
            }
            return null;
        }

        public static bool TryGetInstance<T>(this Collision obj, out T target) where T : SynapseBehaviour
        {
            return Register.TryGetInstance<T>(obj.GetInstanceIdOnGO(), out target);
        }

        public static bool TryGetInstance<T>(this Collision2D obj, out T target) where T : SynapseBehaviour
        {
            return Register.TryGetInstance<T>(obj.GetInstanceIdOnGO(), out target);
        }

        public static bool TryGetInstance<T>(this Collider2D obj, out T target) where T : SynapseBehaviour
        {
            return Register.TryGetInstance<T>(obj.GetInstanceIdOnGO(), out target);
        }

        public static bool TryGetInstance<T>(this Collider obj, out T target) where T : SynapseBehaviour
        {
            return Register.TryGetInstance<T>(obj.GetInstanceIdOnGO(), out target);
        }
    }
}
