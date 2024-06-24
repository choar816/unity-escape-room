// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Unity.Sentis;
// using System;
// using System.IO;
// using System.Linq;
// using Sub;

// public class RecognizeItemWithAI : MonoBehaviour
// {
//     [Header("Image")] // 분석 이미지
//     public Texture2D texture2D;
//     public Texture2D testTexture2D;
//     public Texture2D copyTexture2D;

//     [Header("Common")] // 모델
//     public ModelAsset currentModelAsset;
//     public Model runTimeModel;
//     public string filePath = "Data/Text";
//     public IWorker Engine;

//     [Header("Result Value")] // 결과
//     public List<string> classSpecify = new List<string>();
//     public Prediction prediction;

//     /// <summary>
//     /// 예상치
//     /// </summary>
//     [Serializable]
//     public struct Prediction
//     {
//         public int predictedItdx;
//         public float[] predictedValue;

//         public void SetPrediction(TensorFloat t)
//         {
//             predictedValue = t.ToReadOnlyArray();
//             predictedItdx = Array.IndexOf(predictedValue, predictedValue.Max());
//         }

//         public void SetPredictionAsync(TensorFloat t)
//         {
//             predictedValue = t.ToReadOnlyArray();
//             predictedItdx = Array.IndexOf(predictedValue, predictedValue.Max());
//         }
//     }

//     public static RegconizeItemWithAI Instance;
//     // Start is called before the first frame update

//     private void Awake()
//     {
//         Instance = this;
//     }
//     void Start()
//     {
//         ModelLoad();
//     }

//     void ModelLoad()
//     {
//         runTimeModel = ModelLoader.Load(currentModelAsset);
//         Engine = WorkerFactory.CreateWorker(BackendType.GPUCompute, runTimeModel);

//         prediction = new Prediction();
//     }

//     Coroutine activeAI = null;
//     public bool isActive = false;

//     public void ActiveAI(bool _continue)
//     {
//         if (_continue)
//         {
//             if (activeAI == null)
//                 activeAI = StartCoroutine(ActiveAsync());
//             else
//             {
//                 StopCoroutine(activeAI);
//                 activeAI = StartCoroutine(ActiveAsync());
//             }
//         }
//         else
//         {
//             Active();
//         }
//     }

//     /// <summary>
//     /// 실제 값을 받아서 실행하는 함수
//     /// Async 필요 없을 때
//     /// 비연속적
//     /// </summary>
//     void Active()
//     {
//         if (isActive)
//             return;

//         isActive = true;
//         testTexture2D = Image.CropTexture(texture2D);
//         Image.CopyTexture(testTexture2D, copyTexture2D);
//         testTexture2D = Image.ResizeTextureSoft(testTexture2D, new Vector2(28, 28));
//         TensorFloat input = TextureConverter.ToTensor(testTexture2D);
//         var output = Engine.Execute(input).PeekOutput() as TensorFloat;
//         prediction.SetPrediction(output);
//         input.Dispose();
//         output.Dispose();
//         isActive = false;
//     }

//     /// <summary>
//     /// 실제 값을 받아서 실행하는 함수
//     /// Async 필요
//     /// 연속적
//     /// </summary>
//     /// <returns></returns>
//     IEnumerator ActiveAsync()
//     {
//         if (isActive)
//             yield break;

//         isActive = true;
//         testTexture2D = Image.CropTexture(texture2D);
//         Image.CopyTexture(testTexture2D, copyTexture2D);
//         TensorFloat input = TextureConverter.ToTensor(testTexture2D);
//         yield return Engine.StartManualSchedule(input);
//         var output = Engine.PeekOutput() as TensorFloat;
//         prediction.SetPredictionAsync(output);
//         input.Dispose();
//         output.Dispose();

//         isActive = false;
//     }

//     /// <summary>
//     /// 파괴될 때 자동실행 - 메모리 릴리즈
//     /// </summary>
//     private void OnDestroy()
//     {
//         Engine?.Dispose();
//     }

//     /// <summary>
//     /// 클래스 데이터 메모장에서 로드
//     /// </summary>
//     public void ReadText()
//     {
//         string _filePath = Application.dataPath + "/" + filePath;
//         if (!string.IsNullOrEmpty(_filePath))
//         {
//             StreamReader read = new StreamReader(_filePath);

//             classSpecify = new List<string>();
//             while (!read.EndOfStream)
//                 classSpecify.Add(read.ReadLine());

//             read.Close();
//         }
//     }
// }
