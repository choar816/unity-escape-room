using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using Unity.Barracuda;
using Unity.Barracuda.ONNX;
using Sub;

public class RecognizeItemWithAIBarracuda : MonoBehaviour
{
    [Header("Image")] // 분석 이미지
    public Texture2D texture2D;
    public Texture2D testTexture2D;
    public Texture2D copyTexture2D;

    [Header("Common")] // 모델
    public Dictionary<string, NNModel> modelList;
    public NNModel currentModelAsset;
    public Model runTimeModel;
    public string filePath = "Data/Test";
    public IWorker Engine;

    [Header("Result Value")] // 결과
    public List<string> classSpecify = new List<string>();
    public Prediction prediction;

    /// <summary>
    /// 예상치
    /// </summary>
    [Serializable]
    public struct Prediction
    {
        public int predictedItdx;
        public float[] predictedValue;

        public void SetPrediction(Tensor t)
        {
            predictedValue = t.AsFloats();
            predictedItdx = Array.IndexOf(predictedValue, predictedValue.Max());
            Debug.Log(string.Join(", ", t.AsFloats()));
        }

        public void SetPredictionAsync(Tensor t)
        {
            predictedValue = t.data.Download(t.shape);
            predictedItdx = Array.IndexOf(predictedValue, predictedValue.Max());
            Debug.Log(string.Join(", ", t.AsFloats()));
        }
    }

    public static RecognizeItemWithAIBarracuda Instance;
    // Start is called before the first frame update

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        ModelLoad();
    }

    void ModelLoad()
    {
        runTimeModel = ModelLoader.Load(currentModelAsset);
        Engine = WorkerFactory.CreateWorker(runTimeModel, WorkerFactory.Device.GPU);

        prediction = new Prediction();
    }

    Coroutine activeAI = null;
    public bool isActive = false;

    public void ActiveAI(bool _continue)
    {
        if (_continue)
        {
            if (activeAI == null)
                activeAI = StartCoroutine(ActiveAsync());
            else
            {
                StopCoroutine(activeAI);
                activeAI = StartCoroutine(ActiveAsync());
            }
        }
        else
        {
            Active();
        }
    }

    /// <summary>
    /// 실제 값을 받아서 실행하는 함수
    /// Async 필요 없을 때
    /// 비연속적
    /// </summary>
    void Active()
    {
        if (isActive)
            return;

        isActive = true;
        int channelCount = 1;
        testTexture2D = Image.CropTexture(texture2D);
        Image.CopyTexture(testTexture2D, copyTexture2D);
        testTexture2D = Image.ResizeTextureSoft(testTexture2D, new Vector2(28, 28));
        Tensor input = new Tensor(testTexture2D, channelCount);
        Tensor output = Engine.Execute(input).PeekOutput();
        prediction.SetPrediction(output);
        input.Dispose();
        output.Dispose();
        isActive = false;
    }

    /// <summary>
    /// 실제 값을 받아서 실행하는 함수
    /// Async 필요
    /// 연속적
    /// </summary>
    /// <returns></returns>
    IEnumerator ActiveAsync()
    {
        if (isActive)
            yield break;

        isActive = true;
        int chaanelCount = 1;
        Tensor input = new Tensor(texture2D, chaanelCount);
        testTexture2D = Image.CropTexture(texture2D);
        Image.CopyTexture(testTexture2D, copyTexture2D);

        yield return Engine.StartManualSchedule(input);
        Tensor output = Engine.PeekOutput();
        prediction.SetPredictionAsync(output);

        input.Dispose();
        output.Dispose();

        isActive = false;
    }

    /// <summary>
    /// 파괴될 때 자동실행 - 메모리 릴리즈
    /// </summary>
    private void OnDestroy()
    {
        Engine?.Dispose();
    }

    /// <summary>
    /// 클래스 데이터 메모장에서 로드
    /// </summary>
    public void ReadText()
    {
        string _filePath = Application.dataPath + "/" + filePath;
        if (!string.IsNullOrEmpty(_filePath))
        {
            StreamReader read = new StreamReader(_filePath);

            classSpecify = new List<string>();
            while (!read.EndOfStream)
                classSpecify.Add(read.ReadLine());

            read.Close();
        }
    }

    ///======================================================================================================
    /// <summary>
    /// 모델을 변경할 일 있을때 사용
    /// </summary>
    /// <param name="rawModel"></param>
    /// <returns></returns>
    NNModel LoadBarracudaModel(byte[] rawModel)
    {
        var asset = ScriptableObject.CreateInstance<NNModel>();
        asset.modelData = ScriptableObject.CreateInstance<NNModelData>();
        asset.modelData.Value = rawModel;
        return asset;
    }

    /// <summary>
    /// 모델을 변경할 일 있을 대 사용
    /// </summary>
    /// <param name="rawModel"></param>
    /// <returns></returns>
    NNModel LoadOnnxModel(byte[] rawModel)
    {
        var converter = new ONNXModelConverter(true);
        var onnxModel = converter.Convert(rawModel);

        NNModelData assetData = ScriptableObject.CreateInstance<NNModelData>();
        using (var memoryStream = new MemoryStream())
        using (var writer = new BinaryWriter(memoryStream))
        {
            ModelWriter.Save(writer, onnxModel);
            assetData.Value = memoryStream.ToArray();
        }
        assetData.name = "Data";
        assetData.hideFlags = HideFlags.HideInHierarchy;

        var asset = ScriptableObject.CreateInstance<NNModel>();
        asset.modelData = assetData;
        return asset;
    }
}
