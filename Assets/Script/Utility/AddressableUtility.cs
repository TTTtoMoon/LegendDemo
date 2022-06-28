// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
// using UnityEngine.AddressableAssets;
// using UnityEngine.ResourceManagement.AsyncOperations;
// using UnityEngine.ResourceManagement.ResourceLocations;
//
// namespace RogueGods.Utility
// {
//     public static class AddressableUtility
//     {
//         public static bool AddressableResourceExists(object key)
//         {
//             foreach (var l in Addressables.ResourceLocators)
//             {
//                 if (l.Keys.Contains(key))
//                 {
//                     return true;
//                 }
//                 // IList<IResourceLocation> locs;
//                 // if (l.Locate(key, out locs))
//                 // {
//                 //     return true;
//                 // }
//             }
//
//             return false;
//         }
//
//         /// <summary>
//         /// 不需要透传的加载方式
//         /// </summary>
//         /// <param name="address"></param>
//         /// <param name="callBack"></param>
//         /// <returns></returns>
//         public static AsyncOperationHandle<GameObject> GetGameObject(string address, Action<GameObject> callBack = null)
//         {
//             return GetGameObject<object>(address, (o, o1) =>
//                                                   {
//                                                       if (callBack != null)
//                                                       {
//                                                           callBack(o);
//                                                       }
//                                                   }, null);
//         }
//
//         /// <summary>
//         /// 
//         /// </summary>
//         /// <passThroughParams name="address"></passThroughParams>
//         /// <passThroughParams name="callBack"></passThroughParams>
//         /// <passThroughParams name="params">透传参数</passThroughParams>
//         public static AsyncOperationHandle<GameObject> GetGameObject<T>(string address, Action<GameObject, T> callBack, T passThroughParams)
//         {
//             AsyncOperationHandle<GameObject> handler = Addressables.InstantiateAsync(address);
//             handler.Completed += (handle) =>
//                                  {
//                                      switch (handle.Status)
//                                      {
//                                          case AsyncOperationStatus.None:
//                                              break;
//                                          case AsyncOperationStatus.Succeeded:
//                                              break;
//                                          case AsyncOperationStatus.Failed:
//                                              Debugger.LogError("Addressable加载失败：" + address);
//                                              Addressables.Release(handle);
//                                              break;
//                                      }
//
//                                      if (callBack != null)
//                                      {
//                                          callBack(handle.Result, passThroughParams);
//                                      }
//                                  };
//             return handler;
//         }
//
//         /// <summary>
//         /// 不需要透传的加载方式
//         /// </summary>
//         /// <param name="address"></param>
//         /// <param name="callBack"></param>
//         /// <returns></returns>
//         public static AsyncOperationHandle<GameObject> GetGameObject(string address, Transform parent, Action<GameObject> callBack = null)
//         {
//             return GetGameObject<object>(address, parent, (o, o1) =>
//                                                           {
//                                                               if (callBack != null)
//                                                               {
//                                                                   callBack(o);
//                                                               }
//                                                           }, null);
//         }
//
//         /// <summary>
//         /// 
//         /// </summary>
//         /// <passThroughParams name="address"></passThroughParams>
//         /// <passThroughParams name="callBack"></passThroughParams>
//         /// <passThroughParams name="params">透传参数</passThroughParams>
//         public static AsyncOperationHandle<GameObject> GetGameObject<T>(string address, Transform parent, Action<GameObject, T> callBack, T passThroughParams)
//         {
//             AsyncOperationHandle<GameObject> handler = Addressables.InstantiateAsync(address, parent);
//             handler.Completed += (handle) =>
//                                  {
//                                      switch (handle.Status)
//                                      {
//                                          case AsyncOperationStatus.None:
//                                              break;
//                                          case AsyncOperationStatus.Succeeded:
//                                              break;
//                                          case AsyncOperationStatus.Failed:
//                                              Debugger.LogError("加载失败：" + address);
//                                              Addressables.Release(handle);
//                                              break;
//                                      }
//
//                                      if (callBack != null)
//                                      {
//                                          callBack(handle.Result, passThroughParams);
//                                      }
//                                  };
//             return handler;
//         }
//
//         /// <summary>
//         /// 不需要透传的加载方式
//         /// </summary>
//         /// <param name="address"></param>
//         /// <param name="callBack"></param>
//         /// <returns></returns>
//         public static AsyncOperationHandle<GameObject> GetGameObject(string address, Vector3 position, Quaternion rotation, Action<GameObject> callBack = null)
//         {
//             return GetGameObject<object>(address, position, rotation, (o, o1) =>
//                                                                       {
//                                                                           if (callBack != null)
//                                                                           {
//                                                                               callBack(o);
//                                                                           }
//                                                                       }, null);
//         }
//
//         /// <summary>
//         /// 
//         /// </summary>
//         /// <passThroughParams name="address"></passThroughParams>
//         /// <passThroughParams name="callBack"></passThroughParams>
//         /// <passThroughParams name="params">透传参数</passThroughParams>
//         public static AsyncOperationHandle<GameObject> GetGameObject<T>(string address, Vector3 position, Quaternion rotation, Action<GameObject, T> callBack, T passThroughParams)
//         {
//             AsyncOperationHandle<GameObject> handler = Addressables.InstantiateAsync(address, position, rotation);
//             handler.Completed += (handle) =>
//                                  {
//                                      switch (handle.Status)
//                                      {
//                                          case AsyncOperationStatus.None:
//                                              break;
//                                          case AsyncOperationStatus.Succeeded:
//                                              break;
//                                          case AsyncOperationStatus.Failed:
//                                              Debugger.LogError("加载失败：" + address);
//                                              Addressables.Release(handle);
//                                              break;
//                                      }
//
//                                      if (callBack != null)
//                                      {
//                                          callBack(handle.Result, passThroughParams);
//                                      }
//                                  };
//             return handler;
//         }
//
//         /// <summary>
//         /// 不需要透传的加载方式
//         /// </summary>
//         /// <param name="address"></param>
//         /// <param name="callBack"></param>
//         /// <returns></returns>
//         public static AsyncOperationHandle<T> LoadObject<T>(string address, Action<T> callBack = null)
//         {
//             return LoadObject<T>(address, (o, o1) =>
//                                           {
//                                               if (callBack != null)
//                                               {
//                                                   callBack(o);
//                                               }
//                                           }, null);
//         }
//
//         /// <summary>
//         /// 
//         /// </summary>
//         /// <passThroughParams name="address"></passThroughParams>
//         /// <passThroughParams name="callBack"></passThroughParams>
//         /// <passThroughParams name="params">透传参数</passThroughParams>
//         public static AsyncOperationHandle<T> LoadObject<T>(string address, Action<T, object> callBack, object passThroughParams)
//         {
//             AsyncOperationHandle<T> handler = Addressables.LoadAssetAsync<T>(address);
//             handler.Completed += (handle) =>
//                                  {
//                                      switch (handle.Status)
//                                      {
//                                          case AsyncOperationStatus.None:
//                                              break;
//                                          case AsyncOperationStatus.Succeeded:
//                                              break;
//                                          case AsyncOperationStatus.Failed:
//                                              Debugger.LogWarning("加载失败：" + address);
//                                              Addressables.Release(handle);
//                                              break;
//                                      }
//
//                                      if (callBack != null)
//                                      {
//                                          callBack(handle.Result, passThroughParams);
//                                      }
//                                  };
//             return handler;
//         }
//
//         /// <summary>
//         /// 通过标签或者名字加载一批资源
//         /// </summary>
//         /// <passThroughParams name="address"></passThroughParams>
//         /// <passThroughParams name="callBack"></passThroughParams>
//         /// <passThroughParams name="params">透传参数</passThroughParams>
//         public static AsyncOperationHandle<IList<T>> LoadObjects<T>(string address, Action<T> eachCallBack = null, Action<IList<T>> allCompleteCallBack = null)
//         {
//             AsyncOperationHandle<IList<T>> handler = Addressables.LoadAssetsAsync(address, eachCallBack);
//             AllCompleteCallBack(handler, allCompleteCallBack);
//             return handler;
//         }
//
//         private static async void AllCompleteCallBack<T>(AsyncOperationHandle<IList<T>> handler, Action<IList<T>> allCompleteCallBack)
//         {
//             await handler.Task;
//             allCompleteCallBack?.Invoke(handler.Result);
//         }
//
//         public static void Release(GameObject gameObject)
//         {
//             Addressables.ReleaseInstance(gameObject);
//         }
//
//         public static void Release<T>(T res)
//         {
//             Addressables.Release(res);
//         }
//     }
// }
